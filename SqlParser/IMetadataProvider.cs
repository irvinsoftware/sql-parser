using Irvin.SqlParser.Metadata;

namespace Irvin.SqlParser;

public interface IMetadataProvider
{
    Database GetModelContainer(DatabaseMetadata databaseInfo);
    Task<List<LoginMetadata>> GetInstanceLogins(string source, CancellationToken cancellationToken);
    public Task<List<DatabaseMetadata>> GetDatabases(string source, CancellationToken cancellationToken);
    public Task<List<RelationMetadata>> GetRelations(string source, string databaseName, CancellationToken cancellationToken);
    Task<List<ModuleMetadata>> GetModules(string source, string databaseInfoName, CancellationToken cancellationToken);
    Task<List<SequenceMetadata>> GetSequences(string source, string databaseName, CancellationToken cancellationToken);
    Task<List<RuleMetadata>> GetRules(string source, string databaseName, CancellationToken cancellationToken);
    Task<List<PrincipalMetadata>> GetDatabasePrincipals(string source, string databaseName, CancellationToken cancellationToken);
    Task<List<PermissionMetadata>> GetPermissions(string source, string databaseName, CancellationToken cancellationToken);
}