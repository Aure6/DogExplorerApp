# 🐶 Dog Explorer App

Une application web interactive développée en **Blazor** permettant de découvrir des photos aléatoires de chiens via l'API publique Dog CEO, de créer un compte local et de gérer sa propre galerie de favoris.

## ✨ Fonctionnalités Principales

- **Découverte aléatoire** : Affichage de photos de chiens depuis l'API publique [Dog CEO](https://dog.ceo/dog-api/).
- **Authentification Locale Sécurisée** : Système de création de compte et de connexion. Les données sont stockées dans le navigateur (`LocalStorage`) et les mots de passe sont sécurisés via un **hachage SHA-256**.
- **Gestion des Favoris** : Les utilisateurs connectés peuvent :
  - Ajouter une photo de chien à leurs favoris.
  - Personnaliser le nom du chien et ajouter une description.
  - Supprimer un favori.
- **Extraction de la Race** : L'application analyse automatiquement l'URL de l'image pour extraire, formater et afficher la race du chien.
- **Interface Responsive** : Design moderne et adaptatif utilisant **Bootstrap** et **Bootstrap Icons**.

## 🛠️ Technologies Utilisées

- **Framework** : .NET 8 / Blazor
- **Langage** : C#
- **Stockage** : `Blazored.LocalStorage`
- **Style** : CSS, Bootstrap 5
- **Tests** : `xUnit`, `bUnit` (pour les composants UI) et `Moq` (pour simuler les services).

## 📂 Architecture du Projet

La solution est divisée en deux projets distincts pour séparer le code de production de sa validation :

- **`DogExplorerApp/`** (Projet Principal)
  - `Models/` : Les structures de données (`LocalUser`, `FavoriteDog`).
  - `Services/` : La logique métier (`DogApiService` pour l'API HTTP, `LocalAppState` pour la gestion de l'état, l'authentification et le hachage).
  - `Pages/` : Les vues de l'application (`Index.razor`, `Favorites.razor`).
  - `wwwroot/` : Les fichiers statiques (CSS, images).
- **`DogExplorerApp.Tests/`** (Projet de Tests)
  - Contient les tests unitaires et les tests d'intégration des composants pour garantir la fiabilité de l'application.

## 🚀 Comment lancer le projet

### Prérequis

- [SDK .NET](https://dotnet.microsoft.com/download) (version 8.0 ou supérieure) installé sur votre machine.

### Exécution de l'application

1. Ouvrez un terminal dans le dossier contenant la solution.
2. Déplacez-vous dans le dossier de l'application principale :
   ```bash
   cd DogExplorerApp
   ```
3. Lancez l'application :
   ```bash
   dotnet run
   ```
4. Ouvrez votre navigateur et accédez à l'URL indiquée dans le terminal.
