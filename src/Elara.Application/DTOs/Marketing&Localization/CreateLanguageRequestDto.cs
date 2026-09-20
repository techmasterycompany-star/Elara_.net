namespace Elara.Application.DTOs
{
    public class CreateLanguageRequestDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
    }
}