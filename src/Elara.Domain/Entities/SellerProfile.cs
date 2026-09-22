using Elara.Domain.Common;


namespace Elara.Domain.Entities
{
    public class SellerProfile : SoftDelete
    {
        public long UserId { get; set; }
        public User User { get; set; } = null!;

        public string StoreName { get; set; } = null!;
        public string StoreDescription { get; set; } = null!;
        public bool IsApproved { get; set; }

        public ICollection<Product> Products { get; set; } = [];
        public ICollection<Shipment> Shipments { get; set; } = [];
        public ICollection<Payout> Payouts { get; set; } = [];
    }
}
