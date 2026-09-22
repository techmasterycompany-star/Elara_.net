using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Application.DTOs.Category
{
    public class CreateCategoryDto
    {
        public string Name { get; set; } = null!;
        public long? ParentCategoryId { get; set; }
        public string Description { get; set; } = null!;
    }
}
