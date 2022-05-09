namespace Irvin.SqlParser
{
    public class Schema
    {
        public string Name { get; set; }

        public TabularObject FindOrCreateObject(string objectKind, string objectName)
        {
            throw new System.NotImplementedException();
        }
    }
}