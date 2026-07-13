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

        // GET /api/tasks
        [HttpGet]
        public IActionResult Get([FromQuery] TaskFilterParams filter)
        {
            var result = _service.GetAll(filter);
            return Ok(result);
        }
    }
}