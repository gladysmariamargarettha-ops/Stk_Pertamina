using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

/// <summary>Representasi tabel "log_imelfungsi" — log reminder per fungsi/PIC.</summary>
[Table("log_imelfungsi")]
public class LogImelFungsi
{
    // Tabel asli tidak punya primary key sendiri (idlog+idfungsi berulang),
    // dipetakan sebagai composite key di AppDbContext.
    [Column("idlog")]
    public int IdLog { get; set; }

    [Column("idfungsi")]
    public int IdFungsi { get; set; }

    [Column("tglnotif")]
    public DateTime TglNotif { get; set; }

    [Column("notifer")]
    [MaxLength(50)]
    public string Notifer { get; set; } = string.Empty;

    [Column("ket")]
    [MaxLength(10)]
    public string? Keterangan { get; set; }
}
