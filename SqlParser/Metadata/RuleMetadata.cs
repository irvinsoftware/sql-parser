namespace Irvin.SqlParser.Metadata;

public class RuleMetadata : ISqlObject
{
    public int ID { get; }
    public string SchemaName { get; set; }
    public string Name { get; }
    public bool IsSystemNamed { get; }
    public string ReferencedDatabaseName { get; set; }
    public string ReferencedSchemaName { get; set; }
    public string ReferencedTableName { get; set; }
    public DateTime Created { get; }
    public DateTime LastUpdated { get; }
    public bool IsInDefaultInstallation { get; set; }
}