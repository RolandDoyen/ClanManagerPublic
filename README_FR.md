# ClanManager
ClanManager est une application web ASP.NET Core MVC robuste conçue pour la gestion des clans et de leurs membres. Elle intègre une gestion avancée des rôles, une authentification par session et une architecture en couches pour garantir la scalabilité et la maintenabilité.

> **Note :** Ce dépôt public est une version finalisée du projet destinée à mon portfolio. Le développement et les pipelines CI/CD vers Azure sont gérés via un dépôt privé, ce qui explique l'historique simplifié des commits ici.

## 📌 Table des matières
- [ClanManager](#clanmanager)
  - [📌 Table des matières](#-table-des-matières)
  - [🚀 Démo en direct](#-démo-en-direct)
  - [🚀 Stack Technique](#-stack-technique)
  - [✨ Fonctionnalités Clés](#-fonctionnalités-clés)
  - [🏛️ Architecture \& Philosophie](#️-architecture--philosophie)
  - [📂 Structure du projet](#-structure-du-projet)
  - [⚙️ BLL \& Exceptions personnalisées](#️-bll--exceptions-personnalisées)
  - [🔐 Sécurité \& Logs](#-sécurité--logs)
  - [🚀 Déploiement](#-déploiement)
  - [⚙️ Installation \& Configuration locale](#️-installation--configuration-locale)
  - [🚀 Évolutions futures](#-évolutions-futures)


## 🚀 Démo en direct
Le Clan Manager est déployé et accessible ici :
**[👉 ClanManager app sur Azure](https://clanmanager-rd.azurewebsites.net)**


## 🚀 Stack Technique
- **Framework :** ASP.NET Core 8 MVC pour une architecture structurée et modulaire.
- **ORM & Données :** Entity Framework Core avec SQL Server pour la gestion des relations complexes.
- **Mapping :** AutoMapper pour une conversion fluide entre les ViewModels, les DTOs et les Entités.
- **Logging :** Serilog avec rotation quotidienne des fichiers pour un suivi détaillé des erreurs.
- **Auth :** Authentification basée sur les sessions avec hachage des mots de passe via BCrypt.
- **Frontend :** Vues Razor combinées à Bootstrap 5 et CSS3 pour une interface dynamique.
- **DevOps :** GitHub Actions pour les pipelines CI/CD automatisés et le déploiement continu.


## ✨ Fonctionnalités Clés
- **Contrôle d'accès basé sur les rôles :** Vues et permissions distinctes adaptées pour les Chefs de Clan et les Membres.
- **Authentification Sécurisée :** Sécurité basée sur les sessions avec hachage des mots de passe via BCrypt et filtres d'authentification personnalisés.
- **Contenu Dynamique :** Rendu des données en temps réel via les Vues Razor et des composants Bootstrap réactifs.
- **Résilience aux Erreurs :** Gestion globale des exceptions et journalisation détaillée avec Serilog (rotation quotidienne des fichiers).
- **CI/CD Automatisé :** Flux de déploiement en direct garantissant la mise à jour du site via GitHub Actions.


## 🏛️ Architecture & Philosophie
Le projet suit une **Architecture en Couches** stricte afin de découpler la logique métier de la présentation et de l'accès aux données :

```html
Client (Razor Views)
       ↕ Liaison de données / ViewModels
Controller (Couche Web)
       ↕ AutoMapper / Mapping des modèles
BLL (Business Logic Layer)
       ↕ Services, Exceptions, DTOs
DAL (Data Access Layer)
       ↕ DataContext / DAO
Database (SQL Server)
```

- **BLL** : Gère la validation, les règles métier centrales et le mapping des DTO.
- **DAL** : Gère l'accès à la base de données, les migrations et les entités EF Core.
- **UI/UX** : Utilise les Vues Razor et les ViewModels pour maintenir une séparation nette entre les données et la présentation.
- **Base de données** : Instance SQL Server hébergeant les données relationnelles.


## 📂 Structure du projet
La solution est organisée en plusieurs projets pour assurer une séparation stricte des responsabilités :

- **BLL (Business Logic Layer) :**
  - `BLL/` : Services centraux (`UserBLL`, `ClanBLL`).
  - `DTO/` : Objets de transfert de données pour un échange découplé entre les couches DAL, BLL et WEB.
  - `Interfaces/` : Contrats de services (ex : `IMovieRepository.cs`).
  - `Profiles/` : Configurations AutoMapper pour le mapping des DAO vers les DTO.
  - `Seed/` : Logique d'alimentation initiale des données (ex : création du `SuperAdmin` par défaut).
  - `Services/` : Utilitaires de validation transversaux (ex : `ValidationService` pour vérifier l'existence des entités).

- **Core :**
  - `Exceptions/` : Exceptions métier personnalisées (ex : `UserNotFoundException`, `NoSessionUserException`).
  - `Resources/` : Support multilingue (FR/EN).
  - `Enums.cs` : Énumérations partagées.

- **DAL (Data Access Layer) :**
  - `DAO/` : Entités de base de données (User, Clan, Member).
  - `Interfaces/` : Contrats de services (ex : `IUserRepository.cs`).
  - `Migrations/` : Historique des migrations de la base de données.
  - `Repositories/` : Implémentations d'accès aux données Entity Framework Core (communication SQL).
  - `DataContext.cs` : Contexte EF Core et migrations.

- **WEB (Presentation Layer) :**
  - `Attributes/` : Filtres de sécurité personnalisés (`AuthorizeRole`, `CheckBan`).
  - `Controllers/` : Orchestrateurs traitant les requêtes et le TempData.
  - `Logs/` : Fichiers de log pour le suivi des erreurs et l'audit.
  - `Middleware/` : Composants du pipeline HTTP (ex : `ExceptionMiddleware` pour la capture des erreurs techniques globales).
  - `Models/` : ViewModels structurés pour l'affichage UI et la validation des formulaires.
  - `Profiles/` : Configurations AutoMapper pour le mapping des DTO vers les ViewModels.
  - `Services/` : Gestion de session et utilitaires d'infrastructure.
  - `Views/` : Fichiers Razor (.cshtml) pour le rendu de l'interface utilisateur.
  - `appsettings.json` : Configuration (chaînes de connexion, etc.).
  - `Program.cs` : Configuration des services et pipeline de middlewares.

- **Tests (ClanManager.Tests) :**
  - **Integration/** : Validation de l'infrastructure et du pipeline.
    - `Repository` : Tests d'intégration de base de données avec des requêtes SQL réelles.
  - **Unit/** : Validation de la logique isolée.
    - `AutoMapper` : Validation des profils de mapping Entité-vers-DTO.
    - `BLL` : Validation des règles métier et de la logique.
    - `Controller` : Test des actions du contrôleur en simulant (mocking) les services.
    - `Middleware` : Test de la logique de filtrage isolée (logs, assignation TempData) en simulant `ExceptionContext`.


## ⚙️ BLL & Exceptions personnalisées
Au lieu de codes d'erreur génériques, la BLL utilise des exceptions de haut niveau pour garantir la clarté du code :

**Exceptions clés :**
- `UserNotFoundException` / `ClanNotFoundException` : Ressource manquante.
- `UserAlreadyExistsException` : Prévention des doublons.
- `PasswordValidationException` : Application des règles de sécurité des mots de passe.
- `RoleChangeException` : Violation de la hiérarchie ou des règles métier.
- `WrongRoleException` : Tentative d'action non autorisée.

Cela permet aux contrôleurs de capturer des erreurs spécifiques et de fournir un retour utilisateur pertinent via `TempData`.


## 🔐 Sécurité & Logs
- **Attributs personnalisés** :
  - `[SessionAuthorize]` : Vérifie l'existence d'une session valide.
  - `[AuthorizeRole]` : Restreint l'accès selon les rôles système (SuperAdmin, Admin, Utilisateur).
  - `[CheckBan]` : Empêche les utilisateurs bannis d'effectuer des actions.
- **Logging** : Serilog suit toutes les opérations critiques et les erreurs dans le répertoire `Logs/` pour le débogage et l'audit.


## 🚀 Déploiement
- **Plateforme** : Hébergé sur **Azure App Service (Windows/Linux)**.
- **CI/CD** : Déploiement entièrement automatisé via **GitHub Actions** (déclenché au push) pour une intégration continue.


## ⚙️ Installation & Configuration locale
**Prérequis** .NET 8 SDK, SQL Server, EF Core Tools.

1. **Cloner le dépôt :**
  ```bash
  git clone https://github.com/RolandDoyen/ClanManagerPublic.git
  ```

2. **Restaurer les packages NuGet :**
  ```bash
  dotnet restore
  ```

3. **Configurer la base de données :**
Mettez à jour la chaîne `ConnectionStrings` dans `appsettings.json` avec le nom de votre serveur.

4. **Mettre à jour la base de données et lancer :**
  ```bash
  dotnet ef database update --project DAL --startup-project WEB
  dotnet run --project WEB
  ```

Le navigateur s'ouvrira à l'adresse `https://localhost:XXXX`.


## 🚀 Évolutions futures
- **Système de Notifications** : Alertes en temps réel pour les invitations ou exclusions de clan.
- **Améliorations UX** : Ajout de tableaux interactifs et de fenêtres de confirmation pour les actions destructives.
- **Extension API** : Création d'une API REST pour permettre l'intégration d'une application mobile.