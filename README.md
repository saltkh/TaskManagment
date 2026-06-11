
---

# Task Management System
---

## გამოყენებული ტექნოლოგიები

| ფენა         | ტექნოლოგია                     |
| ------------ | ------------------------------ |
| API          | ASP.NET Core 8 Web API         |
| ORM          | Entity Framework Core 8        |
| ბაზა         | SQL Server (LocalDB)           |
| ვალიდაცია    | FluentValidation               |
| Mapping      | AutoMapper                     |
| ტესტირება    | xUnit + Moq + FluentAssertions |
| დოკუმენტაცია | Swagger / OpenAPI              |

---

## პროექტის სტრუქტურა

```
TaskManagement/
├── src/
│   ├── TaskManagement.Core/           # Entity-ები, DTO-ები, ინტერფეისები
│   ├── TaskManagement.Infrastructure/ # DbContext, Repository, Migrations
│   ├── TaskManagement.Application/    # სერვისები, ვალიდაცია, mapping
│   └── TaskManagement.API/            # Controllers, Middleware, Program.cs
└── tests/
    └── TaskManagement.Tests/          # Unit ტესტები (xUnit)
```

---

## მოთხოვნები

* .NET 8 SDK
* SQL Server ან SQL Server LocalDB
* Visual Studio 2022 ან VS Code

---

## გაშვების ინსტრუქცია

### 1. პროექტის კლონირება

```bash
git clone <your-repo-url>
cd TaskManagement
```

---

### 2. მონაცემთა ბაზის კონფიგურაცია

`src/TaskManagement.API/appsettings.json` ფაილში:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TaskManagementDb;Trusted_Connection=True;"
  }
}
```

თუ იყენებ SQL Server-ს:

```
Server=localhost;Database=TaskManagementDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True;
```

---

### 3. პროექტის გაშვება

```bash
cd src/TaskManagement.API
dotnet run
```

პროექტი ავტომატურად აკეთებს migration-ს გაშვებისას.

---

### 4. Swagger

ბრაუზერში გადადი:

```
https://localhost:PORT/
```

(ავტომატურად გახსნის Swagger UI-ს)

---

## API ენდფოინთები

### Users

* GET `/api/users`
* GET `/api/users/{id}`
* POST `/api/users`
* PUT `/api/users/{id}`
* DELETE `/api/users/{id}`

### Projects

* GET `/api/projects`
* GET `/api/projects/{id}`
* POST `/api/projects`
* PUT `/api/projects/{id}`
* DELETE `/api/projects/{id}`

### Tasks

* GET `/api/taskitems`
* GET `/api/taskitems/{id}`
* POST `/api/taskitems`
* PUT `/api/taskitems/{id}`
* DELETE `/api/taskitems/{id}`

### Comments

* GET `/api/comments`
* GET `/api/comments/{id}`
* POST `/api/comments`
* PUT `/api/comments/{id}`
* DELETE `/api/comments/{id}`

---

## ფილტრაცია და პარამეტრები (GET მოთხოვნებისთვის)

* `page` — გვერდი
* `pageSize` — ელემენტების რაოდენობა
* `sortBy` — დალაგება
* `sortDescending` — descending სორტირება
* `searchName` — ძებნა სახელით
* `status` — task სტატუსი (0-3)
* `projectId` — პროექტის ID
* `assignedUserId` — დავალებული user ID
* `taskId` — task ID (comments)

### Task სტატუსები:

* 0 = Todo
* 1 = InProgress
* 2 = Done
* 3 = Cancelled

---

## მაგალითები

### User შექმნა

```json
POST /api/users
{
  "firstName": "Alice",
  "lastName": "Johnson",
  "email": "alice@example.com"
}
```

### Task შექმნა

```json
POST /api/taskitems
{
  "title": "Login page",
  "description": "JWT authentication",
  "status": 0,
  "projectId": 1,
  "assignedUserId": 2
}
```

---

## ტესტების გაშვება

```bash
cd tests/TaskManagement.Tests
dotnet test
```

ტესტები იყენებს in-memory database-ს, ამიტომ SQL Server არ არის საჭირო.

---

## არქიტექტურა

* Repository Pattern — მონაცემების იზოლაცია
* Service Layer — ბიზნეს ლოგიკა
* Middleware — error handling (404, 400)
* FluentValidation — input validation
* AutoMapper — mapping DTO ↔ Entity
* DTO-ები — API არ აბრუნებს პირდაპირ Entity-ს

---
