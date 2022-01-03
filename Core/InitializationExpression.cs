using System;
using System.Data;
using Irvin.Extensions;
using Irvin.Parser;

namespace Irvin.SqlFountain.Core
{
    public class InitializationExpression : SqlExpression
    {
        public Name Name { get; set; }
        public DataTypeExpression DataType { get; set; }
        public SqlExpression InitialValue { get; set; }

        public void BuildName(TokenCollection tokens)
        {
            Name = Name.CreateFrom(tokens.Current);
            AppendChild(Name);
            tokens.MoveNextSkippingSpaces(Name.AppendChild);
        }

        public virtual void BuildDataType(TokenCollection tokens, StringComparison compareOption)
        {
            DataType = CreateDataTypeExpression(tokens);
            AppendChild(DataType);
        }

        private static DataTypeExpression CreateDataTypeExpression(TokenCollection tokens)
        {
            DataTypeExpression dataTypeExpression = new DataTypeExpression();

            dataTypeExpression.IsNullable = true;
            dataTypeExpression.TypeName = EnumHelper.Parse<SqlDbType>(tokens.Current.Content);
            dataTypeExpression.AppendChild(tokens.Current);
            tokens.MoveNextSkippingSpaces(dataTypeExpression.AppendChild);

            if (tokens.Current.Content.Equals("("))
            {
                dataTypeExpression.AppendChild(tokens.Current);

                tokens.MoveNext();
                if (dataTypeExpression.TypeName == SqlDbType.VarChar)
                {
                    dataTypeExpression.MaximumCharacters = uint.Parse(tokens.Current.Content);
                    dataTypeExpression.AppendChild(tokens.Current);
                }
                else
                {
                    dataTypeExpression.TotalDigits = ushort.Parse(tokens.Current.Content);
                    dataTypeExpression.AppendChild(tokens.Current);
                    tokens.MoveNextSkippingSpaces(dataTypeExpression.AppendChild);
                    dataTypeExpression.AppendChild(tokens.Current); //the comma
                    tokens.MoveNext();
                    dataTypeExpression.TotalFractionalDigits = ushort.Parse(tokens.Current.Content);
                    dataTypeExpression.AppendChild(tokens.Current);
                }

                tokens.MoveNext();
                dataTypeExpression.AppendChild(tokens.Current);
                tokens.MoveNextSkippingSpaces(dataTypeExpression.AppendChild);
            }

            return dataTypeExpression;
        }
    }
}