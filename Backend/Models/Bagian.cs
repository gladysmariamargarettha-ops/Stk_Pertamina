using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

/// <summary>Representasi tabel "bagian" (departemen/seksi di bawah suatu fungsi).</summary>
[Table("bagian")]
public class Bagian
{
    [Key]
    [Column("idBagian")]
    public int IdBagian { get; set; }

    [Column("fungsi_id")]
    public int FungsiId { get; set; }

    [Column("bagian")]
    [MaxLength(100)]
    public string BagianNama { get; set; } = string.Empty;

    [Column("email")]
    [MaxLength(50)]
    public string? Email { get; set; }
}
