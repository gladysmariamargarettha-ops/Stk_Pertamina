using Backend.Common;
using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

/// <summary>
/// Menggantikan login.php (form), vallogin.php (proses validasi LDAP) dan logout.php.
/// Pada versi lama, hasil login disimpan di $_SESSION. Di versi baru ini disimpan
/// sebagai klaim di dalam JWT yang dikembalikan ke Frontend (Blazor WASM).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ILdapAuthService _ldap;
    private readonly IJwtTokenService _jwt;
    private readonly ILogger<AuthController> _logger;

    public AuthController(AppDbContext db, ILdapAuthService ldap, IJwtTokenService jwt, ILogger<AuthController> logger)
    {
        _db = db;
        _ldap = ldap;
        _jwt = jwt;
        _logger = logger;
    }

    /// <summary>
    /// POST /api/auth/login
    /// Port dari vallogin.php: bind ke Active Directory, lalu insert/update baris di tabel employee,
    /// terakhir menerbitkan JWT (pengganti session).
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            // Setara: else { header('location:index.php'); }
            return BadRequest(new { message = "Username dan password harus diisi." });
        }

        var ldapUser = _ldap.Authenticate(request.Username, request.Password);
        if (ldapUser is null)
        {
            // Setara pesan: "Invalid email address/password..." atau
            // "email Anda tidak terdaftar di RU II Dumai.."
            return Unauthorized(new { message = "Username/password salah, atau akun tidak terdaftar di RU II Dumai." });
        }

        var nopek = ldapUser.Nopek;
        // Port persis dari PHP: if ($nopek=='713718'){$nopek='123456';}
        if (nopek == "713718") nopek = "123456";

        var crypted = CryptoHelper.Encrypss(request.Password);

        var employee = await _db.Employees.FirstOrDefaultAsync(e => e.Username == request.Username);
        if (employee is null)
        {
            employee = new Employee
            {
                UserType = 1,
                Nopek = nopek,
                EmpName = ldapUser.DisplayName,
                Username = request.Username,
                Password = crypted,
                Email = ldapUser.Email,
                Jabatan = ldapUser.Jabatan,
                Fungsi = ldapUser.Fungsi,
                Telpon = ldapUser.Telpon,
                Online = 1,
                OnlineTime = DateTime.Now,
            };
            _db.Employees.Add(employee);
        }
        else
        {
            employee.Online = 2;
            employee.OnlineTime = DateTime.Now;
            employee.Password = crypted;
        }

        await _db.SaveChangesAsync();

        var token = _jwt.GenerateToken(employee);

        return Ok(new LoginResponseDto
        {
            Token = token,
            UserId = employee.Id,
            Username = employee.Username ?? string.Empty,
            EmpName = employee.EmpName ?? string.Empty,
            Nopek = employee.Nopek ?? string.Empty,
            UserType = employee.UserType ?? 1,
        });
    }

    /// <summary>
    /// POST /api/auth/logout
    /// Pada PHP asli, logout.php hanya meng-unset session (tidak menyentuh DB).
    /// Karena sekarang autentikasi berbasis JWT (stateless), sisi server tidak
    /// menyimpan apa pun untuk dihapus — Frontend cukup membuang token yang tersimpan.
    /// Endpoint ini disediakan untuk kompatibilitas & agar mudah dikembangkan
    /// (misal audit log) di kemudian hari.
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        return Ok(new { message = "Logout berhasil." });
    }
}
