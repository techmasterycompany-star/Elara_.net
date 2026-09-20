namespace Elara.Application.DTOs
{
    public class UpsertResourceStringRequestDto
    {
        public string Key { get; set; } = string.Empty;
        public int LanguageId { get; set; }
        public string Value { get; set; } = string.Empty;
    }
}