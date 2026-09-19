namespace Elara.Application.DTOs.Product
{
    public class ImageUploadRequest
    {
        public Stream Stream { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public long Length { get; set; }
    }
}
