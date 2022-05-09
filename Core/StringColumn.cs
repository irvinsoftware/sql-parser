using System;

namespace Irvin.SqlParser
{
    public class StringColumn : Column
    {
        public bool IsFixedWidth
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        
        public ushort MinimumCharacters { get; set; }
        public ushort MaximumCharacters { get; set; }
        public string CollationName { get; set; }
    }
}