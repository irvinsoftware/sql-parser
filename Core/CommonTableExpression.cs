using System.Collections.Generic;

namespace Irvin.SqlFountain.Core
{
    public class CommonTableExpression 
        : SqlExpression
    {
        public CommonTableExpression()
        {
            Body = new List<SqlExpression>();
        }

        public Name Name
        {
            get { return GetChildAtProperty<Name>(nameof(Name)); }
            set { SetChildProperty(nameof(Name), value); }
        }

        public bool IsRecursive { get; set; }

        public List<SqlExpression> Body { get; }

        public void AddBodyPart(SelectExpression part)
        {
            AppendChild(part);
            Body.Add(part);
        }
    }
}