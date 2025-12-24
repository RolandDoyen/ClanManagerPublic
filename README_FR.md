# ClanManager
ClanManager est une application web ASP.NET Core MVC robuste conçue pour la gestion des clans et de leurs membres. Elle intègre une gestion avancée des rôles, une authentification par session et une architecture en couches pour garantir la scalabilité et la maintenabilité.

> **Note :** Ce dépôt public est une version finalisée du projet destinée à mon portfolio. Le développement et les pipelines CI/CD vers Azure sont gérés via un dépôt privé, ce qui explique l'historique simplifié des commits ici.

## 📌 Table des matières
- [ClanManager](#clanmanager)
  - [📌 Table des matières](#-table-des-matières)
  - [🚀 Stack Technique](#-stack-technique)
  - [🏛️ Architecture \& Philosophie](#️-architecture--philosophie)
  - [📂 Structure du projet](#-structure-du-projet)
  - [⚙️ BLL \& Exceptions personnalisées](#️-bll--exceptions-personnalisées)
  - [✨ Fonctionnalités](#-fonctionnalités)
  - [🔐 Sécurité \& Logs](#-sécurité--logs)
  - [🚀 Déploiement](#-déploiement)
  - [⚙️ Installation \& Configuration locale](#️-installation--configuration-locale)
  - [🚀 Évolutions futures](#-évolutions-futures)


## 🚀 Stack Technique
- **Framework :** ASP.NET Core 8 MVC
- **ORM & Données :** Entity Framework Core avec SQL Server
- **Mapping :** AutoMapper (DTO ↔ ViewModel et Entité ↔ DTO)
- **Logging :** Serilog avec rotation quotidienne des fichiers
- **Authentification :** Basée sur les sessions avec hachage des mots de passe via BCrypt
- **Frontend :** Razor Views, Bootstrap 5, et CSS3


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

- **BLL** : Encapsule toutes les règles métier et la validation. Elle expose des méthodes asynchrones et utilise des exceptions spécifiques pour communiquer avec les contrôleurs.
- **DAL** : Gère la persistance des données en utilisant la logique du pattern Repository via EF Core.


## 📂 Structure du projet
La solution est divisée en quatre projets principaux :

- **BLL (Business Logic Layer)** :
  - `BLL/` : Services de base (UserBLL, ClanBLL).
  - `Exceptions/` : Exceptions métier personnalisées.
  - `Profiles/` : Configurations de mapping pour les DTOs.
- **Core** :
  - `Resources/` : Support multilingue (FR/EN).
  - `Enums.cs` : Énumérations partagées.
- **DAL (Data Access Layer)** :
  - `DAO/` : Entités de la base de données (User, Clan, Member).
  - `DataContext.cs` : Contexte EF Core et migrations.
- **WEB (Presentation Layer)** :
  - `Attributes/` : Filtres de sécurité personnalisés (`AuthorizeRole`, `CheckBan`).
  - `Controllers/` : Orchestrateurs gérant les requêtes et les TempData.
  - `Services/` : Gestion des sessions et utilitaires d'infrastructure.


## ⚙️ BLL & Exceptions personnalisées
Au lieu de codes d'erreur génériques, la BLL utilise des exceptions de haut niveau pour garantir la clarté du code :

**Exceptions clés :**
- `UserNotFoundException` / `ClanNotFoundException` : Ressource manquante.
- `UserAlreadyExistsException` : Prévention des doublons.
- `PasswordValidationException` : Application des règles de sécurité des mots de passe.
- `RoleChangeException` : Violation de la hiérarchie ou des règles métier.
- `WrongRoleException` : Tentative d'action non autorisée.

Cela permet aux contrôleurs de capturer des erreurs spécifiques et de fournir un retour utilisateur pertinent via `TempData`.


## ✨ Fonctionnalités
**👤 Gestion des utilisateurs**
- Création de compte, Connexion/Déconnexion.
- **Outils Admin** : Bannir/Débannir des utilisateurs et gestion globale des rôles.
**🛡️ Gestion des clans**
- Création, liste et consultation des informations détaillées des clans.
- Flux d'adhésion (rejoindre/quitter) avec gestion des membres basée sur les rôles au sein du clan.
- Possibilité d'activer/désactiver des clans et de mettre à jour les descriptions.


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
**Prérequis**
* **SDK .NET 8**
* **SQL Server** (LocalDB ou Express)

**Étapes**
1. **Cloner le dépôt :**
  ```bash
  git clone https://github.com/RolandDoyen/ClanManagerPublic.git
  cd ClanManager
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


## 🚀 Évolutions futures
- **Tests Unitaires** : Implémentation de tests XUnit pour couvrir les services de la BLL.
- **Système de Notifications** : Alertes en temps réel pour les invitations ou exclusions de clan.
- **Améliorations UX** : Ajout de tableaux interactifs et de fenêtres de confirmation pour les actions destructives.
- **Extension API** : Création d'une API REST pour permettre l'intégration d'une application mobile.