namespace Irvin.SqlParser.Metadata
{
    public class ColumnMetadata : SqlScalar
    {
        public int ID { get; set; }
        public string CollationName { get; set; }
        public DefaultConstraintMetadata DefaultConstraint { get; set; }
        public SqlSequential Identity { get; set; }
        public string ComputationExpression { get; set; }
    }
}