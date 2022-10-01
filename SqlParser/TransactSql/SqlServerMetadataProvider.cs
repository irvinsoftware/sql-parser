using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Irvin.SqlParser.Metadata;

namespace Irvin.SqlParser.TransactSql
{
    public class SqlServerMetadataProvider : IMetadataProvider
    {
        public Database GetModelContainer(DatabaseMetadata databaseInfo)
        {
            throw new NotImplementedException();
        }

        public Task<List<LoginMetadata>> GetInstanceLogins(string source, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<DatabaseMetadata>> GetDatabases(string source, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<RelationMetadata>> GetRelations(string source, string databaseName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<ModuleMetadata>> GetModules(string source, string databaseInfoName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<SequenceMetadata>> GetSequences(string source, string databaseName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<RuleMetadata>> GetRules(string source, string databaseName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<PrincipalMetadata>> GetDatabasePrincipals(string source, string databaseName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<PermissionMetadata>> GetPermissions(string source, string databaseName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}