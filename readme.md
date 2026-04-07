# Task Management System API

A robust Task Management Backend built with **.NET 9** following **Clean Architecture** principles. This system includes JWT authentication, Redis caching for performance, and asynchronous background processing.

##  Features
- **Clean Architecture:** Separated into Domain, Application, Infrastructure, and API layers.
- **Authentication:** Secure JWT-based auth with Role-based access (Admin/User).
- **Redis Caching:** Cache-Aside pattern implemented for Task retrieval to reduce DB load.
- **Background Processing:** Asynchronous task verification using `BackgroundService` and `ConcurrentQueue`.
- **Database:** Entity Framework Core with SQL Server.

##  Setup Instructions
1. **Prerequisites:** - .NET 9 SDK
   - SQL Server (LocalDB or Express)
   - Redis Server (Running on localhost:6379)
2. **Database Migration:**
   - Update the connection string in `appsettings.json` if necessary.
   - Run the following command in the Package Manager Console:
     ```powershell
     Update-Database
     ```
3. **Run the Project:**
   - Set `TaskManagement.Api` as the Startup Project.
   - Press `F5` or run `dotnet run`.

##  Seeded Admin Credentials
- **Email:** admin@example.com
- **Password:** Admin@123

##  Architecture & Assumptions
- **Dependency Inversion:** Used interfaces in the Domain layer to decouple Application logic from Infrastructure (preventing circular dependencies).
- **Security:** Passwords are salted and hashed using **BCrypt**.
- **Performance:** Assumed Redis is available locally to handle caching for `GET` requests.
- **Background Worker:** Designed to simulate post-creation processing (e.g., verification) without blocking the user's request.