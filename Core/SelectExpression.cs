using System.Collections.Generic;
using System.Linq;

namespace Irvin.SqlFountain.Core
{
    public class SelectExpression
        : SqlExpression
    {
        public SelectExpression()
        {
            ColumnExpressions = new List<ColumnExpression>();
            WherePredicates = new ChildGroup(this);
            GroupByColumnExpressions = new List<SqlExpression>();
            HavingPredicates = new List<SqlExpression>();
            Joins = new List<JoinSpecification>();
        }

        public List<ColumnExpression> ColumnExpressions { get; }
        public object TopSpecification { get; set; }
        public bool IsDistinct { get; set; }
        public TableReference PrimaryTable { get; set; }
        public List<JoinSpecification> Joins { get; }

        public bool HasWhereClause
        {
            get { return WherePredicates.Any(); }
        }

        public ChildGroup WherePredicates { get; }

        public bool IsAggregateQuery
        {
            get { return GroupByColumnExpressions.Any(); }
        }

        public List<SqlExpression> GroupByColumnExpressions { get; }
        public bool HasHavingClause { get; private set; }
        public List<SqlExpression> HavingPredicates { get; }
        internal const string SelectKeyword = "SELECT";
    }
}