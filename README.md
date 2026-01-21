# ToDo App - Backend (API)

A robust REST API built with ASP.NET Core 8, providing authentication and task management for the ToDo application.

## ✨ Features
- **Clean Architecture** (Api, Application, Domain, Infrastructure).
- **Identity Framework** for User registration and login.
- **JWT Authentication** for secure access.
- **PostgreSQL** integration using EF Core.
- **Automatic Migrations** via Docker.

## 🛠️ Development Setup

### Prerequisites
- .NET 8 SDK
- PostgreSQL (or Docker)

### Environment Variables
The application uses the following configuration (can be set in `appsettings.json` or Environment Variables):
- `ConnectionStrings:DefaultConnection`: PostgreSQL connection string.
- `Jwt:Key`: Secret key for token signing (min 32 characters).
- `Jwt:Issuer` / `Jwt:Audience`.

### Running Locally
1. Configure your database in `appsettings.json`.
2. Apply migrations:
   ```bash
   dotnet ef database update
Run the app:

Bash
dotnet run --project ToDoApp.Api
📜 API Documentation
When the app is running in Development mode, Swagger UI is available at: http://localhost:7095/swagger
