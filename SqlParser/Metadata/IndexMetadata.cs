using System.Collections.Generic;
using Irvin.Extensions;

namespace Irvin.SqlParser.Metadata
{
    public class IndexMetadata
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public IndexStructureKind Structure { get; set; }
        public List<string> CoveredColumns { get; set; }
        public List<string> IncludedColumns { get; set; }
        public bool IsUnique { get; set; }
        public bool IgnoreDuplicates { get; set; }
        public string FilterExpression { get; set; }
        public UInt8 FillFactor { get; set; }
        public bool IsEnabled { get; set; }
    }
}