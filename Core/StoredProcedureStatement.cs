using System.Collections.Generic;

namespace Irvin.SqlFountain.Core
{
    public class StoredProcedureStatement : SqlExpression
    {
        public StoredProcedureStatement()
        {
            Parameters = new List<ModuleParameter>();
        }

        public ObjectDefintionAction Action { get; set; }
        public bool UsesFullKeyword { get; set; }

        public Name SchemaName
        {
            get { return GetChildAtProperty<Name>(nameof(SchemaName)); }
            set { SetChildProperty(nameof(SchemaName), value); }
        }

        public Name ProcedureName
        {
            get { return GetChildAtProperty<Name>(nameof(ProcedureName)); }
            set { SetChildProperty(nameof(ProcedureName), value); }
        }

        public List<ModuleParameter> Parameters { get; private set; }
        public bool BodyIsDelimited { get; set; }

        public SqlExpression Body
        {
            get { return GetChildAtProperty<SqlExpression>(nameof(Body)); }
            set { SetChildProperty(nameof(Body), value); }
        }

        public override void AppendChild(SqlExpression sqlExpression)
        {
            if (sqlExpression is ModuleParameter)
            {
                Parameters.Add(sqlExpression as ModuleParameter);
            }

            base.AppendChild(sqlExpression);
        }
    }
}