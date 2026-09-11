using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

/// <summary>
/// Representasi tabel "employee" pada database lama (db_stk).
/// Menyimpan data pegawai hasil sinkronisasi dari Active Directory (LDAP)
/// pada saat login (lihat: vallogin.php).
/// </summary>
[Table("employee")]
public class Employee
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>1 = user biasa, 2 = admin (lihat menu.php: USR_TYP > 1)</summary>
    [Column("usertype")]
    public byte? UserType { get; set; }

    [Column("nopek")]
    [MaxLength(10)]
    public string? Nopek { get; set; }

    [Column("emp_name")]
    [MaxLength(100)]
    public string? EmpName { get; set; }

    [Column("username")]
    [MaxLength(100)]
    public string? Username { get; set; }

    /// <summary>Password terenkripsi dengan algoritma legacy (lihat CryptoHelper.Encrypss)</summary>
    [Column("password")]
    [MaxLength(100)]
    public string? Password { get; set; }

    [Column("email")]
    [MaxLength(115)]
    public string? Email { get; set; }

    [Column("jabatan")]
    [MaxLength(100)]
    public string? Jabatan { get; set; }

    [Column("fungsi")]
    [MaxLength(100)]
    public string? Fungsi { get; set; }

    [Column("telpon")]
    [MaxLength(15)]
    public string? Telpon { get; set; }

    /// <summary>1 = baru dibuat/offline, 2 = sedang online (lihat vallogin.php)</summary>
    [Column("online")]
    public byte? Online { get; set; }

    [Column("onlinetime")]
    public DateTime? OnlineTime { get; set; }
}
