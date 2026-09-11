namespace Backend.Models;

/// <summary>
/// Mirip kolom ENUM('Pedoman','TKO','TKI','TKPA','Video') pada tabel dokumen (MySQL).
/// Tabel dokumen_lalu tidak memiliki nilai 'Video', tapi kita pakai enum yang sama
/// dan cukup tidak memakai nilai Video di sana.
/// </summary>
public enum JenisDokumen
{
    Pedoman,
    TKO,
    TKI,
    TKPA,
    Video
}
