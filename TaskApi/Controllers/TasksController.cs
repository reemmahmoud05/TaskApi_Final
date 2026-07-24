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
        [ProducesResponseType(typeof(PagedResult<TaskItem>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public IActionResult Get([FromQuery] TaskFilterParams filter)
        {
            var result = _service.GetAll(filter);
            return Ok(result);
        }
    }
}