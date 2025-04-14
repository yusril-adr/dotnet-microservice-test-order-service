using System.ComponentModel.DataAnnotations;

namespace DotNetOrderService.Infrastructure.Dtos
{
    public enum SortOrder
    {
        Asc,
        Desc,
    }

    public class QueryDto
    {
        public QueryDto()
        {
            Order = SortOrder.Desc;
            PerPage = 10;
            Page = 1;
        }

        public string Search { get; set; }

        [Range(1, 100)]
        public int PerPage { get; set; }

        [Range(1, int.MaxValue)]
        public int Page { get; set; }

        public string SortBy { get; set; }

        public SortOrder Order { get; set; }
    }
}
