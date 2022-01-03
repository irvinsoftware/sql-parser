using System.Data;

namespace Irvin.SqlParser
{
    public interface IParameterInfo
    {
        ParameterDirection Direction { get; set; }
    }
}