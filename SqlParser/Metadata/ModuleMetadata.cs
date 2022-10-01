namespace Irvin.SqlParser.Metadata;

public class ModuleMetadata : ISqlObject
{
    public ModuleMetadata()
    {
        Parameters = new List<ParameterMetadata>();
    }
    
    public int ID { get; }
    public string SchemaName { get; set; }
    public string Name { get; set; }
    public bool IsSystemNamed { get; }
    public ModuleKind Kind { get; set; }
    public string ParentTableName { get; set; }
    public string Text { get; set; }
    public List<ParameterMetadata> Parameters { get; }
    public DateTime Created { get; }
    public DateTime LastUpdated { get; }
    public bool IsInDefaultInstallation { get; set; }
    public bool IsEnabled { get; set; }
}