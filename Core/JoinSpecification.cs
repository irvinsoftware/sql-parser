using System.Collections.Generic;

namespace Irvin.SqlFountain.Core
{
    public class JoinSpecification
        : SqlExpression
    {
        public JoinSpecification()
        {
            Predicates = new List<EqualityExpression>();
        }

        public JoinKind Kind { get; set; }
        public TableReference Source { get; set; }
        public List<EqualityExpression> Predicates { get; set; }
        public bool UsesFullKind { get; set; }
    }
}