using Irvin.Extensions;

namespace Irvin.SqlParser
{
    public class NumericColumn : Column
    {
        public UInt8 Precision { get; set; }
        public UInt8 Scale { get; set; }
        public bool IsIdentity { get; set; }
        public long IdentitySeed { get; set; }
        public long IdentityIncrement { get; set; }
    }
}