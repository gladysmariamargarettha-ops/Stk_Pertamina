namespace Frontend.Models;

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string EmpName { get; set; } = string.Empty;
    public string Nopek { get; set; } = string.Empty;
    public int UserType { get; set; }
}

public class MenuItem
{
    public string Menu { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
}
