using System.DirectoryServices.Protocols;
using System.Net;
using Microsoft.Extensions.Options;

namespace Backend.Services;

public class LdapOptions
{
    /// <summary>Host/IP server domain controller, contoh: "ptplkpad1" (tanpa "ldap://", lihat vallogin.php).</summary>
    public string Server { get; set; } = string.Empty;

    /// <summary>NetBIOS domain, contoh: "pertamina" -> dipakai sebagai "pertamina\username".</summary>
    public string Domain { get; set; } = "pertamina";

    /// <summary>Base DN pencarian, contoh: "dc=PERTAMINA,dc=COM".</summary>
    public string BaseDn { get; set; } = string.Empty;

    public int Port { get; set; } = 389;
}

/// <summary>
/// Port dari logika LDAP bind + search pada vallogin.php:
///   $ldap = ldap_connect($adServer);
///   $ldaprdn = 'pertamina\' . $name;
///   $bind = ldap_bind($ldap, $ldaprdn, $pass);
///   $result = ldap_search($ldap, "dc=PERTAMINA,dc=COM", "(sAMAccountName=$name)");
/// </summary>
public class LdapAuthService : ILdapAuthService
{
    private readonly LdapOptions _options;
    private readonly ILogger<LdapAuthService> _logger;

    public LdapAuthService(IOptions<LdapOptions> options, ILogger<LdapAuthService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public LdapUserInfo? Authenticate(string username, string password)
    {
        try
        {
            var identifier = new LdapDirectoryIdentifier(_options.Server, _options.Port);
            using var connection = new LdapConnection(identifier)
            {
                AuthType = AuthType.Basic
            };
            connection.SessionOptions.ProtocolVersion = 3;
            connection.SessionOptions.ReferralChasing = ReferralChasingOptions.None;

            // Setara "pertamina\username" pada PHP.
            var ldapRdn = $"{_options.Domain}\\{username}";
            connection.Credential = new NetworkCredential(ldapRdn, password);

            // Setara ldap_bind() — akan melempar LdapException bila kredensial salah.
            connection.Bind();

            var filter = $"(sAMAccountName={EscapeLdapFilterValue(username)})";
            var request = new SearchRequest(
                _options.BaseDn,
                filter,
                System.DirectoryServices.Protocols.SearchScope.Subtree,
                "employeeID", "displayName", "mail", "title", "telephoneNumber", "department");

            var response = (SearchResponse)connection.SendRequest(request);

            // Setara: if ($info['count'] > 1) break;  -> ambigu, dianggap gagal.
            if (response.Entries.Count != 1)
            {
                _logger.LogWarning(
                    "LDAP search untuk user {Username} mengembalikan {Count} hasil (diharapkan tepat 1).",
                    username, response.Entries.Count);
                return null;
            }

            var entry = response.Entries[0];
            return new LdapUserInfo
            {
                Nopek = GetAttr(entry, "employeeID"),
                DisplayName = GetAttr(entry, "displayName"),
                Email = GetAttr(entry, "mail"),
                Jabatan = GetAttr(entry, "title"),
                Telpon = GetAttr(entry, "telephoneNumber"),
                Fungsi = GetAttr(entry, "department"),
            };
        }
        catch (LdapException ex)
        {
            _logger.LogWarning(ex, "LDAP bind gagal untuk user {Username}", username);
            return null;
        }
    }

    private static string GetAttr(SearchResultEntry entry, string name)
    {
        if (!entry.Attributes.Contains(name)) return string.Empty;
        // GetValues(typeof(string)) memastikan nilai dikembalikan sebagai string,
        // bukan byte[] mentah (beberapa atribut AD default dikembalikan sebagai byte array).
        var values = entry.Attributes[name].GetValues(typeof(string));
        return values.Length > 0 ? (values[0] as string ?? string.Empty) : string.Empty;
    }

    private static string EscapeLdapFilterValue(string value)
    {
        return value
            .Replace("\\", "\\5c")
            .Replace("*", "\\2a")
            .Replace("(", "\\28")
            .Replace(")", "\\29")
            .Replace("\0", "\\00");
    }
}
