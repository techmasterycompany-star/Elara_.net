using Elara.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Application.DTOs.SellerProfile
{
    public class SellerProfileDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }

        public string StoreName { get; set; } = null!;
        public string StoreDescription { get; set; } = null!;

        public bool IsApproved { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class SellerProductDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string? MainImageUrl { get; set; }
    }

    public class GetSellerProductsRequest : PaginationRequest<SellerProductsSortBy>
    {
        public string? Search { get; set; }
        public bool? HasStock { get; set; }
    }

    public enum SellerProductsSortBy
    {
        Name,
        Price
    }
}
