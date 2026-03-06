using Blazored.LocalStorage;
using DogExplorerApp.Models;

namespace DogExplorerApp.Services;

public class LocalAppState
{
    private readonly ILocalStorageService _localStorage;
    private const string FavoritesKey = "mes_chiens_favoris";

    public LocalUser CurrentUser { get; private set; } = new LocalUser();
    public List<FavoriteDog> Favorites { get; private set; } = new List<FavoriteDog>();

    public event Action? OnChange;

    // On injecte le service de LocalStorage ici
    public LocalAppState(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task LoginAsync(string? username, string? password)
    {
        if (!string.IsNullOrWhiteSpace(username) && password == "admin123")
        {
            CurrentUser = new LocalUser { Username = username, IsAuthenticated = true };

            // On charge les favoris depuis le stockage local lors de la connexion !
            Favorites = await _localStorage.GetItemAsync<List<FavoriteDog>>(FavoritesKey)
                        ?? new List<FavoriteDog>();

            NotifyStateChanged();
        }
    }

    public void Logout()
    {
        CurrentUser = new LocalUser();
        Favorites.Clear();
        NotifyStateChanged();
    }

    public async Task AddFavoriteAsync(string imageUrl)
    {
        if (!Favorites.Any(f => f.ImageUrl == imageUrl))
        {
            Favorites.Add(new FavoriteDog { ImageUrl = imageUrl });
            await SaveFavoritesAsync();
            NotifyStateChanged();
        }
    }

    public async Task RemoveFavoriteAsync(Guid id)
    {
        var dog = Favorites.FirstOrDefault(f => f.Id == id);
        if (dog != null)
        {
            Favorites.Remove(dog);
            await SaveFavoritesAsync();
            NotifyStateChanged();
        }
    }

    public async Task UpdateFavoriteAsync(FavoriteDog updatedDog)
    {
        var index = Favorites.FindIndex(f => f.Id == updatedDog.Id);
        if (index != -1)
        {
            Favorites[index].CustomName = updatedDog.CustomName;
            Favorites[index].Description = updatedDog.Description;
            await SaveFavoritesAsync();
            NotifyStateChanged();
        }
    }

    // Méthode privée pour sauvegarder la liste actuelle dans le navigateur
    private async Task SaveFavoritesAsync()
    {
        await _localStorage.SetItemAsync(FavoritesKey, Favorites);
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}