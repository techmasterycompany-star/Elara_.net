namespace Elara.Application.DTOs.Common
{
    public class PaginationRequest<TSortBy>
    {
        public int PageNumber { get; set; } = 1;
        public int Limit { get; set; } = 20;
        public TSortBy? SortBy { get; set; }
        public SortOrderEnum? SortOrder { get; set; } = SortOrderEnum.Asc;
    }

    public class PaginationRequest : PaginationRequest<string> { }

    public enum SortOrderEnum
    {
        Asc,
        Desc
    }
}
