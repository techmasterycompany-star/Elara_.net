namespace Elara.Application.DTOs.Product
{
    public class ProductImageDto
    {
        public long Id { get; set; }
        public string ImageUrl { get; set; } = null!;
        public int DisplayOrder { get; set; }
    }
}
