namespace Irvin.SqlParser
{
    public class ColumnAddress : ObjectAddress
    {
        public Name ColumnName
        {
            get { return GetChildAtProperty<Name>(nameof(ColumnName)); }
            set { SetChildProperty(nameof(ColumnName), value); }
        }

        public override void AddName(Name name)
        {
            if (ColumnName == null)
            {
                ColumnName = name;
            }
            else 
            {
                if (ObjectName == null)
                {
                    Promote(nameof(ColumnName), nameof(ObjectName), name);
                }
                else
                {
                    base.AddName(name);
                }
            }
        }
    }
}