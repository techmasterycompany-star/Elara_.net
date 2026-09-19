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

    public class ProductListDto
    {
        public long Id { get; set; }
        public long SellerProfileId { get; set; }
        public string StoreName { get; set; } = null!;

        public long CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;

        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }

        public string? MainImageUrl { get; set; }
    }

    public class ProductDetailsDto
    {
        public long Id { get; set; }

        public long SellerProfileId { get; set; }
        public string StoreName { get; set; } = null!;

        public long CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;

        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }

        public List<ProductImageDto> Images { get; set; } = [];
    }

    public class ProductImageDto
    {
        public long Id { get; set; }
        public string ImageUrl { get; set; } = null!;
        public int DisplayOrder { get; set; }
    }

    public class AdminCreateProductDto
    {
        public long SellerProfileId { get; set; }
        public long CategoryId { get; set; }

        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;

        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class SellerCreateProductDto
    {
        public long CategoryId { get; set; }

        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;

        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }


    public class AdminUpdateProductDto
    {
        public long SellerProfileId { get; set; }
        public long CategoryId { get; set; }

        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;

        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }

    public class SellerUpdateProductDto
    {
        public long CategoryId { get; set; }

        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;

        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }

    public class UpdateProductStatusDto
    {
        public bool IsActive { get; set; }
    }

    public class ReorderProductImagesDto
    {
        public List<ImageDisplayOrderDto> Images { get; set; } = [];
    }

    public class ImageDisplayOrderDto
    {
        public long ImageId { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class CloudinaryUploadResult
    {
        public string Url { get; set; } = null!;
        public string PublicId { get; set; } = null!;
    }

    public class ImageUploadRequest
    {
        public Stream Stream { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public long Length { get; set; }
    }

    public class UpdateImageDisplayOrderDto
    {
        public int DisplayOrder { get; set; }
    }
}
