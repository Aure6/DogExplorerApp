namespace DogExplorerApp.Models;

public class FavoriteDog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ImageUrl { get; set; }
    public string CustomName { get; set; } = "Mon Chien Favori";
    public string Description { get; set; } = "Ajoutez une description...";
}