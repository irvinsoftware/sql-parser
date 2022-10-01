using Irvin.SqlParser.Metadata;

namespace Irvin.SqlParser;

public abstract class Database : DatabaseMetadata
{
    public Database(DatabaseMetadata databaseMetadata)
    {
        throw new NotImplementedException();
    }

    public void AddSecurityPrincipals(List<LoginMetadata> logins, List<PrincipalMetadata> usersAndRoles)
    {
        throw new NotImplementedException();
    }

    public void AddTable(RelationMetadata tableMetadata)
    {
        throw new NotImplementedException();
    }

    public void AddView(RelationMetadata viewMetadata)
    {
        throw new NotImplementedException();
    }

    public void AddTableType(RelationMetadata typeMetadata)
    {
        throw new NotImplementedException();
    }

    public void AddStoredProcedure(ModuleMetadata procedureMetadata)
    {
        throw new NotImplementedException();
    }

    public void AddFunction(ModuleMetadata functionMetadata)
    {
        throw new NotImplementedException();
    }

    public void AddSequences(IEnumerable<SequenceMetadata> sequencesMetadata)
    {
        throw new NotImplementedException();
    }

    public void AddRules(IEnumerable<RuleMetadata> rulesMetadata)
    {
        throw new NotImplementedException();
    }

    public void AddPermissions(IEnumerable<PermissionMetadata> permissions)
    {
        throw new NotImplementedException();
    }
}