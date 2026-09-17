using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Application.DTOs.ShippingMethods
{
    public class CreateShippingMethodDto
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal BaseCost { get; set; }
        public int EstimatedDays { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
