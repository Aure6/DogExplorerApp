using Blazored.LocalStorage;
using DogExplorerApp.Models;
using System.Security.Cryptography;
using System.Text;

namespace DogExplorerApp.Services;

public class LocalAppState
{
    private readonly ILocalStorageService _localStorage;
    private string GetUserFavoritesKey() => $"favoris_{CurrentUser.Username}";

    public LocalUser CurrentUser { get; private set; } = new LocalUser();
    public List<FavoriteDog> Favorites { get; private set; } = new List<FavoriteDog>();

    public event Action? OnChange;

    // On injecte le service de LocalStorage ici
    public LocalAppState(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    // Clé pour stocker la liste des utilisateurs (Nom d'utilisateur -> Mot de passe)
    private const string UsersKey = "app_registered_users";

    // Pour afficher les erreurs sur la page de connexion
    public string? ErrorMessage { get; private set; }

    public async Task LoginAsync(string? username, string? password)
    {
        ErrorMessage = null;
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ErrorMessage = "Veuillez remplir tous les champs.";
            NotifyStateChanged();
            return;
        }

        // On récupère le dictionnaire des utilisateurs sauvegardés
        var users = await _localStorage.GetItemAsync<Dictionary<string, string>>(UsersKey)
                    ?? new Dictionary<string, string>();

        // On hache le mot de passe tapé pour le comparer
        var hashedInput = HashPassword(password);

        // On compare les deux HASHES
        if (users.TryGetValue(username, out var storedPasswordHash) && storedPasswordHash == hashedInput)
        {
            CurrentUser = new LocalUser { Username = username, IsAuthenticated = true };
            Favorites = await _localStorage.GetItemAsync<List<FavoriteDog>>(GetUserFavoritesKey())
                        ?? new List<FavoriteDog>();
        }
        else
        {
            ErrorMessage = "Nom d'utilisateur ou mot de passe incorrect.";
        }

        NotifyStateChanged();
    }

    public async Task RegisterAsync(string? username, string? password)
    {
        ErrorMessage = null;
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ErrorMessage = "Veuillez remplir tous les champs.";
            NotifyStateChanged();
            return;
        }

        var users = await _localStorage.GetItemAsync<Dictionary<string, string>>(UsersKey)
                    ?? new Dictionary<string, string>();

        // On vérifie si le nom d'utilisateur est déjà pris
        if (users.ContainsKey(username))
        {
            ErrorMessage = "Ce nom d'utilisateur existe déjà.";
            NotifyStateChanged();
        }
        else
        {
            // On hache le mot de passe avant de le sauvegarder.
            users[username] = HashPassword(password);

            await _localStorage.SetItemAsync(UsersKey, users);
            await LoginAsync(username, password); // On passe le mot de passe en clair ici, LoginAsync le hachera
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
            Favorites.Add(new FavoriteDog
            {
                ImageUrl = imageUrl,
                Breed = DogApiService.GetBreedFromUrl(imageUrl)
            });
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
        // On sauvegarde sous la clé SPÉCIFIQUE à l'utilisateur
        if (CurrentUser.IsAuthenticated)
        {
            await _localStorage.SetItemAsync(GetUserFavoritesKey(), Favorites);
        }
    }

    private void NotifyStateChanged() => OnChange?.Invoke();

    private string HashPassword(string password)
    {
        // On crée une instance de l'algorithme SHA256
        using (var sha256 = SHA256.Create())
        {
            // On transforme le texte en un tableau d'octets (bytes)
            var bytes = Encoding.UTF8.GetBytes(password);

            // On calcule le hash
            var hash = sha256.ComputeHash(bytes);

            // On le convertit en une chaîne de texte lisible (Base64) pour le sauvegarder
            return Convert.ToBase64String(hash);
        }
    }
}