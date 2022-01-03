using System.Data;

namespace Irvin.SqlFountain.Core
{
    public interface IParameterInfo
    {
        ParameterDirection Direction { get; set; }
    }
}