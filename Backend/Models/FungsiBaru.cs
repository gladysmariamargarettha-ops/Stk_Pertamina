using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

/// <summary>Representasi tabel "fungsibaru" — daftar fungsi/PIC penerima notifikasi (lihat stkfungsi.php, stkpic.php).</summary>
[Table("fungsibaru")]
public class FungsiBaru
{
    [Key]
    [Column("idfungsi")]
    public int IdFungsi { get; set; }

    [Column("fungsi")]
    [MaxLength(80)]
    public string NamaFungsi { get; set; } = string.Empty;

    [Column("email")]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Column("ket")]
    [MaxLength(5)]
    public string? Keterangan { get; set; } = "sh";

    [Column("urut")]
    public int? Urut { get; set; }
}
