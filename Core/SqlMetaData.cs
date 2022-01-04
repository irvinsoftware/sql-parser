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
            Extractor actor = new Extractor(progressReporter);
            actor.ConnectionString = connectionString;
            actor.CancellationToken = cancellationToken;
            
            await actor.Load();

            return actor.Database;
        }

        private class Extractor
        {
            private readonly IProgress<int> _progressReporter;

            public Extractor(IProgress<int> progressReporter)
            {
                _progressReporter = progressReporter;
            }
            
            public string ConnectionString { get; set; }
            public CancellationToken CancellationToken { get; set; }
            public Database Database { get; private set; }

            public async Task Load()
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
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
                    
                        await connection.OpenAsync(CancellationToken);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync(CancellationToken))
                        {
                            var dataSetCount = 11.0M;
                        
                            Database.Filegroups = await ReadSimpleList(reader, ReadFilegroup);
                            _progressReporter.Report((int)(1 / dataSetCount * 100));
                        
                            Database.Schemas = await ReadSimpleList(reader, ReadSchema);
                            _progressReporter.Report((int)(2 / dataSetCount * 100));
                        }
                    }
                }
            }
            
            private async Task<List<T>> ReadSimpleList<T>(SqlDataReader reader, Func<IDataRecord, T> builder)
            {
                List<T> list = new List<T>();

                while (await reader.ReadAsync(CancellationToken))
                {
                    list.Add(builder(reader));
                }

                await reader.NextResultAsync(CancellationToken);

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
}