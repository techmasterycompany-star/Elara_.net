using Elara.Domain.Entities;
using Elara.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Application.DTOs.SellerApplication
{
    public class SellerApplicationListDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }

        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;

        public string StoreName { get; set; } = null!;
        public SellerApplicationStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }

}
