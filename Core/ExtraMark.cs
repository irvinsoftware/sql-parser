using System;
using System.Collections.Generic;
using Irvin.Parser;

namespace Irvin.SqlFountain.Core
{
    public class ExtraMark : SqlExpression
    {
        private Token _token;

        public static implicit operator ExtraMark(Token token)
        {
            return new ExtraMark { _token = token };
        }

        public static implicit operator Token(ExtraMark mark)
        {
            return mark._token;
        }

        public override string ToString()
        {
            return _token.Content;
        }

        public override void AppendChild(Token token)
        {
            throw new NotSupportedException();
        }

        public override void AppendChildren(IEnumerable<Token> tokens)
        {
            throw new NotSupportedException();
        }
    }
}