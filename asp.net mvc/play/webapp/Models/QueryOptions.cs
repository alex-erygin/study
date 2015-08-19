namespace webapp.Models
{
    public class QueryOptions
    {
        public string SortField { get; set; } = "Id";

        public SortOrder SortOrder { get; set; } = SortOrder.Asc;

        public string Sort => $"{SortField} {SortOrder}";

        public int CurrentPage { get; set; } = 1;

        public int TotalPages { get; set; }

        public int PageSize { get; set; } = 1;
    }
}