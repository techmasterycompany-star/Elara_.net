using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Application.DTOs.Common
{
    public class PaginatedResponse<T>
    {
        public IEnumerable<T> Data { get; set; } = [];
        public int PageNumber { get; set; }
        public int Limit { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}
