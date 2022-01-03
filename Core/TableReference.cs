using System.Collections.Generic;

namespace Irvin.SqlFountain.Core
{
    public class TableReference
        : SqlExpression
    {
        public TableReference()
        {
            TableHints = new List<object>();
        }

        public ObjectAddress Name
        {
            get { return GetChildAtProperty<ObjectAddress>(nameof(Name)); }
            set { SetChildProperty(nameof(Name), value); }
        }

        public List<object> TableHints { get; set; }

        public Name Alias { get; set; }
    }
}