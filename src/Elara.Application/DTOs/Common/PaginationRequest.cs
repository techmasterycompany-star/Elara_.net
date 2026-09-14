namespace Elara.Application.DTOs.Common
{
    public class PaginationRequest
    {
        public int PageNumber { get; set; } = 1;
        public int Limit { get; set; } = 20;
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; } = "asc";
    }
}
