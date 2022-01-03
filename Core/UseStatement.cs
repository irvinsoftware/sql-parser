namespace Irvin.SqlParser
{
    public class UseStatement : SqlStatement
    {
        public Name DatabaseName { get; set; }
    }
}