using System;
using System.Data;
using Irvin.Parser;

namespace Irvin.SqlParser
{
    public class ModuleParameter : VariableInitializationExpression, IParameterInfo
    {
        public ModuleParameter()
        {
            Direction = ParameterDirection.Input;
        }

        public ParameterDirection Direction { get; set; }

        public override void BuildDataType(TokenCollection tokens, StringComparison compareOption)
        {
            base.BuildDataType(tokens, compareOption);
            SetParameterDirection(this, tokens, compareOption);
        }

        public static void SetParameterDirection<T>(T expression, TokenCollection tokens, StringComparison compareOption)
            where T : SqlExpression, IParameterInfo
        {
            if (tokens.Current.Content.Equals(SqlParserSettings.OutputKeyword, compareOption))
            {
                expression.Direction = ParameterDirection.InputOutput;
                expression.AppendChild(tokens.Current);
                tokens.MoveNext();
            }
        }
    }
}