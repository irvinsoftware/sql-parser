namespace Irvin.SqlParser
{
    public class SqlComment : SqlExpression
    {
        public string Comment { get; set; }
        public bool UsesMultiLineMarkup { get; set; }
    }
}