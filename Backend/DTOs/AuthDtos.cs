namespace Backend.DTOs;

public class LoginRequestDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string EmpName { get; set; } = string.Empty;
    public string Nopek { get; set; } = string.Empty;
    public int UserType { get; set; }
}
