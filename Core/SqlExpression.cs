using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Irvin.Extensions.Collections;
using Irvin.Parser;

namespace Irvin.SqlParser
{
    public class SqlExpression
    {
        private List<SqlExpression> _children;
        private readonly Dictionary<string, int> _propertyMappings;

        public SqlExpression()
        {
            _children = new List<SqlExpression>();
            _propertyMappings = new Dictionary<string, int>();
        }

        public ReadOnlyCollection<SqlExpression> Children
        {
            get { return new ReadOnlyCollection<SqlExpression>(_children); }
        }

        protected internal int LastIndex
        {
            get { return _children.LastIndex(); }
        }

        public void InsertChildren(List<Token> tokens)
        {
            List<SqlExpression> newChildren = new List<SqlExpression>();
            AppendTokens(newChildren, tokens);
            newChildren.AddRange(_children);
            _children = newChildren;

            string[] propertyNames = new string[_propertyMappings.Keys.Count];
            _propertyMappings.Keys.CopyTo(propertyNames, 0);
            foreach (string propertyName in propertyNames)
            {
                _propertyMappings[propertyName] += tokens.Count;
            }
        }

        public virtual void AppendChild(SqlExpression sqlExpression)
        {
            _children.Add(sqlExpression);
        }

        public virtual void AppendChild(Token token)
        {
            _children.Add((ExtraMark)token);
        }

        public virtual void AppendChildren(IEnumerable<Token> tokens)
        {
            AppendTokens(_children, tokens);
        }

        private static void AppendTokens(List<SqlExpression> target, IEnumerable<Token> tokens)
        {
            if (tokens != null)
            {
                foreach (Token token in tokens)
                {
                    target.Add((ExtraMark) token);
                }
            }
        }

        internal void AppendUntil(TokenCollection tokens, Func<Token, bool> endCondition)
        {
            AppendChildren(tokens.MoveUntil(endCondition));
        }

        public override string ToString()
        {
            return string.Join("", Children);
        }

        protected T GetChildAtProperty<T>(string propertyName)
            where T : SqlExpression
        {
            int? index = GetPropertyChildIndex(propertyName);
            return index.HasValue ? (T) _children[index.Value] : null;
        }

        protected int? GetPropertyChildIndex(string propertyName) 
        {
            return _propertyMappings.ContainsKey(propertyName)
                ? (int?) _propertyMappings[propertyName]
                : null;
        }

        protected void SetChildProperty(string propertyName, SqlExpression value)
        {
            if (_propertyMappings.ContainsKey(propertyName))
            {
                int index = _propertyMappings[propertyName];
                _children[index] = value;
            }
            else
            {
                AppendChild(value);
                _propertyMappings.Add(propertyName, LastIndex);
            }
        }

        protected void SetPropertyChildIndex(string propertyName, int? index)
        {
            if(index.HasValue)
            {
                _propertyMappings[propertyName] = index.Value;
            }
            else
            {
                _propertyMappings.Remove(propertyName);
            }
        }
    }
}