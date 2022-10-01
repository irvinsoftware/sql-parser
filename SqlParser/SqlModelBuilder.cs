using Irvin.SqlParser.Metadata;

namespace Irvin.SqlParser;

public class SqlModelBuilder
{
    private readonly IMetadataProvider _metadataProvider;

    public SqlModelBuilder(IMetadataProvider metadataProvider)
    {
        _metadataProvider = metadataProvider;
    }
    
    public async Task<List<Database>> BuildFromConnectionString(string connectionString, IProgress<WorkState> progressReporter, CancellationToken cancellationToken)
    {
        List<Database> instance = new List<Database>();
        
        WorkState workState = new WorkState(progressReporter);
        workState.DefineStep("Retrieving logins", 1);
        workState.DefineStep("Retrieving database list", 1);
        workState.DefineStep("Analyzing databases");

        workState.StartNextStep();
        List<LoginMetadata> logins = await _metadataProvider.GetInstanceLogins(connectionString, cancellationToken);
        workState.StepFinished();
        
        workState.StartNextStep();
        List<DatabaseMetadata> databases = await _metadataProvider.GetDatabases(connectionString, cancellationToken);
        workState.StepFinished();

        workState.StartNextStep((uint?)databases.Count);
        foreach (DatabaseMetadata databaseInfo in databases)
        {
            workState.StartStep($"Analyzing {databaseInfo.Name}");
            Database databaseModel = _metadataProvider.GetModelContainer(databaseInfo);
            
            workState.StartStep("Analyzing logins & users", 1);
            List<PrincipalMetadata> usersAndRoles = await _metadataProvider.GetDatabasePrincipals(connectionString, databaseInfo.Name, cancellationToken);
            databaseModel.AddSecurityPrincipals(logins, usersAndRoles);
            workState.StepFinished();

            workState.StartStep("Retrieving tables, views, and table types");
            List<RelationMetadata> relations = await _metadataProvider.GetRelations(connectionString, databaseInfo.Name, cancellationToken);
            workState.StepFinished();

            workState.StartStep("Analyzing tables, views and table types", (uint?)relations.Count);
            foreach (RelationMetadata relation in relations)
            {
                workState.StartStep($"Analyzing {relation.Name}", 1);
                
                if (relation.Kind == RelationKind.StandardTable)
                {
                    databaseModel.AddTable(relation);    
                }
                else if (relation.Kind == RelationKind.View)
                {
                    databaseModel.AddView(relation);
                }
                else if(relation.Kind == RelationKind.TableType)
                {
                    databaseModel.AddTableType(relation);
                }
                
                workState.StepFinished();
            }
            
            workState.StartStep("Retrieving stored procedures, functions and triggers", 1);
            List<ModuleMetadata> modules = await _metadataProvider.GetModules(connectionString, databaseInfo.Name, cancellationToken);
            workState.StepFinished();
            
            workState.StartStep("Analyzing stored procedures, functions and triggers", (uint?)modules.Count);
            foreach (ModuleMetadata module in modules)
            {
                workState.StartStep($"Analyzing {module.Name}", 1);

                if (module.Kind == ModuleKind.StoredProcedure)
                {
                    databaseModel.AddStoredProcedure(module);
                }
                else if (module.Kind == ModuleKind.Function)
                {
                    databaseModel.AddFunction(module);
                }
                else if (module.Kind == ModuleKind.Trigger)
                {
                    databaseModel.AddFunction(module);
                }
                
                workState.StepFinished();
            }
            workState.StepFinished();
            
            workState.StartStep("Analyzing permissions", 1);
            List<PermissionMetadata> permissions = await _metadataProvider.GetPermissions(connectionString, databaseInfo.Name, cancellationToken);
            databaseModel.AddPermissions(permissions);
            workState.StepFinished();
            
            workState.StartStep("Analyzing sequences", 1);
            List<SequenceMetadata> sequences = await _metadataProvider.GetSequences(connectionString, databaseInfo.Name, cancellationToken);
            databaseModel.AddSequences(sequences);
            workState.StepFinished();

            workState.StartStep("Analyzing synonyms, rules, defaults, etc.", 1);
            List<RuleMetadata> rules = await _metadataProvider.GetRules(connectionString, databaseInfo.Name, cancellationToken);
            databaseModel.AddRules(rules);
            workState.StepFinished();
            
            instance.Add(databaseModel);
        }
        workState.StepFinished();

        return instance;
    }
}