using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

/// <summary>Representasi tabel "menu" — daftar menu navigasi dinamis (lihat menu.php).</summary>
[Table("menu")]
public class MenuItem
{
    [Key]
    [Column("idmenu")]
    public sbyte IdMenu { get; set; }

    [Column("menu")]
    [MaxLength(20)]
    public string Menu { get; set; } = string.Empty;

    [Column("link")]
    [MaxLength(50)]
    public string Link { get; set; } = string.Empty;
}
