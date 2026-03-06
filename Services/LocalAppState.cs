using DogExplorerApp.Models;

namespace DogExplorerApp.Services;

public class LocalAppState
{
    public LocalUser CurrentUser { get; private set; } = new LocalUser();
    public List<FavoriteDog> Favorites { get; private set; } = new List<FavoriteDog>();

    public event Action OnChange; // Pour rafraîchir l'UI quand l'état change

    public void Login(string username, string password)
    {
        // Authentification simulée 100% locale
        if (!string.IsNullOrWhiteSpace(username) && password == "admin123")
        {
            CurrentUser = new LocalUser { Username = username, IsAuthenticated = true };
            NotifyStateChanged();
        }
    }

    public void Logout()
    {
        CurrentUser = new LocalUser();
        Favorites.Clear(); // Vider les favoris de la session
        NotifyStateChanged();
    }

    public void AddFavorite(string imageUrl)
    {
        if (!Favorites.Any(f => f.ImageUrl == imageUrl))
        {
            Favorites.Add(new FavoriteDog { ImageUrl = imageUrl });
            NotifyStateChanged();
        }
    }

    public void RemoveFavorite(Guid id)
    {
        var dog = Favorites.FirstOrDefault(f => f.Id == id);
        if (dog != null)
        {
            Favorites.Remove(dog);
            NotifyStateChanged();
        }
    }

    public void UpdateFavorite(FavoriteDog updatedDog)
    {
        var index = Favorites.FindIndex(f => f.Id == updatedDog.Id);
        if (index != -1)
        {
            Favorites[index].CustomName = updatedDog.CustomName;
            Favorites[index].Description = updatedDog.Description;
            NotifyStateChanged();
        }
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}