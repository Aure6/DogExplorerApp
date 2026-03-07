using System.Net.Http.Json;
using DogExplorerApp.Models;

namespace DogExplorerApp.Services;

public class DogApiService
{
    private readonly HttpClient _http;

    public DogApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<string> GetRandomDogAsync()
    {
        var response = await _http.GetFromJsonAsync<DogApiResponse>("https://dog.ceo/api/breeds/image/random");
        return response?.Message;
    }

    public static string GetBreedFromUrl(string? url)
    {
        if (string.IsNullOrEmpty(url)) return "Inconnu";

        try
        {
            // On coupe l'URL à chaque barre oblique '/'
            // Exemple : https://images.dog.ceo/breeds/bulldog-french/image.jpg
            var parts = url.Split('/');

            // La race se trouve toujours à la 5ème position (index 4)
            var breedPart = parts[4];

            // On remplace les tirets par des espaces (bulldog-french -> bulldog french)
            breedPart = breedPart.Replace("-", " ");

            // On met la première lettre en majuscule (Bulldog french)
            return char.ToUpper(breedPart[0]) + breedPart.Substring(1);
        }
        catch
        {
            return "Race inconnue";
        }
    }
}