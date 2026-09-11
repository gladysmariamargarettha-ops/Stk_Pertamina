using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

/// <summary>Representasi tabel "fungsi" (daftar fungsi/direktorat versi lama).</summary>
[Table("fungsi")]
public class Fungsi
{
    [Key]
    [Column("idFungsi")]
    public int IdFungsi { get; set; }

    [Column("namaFungsi")]
    [MaxLength(50)]
    public string NamaFungsi { get; set; } = string.Empty;
}
