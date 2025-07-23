using TodoApp.Core.Entities;

namespace TodoApp.Core.Interfaces
{
    /// <summary>
    /// Interface for the Todo repository.
    /// </summary>
    public interface ITodoRepository
    {
        /// <summary>
        /// Gets all todo items.
        /// </summary>
        /// <returns>A list of todo items.</returns>
        IEnumerable<TodoItem> GetAll();

        /// <summary>
        /// Gets a todo item by its ID.
        /// </summary>
        /// <param name="id">The ID of the todo item.</param>
        /// <returns>The todo item with the specified ID.</returns>
        TodoItem? GetTodoById(int id);

        /// <summary>
        /// Adds a new todo item.
        /// </summary>
        /// <param name="item">The todo item to add.</param>
        void Add(TodoItem item);

        /// <summary>
        /// Updates an existing todo item.
        /// </summary>
        /// <param name="item">The todo item to update.</param>
        /// <returns>True if the update was successful; otherwise, false.</returns>
        bool Update(int id, string? newTitle, bool? isCompleted, DateTime? dueDate);

        /// <summary>
        /// Deletes a todo item by its ID.
        /// </summary>
        /// <param name="id">The ID of the todo item to delete.</param>
        /// <returns>True if the deletion was successful; otherwise, false.</returns>
        bool Delete(int id);

        /// <summary>
        /// Clears all todo items.
        /// /// </summary>
        /// <remarks>
        /// This method is used to clear all todo items from the repository.
        void ClearAll();

        /// <summary>
        /// Checks if a todo item exists by its ID.
        /// </summary>
        /// <param name="id">The ID of the todo item.</param>
        /// <returns>True if the todo item exists; otherwise, false.</returns> 
        bool Exists(int id);

        /// <summary>
        /// Gets all completed todo items.
        /// </summary>
        /// <returns>A list of completed todo items.</returns>
        IEnumerable<TodoItem> GetCompletedItems();

        /// <summary>
        /// Marks all incomplete todo items as completed and saves the updated list to persistent storage.
        /// </summary>
        /// <remarks>
        /// This method iterates through all todo items and sets <c>IsCompleted</c> to <c>true</c>
        /// for items that are not yet completed. It then persists the changes using <see cref="Save"/>.
        /// Structured logging is used to record each updated item and a summary of the operation.
        /// </remarks>
        void CheckAll();
    }
}