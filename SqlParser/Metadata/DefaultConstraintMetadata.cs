using System;

namespace Irvin.SqlParser.Metadata
{
    public class DefaultConstraintMetadata : ISqlObject
    {
        public int ID { get; }
        public string SchemaName { get; set; }
        public string Name { get; }
        public bool IsSystemNamed { get; }
        public string ColumnName { get; set; }
        public object Value { get; set; }
        public DateTime Created { get; }
        public DateTime LastUpdated { get; }
        public bool IsInDefaultInstallation { get; set; }
    }
}