namespace Service;

public class TodoService : ITodoService
{
    private List<Todo> _todos = [];
    private int _nextId = 1;

    public List<Todo> GetAllTodos()
    {
        return _todos;
    }

    public Todo AddTodo(string description)
    {
        var todo = new Todo { Id = _nextId++, Description = description, IsCompleted = false };
        _todos.Add(todo);

        return todo;
    }

    public void Delete(int id)
    {
        try
        {
            _todos.RemoveAll(x => x.Id == id);
        }
        catch (Exception)
        {
            throw new Exception("Failed to remove the todo task, please try again");
        }
    }
}
