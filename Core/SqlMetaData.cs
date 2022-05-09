using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
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

							SELECT 
								objects.[object_id],
							    CASE WHEN table_types.[name] IS NULL THEN schemas.schema_name ELSE SCHEMA_NAME(table_types.[schema_id]) END AS [schema_name],
								objects.[type_desc] AS [object_type],
								COALESCE(table_types.[name], objects.[name]) AS [object_name],

								columns.column_id AS column_order,
								columns.[name] AS column_name,
								CASE WHEN computed_columns.[definition] IS NULL THEN column_types.[name] ELSE NULL END  AS column_data_type,
								CASE WHEN computed_columns.[definition] IS NOT NULL THEN NULL 
									 WHEN column_types.[name] LIKE '%char' OR column_types.[name] LIKE '%binary' THEN columns.max_length 
									 ELSE NULL END AS column_length,
								CASE WHEN computed_columns.[definition] IS NOT NULL THEN NULL 
									 WHEN column_types.[name] IN ('decimal','numeric') THEN columns.[precision] 
									 ELSE NULL END column_precision,
								CASE WHEN computed_columns.[definition] IS NOT NULL THEN NULL 
									 WHEN column_types.[name] IN ('decimal','numeric') THEN columns.[scale] 
								     ELSE NULL END column_scale,
								CASE WHEN computed_columns.[definition] IS NULL THEN columns.is_nullable ELSE NULL END AS is_nullable,
								columns.is_identity,
								identity_columns.seed_value,
								identity_columns.increment_value,
								columns.collation_name,
								computed_columns.[definition] AS computed_column_definition,	

								CASE WHEN pk_columns.index_column_id IS NULL THEN NULL 
									 WHEN table_types.[name] IS NOT NULL THEN NULL
									 ELSE pk.[name] END AS primary_key_name,	
								pk.fill_factor AS primary_key_fill_factor,
								CASE WHEN pk_columns.index_column_id IS NULL THEN NULL 
									 ELSE pk.[type_desc] END AS primary_key_index_type,
								pk_columns.index_column_id AS order_in_primary_key,

								foreign_keys.[name] AS foreign_key_name,
								foreign_keys.is_system_named AS foreign_key_autogen_name,
								foreign_keys.update_referential_action_desc AS foreign_key_update_action,
								foreign_keys.delete_referential_action_desc AS foreign_key_delete_action,
								foreign_keys.is_not_trusted,
								foreign_keys.is_disabled AS foreign_key_disabled,
								fk_schemas.[name] AS foreign_key_schema_name,
								fk_tables.[name] AS foreign_key_table_name,
								fk_columns.[name] AS foreign_key_column_name,
								foreign_key_columns.constraint_column_id AS order_in_foreign_key,
								
								default_constraints.[name] AS default_constraint_name,
								default_constraints.is_system_named AS default_constraint_name_was_autogenerated,
								default_constraints.[definition] AS default_constraint_sql,

								sql_modules.[definition] AS object_defintion_sql,
								sql_modules.is_schema_bound,
								
								user_table.is_tracked_by_cdc
							FROM #schemas AS schemas
								INNER JOIN sys.objects
									ON schemas.[schema_id] = objects.[schema_id]		
								LEFT JOIN sys.columns
									ON objects.[object_id] = columns.[object_id]
								LEFT JOIN sys.types AS column_types
									ON columns.system_type_id = column_types.system_type_id
									AND columns.user_type_id = column_types.user_type_id
								LEFT JOIN sys.default_constraints
									ON columns.default_object_id = default_constraints.[object_id]
								LEFT JOIN sys.computed_columns
									ON columns.[object_id] = computed_columns.[object_id]
									AND columns.column_id = computed_columns.column_id
								LEFT JOIN sys.foreign_key_columns
									INNER JOIN sys.tables AS fk_tables
										ON foreign_key_columns.referenced_object_id = fk_tables.[object_id]
									INNER JOIN sys.schemas AS fk_schemas
										ON fk_tables.[schema_id] = fk_schemas.[schema_id]
									INNER JOIN sys.foreign_keys
										ON foreign_key_columns.constraint_object_id = foreign_keys.[object_id]
									INNER JOIN sys.columns AS fk_columns
										ON foreign_key_columns.referenced_object_id = fk_columns.[object_id]
										AND foreign_key_columns.referenced_column_id = fk_columns.column_id
									ON columns.[object_id] = foreign_key_columns.parent_object_id
									AND columns.column_id = foreign_key_columns.parent_column_id	
								LEFT JOIN sys.indexes AS pk		
									ON objects.[object_id] = pk.[object_id]
									AND pk.is_primary_key = 1	
								LEFT JOIN sys.index_columns AS pk_columns
									ON pk.[object_id] = pk_columns.[object_id]
									AND columns.column_id = pk_columns.column_id
								LEFT JOIN sys.sql_modules
									ON objects.[object_id] = sql_modules.[object_id]
								LEFT JOIN sys.table_types
									ON objects.[object_id] = table_types.type_table_object_id	
								LEFT JOIN sys.identity_columns
									ON columns.[object_id] = identity_columns.[object_id]
									AND columns.column_id = identity_columns.column_id	
								LEFT JOIN sys.tables AS user_table
									ON objects.[object_id] = user_table.[object_id]
							WHERE 
								(
									(schemas.[schema_name] = 'sys' AND objects.[type_desc] = 'TYPE_TABLE')
									OR 
									(
										schemas.[schema_name] NOT IN ('sys')
										AND
										objects.[type_desc] NOT IN 
										(
											'SERVICE_QUEUE',
											'DEFAULT_CONSTRAINT', 'PRIMARY_KEY_CONSTRAINT', 'FOREIGN_KEY_CONSTRAINT', 
											'SQL_TRIGGER', 'CHECK_CONSTRAINT', 'UNIQUE_CONSTRAINT',
											'SQL_STORED_PROCEDURE', 'SQL_SCALAR_FUNCTION', 'SYNONYM'
										)
										AND 
										[objects].is_ms_shipped = 0
										AND 
										[objects].[name] NOT IN ('__RefactorLog','sysdiagrams')
									)
								)
							ORDER BY
								objects.[object_id],
								columns.column_id

                        ";
                    
                        await connection.OpenAsync(CancellationToken);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync(CancellationToken))
                        {
                            var dataSetCount = 11.0M;
                        
                            Database.Filegroups = await ReadSimpleList(reader, ReadFilegroup);
                            _progressReporter.Report((int)(1 / dataSetCount * 100));
                        
                            Database.Schemas = await ReadSimpleList(reader, ReadSchema);
                            _progressReporter.Report((int)(2 / dataSetCount * 100));

                            while (await reader.ReadAsync(CancellationToken))
                            {
	                            string schemaName = reader["schema_name"].ToString();
	                            string objectKind = reader["object_type"].ToString();
	                            string objectName = reader["object_name"].ToString();
	                            //int columnId = (int)reader["column_id"];
	                            string columnName = reader["column_name"].ToString();
	                            string columnTypeSqlName = reader["column_type"].ToString();
	                            
	                            Schema schema = Database.FindOrCreateSchema(schemaName);
	                            TabularObject objDef = schema.FindOrCreateObject(objectKind, objectName);
	                            objDef.ObjectId = (int) reader["object_id"];
	                            Column column = objDef.AddColumn(columnName, columnTypeSqlName);
	                            column.IsNullable = (bool)reader["is_nullabie"];

	                            ComputedColumn computedColumn = column as ComputedColumn;
	                            if (computedColumn != null)
	                            {
		                            throw new NotImplementedException();
	                            }
	                            
	                            NumericColumn numericColumn = column as NumericColumn;
	                            if (numericColumn != null)
	                            {
		                            numericColumn.IsIdentity = (bool)reader["is_identity"];

		                            int? precision = GetInt32(reader, "column_precision");
		                            int? scale = GetInt32(reader, "column_scale");
		                            if (precision.HasValue || scale.HasValue)
		                            {
			                            numericColumn.Precision = precision.Value;
			                            numericColumn.Scale = scale.Value;
		                            }

		                            int? seedValue = GetInt32(reader, "seed_value");
		                            int? incrementValue = GetInt32(reader, "increment_value");
		                            if (seedValue.HasValue || incrementValue.HasValue)
		                            {
			                            numericColumn.IdentitySeed = seedValue.Value;
			                            numericColumn.IdentityIncrement = seedValue.Value;
		                            }
	                            }

	                            StringColumn stringColumn = column as StringColumn;
	                            if (stringColumn != null)
	                            {
		                            stringColumn.MaximumCharacters = (ushort)reader["column_length"];
		                            if (stringColumn.IsFixedWidth)
		                            {
			                            stringColumn.MinimumCharacters = stringColumn.MaximumCharacters;
		                            }
		                            stringColumn.CollationName = reader["collation_name"].ToString();
	                            }


                            }
                            await reader.NextResultAsync(CancellationToken);
                            _progressReporter.Report((int)(3 / dataSetCount * 100));
                        }
                    }
                }
            }

            private static int? GetInt32(IDataRecord record, string columName)
            {
	            return record[columName] != DBNull.Value && record[columName] != null
		            ? Convert.ToInt32(record[columName])
		            : (int?)null;
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