## ClanManager
ClanManager is an ASP.NET Core MVC web application for managing clans and their members, with role management and simple authentication

## Table of Contents
- [ClanManager](#clanmanager)
- [Table of Contents](#table-of-contents)
- [Technologies](#technologies)
- [Architecture](#architecture)
- [BLL Layer](#bll-layer)
- [Main BLL Exceptions](#main-bll-exceptions)
- [DAL Layer](#dal-layer)
- [Installation and Execution](#installation-and-execution)
- [Configuration](#configuration)
- [Project Structure](#project-structure)
- [Features](#features)
- [Error and Log Management](#error-and-log-management)
- [Best Practices and Remarks](#best-practices-and-remarks)
- [Possible Improvements and Evolutions](#possible-improvements-and-evolutions)

## Technologies
- Framework: ASP.NET Core 8 MVC
- ORM: Entity Framework Core
- DTO ↔ ViewModel Mapping: AutoMapper
- Logging: Serilog
- Database: SQL Server
- Authentication and Sessions: Session-based
- Frontend: Razor Views, Bootstrap 5

## Architecture
- The architecture follows the MVC pattern with a BLL (Business Logic Layer) and a DAL (Data Access Layer)

  ```html
  Client (Razor Views)
          ↓
  Controller (Web)
          ↓
  BLL (Services, Exceptions, DTOs)
          ↓
  DAL (Entity Framework DbContext, DAO)
          ↓
  Database (SQL Server)
  ```

## BLL Layer
- Contains the business logic
- Exposes asynchronous methods (CreateAsync, GetByIdAsync, ToggleBanAsync, etc)
- Uses DTOs for communication with controllers
- Manages specific exceptions (UserNotFoundException, UserAlreadyExistsException, RoleChangeException, etc)

## Main BLL Exceptions
1. The main business exceptions of the BLL are:
- `UserNotFoundException`: the requested user does not exist
- `UserAlreadyExistsException`: attempt to create an already existing user
- `PasswordValidationException`: invalid password
- `RoleChangeException`: violation of role change rules
- `ClanNotFoundException`: the requested clan does not exist
- `MemberNotFoundException`: a specific member is not found in a clan
- `WrongRoleException`: the current user does not have the required role for the action

2. Objective of these exceptions
- Encapsulate business rules clearly
- Avoid generic error returns
- Allow controllers to adapt the response (TempData message, redirection, HTTP code, log)
- Facilitate debugging via Serilog

## DAL Layer
- Uses Entity Framework Core
- Contains DataContext and the User, Clan, ClanMember entities

## Installation and Execution
1. Clone the repository:
  ```html
  git clone https://github.com/RolandDoyen/ClanManagement
  cd ClanManager
  ```
  
2. Restore NuGet packages:
  ```html
  dotnet restore
  ```

3. Configure the database in appsettings json:
  ```html
  "ConnectionStrings": {<
    "DefaultConnection": "Server=SERVERNAME;Database=ClanManager;Trusted_Connection=True;TrustServerCertificate=True;"
  }
  ```

4. Apply migrations:
  ```html
  dotnet ef database update
  ```

5. Launch the application:
  ```html
  dotnet run
  ```

6. Access the application:
  ```html
  https://localhost:5001/
  ```

## Configuration
- Logger (Serilog): logs in Logs/log--log with daily rotation Levels: Information (default), Fatal (Microsoft/System)
- Sessions: Timeout 30 min, HTTP only Stores UserId, UserEmail, UserRole
- Localization: EN/FR support, default language EN

## Project Structure
- BLL
  - BLL
    - UserBLL.cs
    - ClanBLL.cs
  - DTO
    - ClanDTO.cs
    - ClanMemberContextDTO.cs
    - ClanMemberDTO.cs
    - ClanUserContextDTO.cs
    - UserContextDTO.cs
    - UserDTO.cs
  - Exceptions
    - PasswordValidationException.cs
    - UserAlreadyExistsException.cs
    - UserNotFoundException.cs
    - RoleChangeException.cs
    - Etc...
  - Interfaces
    - IClanBLL.cs
    - IUserBLL.cs
  - Profiles
    - ClanProfile.cs
    - UserProfile.cs
  - Seed
    - DataSeeder.cs
  - Services
    - DatabaseService.cs
- Core
  - Resources
    - Resources.en.resx
    - Resources.fr.resx
    - Resources.resx
  - Enums.cs
- DAL
  - DAO
    - Clan.cs
    - ClanMember.cs
    - User.cs
  - DataContext.cs
- WEB
  - Attributes
    - AuthorizeRoleAttribute.cs
    - SessionAuthorizeAttribute.cs
    - Etc...
  - Controllers
    - AdminController.cs
    - BaseController.cs
    - ClanController.cs
    - CultureController.cs
    - HomeController.cs
    - UserController.cs
  - Logs
    - Contains logs
  - Models
    - Clan
      - ClanViewModel.cs
      - ClanListViewModel.cs
      - ClanMemberViewModel.cs
    - User
      - UserDetailViewModel.cs
      - UserFormViewModel.cs
      - UserListViewModel.cs
    - ErrorViewModel.cs
  - Profiles
    - ClanProfile.cs
    - UserProfile.cs
  - Services
    - Interfaces
      - ISessionService.cs
    - SessionService.cs
  - Views
    - Clan
      - Create.cshtml
      - Detail.cshtml
      - List.cshtml
    - Home
      - Index.cshtml
    - Shared
      - _LanguageSwitch.cshtml
      - _Layout.cshtml
      - _ValidationScriptsPartial.cshtml
      - Error.cshtml
      - Error404.cshtml
      - Error500.cshtml
    - User
      - Create.cshtml
      - Login.cshtml
      - Detail.cshtml
      - List.cshtml
    - _ViewImports.cshtml
    - _ViewStart.cshtml
  - Program.cs

## Features
1. Users
- Account creation (UserController Create)
- Login / Logout (UserController Login, Logout)
- Role management (ChangeRole)
- (De)ban users (ToggleBan)
- Email and password validation

2. Clans
- Clan creation (ClanController Create)
- List of active or all clans (ClanController List)
- Clan detail (ClanController Detail)
- Join / Leave a clan (Join, Quit)
- Member role management (ChangeMemberRole)
- Activate / deactivate a clan (ToggleActive)
- Clan description modification (UpdateDescription)

3. Security
- Role filtering (AuthorizeRole attribute)
- Session verification (SessionAuthorize attribute, with allowPublic option)
- Ban verification (CheckBan attribute)
- Session-based for authentication
- Password hashing via BCrypt

## Error and Log Management
- Business exceptions captured in the BLL: UserNotFoundException, UserAlreadyExistsException, RoleChangeException, etc
- Controllers handle exceptions to display clear messages or redirect the user
- TempData for post-redirect messages:
  ```html
  TempData["ErrorMessage"] = "User not found"
  ```
- In the views:
  ```html
  @if (TempData["ErrorMessage"] != null)
  {
      <div class="alert alert-danger">@TempData["ErrorMessage"]</div>
  }
  ```

## Best Practices and Remarks
- BLL = business logic + validation + specific exceptions
- Controller = orchestrator (BLL call, DTO ↔ ViewModel mapping, sessions, TempData messages)
- AutoMapper to map DTO ↔ ViewModel and Entity ↔ DTO
- Logger always used for important actions and errors

## Possible Improvements and Evolutions
- Unit Tests: Adding simple unit tests to cover the main cases
- Clan Management: Activity history (join, quit, kick), internal notifications
- User Management: Recent activity, ban/deactivation history
- Interface / UX: Confirmation of destructive actions, notifications, tooltips, interactive tables, responsive design
- Architecture / Code Quality: Centralized permissions, global error system
- Possible Extensions: Internal messaging, events for clans, external REST API, multiples games per clan