namespace ProxyServerSearcher.Domain.ValueObjects;

public class ProxyServerCredentials
{
    public string Username { get; set; }

    public string Password { get; set; }
    
    public ProxyServerCredentials(string username, string password)
    {
        Username = username;
        Password = password;
    } 
}