using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Irvin.SqlFountain.Core
{
    public class ChildGroup : IEnumerable<SqlExpression>
    {
        private readonly SqlExpression _parent;
        private readonly List<int> _members;

        public ChildGroup(SqlExpression parent)
        {
            _parent = parent;
            _members = new List<int>();
        }
        
        public void Add(SqlExpression child)
        {
            _parent.AppendChild(child);
            _members.Add(_parent.LastIndex);
        }

        public int Count
        {
            get { return _members.Count; }
        }

        public IEnumerator<SqlExpression> GetEnumerator()
        {
            ReadOnlyCollectionBuilder<SqlExpression> builder = new ReadOnlyCollectionBuilder<SqlExpression>();
            foreach (int memberIndex in _members)
            {
                builder.Add(_parent.Children[memberIndex]);
            }
            return builder.ToReadOnlyCollection().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}