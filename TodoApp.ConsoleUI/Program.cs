using TodoApp.Infrastructure.Repositories;
using TodoApp.Core.Entities;
using System;

class Program
{
    static void Main(string[] args)
    {
        var repository = new FileTodoRepository();
        var newTodo = new TodoItem { Id = 1, Title = "Finish .NET tutorial", IsCompleted = false };
        repository.Add(newTodo);
        //Read all todos after adding
        var allTodos = repository.GetAll();
        Console.WriteLine("All Todos after adding a new one:");
        foreach (var todo in allTodos)
        {
            Console.WriteLine($"[{todo.Id}] {todo.Title} - Completed: {todo.IsCompleted}");
        }

        // Get a specific todo by ID
        var todoById = repository.GetTodoById(1);
        if (todoById != null)
        {
            Console.WriteLine($"Retrieved Todo: [{todoById.Id}] {todoById.Title}");
        }
        else
        {
            Console.WriteLine("Todo not found.");
        }

        //Update a todo item
        newTodo.Title = "Finish .NET tutorial - Updated";
        newTodo.IsCompleted = true;
        newTodo.CompletedAt = DateTime.UtcNow; // Set completed time
        repository.Update(newTodo);

        //delete a todo item
        repository.Delete(1);



        //Console.WriteLine("Welcome to the Todo App!");
    }
}