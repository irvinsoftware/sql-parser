using System;

namespace Irvin.SqlParser
{
    public class ObjectAddress
        : SqlExpression
    {
        public ObjectAddress()
        {
            
        }

        public ObjectAddress(Name objectName)
        {
            ObjectName = objectName;
        }

        public ObjectAddress(Name schemaName, Name objectName)
        {
            SchemaName = schemaName;
            ObjectName = objectName;
        }

        public Name DatabaseName
        {
            get { return GetChildAtProperty<Name>(nameof(DatabaseName)); }
            set { SetChildProperty(nameof(DatabaseName), value); }
        }

        public Name SchemaName
        {
            get { return GetChildAtProperty<Name>(nameof(SchemaName)); }
            set { SetChildProperty(nameof(SchemaName), value); }
        }

        public Name ObjectName
        {
            get { return GetChildAtProperty<Name>(nameof(ObjectName)); }
            set { SetChildProperty(nameof(ObjectName), value); }
        }

        public virtual void AddName(Name name)
        {
            if (ObjectName == null)
            {
                ObjectName = name;
            }
            else if (ObjectName != null && SchemaName == null)
            {
                Promote(nameof(ObjectName), nameof(SchemaName), name);
            }
            else if (ObjectName != null && SchemaName != null && DatabaseName == null)
            {
                throw new NotImplementedException();
            }
        }

        protected void Promote(string currentTailName, string promoteTo, Name newTailValue)
        {
            Promote(currentTailName, promoteTo);
            AppendChild(newTailValue);
            SetPropertyChildIndex(currentTailName, LastIndex);
        }

        protected void Promote(string currentTailName, string promoteTo)
        {
            int? currentTailIndex = GetPropertyChildIndex(currentTailName);
            SetPropertyChildIndex(promoteTo, currentTailIndex.Value);
            SetPropertyChildIndex(currentTailName, null);
        }
    }
}