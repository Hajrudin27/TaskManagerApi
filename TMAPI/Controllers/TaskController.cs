using Microsoft.AspNetCore.Mvc;
using TMAPI.Models;

namespace TMAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private static readonly List<TaskItem> _tasks = new();
    private static int _nextId = 1;

    [HttpGet]
    public ActionResult<IEnumerable<TaskItem>> GetAll()
        => Ok(_tasks.OrderByDescending(t => t.CreatedAtUtc));


    [HttpGet("{id:int}")]
    public ActionResult<TaskItem> GetByID(int id)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);
        return task is null ? NotFound() : Ok(task);
    }

    [HttpPost]
    public ActionResult<TaskItem> Create([FromQuery] string title) {
        if (string.IsNullOrWhiteSpace(title))
            return BadRequest("Title cannot be empty.");

        var task = new TaskItem
        {
            Id = _nextId++,
            Title = title.Trim(),
            IsDone = false,
        };
        _tasks.Add(task);
        return CreatedAtAction(nameof(GetByID), new { id = task.Id }, task);
    }

    [HttpPut("{id:int}")]
    public ActionResult<TaskItem> Update(int id, [FromQuery] string title, [FromQuery] bool isDone)
    {
        if (string.IsNullOrWhiteSpace(title))
            return BadRequest("Title cannot be empty.");

        var task = _tasks.FirstOrDefault(t => t.Id == id);
        if (task is null)
            return NotFound();

        task.Title = title.Trim();
        task.IsDone = isDone;
        return Ok(task);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);
        if (task is null)
            return NotFound();

        _tasks.Remove(task);
        return NoContent();
    }
}