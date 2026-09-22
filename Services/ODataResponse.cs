using System.Collections.Generic;

namespace PORTAL.Models
{
    public class ODataResponse<T>
    {
        public List<T> Value { get; set; }
    }
}