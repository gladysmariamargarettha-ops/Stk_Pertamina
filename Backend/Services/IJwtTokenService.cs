using Backend.Models;

namespace Backend.Services;

public interface IJwtTokenService
{
    /// <summary>Membuat JWT berisi klaim identitas pegawai yang sudah login (menggantikan session PHP).</summary>
    string GenerateToken(Employee employee);
}
