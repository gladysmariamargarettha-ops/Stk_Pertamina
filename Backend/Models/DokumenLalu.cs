using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

/// <summary>
/// Representasi tabel "dokumen_lalu" — arsip riwayat dokumen yang sudah direvisi/kadaluarsa
/// (lihat stkarsip.php).
/// </summary>
[Table("dokumen_lalu")]
public class DokumenLalu
{
    [Key]
    [Column("idDokumen")]
    public int IdDokumen { get; set; }

    [Column("bagian_id")]
    public int BagianId { get; set; }

    [Column("jenis")]
    public JenisDokumen Jenis { get; set; }

    [Column("noDokumen")]
    [MaxLength(50)]
    public string NoDokumen { get; set; } = string.Empty;

    [Column("judul")]
    [MaxLength(200)]
    public string Judul { get; set; } = string.Empty;

    [Column("rev")]
    public sbyte Rev { get; set; }

    [Column("tglBerlaku")]
    public DateTime TglBerlaku { get; set; }

    [Column("keterangan")]
    [MaxLength(255)]
    public string Keterangan { get; set; } = string.Empty;

    [Column("modified")]
    public DateTime Modified { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("file")]
    [MaxLength(255)]
    public string File { get; set; } = string.Empty;

    [Column("kadaluarsa")]
    public int Kadaluarsa { get; set; }
}
