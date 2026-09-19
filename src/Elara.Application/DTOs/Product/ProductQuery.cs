using Elara.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elara.Application.DTOs.Product
{
    public enum ProductSortBy
    {
        Name,
        Price,
        StockQuantity,
        CreatedAt,
        UpdatedAt
    }
    public class ProductQuery : PaginationRequest<ProductSortBy>
    {
        public string? Search { get; set; }
        public long? CategoryId { get; set; }
        public long? SellerProfileId { get; set; }
        public bool? IsActive { get; set; }
    }
}
