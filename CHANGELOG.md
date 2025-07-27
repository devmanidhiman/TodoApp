# [](https://github.com/devmanidhiman/TodoApp/compare/v1.2.0...v) (2025-07-27)



# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [v1.2.0] - 2025-07-26

### 🚀 Added
- `status <id>` CLI command to inspect task status and due date
- Support for `--status` flag in `update` command
- `TaskStatus` enum introduced to replace `IsCompleted` boolean
- Structured logging for task status transitions and CLI operations

### 🔧 Changed
- Refactored CLI commands (`complete`, `pending`, `inprogress`) to use `TaskStatus` enum
- Enhanced CLI feedback messages for better user experience
- Improved argument parsing logic to support multi-word inputs

### 🧹 Removed
- Deprecated `IsCompleted` property from `TodoItem.cs`

### 🐞 Fixed
- CLI now handles non-sequential task IDs gracefully
- Prevents misleading success messages when updates fail
- Improved error handling for invalid or missing arguments

---

 ## v1.2.1 — 2025-07-27
 Fix Continuous Display IDs in CLI
🛠️ Fixed
- Resolved issue where display IDs in CLI output were not incrementing correctly across multiple tasks
- Ensured consistent and predictable task indexing for improved UX during task listing
🔍 Notes
- This fix improves clarity when viewing tasks via CLI, especially after multiple additions or deletions
- No breaking changes introduced


## [1.3.0] - 2025-07-27

### ✨ Features
- **logging**: integrate Serilog with console and file sinks

### 📁 Configuration
- Added Serilog configuration to `appsettings.json`
- Enabled rolling file logging to `Logs/log-<date>.txt`
- Console output now includes timestamped, leveled logs

### ✅ Validation
- Confirmed log output in console and file
- Verified structured logging with `Log.Information(...)`


---

## [Unreleased]

### Planned
- Front-end integration using TypeScript
- API endpoint for task status filtering
- Unit tests for CLI command handlers


[1.3.0]: https://github.com/devmanidhiman/TodoApp/compare/v1.2.1...v1.3.0
