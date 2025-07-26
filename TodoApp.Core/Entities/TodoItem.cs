namespace TodoApp.Core.Entities;

public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public PriorityLevel? Priority { get; set; }
    public TaskStatus? Status { get; set; } = TaskStatus.Pending;

    public override string ToString()
    {
        string statusSymbol = Status switch
        {
            TaskStatus.Completed => "[✓]",
            TaskStatus.InProgress => "[~]",
            TaskStatus.Pending => "[ ]",
            _ => "[?]"
        };

        string? priority = Priority.HasValue ? Priority.ToString() : "None";
        string due = DueDate.HasValue ? $" — Due: {DueDate.Value.ToShortDateString()}" : "";

        return $"{statusSymbol} {Id}. {Title} ({priority} Priority){due}";
    }

}