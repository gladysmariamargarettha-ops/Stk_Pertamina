using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

/// <summary>Representasi tabel "log_imel" — log pengiriman email reminder dokumen (stkemail.php).</summary>
[Table("log_imel")]
public class LogImel
{
    [Key]
    [Column("idlog")]
    public int IdLog { get; set; }

    [Column("tglnotif")]
    public DateTime TglNotif { get; set; }

    [Column("notifer")]
    [MaxLength(50)]
    public string Notifer { get; set; } = string.Empty;

    [Column("ket")]
    [MaxLength(10)]
    public string? Keterangan { get; set; }
}
