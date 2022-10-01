using System;

namespace Irvin.SqlParser.Metadata
{
    public class DatabaseMetadata
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public string CollationName { get; set; }
        public bool IsReadOnly { get; set; }
        public DatabaseState State { get; set; }
    }
}