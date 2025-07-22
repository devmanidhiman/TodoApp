using System;
public static class TodoValidator
{
    public static bool IsValid(string? title, DateTime? dueDate, out string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            errorMessage = "Title Cannot be empty.";
            return false;
        }

        if (dueDate.HasValue && dueDate.Value.Date < DateTime.Today)
        {
            errorMessage = "Due Date cannot be in the past.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }
}