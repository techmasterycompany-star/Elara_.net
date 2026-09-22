namespace Elara.Domain.Entities
{
    public class Language
    {
        public int Id { get; set; }
        public string Code { get; set; }          // "en", "ar", "fr"
        public string Name { get; set; }          // "English", "Arabic"
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
    }
}
