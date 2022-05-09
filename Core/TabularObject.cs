namespace Irvin.SqlParser
{
    public abstract class TabularObject
    {
        public int ObjectId { get; set; }

        public Column AddColumn(string columnName, string sqlTypeName)
        {
            throw new System.NotImplementedException();
        }
    }
}