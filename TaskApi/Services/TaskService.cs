using System;
using System.Collections.Generic;
using System.Linq;
using TaskApi.Models;

namespace TaskApi.Services
{
    public class TaskService
    {
        // Mock data to simulate records inside a database
        private readonly List<TaskItem> _tasks = new List<TaskItem>
        {
            new TaskItem { Id = 1, Title = "Morning Team Meeting", IsCompleted = true, CreatedDate = DateTime.UtcNow.AddDays(-5) },
            new TaskItem { Id = 2, Title = "Review Project Requirements", IsCompleted = false, CreatedDate = DateTime.UtcNow.AddDays(-4) },
            new TaskItem { Id = 3, Title = "Write Unit Tests", IsCompleted = false, CreatedDate = DateTime.UtcNow.AddDays(-3) },
            new TaskItem { Id = 4, Title = "Fix API Pagination Bug", IsCompleted = true, CreatedDate = DateTime.UtcNow.AddDays(-2) },
            new TaskItem { Id = 5, Title = "Deploy to Production", IsCompleted = false, CreatedDate = DateTime.UtcNow.AddDays(-1) },
            new TaskItem { Id = 6, Title = "Code Review Checklist", IsCompleted = false, CreatedDate = DateTime.UtcNow }
        };

        public PagedResult<TaskItem> GetAll(TaskFilterParams filter)
        {
            var query = _tasks.AsQueryable();

            // 1. SEARCHING (Step 10)
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(t => t.Title.Contains(filter.Search, StringComparison.OrdinalIgnoreCase));
            }

            // 2. FILTERING (Step 11)
            if (filter.IsCompleted.HasValue)
            {
                query = query.Where(t => t.IsCompleted == filter.IsCompleted.Value);
            }

            // BONUS: DATE RANGE FILTERING (Step 12)
            if (filter.CreatedAfter.HasValue)
            {
                query = query.Where(t => t.CreatedDate >= filter.CreatedAfter.Value);
            }
            if (filter.CreatedBefore.HasValue)
            {
                query = query.Where(t => t.CreatedDate <= filter.CreatedBefore.Value);
            }

            // 3. SORTING WITH A DICTIONARY WHITELIST (Step 13)
            var sortOptions = new Dictionary<string, Func<TaskItem, object>>
            {
                { "title", t => t.Title },
                { "createddate", t => t.CreatedDate },
                { "iscompleted", t => t.IsCompleted },
                { "id", t => t.Id }
            };

            // Normalize layout string casing and fallback to "title" if input is unknown
            string sortByLower = (filter.SortBy ?? "title").ToLower();
            if (!sortOptions.ContainsKey(sortByLower))
            {
                sortByLower = "title";
            }

            query = filter.Descending
                ? query.OrderByDescending(sortOptions[sortByLower]).AsQueryable()
                : query.OrderBy(sortOptions[sortByLower]).AsQueryable();

            // 4. PAGINATION CALCULATIONS (Step 14 & 15)
            int totalCount = query.Count();

            var pagedItems = query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            int totalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize);

            return new PagedResult<TaskItem>
            {
                Items = pagedItems,
                TotalCount = totalCount,
                TotalPages = totalPages == 0 ? 1 : totalPages,
                HasNextPage = filter.Page < totalPages,
                HasPreviousPage = filter.Page > 1
            };
        }
    }
}