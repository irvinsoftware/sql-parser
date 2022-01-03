using System.Collections.Generic;

namespace Irvin.SqlParser
{
    public class ProcedureInvocation : SqlStatement
    {
        public ProcedureInvocation()
        {
            Parameters = new List<ParameterInvocation>();    
        }

        public bool UsesLongKeyword { get; set; }
        public ObjectAddress ProcedureName { get; set; }
        public List<ParameterInvocation> Parameters { get; set; }
    }
}