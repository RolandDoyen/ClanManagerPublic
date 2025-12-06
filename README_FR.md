## ClanManager
ClanManager est une application web ASP.NET Core MVC permettant de gérer des clans et leurs membres, avec gestion des rôles et authentification simple

## Table des matières
- [ClanManager](#clanmanager)
- [Table des matières](#table-des-matieres)
- [Technologies](#technologies)
- [Architecture](#architecture)
- [Couche BLL](#couche-bll)
- [Exceptions principales de la BLL](#exceptions-principales-de-la-bll)
- [Couche DAL](#couche-dal)
- [Installation et exécution](#installation-et-execution)
- [Configuration](#configuration)
- [Structure du projet](#structure-du-projet)
- [Fonctionnalités](#fonctionnalites)
- [Gestion des erreurs et logs](#gestion-des-erreurs-et-logs)
- [Bonnes pratiques et remarques](#bonnes-pratiques-et-remarques)
- [Améliorations et évolutions possibles](#ameliorations-et-evolutions-possibles)

## Technologies
- Framework : ASP.NET Core 8 MVC
- ORM : Entity Framework Core
- Mapping DTO ↔ ViewModel : AutoMapper
- Logging : Serilog
- Base de données : SQL Server
- Authentification et sessions : Session-based
- Frontend : Razor Views, Bootstrap 5

## Architecture
L’architecture suit le pattern MVC avec une couche BLL (Business Logic Layer) et une DAL (Data Access Layer)

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

## Couche BLL
- Contient la logique métier.
- Expose des méthodes asynchrones (CreateAsync, GetByIdAsync, ToggleBanAsync, etc.)
- Utilise DTOs pour la communication avec les controllers
- Gère les exceptions spécifiques (UserNotFoundException, UserAlreadyExistsException, RoleChangeException, etc.)

## Exceptions principales de la BLL
1. Les exceptions métiers principales de la BLL sont :
- `UserNotFoundException` : l’utilisateur demandé n’existe pas
- `UserAlreadyExistsException` : tentative de création d’un utilisateur déjà existant
- `PasswordValidationException` : mot de passe invalide
- `RoleChangeException` : violation des règles de changement de rôle
- `ClanNotFoundException` : le clan demandé n’existe pas
- `MemberNotFoundException` : un membre spécifique n’est pas trouvé dans un clan
- `WrongRoleException` : l’utilisateur courant n’a pas le rôle requis pour l’action

2. Objectif de ces exceptions
- Encapsuler les règles métier de manière claire.
- Éviter les retours d’erreurs génériques.
- Permettre aux controllers d’adapter la réponse (message TempData, redirection, code HTTP, log).
- Faciliter le débogage via Serilog.

## Couche DAL
- Utilise Entity Framework Core.
- Contient DataContext et les entités User, Clan, ClanMember.

## Installation et exécution
1. Cloner le repository :
```html
git clone https://github.com/RolandDoyen/ClanManagement
cd ClanManager
```

2. Restaurer les packages NuGet :
```html
dotnet restore
```

3. Configurer la base de données dans appsettings.json :
```html
"ConnectionStrings": {
  "DefaultConnection": "Server=SERVERNAME;Database=ClanManager;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

4. Appliquer les migrations :
```html
dotnet ef database update
```

5. Lancer l’application :
```html
dotnet run
```

6. Accéder à l’application :
```html
https://localhost:5001/
```

## Configuration
- Logger (Serilog) : logs dans Logs/log-.log avec rotation journalière. Niveaux : Information (défaut), Fatal (Microsoft/System)
- Sessions : Timeout 30 min, HTTP only. Stocke UserId, UserEmail, UserRole
- Localization : support EN/FR, langue par défaut EN

## Structure du projet
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
    - Contient les logs
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

## Fonctionnalités
1. Utilisateurs
- Création de compte (Create)
- Login / Logout (Login, Logout)
- Gestion des rôles (ChangeRole)
- (Dé)ban d’utilisateurs (ToggleBan)
- Validation des emails et mots de passe

2. Clans
- Création de clan (Create)
- Liste de clans actifs ou tous (List)
- Détail d’un clan (Detail)
- Rejoindre / Quitter un clan (Join, Quit)
- Gestion des rôles des membres (ChangeMemberRole)
- Activer / désactiver un clan (ToggleActive)
- Modification de la description d'un Clan (UpdateDescription)

3. Sécurité
- Filtrage par rôle (AuthorizeRole attribute)
- Vérification des sessions (SessionAuthorize attribute, avec option allowPublic)
- Vérification des bans (CheckBan attribute)
- Session-based pour l’authentification
- Hashing des mots de passe via BCrypt

## Gestion des erreurs et logs
- Exceptions métier capturées dans la BLL : UserNotFoundException, UserAlreadyExistsException, RoleChangeException, etc.
- Controllers gèrent les exceptions pour afficher des messages clairs ou rediriger l’utilisateur
- TempData pour messages post-redirect :
  ```html
  TempData["ErrorMessage"] = "User not found.";
  ```
- Dans les vues :
  ```html
  @if (TempData["ErrorMessage"] != null)
  {
      <div class="alert alert-danger">@TempData["ErrorMessage"]</div>
  }
  ```

## Bonnes pratiques et remarques
- BLL = logique métier + validation + exceptions spécifiques
- Controller = orchestrateur (appel BLL, mapping DTO ↔ ViewModel, sessions, messages TempData)
- AutoMapper pour mapper DTO ↔ ViewModel et Entitiy ↔ DTO
- Logger toujours utilisé pour les actions importantes et les erreurs

## Améliorations et évolutions possibles
- Unit Tests : Ajout de tests unitaires simples pour couvrir les cas principaux
- Gestion des clans : Historique des activités (join, quit, kick), notifications internes
- Gestion des utilisateurs : Activité récente, historique bans/désactivations
- Interface / UX : Confirmation des actions destructrices, notifications, tooltips, tableaux interactifs, responsive design
- Architecture / Qualité du code : Centralisation permissions, système global d’erreurs
- Extensions possibles : Messagerie interne, événements pour clans, API REST externe, multiples jeux par clan