using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

/// <summary>Representasi tabel "notif" — status pengiriman reminder bulanan.</summary>
[Table("notif")]
public class Notif
{
    [Key]
    [Column("id_notif")]
    public int IdNotif { get; set; }

    [Column("bulan")]
    public DateTime Bulan { get; set; }

    /// <summary>"not" atau "yes"</summary>
    [Column("send")]
    [MaxLength(3)]
    public string Send { get; set; } = "not";
}
