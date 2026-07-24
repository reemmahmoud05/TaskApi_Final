using Microsoft.EntityFrameworkCore;
using TaskApi.Data;
using TaskApi.Models;

namespace TaskApi.Services
{
    public class TaskService
    {
        private readonly AppDbContext _context;

        public TaskService(AppDbContext context)
        {
            _context = context;
        }

        // 1. CREATE (Async)
        public async Task<TaskItem> CreateAsync(TaskItem task)
        {
            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();
            return task;
        }
        // 2. READ ALL (With Search, Filter, and Pagination)
        public async Task<object> GetAllAsync(string? search, bool? completed, int page = 1, int pageSize = 5)
        {
            // Start with a composable IQueryable query expression
            var query = _context.Tasks.AsQueryable();

            // Step 11: Case-insensitive Title Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(t => t.Title.ToLower().Contains(search.ToLower()));
            }

            // Step 12: Completion Status Filter
            if (completed.HasValue)
            {
                query = query.Where(t => t.IsCompleted == completed.Value);
            }

            // Step 13: Calculate metadata counts and paginate at the database level
            var totalCount = await query.CountAsync();

            var tasks = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Return the clean paginated structure
            return new
            {
                totalCount,
                items = tasks
            };
        }
        // 3. READ BY ID (Async)
        public async Task<TaskItem?> GetByIdAsync(int id)
        {
            return await _context.Tasks.FindAsync(id);
        }

        // 4. UPDATE (Async)
        public async Task<TaskItem?> UpdateAsync(int id, TaskItem updatedTask)
        {
            var existingTask = await _context.Tasks.FindAsync(id);
            if (existingTask == null) return null;

            existingTask.Title = updatedTask.Title;
            existingTask.Description = updatedTask.Description;
            existingTask.IsCompleted = updatedTask.IsCompleted;

            _context.Tasks.Update(existingTask);
            await _context.SaveChangesAsync();
            return existingTask;
        }

        // 5. DELETE (Async)
        public async Task<bool> DeleteAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return false;

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}