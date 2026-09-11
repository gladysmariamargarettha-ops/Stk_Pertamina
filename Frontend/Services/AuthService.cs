using System.Net.Http.Json;
using Frontend.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace Frontend.Services;

public class AuthResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Menggantikan alur POST ke vallogin.php dan logout.php.
/// </summary>
public class AuthService
{
    private readonly HttpClient _http;
    private readonly CustomAuthStateProvider _authStateProvider;

    public AuthService(HttpClient http, AuthenticationStateProvider authStateProvider)
    {
        _http = http;
        // Kita tahu instance sebenarnya adalah CustomAuthStateProvider karena didaftarkan sebagai itu di Program.cs.
        _authStateProvider = (CustomAuthStateProvider)authStateProvider;
    }

    public async Task<AuthResult> LoginAsync(string username, string password)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", new LoginRequest
        {
            Username = username,
            Password = password,
        });

        if (!response.IsSuccessStatusCode)
        {
            // Setara pesan error PHP: "Invalid email address/password..." / "email Anda tidak terdaftar..."
            string message = response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                ? "Username/password salah, atau email Anda tidak terdaftar di RU II Dumai."
                : "Login gagal. Silakan coba lagi.";
            return new AuthResult { Success = false, ErrorMessage = message };
        }

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        if (result is null || string.IsNullOrEmpty(result.Token))
        {
            return new AuthResult { Success = false, ErrorMessage = "Respons server tidak valid." };
        }

        await _authStateProvider.MarkUserAsLoggedInAsync(result.Token);
        return new AuthResult { Success = true };
    }

    public async Task LogoutAsync()
    {
        try
        {
            await _http.PostAsync("api/auth/logout", null);
        }
        catch
        {
            // Tetap lanjutkan logout di sisi client meskipun panggilan API gagal (mis. token sudah kadaluarsa).
        }
        await _authStateProvider.MarkUserAsLoggedOutAsync();
    }
}
