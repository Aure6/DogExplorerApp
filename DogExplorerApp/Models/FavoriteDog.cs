namespace DogExplorerApp.Models;

public class FavoriteDog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ImageUrl { get; set; }

    public string Breed { get; set; } = string.Empty;
    public string CustomName { get; set; } = "Mon chien favori";
    public string Description { get; set; } = "Ajoutez une description...";
}