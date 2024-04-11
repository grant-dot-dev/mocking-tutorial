using Microsoft.AspNetCore.Mvc;
using Service;

namespace WebApi;

[ApiController]
[Route("[controller]")]
public class TodoController : ControllerBase
{
    private readonly ITodoService _todoService;
    private readonly INotificationService _notificationService;

    public TodoController(ITodoService todoService, INotificationService notificationService)
    {
        _todoService = todoService;
        _notificationService = notificationService;
    }

    [HttpPost("todoitems")]
    public IActionResult AddTodoItem([FromBody] string todoDescription)
    {
        var todo = _todoService.AddTodo(todoDescription);
        return Created($"/todoitems/{todo.Id}", todo);
    }

    [HttpGet("getall")]
    public IActionResult GetAllTodoItems()
    {
        var todos = _todoService.GetAllTodos();
        return Ok(todos);
    }

    [HttpDelete("todoitems/{id}")]
    public IActionResult DeleteTodoItem(int id)
    {
        try
        {
            _todoService.Delete(id);
         
            // hard coded user id for tutorial purposes.
            _notificationService.NotifyUserTaskCompleted(id, 1);
            return Ok();
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
}
