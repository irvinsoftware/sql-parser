using System.Collections.Generic;
using Irvin.Parser;

namespace Irvin.SqlParser
{
    public class FunctionInvocation : SqlExpression
    {
        public FunctionInvocation(TokenCollection fromTokens)
        {
            Parameters = new List<ModuleParameter>();

            Name objectName = Name.CreateFrom(fromTokens.Current);
            FunctionName = new ObjectAddress(objectName);
        }

        public ObjectAddress FunctionName
        {
            get { return GetChildAtProperty<ObjectAddress>(nameof(FunctionName)); }
            set { SetChildProperty(nameof(FunctionName), value); }
        }

        public List<ModuleParameter> Parameters { get; }

        public void AddParameter(ModuleParameter moduleParameter)
        {
            AppendChild(moduleParameter);
            Parameters.Add(moduleParameter);
        }
    }
}