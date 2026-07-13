using System;

namespace TaskApi.Models
{
    public class TaskFilterParams : PaginationParams
    {
        public string? Search { get; set; }
        public bool? IsCompleted { get; set; }
        public string? SortBy { get; set; }
        public bool Descending { get; set; }

        // Bonus fields (Optional date filters)
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
    }
}
