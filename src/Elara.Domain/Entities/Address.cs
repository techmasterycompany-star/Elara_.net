using Elara.Domain.Common;

namespace Elara.Domain.Entities
{
    public class Address : BaseEntity
    {
        public long UserId { get; set; }
        public User User { get; set; } = null!;

        public string Label { get; set; } = null!;
        public string Street { get; set; } = null!;
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
        public string Country { get; set; } = null!;
        public bool IsDefault { get; set; }
    }
}
