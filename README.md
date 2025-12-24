# ClanManager
ClanManager is a robust ASP.NET Core MVC web application designed for managing clans and their members. It features advanced role management, session-based authentication, and a layered architecture to ensure scalability and maintainability.

> **Note:** This public repository is a polished version of the project for showcase purposes. Development and automated CI/CD pipelines to Azure are managed through a private repository, which explains the simplified commit history here.

## 📌 Table of Contents
- [ClanManager](#clanmanager)
  - [📌 Table of Contents](#-table-of-contents)
  - [🚀 Tech Stack](#-tech-stack)
  - [🏛️ Architecture \& Philosophy](#️-architecture--philosophy)
  - [📂 Project Structure](#-project-structure)
  - [⚙️ BLL \& Custom Exceptions](#️-bll--custom-exceptions)
  - [✨ Features](#-features)
  - [🔐 Security \& Logs](#-security--logs)
  - [🚀 Deployment](#-deployment)
  - [⚙️ Installation \& Local Setup](#️-installation--local-setup)
  - [🚀 Future Roadmap](#-future-roadmap)


## 🚀 Tech Stack
- **Framework:** ASP.NET Core 8 MVC
- **ORM & Data:** Entity Framework Core with SQL Server
- **Mapping:** AutoMapper (DTO ↔ ViewModel and Entity ↔ DTO)
- **Logging:** Serilog with daily file rotation
- **Auth:** Session-based authentication with BCrypt password hashing
- **Frontend:** Razor Views, Bootstrap 5, and CSS3


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

- **BLL:** Encapsulates all business rules and validation. It exposes asynchronous methods and uses specific exceptions to communicate with the controllers.
- **DAL:** Manages database persistence using the Repository pattern logic via EF Core.


## 📂 Project Structure
The solution is divided into four main projects:

- **BLL (Business Logic Layer):**
  - `BLL/`: Core services (UserBLL, ClanBLL).
  - `Exceptions/`: Custom business exceptions.
  - `Profiles/`: Mapping configurations for DTOs.
- **Core:**
  - `Resources/`: Multi-language support (FR/EN).
  - `Enums.cs`: Shared enumerations.
- **DAL (Data Access Layer):**
  - `DAO/`: Database entities (User, Clan, Member).
  - `DataContext.cs`: EF Core context and migrations.
- **WEB (Presentation Layer):**
  - `Attributes/`: Custom security filters (`AuthorizeRole`, `CheckBan`).
  - `Controllers/`: Orchestrators handling requests and TempData.
  - `Services/`: Session management and infrastructure utilities.


## ⚙️ BLL & Custom Exceptions
Instead of generic error codes, the BLL uses high-level exceptions to ensure clarity:

**Key Exceptions:**
- `UserNotFoundException` / `ClanNotFoundException`: Resource missing.
- `UserAlreadyExistsException`: Duplicate entry prevention.
- `PasswordValidationException`: Passwords security rule enforcement.
- `RoleChangeException`: Violation of hierarchy or business rules.
- `WrongRoleException`: Unauthorized action attempt.

This allows the Controllers to catch specific errors and provide user-friendly feedback via `TempData`.


## ✨ Features
**👤 User Management** 
- Account creation, Login/Logout.
- **Admin Tools:** Ban/Unban users and global role management.
**🛡️ Clan Management**
- Create, list, and view detailed clan informations.
- Join/Leave workflows with role-based member management within the clan.
- Ability to activate/deactivate clans and update metadata.


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
**Prerequisites**
* **.NET 8 SDK**
* **SQL Server** (LocalDB or Express)

**Steps**
1. **Clone the repository:**
  ```bash
  git clone https://github.com/RolandDoyen/ClanManagerPublic.git
  cd ClanManager
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


## 🚀 Future Roadmap
- **Unit Testing:** Implementing XUnit tests for BLL service coverage.
- **Notification System:** Real-time alerts for clan invites or kicks.
- **UX Improvements:** Adding interactive tables and confirmation modals for destructive actions.
- **API Extension:** Building a REST API to allow mobile app integration.