using Elara.Application.DTOs.Common;
using Elara.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Application.DTOs.SellerApplication
{
    public class GetSellerApplicationsRequest : PaginationRequest
    {
        public long SellerId { get; set; }
        public SellerApplicationStatus? Status { get; set; }
    }

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

    public class SellerApplicationDetailsDto
    {
        public long Id { get; set; }

        public long UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;

        public string StoreName { get; set; } = null!;
        public string StoreDescription { get; set; } = null!;

        public SellerApplicationStatus Status { get; set; }
        public string? RejectionReason { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class RejectSellerApplicationDto
    {
        public string RejectionReason { get; set; } = null!;
    }
}
