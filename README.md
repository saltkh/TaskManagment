# TaskManagment
=======
# Task Management System

A full-featured RESTful API built with **ASP.NET Core 8**, implementing clean architecture with complete CRUD operations for Users, Projects, Tasks, and Comments.

---

## Tech Stack

| Layer | Technology |
|---|---|
| API | ASP.NET Core 8 Web API |
| ORM | Entity Framework Core 8 |
| Database | SQL Server (LocalDB for dev) |
| Validation | FluentValidation |
| Mapping | AutoMapper |
| Testing | xUnit + Moq + FluentAssertions |
| Docs | Swagger / OpenAPI |

---

## Project Structure

```
TaskManagement/
├── src/
│   ├── TaskManagement.Core/           # Entities, DTOs, Interfaces (no dependencies)
│   ├── TaskManagement.Infrastructure/ # EF Core DbContext, Repositories, Migrations
│   ├── TaskManagement.Application/    # Services, Validators, AutoMapper profiles
│   └── TaskManagement.API/            # Controllers, Middleware, Program.cs
└── tests/
    └── TaskManagement.Tests/          # xUnit unit tests for all services
```

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or **SQL Server LocalDB** (included with Visual Studio)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

---

## Getting Started

### 1. Clone the repository

```bash
git clone <your-repo-url>
cd TaskManagement
```

### 2. Configure the database connection

Edit `src/TaskManagement.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TaskManagementDb;Trusted_Connection=True;"
  }
}
```

For a full SQL Server instance, use:
```
Server=localhost;Database=TaskManagementDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True;
```

### 3. Run the application

The app **automatically applies migrations on startup**, so no manual `dotnet ef` commands are needed.

```bash
cd src/TaskManagement.API
dotnet run
```

Alternatively, use the SQL script directly:
```bash
# Run database-setup.sql in SQL Server Management Studio or sqlcmd
sqlcmd -S (localdb)\mssqllocaldb -i database-setup.sql
```

### 4. Open Swagger UI

Navigate to: **https://localhost:PORT/** (the root URL redirects to Swagger)

---

## API Endpoints

### Users
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/users` | Get all users (paginated, filterable) |
| GET | `/api/users/{id}` | Get user by ID |
| POST | `/api/users` | Create user |
| PUT | `/api/users/{id}` | Update user |
| DELETE | `/api/users/{id}` | Delete user |

### Projects
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/projects` | Get all projects (paginated, filterable) |
| GET | `/api/projects/{id}` | Get project by ID |
| POST | `/api/projects` | Create project |
| PUT | `/api/projects/{id}` | Update project |
| DELETE | `/api/projects/{id}` | Delete project |

### Task Items
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/taskitems` | Get all tasks (paginated, filterable by project/user/status) |
| GET | `/api/taskitems/{id}` | Get task by ID |
| POST | `/api/taskitems` | Create task |
| PUT | `/api/taskitems/{id}` | Update task |
| DELETE | `/api/taskitems/{id}` | Delete task |

### Comments
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/comments` | Get all comments (paginated, filterable by task) |
| GET | `/api/comments/{id}` | Get comment by ID |
| POST | `/api/comments` | Create comment |
| PUT | `/api/comments/{id}` | Update comment |
| DELETE | `/api/comments/{id}` | Delete comment |

---

## Query Parameters (all GET list endpoints)

| Parameter | Type | Description | Example |
|-----------|------|-------------|---------|
| `page` | int | Page number (default: 1) | `?page=2` |
| `pageSize` | int | Items per page (default: 10, max: 50) | `?pageSize=20` |
| `sortBy` | string | Field to sort by | `?sortBy=name` |
| `sortDescending` | bool | Sort direction | `?sortDescending=true` |
| `searchName` | string | Filter users/projects by name | `?searchName=alice` |
| `status` | int | Filter tasks by status (0-3) | `?status=1` |
| `projectId` | int | Filter tasks/comments by project | `?projectId=1` |
| `assignedUserId` | int | Filter tasks by assigned user | `?assignedUserId=2` |
| `taskId` | int | Filter comments by task | `?taskId=3` |

**Task Status Values:** `0` = Todo, `1` = InProgress, `2` = Done, `3` = Cancelled

---

## Request / Response Examples

### Create User
```json
POST /api/users
{
  "firstName": "Alice",
  "lastName": "Johnson",
  "email": "alice@example.com"
}
```

### Create Task
```json
POST /api/taskitems
{
  "title": "Build login page",
  "description": "Implement JWT authentication",
  "status": 0,
  "projectId": 1,
  "assignedUserId": 2
}
```

### Paginated Response
```json
{
  "items": [ ... ],
  "totalCount": 42,
  "page": 1,
  "pageSize": 10,
  "totalPages": 5,
  "hasNextPage": true,
  "hasPreviousPage": false
}
```

---

## Running Tests

```bash
cd tests/TaskManagement.Tests
dotnet test
```

Tests use an **in-memory database** — no SQL Server required to run them.

### Test coverage includes:
- ✅ Create: successful creation + invalid data handling
- ✅ Read: existing record + non-existing record (404)
- ✅ Update: successful update + non-existing record
- ✅ Delete: successful delete + non-existing record
- ✅ Filtering, pagination, sorting

---

## Architecture Decisions

- **Repository Pattern** — abstracts data access; services depend on interfaces, not EF Core directly
- **Service Layer** — all business logic lives here; controllers only handle HTTP concerns
- **Global Exception Middleware** — converts `NotFoundException` → 404, `BadRequestException` → 400
- **FluentValidation** — input validation is separate from business logic
- **AutoMapper** — eliminates manual property-by-property mapping between entities and DTOs
- **DTOs** — API never exposes raw entity objects; responses are shaped specifically for the client

---

## Author

Built for the ASP.NET Core Web API assignment.
>>>>>>> dfc5d59 (need work)
