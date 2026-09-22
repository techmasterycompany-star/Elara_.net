namespace Elara.Application.DTOs.Checkout
{
    public class ShippingAddressDto
    {
        public string FullName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Street { get; set; } = null!;
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
        public string Country { get; set; } = null!;
    }
}
