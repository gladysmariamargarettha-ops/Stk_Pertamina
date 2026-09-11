using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

/// <summary>
/// Representasi tabel "user" (nama tabel asli MySQL: user).
/// Diberi nama kelas AppUser di C# supaya tidak bentrok dengan konsep "User" bawaan .NET.
/// </summary>
[Table("user")]
public class AppUser
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("username")]
    [MaxLength(12)]
    public string Username { get; set; } = string.Empty;

    [Column("password")]
    [MaxLength(50)]
    public string Password { get; set; } = string.Empty;

    [Column("nama")]
    [MaxLength(50)]
    public string Nama { get; set; } = string.Empty;

    [Column("type")]
    public int Type { get; set; } = 1;
}
