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
}