using Moq;
using Xunit;
using Blazored.LocalStorage;
using DogExplorerApp.Services;
using DogExplorerApp.Models;

namespace DogExplorerApp.Tests;

public class LocalAppStateTests
{
    [Fact]
    public async Task LoginAsync_AvecBonsIdentifiants_ConnecteUtilisateur()
    {
        // 1. Arrange (Préparation)
        // On crée un faux LocalStorage qui renvoie une liste vide quand on lui demande les favoris
        var mockLocalStorage = new Mock<ILocalStorageService>();
        // Pour le Setup (Préparation) :
        mockLocalStorage
            .Setup(ls => ls.GetItemAsync<List<FavoriteDog>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FavoriteDog>());

        var appState = new LocalAppState(mockLocalStorage.Object);

        // 2. Act (Action)
        await appState.LoginAsync("Alice", "admin123");

        // 3. Assert (Vérification)
        Assert.True(appState.CurrentUser.IsAuthenticated);
        Assert.Equal("Alice", appState.CurrentUser.Username);
        Assert.Empty(appState.Favorites); // Vérifie que la liste est bien initialisée
    }

    [Fact]
    public async Task LoginAsync_AvecMauvaisMotDePasse_NeConnectePas()
    {
        // 1. Arrange
        var mockLocalStorage = new Mock<ILocalStorageService>();
        var appState = new LocalAppState(mockLocalStorage.Object);

        // 2. Act
        await appState.LoginAsync("Alice", "mauvais_mot_de_passe");

        // 3. Assert
        Assert.False(appState.CurrentUser.IsAuthenticated);
        Assert.Null(appState.CurrentUser.Username);
    }

    [Fact]
    public async Task AddFavoriteAsync_AjouteLeChienEtSauvegardeEnLocal()
    {
        // 1. ARRANGE (Préparation)
        var mockLocalStorage = new Mock<ILocalStorageService>();

        // On utilise It.IsAny<CancellationToken>() ici pour le Setup
        mockLocalStorage
            .Setup(ls => ls.GetItemAsync<List<FavoriteDog>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FavoriteDog>());

        var appState = new LocalAppState(mockLocalStorage.Object);

        // On connecte l'utilisateur
        await appState.LoginAsync("Alice", "admin123");

        string urlChienTest = "https://images.dog.ceo/breeds/beagle/test.jpg";

        // 2. ACT (Action)
        await appState.AddFavoriteAsync(urlChienTest);

        // 3. ASSERT (Vérification)
        Assert.Single(appState.Favorites);
        Assert.Equal(urlChienTest, appState.Favorites[0].ImageUrl);

        // On vérifie que la sauvegarde a bien été déclenchée avec la bonne clé et le CancellationToken
        mockLocalStorage.Verify(
            ls => ls.SetItemAsync("favoris_Alice", appState.Favorites, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}