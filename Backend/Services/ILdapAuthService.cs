namespace Backend.Services;

public class LdapUserInfo
{
    public string Nopek { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Jabatan { get; set; } = string.Empty;
    public string Telpon { get; set; } = string.Empty;
    public string Fungsi { get; set; } = string.Empty;
}

public interface ILdapAuthService
{
    /// <summary>
    /// Melakukan bind ke Active Directory dengan username/password, lalu mengambil
    /// data pegawai (mirip proses di vallogin.php). Mengembalikan null jika bind gagal
    /// atau data pegawai tidak ditemukan / ambigu (lebih dari satu hasil).
    /// </summary>
    LdapUserInfo? Authenticate(string username, string password);
}
