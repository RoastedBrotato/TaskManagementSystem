Certainly! Here’s a revised version of your README.md with improved heading hierarchy, proper Markdown bullet points, and enhanced readability. All sections are clearly formatted, code samples are in code blocks, and lists are consistent.

---

# Task Management System

## Overview

This project is a full-stack task management application with role-based access control, built using **.NET 9** for the backend API and **Angular** for the frontend. The system enables administrators to manage users and tasks while providing regular users with a streamlined interface for tracking their assigned tasks.

---

## Table of Contents

- [Project Structure](#project-structure)
- [Technologies Used](#technologies-used)
- [Backend Features](#backend-features)
- [Frontend Features](#frontend-features)
- [API Endpoints](#api-endpoints)
- [Database Design](#database-design)
- [Authentication & Authorization](#authentication--authorization)
- [Getting Started](#getting-started)
- [Testing](#testing)
- [Seeded Test Data](#seeded-test-data)
- [Project Dependencies](#project-dependencies)

---

## Project Structure

The solution is organized into multiple projects following clean architecture principles:

```
TaskManagementSystem/
├── TaskManagementSystem.Api/            # API controllers, middleware, configuration
├── TaskManagementSystem.Core/           # Business logic, entities, interfaces
├── TaskManagementSystem.Infrastructure/ # Data access layer, repositories
├── TaskManagementSystem.Tests/          # Unit tests
└── task-management-client/              # Angular frontend
```

---

## Technologies Used

### Backend

- .NET 9.0
- Entity Framework Core (in-memory database)
- JWT Authentication
- Swagger/OpenAPI
- Clean architecture with repository pattern
- Dependency Injection
- XUnit for testing

### Frontend

- Angular
- Angular Material
- RxJS
- JWT authentication with HTTP interceptors
- Responsive design

---

## Backend Features

- **Authentication:** JWT token-based authentication
- **Role-based access control:** Admin and User roles with different permissions
- **Task Management:** Create, retrieve, update, and delete tasks
- **User Management:** Create, retrieve, update, and delete users
- **In-memory database:** Pre-seeded with sample data
- **Data Transfer Objects (DTOs):** Clean separation between API models and domain entities

---

## Frontend Features

- User-friendly, responsive UI
- Secure login/logout with JWT
- Role-based views and actions
- Task and user management interfaces
- Real-time updates with RxJS

---

## Code Samples

### Program.cs Configuration

```csharp
// Register repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();

// Register services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<IPasswordHashService, PasswordHashService>();
```

### API Controllers with Role-Based Authorization

```csharp
[HttpGet]
[Authorize(Roles = "Admin")]
public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
{
    var users = await _userService.GetAllUsersAsync();
    var userDtos = users.Select(u => new UserDto
    {
        Id = u.Id,
        Username = u.Username,
        Email = u.Email,
        Role = u.Role
    });

    return Ok(userDtos);
}
```

### Task Management with Role Protection

```csharp
[HttpPut("{id}")]
public async Task<IActionResult> UpdateTask(int id, UpdateTaskRequest request)
{
    var existingTask = await _taskService.GetTaskByIdAsync(id);
    if (existingTask == null)
    {
        return NotFound();
    }

    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    var isAdmin = User.IsInRole("Admin");

    // If not admin, can only update status of assigned tasks
    if (!isAdmin)
    {
        if (existingTask.AssignedUserId != userId)
        {
            return Forbid();
        }

        // Only update status
        existingTask.Status = request.Status;
    }
    else
    {
        // Admin can update everything
        existingTask.Title = request.Title;
        existingTask.Description = request.Description;
        existingTask.DueDate = request.DueDate;
        existingTask.Status = request.Status;
        existingTask.AssignedUserId = request.AssignedUserId;
    }

    await _taskService.UpdateTaskAsync(existingTask);
    return NoContent();
}
```

---

## API Endpoints

### Authentication

- `POST /api/auth/login` — Authenticate user and get JWT token

### Users

- `GET /api/users` — Get all users (Admin only)
- `GET /api/users/{id}` — Get user by ID
- `POST /api/users` — Create a new user (Admin only)
- `PUT /api/users/{id}` — Update user (Admin only)
- `DELETE /api/users/{id}` — Delete user (Admin only)

### Tasks

- `GET /api/tasks` — Get all tasks (Admin only)
- `GET /api/tasks/{id}` — Get task by ID (Admin or assigned user)
- `GET /api/tasks/user` — Get current user's tasks
- `POST /api/tasks` — Create a new task (Admin only)
- `PUT /api/tasks/{id}` — Update task (Admin can update all fields; Users can only update status of assigned tasks)
- `PUT /api/tasks/{id}/status` — Update task status (Admin or assigned user)
- `DELETE /api/tasks/{id}` — Delete task (Admin only)

---

## Database Design

The application uses an in-memory database with two main entities:

### User Entity

- **Id**
- **Username**
- **Password** (hashed)
- **Email**
- **Role** (Admin or User)

### TaskItem Entity

- **Id**
- **Title**
- **Description**
- **DueDate**
- **Status** (Pending, InProgress, Completed)
- **AssignedUserId** (foreign key to User)

---

## Authentication & Authorization

- Users log in with username and password
- Successful login returns a JWT token valid for 7 days
- Token contains user ID, username, and role claims
- Token is sent with each request in the `Authorization` header
- Role-based policies restrict access to endpoints based on user role

---

## Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- [Node.js and npm](https://nodejs.org/)
- [Angular CLI](https://angular.io/cli)

### Backend Setup

1. Clone the repository
2. Create an `appsettings.json` file in `TaskManagementSystem.Api` directory:

   ```json
   {
     "Jwt": {
       "Secret": "your-super-secret-key-with-at-least-32-characters"
     },
     "Logging": {
       "LogLevel": {
         "Default": "Information",
         "Microsoft.AspNetCore": "Warning"
       }
     },
     "AllowedHosts": "*"
   }
   ```

3. Build and run the API:

   ```bash
   cd TaskManagementSystem.Api
   dotnet build
   dotnet run
   ```

   The API will be available at [https://localhost:7037](https://localhost:7037) and Swagger docs at [https://localhost:7037/swagger](https://localhost:7037/swagger).

### Frontend Setup

1. Navigate to the frontend directory:

   ```bash
   cd task-management-client
   ```

2. Install dependencies:

   ```bash
   npm install
   ```

3. Update the API URL in `src/environments/environment.ts`:

   ```typescript
   export const environment = {
     production: false,
     apiUrl: 'https://localhost:7037'
   };
   ```

4. Run the Angular application:

   ```bash
   ng serve
   ```

   The frontend will be available at [http://localhost:4200](http://localhost:4200).

---

## Testing

The project includes unit tests for services and controllers.

```bash
cd TaskManagementSystem.Tests
dotnet test
```

---

## Seeded Test Data

When the application starts, the database is seeded with:

**Users:**

1. **Admin user**
   - Username: `admin`
   - Password: `admin123`
   - Role: Admin

2. **Regular user**
   - Username: `user`
   - Password: `user123`
   - Role: User

**Tasks:**

1. **Complete project proposal** (Admin)
2. **Review code changes** (Regular user)
3. **Fix login bug** (Regular user)

---

## Project Dependencies

### TaskManagementSystem.Api

```xml
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="9.0.5" />
<PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="9.0.5" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.5" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="8.1.1" />
```

### TaskManagementSystem.Infrastructure

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.5" />
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="9.0.5" />
```

### TaskManagementSystem.Tests

```xml
<PackageReference Include="coverlet.collector" Version="6.0.2" />
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="9.0.5" />
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.12.0" />
<PackageReference Include="Moq" Version="4.20.72" />
<PackageReference Include="xunit" Version="2.9.2" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.8.2" />
```

---

Feel free to copy and replace your existing README.md with the above improved version! If you’d like further customization (badges, screenshots, etc.), let me know.
