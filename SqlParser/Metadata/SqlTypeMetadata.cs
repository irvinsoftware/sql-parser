using System.Data;
using Irvin.Extensions;

namespace Irvin.SqlParser.Metadata;

public class SqlTypeMetadata
{
    public string SchemaName { get; set; }
    public string Name { get; set; }
    public SqlDbType BaseType { get; set; }
    public bool IsNullable { get; set; }
    public UInt8? Precision { get; set; }
    public UInt8? Scale { get; set; }
    public ushort? MaximumLength { get; set; }
}