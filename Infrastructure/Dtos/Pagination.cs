
namespace DotNetService.Infrastructure.Dtos
{
    public class PaginationMeta
    {
        public int TotalPage { get; set; }
        public int Total { get; set; }
        public int Page { get; set; }
        public int PerPage { get; set; }
    }

    public class PaginationModel<T>
    {
        public List<T> Data { get; set; }

        public PaginationMeta Meta { get; set; }

        public static PaginationModel<T> Parse(List<T> data, int total, QueryDto query)
        {
            decimal pageInCount = ((decimal)total) / query.PerPage;

            return new PaginationModel<T>
            {
                Data = data,
                Meta = new PaginationMeta
                {
                    TotalPage = (int)Math.Ceiling(pageInCount),
                    Total = total,
                    Page = query.Page,
                    PerPage = query.PerPage,
                }
            };
        }
    }
}