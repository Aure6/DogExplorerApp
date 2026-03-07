using Bunit;
using Xunit;
using Moq;
using Microsoft.Extensions.DependencyInjection;
using Blazored.LocalStorage;
using DogExplorerApp.Pages;
using DogExplorerApp.Services;

namespace DogExplorerApp.Tests;

// On hérite de TestContext (fourni par bUnit)
public class IndexTests : TestContext
{
    [Fact]
    public void Index_NonAuthentifie_AfficheEcranConnexion()
    {
        // 1. Arrange : Préparation des services simulés
        var mockLocalStorage = new Mock<ILocalStorageService>();
        var appState = new LocalAppState(mockLocalStorage.Object);

        // On enregistre nos services dans le contexte de test bUnit
        Services.AddSingleton(appState);
        Services.AddSingleton(new DogApiService(new HttpClient())); // Faux client HTTP

        // 2. Act : On fait le rendu du composant (la page Index)
        var cut = Render<DogExplorerApp.Pages.Index>();

        // 3. Assert : On vérifie ce qui est généré en HTML
        // On s'assure que le titre "Connexion Locale" est bien présent dans le HTML
        Assert.Contains("Connexion Locale", cut.Markup);

        // On peut aussi vérifier qu'un élément spécifique existe
        var loginButton = cut.Find("button.btn-primary");
        Assert.Equal("Se connecter", loginButton.TextContent);
    }
}