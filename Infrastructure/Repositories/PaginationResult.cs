namespace DotNetService.Infrastructure.Repositories
{
    public class PaginationResult<T>
    {
        public List<T> Data { get; set; }
        public int Count { get; set; }
    }
}