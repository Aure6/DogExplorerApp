using Moq;
using Xunit;
using Blazored.LocalStorage;
using DogExplorerApp.Services;
using DogExplorerApp.Models;

namespace DogExplorerApp.Tests;

public class LocalAppStateTests
{
    [Fact]
    public async Task LoginAsync_UtilisateurNonInscrit_AfficheErreur()
    {
        // 1. Arrange : On simule un dictionnaire d'utilisateurs VIDE
        var mockLocalStorage = new Mock<ILocalStorageService>();
        mockLocalStorage
            .Setup(ls => ls.GetItemAsync<Dictionary<string, string>>("app_registered_users", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<string, string>());

        var appState = new LocalAppState(mockLocalStorage.Object);

        // 2. Act : On essaie de se connecter avec Alice
        await appState.LoginAsync("Alice", "admin123");

        // 3. Assert : Ça doit échouer et afficher un message d'erreur
        Assert.False(appState.CurrentUser.IsAuthenticated);
        Assert.Equal("Nom d'utilisateur ou mot de passe incorrect.", appState.ErrorMessage);
    }

    [Fact]
    public async Task RegisterAsync_NouvelUtilisateur_ConnecteEtSauvegarde()
    {
        // 1. Arrange
        var mockLocalStorage = new Mock<ILocalStorageService>();

        // Simule qu'il n'y a personne d'inscrit au départ
        mockLocalStorage
            .Setup(ls => ls.GetItemAsync<Dictionary<string, string>>("app_registered_users", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<string, string>());

        // Simule des favoris vides pour la connexion qui suit l'inscription
        mockLocalStorage
            .Setup(ls => ls.GetItemAsync<List<FavoriteDog>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FavoriteDog>());

        var appState = new LocalAppState(mockLocalStorage.Object);

        // 2. Act : On inscrit Bob
        await appState.RegisterAsync("Bob", "supermdp");

        // 3. Assert : Bob doit être connecté
        Assert.True(appState.CurrentUser.IsAuthenticated);
        Assert.Equal("Bob", appState.CurrentUser.Username);
        Assert.Null(appState.ErrorMessage); // Pas d'erreur

        // Vérifie que le dictionnaire des utilisateurs a bien été mis à jour dans le LocalStorage
        mockLocalStorage.Verify(
            ls => ls.SetItemAsync("app_registered_users", It.IsAny<Dictionary<string, string>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task AddFavoriteAsync_AjouteLeChienEtSauvegardeEnLocal()
    {
        // 1. Arrange
        var mockLocalStorage = new Mock<ILocalStorageService>();

        // On calcule le hash mathématique de "admin123" pour le test
        string hashAdmin123;
        using (var sha256 = System.Security.Cryptography.SHA256.Create())
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes("admin123");
            hashAdmin123 = Convert.ToBase64String(sha256.ComputeHash(bytes));
        }

        // On insère ce vrai hash dans notre fausse base de données
        var fakeUsers = new Dictionary<string, string> { { "Alice", hashAdmin123 } };

        mockLocalStorage
            .Setup(ls => ls.GetItemAsync<Dictionary<string, string>>("app_registered_users", It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeUsers);

        mockLocalStorage
            .Setup(ls => ls.GetItemAsync<List<FavoriteDog>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FavoriteDog>());

        var appState = new LocalAppState(mockLocalStorage.Object);

        // On connecte Alice
        await appState.LoginAsync("Alice", "admin123");

        string urlChienTest = "https://images.dog.ceo/breeds/beagle/test.jpg";

        // 2. Act
        await appState.AddFavoriteAsync(urlChienTest);

        // 3. Assert
        Assert.Single(appState.Favorites);
        Assert.Equal(urlChienTest, appState.Favorites[0].ImageUrl);
        Assert.Equal("Beagle", appState.Favorites[0].Breed);

        mockLocalStorage.Verify(
            ls => ls.SetItemAsync("favoris_Alice", appState.Favorites, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}