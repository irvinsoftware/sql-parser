using System;

namespace Irvin.SqlParser.Metadata
{
    public interface ISqlObject
    {
        int ID { get; }
        string SchemaName { get; set; }
        string Name { get; }
        bool IsSystemNamed { get; }
        DateTime Created { get; }
        DateTime LastUpdated { get; }
        public bool IsInDefaultInstallation { get; set; }
    }
}