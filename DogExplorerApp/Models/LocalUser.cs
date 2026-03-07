namespace DogExplorerApp.Models;

public class LocalUser
{
    public string Username { get; set; }
    public string Password { get; set; }
    public bool IsAuthenticated { get; set; }
}