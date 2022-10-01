namespace Irvin.SqlParser.Metadata;

public class KeyMetadata : ISqlObject
{
    public int ID { get; }
    public string SchemaName { get; set; }
    public string Name { get; }
    public bool IsSystemNamed { get; }
    public KeyConstraintKind Kind { get; set; }
    public List<string> CoveredColumns { get; set; }
    public string ReferenceTableSchema { get; set; }
    public string ReferenceTableName { get; set; }
    public List<string> ReferencedColumnNames { get; set; }
    public DateTime Created { get; }
    public DateTime LastUpdated { get; }
    public bool IsInDefaultInstallation { get; set; }
}