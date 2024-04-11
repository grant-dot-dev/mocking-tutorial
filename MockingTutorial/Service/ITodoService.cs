namespace Service;

public interface ITodoService
{
    List<Todo> GetAllTodos();
    Todo AddTodo(string description);

    void Delete(int id);
}
