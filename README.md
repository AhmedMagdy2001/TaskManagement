# 🗂️ Task Management Backend API (.NET)

A backend Task Management system built using **ASP.NET Core Web API** following a **Clean Architecture (DDD-inspired)** approach.  
The project demonstrates authentication, authorization, caching, background processing, and scalable backend design patterns.

---

# 🚀 Features

## 👤 User Module
- User registration
- User login with JWT authentication
- Retrieve current user profile

## 🛡️ Admin Module
- Seeded default admin user on startup
- Create users (Admin only)
- Delete users (Admin only)
- View all users

## 📌 Task Module
- Create task
- Get task by ID
- Get all tasks
- Update task status
- Ownership-based access control

---

# ⚙️ Technical Highlights

- ASP.NET Core Web API
- Clean Architecture (Domain / Application / Infrastructure / API)
- JWT Authentication & Authorization
- Refresh Token Support
- Redis Caching
- Background Service (Task Processing Queue)
- SQL Server Database
- Swagger / OpenAPI documentation
- Dependency Injection
- Global Exception Handling
- Unit Testing (xUnit)
- Docker Support

---

# 🔐 Authentication Flow

1. User logs in using email & password
2. System returns:
   - JWT Access Token
   - Refresh Token
3. Access token is used for protected endpoints
4. Refresh token is used to generate new access tokens

---

# 👮 Seeded Admin User

On application startup, a default admin user is seeded:

Email: admin@example.com  
Password: Admin@123

---

# 🐳 Running the Project (Docker)

## Start all services:
```bash
docker-compose up --build
```

This will run:
- ASP.NET Core API
- SQL Server
- Redis

---

# 🌐 API Access

Swagger UI:
http://localhost:5000/swagger

---

# 🧊 Redis Caching

- Task details are cached on first request
- Subsequent requests are served from Redis
- Cache is invalidated when task is updated

---

# ⚡ Background Processing

- Tasks are queued upon creation
- A background worker processes tasks asynchronously
- Task status transitions:
  - Pending → InProgress → Done (simulated)

---

# 🧠 Business Rules

- Users can only access their own tasks
- Duplicate tasks (same title on same day) are not allowed
- Only admin can manage users
- Tasks are automatically sorted by priority and creation date

---

# 🧪 Unit Testing

Run tests:
```bash
dotnet test
```

Includes:
- Authentication validation tests
- Business rule tests
- Task status validation tests

---

# 📦 Assumptions

- In-memory queue used for background processing
- Redis used only for caching
- Refresh tokens stored in database

---

# 📁 Project Structure

TaskManagement
├── TaskManagement.API
├── TaskManagement.Application
├── TaskManagement.Domain
├── TaskManagement.Infrastructure
└── TaskManagement.Tests

---

# 👨‍💻 Tech Stack

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Redis
- JWT
- Docker
- xUnit

---

# 🏁 Author

Built as a backend technical assessment project.
