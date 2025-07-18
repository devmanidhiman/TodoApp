using TodoApp.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TodoApp.Core.Interfaces;
using TodoApp.Core.Entities;

class Program
{
    static void Main(string[] args)
    {
        // Create a HostBuilder to set up dependency injection and logging
        var host = Host.CreateDefaultBuilder(args)
        .ConfigureServices((context, services) =>
        { services.AddSingleton<ITodoRepository, FileTodoRepository>(); })
        .ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Information);
        }).Build();
        // Resolve the ITodoRepository from the service provider
        var repo = host.Services.GetRequiredService<ITodoRepository>();
        Console.WriteLine($"Resolved repo type: {repo.GetType().Name}");
        repo.Add(new TodoItem { Id = 1, Title = "Test Add", IsCompleted = false });

        //repo.ClearAll();

    }
}