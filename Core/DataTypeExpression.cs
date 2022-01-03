using System.Data;

namespace Irvin.SqlFountain.Core
{
    public class DataTypeExpression : SqlExpression
    {
        public SqlDbType TypeName { get; set; }
        public bool IsQuoted { get; set; }

        public int? MinimumCharacters
        {
            get
            {
                switch (TypeName)
                {
                    case SqlDbType.VarChar:
                        return 0;
                }
                return null;
            }
        }

        public uint? MaximumCharacters { get; set; }
        public bool IsNullable { get; set; }

        public ushort? Precision
        {
            get { return TotalDigits; }
            set { TotalDigits = value; }
        }

        public ushort? TotalDigits { get; set; }

        public ushort? Scale
        {
            get { return TotalFractionalDigits; }
            set { TotalFractionalDigits = value; }
        }

        public ushort? TotalFractionalDigits { get; set; }

        public ushort? TotalIntegralDigits
        {
            get
            {
                if (TypeName == SqlDbType.Decimal)
                {
                    return (ushort)(TotalDigits - TotalFractionalDigits);
                }
                return null;
            }
        }
    }
}