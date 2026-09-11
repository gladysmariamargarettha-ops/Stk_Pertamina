using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

/// <summary>Representasi tabel "t_user" — tabel user legacy (tersisa dari modul lama, tidak dipakai alur login utama).</summary>
[Table("t_user")]
public class TUser
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("t_user_type")]
    public int TUserType { get; set; }

    [Column("nopek")]
    [MaxLength(8)]
    public string Nopek { get; set; } = string.Empty;

    [Column("nama")]
    [MaxLength(150)]
    public string Nama { get; set; } = string.Empty;

    [Column("userid")]
    [MaxLength(255)]
    public string UserId { get; set; } = "-";

    [Column("email")]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Column("cdcctr")]
    [MaxLength(8)]
    public string CdcCtr { get; set; } = "-";

    [Column("coctr")]
    [MaxLength(100)]
    public string CoCtr { get; set; } = "-";

    [Column("tgllhr")]
    public DateTime TglLahir { get; set; }

    [Column("jabatan")]
    [MaxLength(100)]
    public string? Jabatan { get; set; }

    [Column("crypted")]
    [MaxLength(150)]
    public string? Crypted { get; set; }

    [Column("wrong_log")]
    public int? WrongLog { get; set; } = 0;

    [Column("online")]
    public int? Online { get; set; } = 0;

    [Column("online_time")]
    public DateTime? OnlineTime { get; set; }

    [Column("stat")]
    public sbyte? Stat { get; set; }
}
