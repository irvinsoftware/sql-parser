using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace Irvin.SqlParser
{
    public class SqlMetaData
    {
        public async Task<Database> LoadByConnectionString(string connectionString, IProgress<int> progressReporter, CancellationToken cancellationToken)
        {
            Database database = new Database();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT [name] AS [filegroup_name], is_default 
                        FROM sys.filegroups
                        ORDER BY 1
                        
                        SELECT
	                        [schemas].[schema_id],
	                        [schemas].[name] AS [schema_name]
                        INTO #schemas
                        FROM sys.schemas
                            LEFT JOIN sys.database_principals
                                ON schemas.principal_id = database_principals.principal_id
                                AND database_principals.[type_desc] = 'DATABASE_ROLE'
                        WHERE schemas.[name] NOT IN ('INFORMATION_SCHEMA','cdc','guest')
                            AND database_principals.[name] IS NULL	

                        SELECT [schema_name] 
                        FROM #schemas
                        WHERE [schema_name] NOT IN ('sys')
                        ORDER BY 1
                    ";
                    
                    await connection.OpenAsync(cancellationToken);
                    using (SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken))
                    {
                        var dataSetCount = 11.0M;
                        
                        database.Filegroups = await ReadSimpleList(reader, ReadFilegroup, cancellationToken);
                        progressReporter.Report((int)(1 / dataSetCount * 100));
                        
                        database.Schemas = await ReadSimpleList(reader, ReadSchema, cancellationToken);
                        progressReporter.Report((int)(2 / dataSetCount * 100));
                    }
                }
            }

            return database;
        }

        private static async Task<List<T>> ReadSimpleList<T>(SqlDataReader reader, Func<IDataRecord, T> builder, CancellationToken cancellationToken)
        {
            List<T> list = new List<T>();

            while (await reader.ReadAsync(cancellationToken))
            {
                list.Add(builder(reader));
            }

            await reader.NextResultAsync(cancellationToken);

            return list;
        }

        private static Filegroup ReadFilegroup(IDataRecord record)
        {
            Filegroup filegroup = new Filegroup();
            filegroup.Name = record["filegroup_name"].ToString();
            filegroup.IsDefault = Convert.ToBoolean(record["is_default"]);
            return filegroup;
        }

        private static Schema ReadSchema(IDataRecord record)
        {
            Schema schema = new Schema();
            schema.Name = record["schema_name"].ToString();
            return schema;
        }
    }
}