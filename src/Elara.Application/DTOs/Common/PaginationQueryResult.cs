namespace Elara.Application.DTOs.Common
{
    public class PaginationQueryResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
    }
}
