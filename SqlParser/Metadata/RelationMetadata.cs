namespace Irvin.SqlParser.Metadata;

public class RelationMetadata : ISqlObject
{
    public RelationMetadata()
    {
        Columns = new List<ColumnMetadata>();
        Keys = new List<KeyMetadata>();
        Indexes = new List<IndexMetadata>();
        CheckConstraints = new List<CheckConstraint>();
    }
    
    public int ID { get; }
    public string SchemaName { get; set; }
    public string Name { get; set; }
    public bool IsSystemNamed { get; }
    public RelationKind Kind { get; set; }
    public List<ColumnMetadata> Columns { get; }
    public List<KeyMetadata> Keys { get; }
    public List<IndexMetadata> Indexes { get; }
    public List<CheckConstraint> CheckConstraints { get; }
    public DateTime Created { get; }
    public DateTime LastUpdated { get; }
    public bool IsInDefaultInstallation { get; set; }
}