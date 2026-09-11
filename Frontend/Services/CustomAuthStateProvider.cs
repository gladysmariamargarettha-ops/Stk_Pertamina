using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace Frontend.Services;

/// <summary>
/// Menggantikan pengecekan $_SESSION['USR_TYP'] dkk pada PHP.
/// Token JWT disimpan di localStorage browser, klaimnya dibaca untuk membangun
/// ClaimsPrincipal yang dipakai oleh &lt;AuthorizeView&gt; / [Authorize] di Blazor.
/// </summary>
public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private const string TokenKey = "stk_auth_token";
    private readonly IJSRuntime _js;
    private readonly HttpClient _http;

    public CustomAuthStateProvider(IJSRuntime js, HttpClient http)
    {
        _js = js;
        _http = http;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await GetTokenAsync();
        var identity = new ClaimsIdentity();
        _http.DefaultRequestHeaders.Authorization = null;

        if (!string.IsNullOrWhiteSpace(token))
        {
            try
            {
                var claims = ParseClaimsFromJwt(token);
                identity = new ClaimsIdentity(claims, "jwt");
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            catch
            {
                // Token rusak/kadaluarsa -> perlakukan sebagai belum login.
                await ClearTokenAsync();
                identity = new ClaimsIdentity();
            }
        }

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public async Task MarkUserAsLoggedInAsync(string token)
    {
        await _js.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task MarkUserAsLoggedOutAsync()
    {
        await ClearTokenAsync();
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task<string?> GetTokenAsync()
    {
        try
        {
            return await _js.InvokeAsync<string?>("localStorage.getItem", TokenKey);
        }
        catch
        {
            // Terjadi saat prerendering / JS belum siap.
            return null;
        }
    }

    private Task ClearTokenAsync() => _js.InvokeVoidAsync("localStorage.removeItem", TokenKey).AsTask();

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes)
            ?? new Dictionary<string, object>();

        var claims = new List<Claim>();
        foreach (var kvp in keyValuePairs)
        {
            if (kvp.Value is JsonElement { ValueKind: JsonValueKind.Array } arrayElement)
            {
                claims.AddRange(arrayElement.EnumerateArray().Select(v => new Claim(kvp.Key, v.ToString())));
            }
            else
            {
                claims.Add(new Claim(kvp.Key, kvp.Value?.ToString() ?? string.Empty));
            }
        }
        return claims;
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        base64 = base64.Replace('-', '+').Replace('_', '/');
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}
