namespace Elara.Domain.Entities
{
    public class ResourceString
    {
        public int Id { get; set; }
        public string Key { get; set; }           // "order.confirmed.subject"
        public int LanguageId { get; set; }
        public string Value { get; set; }

        public Language Language { get; set; }
    }
}
