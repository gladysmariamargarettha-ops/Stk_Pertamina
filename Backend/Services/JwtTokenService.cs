using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services;

public class JwtOptions
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = "StkPertaminaBackend";
    public string Audience { get; set; } = "StkPertaminaFrontend";
    public int ExpireMinutes { get; set; } = 480; // 8 jam kerja
}

/// <summary>
/// Menggantikan mekanisme $_SESSION['USR_...'] pada PHP dengan JWT,
/// supaya Backend (Web API) bisa bersifat stateless dan dipanggil dari Frontend (Blazor WASM)
/// lintas origin.
///
/// Klaim yang dibawa persis meniru variabel session lama:
///   USR_IDM -> NameIdentifier (id)
///   USR_USM -> Name (username)
///   USR_NPK -> "nopek"
///   USR_TYP -> Role (usertype)
///   USR_NAME -> "emp_name"
/// </summary>
public class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _options;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public string GenerateToken(Employee employee)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, employee.Id.ToString()),
            new(ClaimTypes.Name, employee.Username ?? string.Empty),
            new("nopek", employee.Nopek ?? string.Empty),
            new("emp_name", employee.EmpName ?? string.Empty),
            new(ClaimTypes.Role, (employee.UserType ?? 1).ToString()),
            new(ClaimTypes.Email, employee.Email ?? string.Empty),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_options.ExpireMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
