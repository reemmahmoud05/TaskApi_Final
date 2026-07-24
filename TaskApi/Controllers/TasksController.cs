using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskApi.Models;
using TaskApi.Services;

namespace TaskApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly TaskService _service;

        // The application injects the registered service here
        public TasksController(TaskService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retrieves a paginated list of task items based on search filters.
        /// </summary>
        /// <param name="filter">Optional query parameters to filter, sort, or paginate tasks.</param>
        /// <response code="200">The filtered list of tasks was successfully retrieved.</response>
        /// <response code="400">The provided filter parameters are invalid.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get([FromQuery] TaskFilterParams filter)
        {
            // Maps your custom filter object to the asynchronous database service parameters
            var result = await _service.GetAllAsync(
                filter.Search,
                filter.IsCompleted,
                filter.Page,
                filter.PageSize
            );
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TaskItem), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var task = await _service.GetByIdAsync(id);
            return task == null ? NotFound() : Ok(task);
        }

        [HttpPost]
        [ProducesResponseType(typeof(TaskItem), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] TaskItem task)
        {
            var createdTask = await _service.CreateAsync(task);
            return CreatedAtAction(nameof(GetById), new { id = createdTask.Id }, createdTask);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TaskItem), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] TaskItem updatedTask)
        {
            var result = await _service.UpdateAsync(id, updatedTask);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            return !success ? NotFound() : NoContent();
        }
    }
}