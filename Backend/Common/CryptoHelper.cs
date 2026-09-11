using System.Text;

namespace Backend.Common;

/// <summary>
/// Port 1:1 dari fungsi encrypss() pada inc/db.php (PHP lama).
/// Algoritma: setiap karakter password dijumlahkan (per kode ASCII) dengan
/// karakter kunci yang berulang (mirip Vigenere cipher sederhana),
/// lalu hasilnya di-base64 + urlencode.
///
/// Dipertahankan hanya untuk kompatibilitas/pembacaan data lama pada kolom
/// employee.password. Autentikasi utama tetap memakai LDAP (Active Directory).
/// </summary>
public static class CryptoHelper
{
    private const string Key = "979a218e0632dferfert45dfg98d47956c7";

    public static string Encrypss(string str)
    {
        var done = new StringBuilder();
        for (int i = 0; i < str.Length; i++)
        {
            char c = str[i];
            // Meniru: substr($key, ($i % strlen($key)) - 1, 1)
            int keyIndex = (i % Key.Length) - 1;
            if (keyIndex < 0) keyIndex += Key.Length;
            char keyChar = Key[keyIndex];

            char combined = (char)(c + keyChar);
            done.Append(combined);
        }

        var bytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(done.ToString());
        var base64 = Convert.ToBase64String(bytes);
        return Uri.EscapeDataString(base64);
    }
}
