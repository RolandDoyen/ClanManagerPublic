# ClanManager
ClanManager is a robust web application, powered by **.NET 8**, **Entity Framework Core**, and **AutoMapper**, designed for managing clans and their members. It features advanced role management, session-based authentication, and a layered architecture to ensure scalability and maintainability.

> **Note:** This public repository is a polished version of the project for showcase purposes. Development and automated CI/CD pipelines to Azure are managed through a private repository, which explains the simplified commit history here.

## 📌 Table of Contents
- [ClanManager](#clanmanager)
  - [📌 Table of Contents](#-table-of-contents)
  - [🚀 Live Demo](#-live-demo)
  - [🚀 Tech Stack](#-tech-stack)
  - [✨ Key Features](#-key-features)
  - [🏛️ Architecture \& Philosophy](#️-architecture--philosophy)
  - [📂 Project Structure](#-project-structure)
  - [⚙️ BLL \& Custom Exceptions](#️-bll--custom-exceptions)
  - [🔐 Security \& Logs](#-security--logs)
  - [🚀 Deployment](#-deployment)
  - [⚙️ Installation \& Local Setup](#️-installation--local-setup)
  - [🚀 Future Roadmap](#-future-roadmap)


## 🚀 Live Demo
The Clan Manager is deployed and accessible here:  
**[👉 ClanManager app on Azure](https://clanmanager-rd.azurewebsites.net)**


## 🚀 Tech Stack
- **Framework:** ASP.NET Core 8 MVC for a structured and modular architecture.
- **ORM & Data:** Entity Framework Core with SQL Server for managing complex relationships.
- **Mapping:** AutoMapper for fluid conversion between ViewModels, DTOs, and Entities.
- **Logging:** Serilog with daily file rotation for detailed error tracking.
- **Auth:** Session-based authentication with BCrypt password hashing.
- **Frontend:** Razor Views combined with Bootstrap 5 and CSS3 for a dynamic interface.
- **DevOps:** GitHub Actions for automated CI/CD pipelines and continuous deployment.


## ✨ Key Features
- **Role-Based Access Control:** Distinct views and permissions tailored for Clan Leaders and Members.
- **Secure Authentication:** Session-based security with BCrypt password hashing and custom Auth filters.
- **Dynamic Content:** Real-time data rendering using Razor Views and responsive Bootstrap components.
- **Error Resiliency:** Global exception handling and detailed logging with Serilog (Daily file rotation).
- **Automated CI/CD:** Live deployment workflow ensuring the site is updated via GitHub Actions.


## 🏛️ Architecture & Philosophy
The project follows a strict **Layered Architecture** to decouple business logic from the presentation and data access layers:

```html
Client (Razor Views)
       ↕ Data Binding / ViewModels
Controller (Web Layer)
       ↕ AutoMapper / Model Mapping
BLL (Business Logic Layer)
       ↕ Services, Exceptions, DTOs
DAL (Data Access Layer)
       ↕ DataContext / DAO
Database (SQL Server)
```

- **BLL**: Handles validation, core business rules, and DTO mapping.
- **DAL**: Manages database access, migrations, and EF Core entities.
- **UI/UX:** Leverages Razor Views and ViewModels to maintain a clean separation between data and presentation.
- **Database**: SQL Server instance hosting the relational datas.


## 📂 Project Structure
The solution is organized into multiple projects to ensure a strict separation of concerns:

- **BLL (Business Logic Layer):**
  - `BLL/`: Core services (`UserBLL`, `ClanBLL`).
  - `DTO/`: Data Transfer Objects for decoupled data exchange between DAL, BLL, and WEB layers.
  - `Interfaces/`: Service contracts (e.g., `IMovieRepository.cs`).
  - `Profiles/`: AutoMapper configurations for mapping DAOs to DTOs.
  - `Seed/`: Initial data population logic (e.g., creating the default `SuperAdmin`).
  - `Services/`: Cross-cutting validation helpers (e.g., `ValidationService` for entity existence checks).
  
- **Core:**
  - `Exceptions/`: Custom business exceptions (e.g., `UserNotFoundException`, `NoSessionUserException`).
  - `Resources/`: Multi-language support (FR/EN).
  - `Enums.cs`: Shared enumerations.
  
- **DAL (Data Access Layer):**
  - `DAO/`: Database entities (User, Clan, Member).
  - `Interfaces/`: Service contracts (e.g., `IUserRepository.cs`).
  - `Migrations/`: Database migration history.
  - `Repositories/`: Entity Framework Core data access implementations (SQL database communication).
  - `DataContext.cs`: EF Core context and migrations.
  
- **WEB (Presentation Layer):**
  - `Attributes/`: Custom security filters (`AuthorizeRole`, `CheckBan`).
  - `Controllers/`: Orchestrators handling requests and TempData.
  - `Logs/`: Log files for error tracking and auditing.
  - `Middleware/`: HTTP pipeline components (e.g., `ExceptionMiddleware` for global technical error catching).
  - `Models/`: ViewModels structured for UI display and form validation.
  - `Profiles/`: AutoMapper configurations for mapping DTOs to ViewModels.
  - `Services/`: Session management and infrastructure utilities.
  - `Views/`: Razor (.cshtml) files for user interface rendering.
  - `appsettings.json`: Configuration (Database strings, etc.).
  - `Program.cs`: Service configuration and middleware pipeline.
  
- **Movies.Tests**: 
  - **Integration/**: Infrastructure and pipeline validation.
    - `Repository`: Database integration tests with real SQL queries.
  - **Unit/**: Isolated logic validation.
    - `AutoMapper`: Validation of entity-to-DTO mapping profiles.
    - `BLL`: Business rules and logic validation.
    - `Controller`: Testing controller actions by mocking services.
    - `Middleware`: Testing isolated filter logic (logging, TempData assignment) by mocking `ExceptionContext`.


## ⚙️ BLL & Custom Exceptions
Instead of generic error codes, the BLL uses high-level exceptions to ensure clarity:

**Key Exceptions:**
- `UserNotFoundException` / `ClanNotFoundException`: Resource missing.
- `UserAlreadyExistsException`: Duplicate entry prevention.
- `PasswordValidationException`: Passwords security rule enforcement.
- `RoleChangeException`: Violation of hierarchy or business rules.
- `WrongRoleException`: Unauthorized action attempt.

This allows the Controllers to catch specific errors and provide user-friendly feedback via `TempData`.


## 🔐 Security & Logs
- **Custom Attributes:** 
  - `[SessionAuthorize]`: Ensures a valid session exists.
  - `[AuthorizeRole]`: Restricts access based on system roles (SuperAdmin, Admin, User).
  - `[CheckBan]`: Prevents banned users from performing actions.
- **Logging:** Serilog tracks all critical operations and errors in the `Logs/` directory for debugging and auditing.


## 🚀 Deployment
- **Platform**: Hosted on **Azure App Service (Windows/Linux)**.
- **CI/CD**: Fully automated deployment via **GitHub Actions** (triggered on push) for seamless integration.


## ⚙️ Installation & Local Setup
**Prerequisites:** .NET 8 SDK, SQL Server, EF Core Tools.

1. **Clone the repository:**
  ```bash
  git clone https://github.com/RolandDoyen/ClanManagerPublic.git
  ```

2. **Restore NuGet packages:**
  ```bash
  dotnet restore
  ```

3. **Configure Database:**
Update the `ConnectionStrings` in `appsettings.json` with your server name.

4. **Update Database & Run:**
  ```bash
  dotnet ef database update --project DAL --startup-project WEB
  dotnet run --project WEB
  ```

The browser will open at `https://localhost:XXXX`.


## 🚀 Future Roadmap
- **Notification System:** Real-time alerts for clan invites or kicks.
- **UX Improvements:** Adding interactive tables and confirmation modals for destructive actions.
- **API Extension:** Building a REST API to allow mobile app integration.