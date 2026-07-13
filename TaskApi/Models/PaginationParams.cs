namespace TaskApi.Models
{
    public class PaginationParams
    {
        public int Page { get; set; } = 1;

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > 100 ? 100 : value; // Caps page size at 100
        }
    }
}