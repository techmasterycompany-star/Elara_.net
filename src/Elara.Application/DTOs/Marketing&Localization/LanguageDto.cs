namespace Elara.Application.DTOs
{
    public class LanguageDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
    }
}