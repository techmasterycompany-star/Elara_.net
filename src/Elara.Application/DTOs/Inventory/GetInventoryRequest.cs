using Elara.Application.DTOs.Common;

namespace Elara.Application.DTOs.Inventory
{
    public class GetInventoryRequest : PaginationRequest
    {
        public string? SearchTerm { get; set; }
        public bool? IsActive { get; set; }
        public long? CategoryId { get; set; }
    }
}
