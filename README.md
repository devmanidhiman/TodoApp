**# 📝 TodoApp (.NET Clean Architecture)**

A simple, scalable To-Do List Console Application built using C# and the Clean Architecture pattern. Designed to demonstrate modular design principles, separation of concerns, and file-based persistence — perfect for learning and job readiness.

---

**## 📦 Project Structure**
TodoApp/ ├── TodoApp.Core/           # Domain models & interfaces 
         ├── TodoApp.Application/    # Business logic (optional for future features) 
         ├── TodoApp.Infrastructure/ # File-based repository (JSON persistence) 
         ├── TodoApp.ConsoleUI/      # Console UI for interaction 
         ├── todoitems.json          # Local data store (auto-generated) 
         └── TodoApp.sln             # Solution fil


---

**## ⚙️ Tech Stack**

- **Language:** C# (.NET 9)
- **Architecture:** Clean Architecture Principles
- **Persistence:** Local JSON file
- **UI:** Console Application
- **Tools:** Visual Studio Code, Git, GitHub

---

**## 🚀 Features**

- [x] Add new todo items
- [x] View all todos
- [x] Get a todo by ID
- [x] Update existing todos
- [x] Delete todos by ID
- [x] Persist data to `todoitems.json`
- [ ] Logging with `ILogger<T>`
- [ ] GUI layer (WinForms/WPF) or Web UI (Blazor) [planned]
- [ ] Unit tests with xUnit/Moq

---

**## 🛠️ Getting Started**

```bash
# Clone the repo
git clone https://github.com/devmanidhiman/TodoApp.git

# Open in VS Code
code TodoApp

# Run the app
cd TodoApp.ConsoleUI
dotnet run

**## 💾 Data Persistence**
All todo items are saved locally in todoitems.json under the /Data folder (or project root).
You can safely exclude it from version control using .gitignore.
To create sample data manually, use:

**🧱 Architecture Style**
This project implements Clean Architecture principles:
- Domain-centric design — business logic isolated in Core
- Loosely coupled layers — infrastructure can be replaced with DB or API easily
- Interface-driven — repositories follow abstraction (ITodoRepository)
- Test-friendly — easy to inject mock dependencies in the future

🧭 Roadmap
- Add logging with ILogger<T>
- Implement file-based backup or timestamped snapshots
- Integrate GUI using WinForms or MAUI
- Migrate from file to database (e.g., SQLite or SQL Server)
- Add xUnit unit tests and coverage reporting

**🙋‍♂️ Author**
Made with 💻 by Dev Dhiman
A software engineer with 3+ years of experience, currently focused on mastering the .NET ecosystem.
📍 Based in Rishikesh, India

