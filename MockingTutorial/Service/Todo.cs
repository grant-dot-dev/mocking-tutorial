namespace Service;

public class Todo
{
    public int Id { get; set; }
    public string? Description { get; init; }
    public bool IsCompleted { get; set; }
}
