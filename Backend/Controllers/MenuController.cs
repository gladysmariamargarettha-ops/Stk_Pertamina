using System.Security.Claims;
using Backend.Data;
using Backend.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

/// <summary>
/// Menggantikan bagian dinamis pada menu.php:
///   SELECT * FROM menu ORDER BY idmenu ASC   (untuk semua user login, USR_TYP > 0)
///   + 3 menu tambahan khusus admin (USR_TYP > 1): Dokumen Kadaluarsa, PIC/Fungsi, Download to xls.
///
/// Link ".php" lama dipetakan ke route Blazor yang baru supaya bisa langsung dipakai
/// sebagai href pada NavLink di Frontend.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // hanya user yang sudah login (setara: if ($_SESSION['USR_TYP']>0))
public class MenuController : ControllerBase
{
    private readonly AppDbContext _db;

    // Pemetaan link lama (.php) -> route baru di Blazor Frontend.
    private static readonly Dictionary<string, string> LinkMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["home.php"] = "/home",
        ["stkfungsi.php"] = "/fungsi",
        ["stkgrafik.php"] = "/grafik",
        ["stkarsip.php"] = "/arsip",
        ["stkpic.php"] = "/pic",
        ["stktoxls.php"] = "/export",
    };

    public MenuController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<MenuItemDto>>> GetMenu()
    {
        var userType = GetUserType();
        var result = new List<MenuItemDto>();

        if (userType > 0)
        {
            var rows = await _db.MenuItems
                .OrderBy(m => m.IdMenu)
                .ToListAsync();

            result.AddRange(rows.Select(m => new MenuItemDto
            {
                Menu = m.Menu,
                Link = MapLink(m.Link),
            }));
        }

        if (userType > 1)
        {
            result.Add(new MenuItemDto { Menu = "Dokumen Kadaluarsa", Link = MapLink("stkarsip.php") });
            result.Add(new MenuItemDto { Menu = "PIC /Fungsi", Link = MapLink("stkpic.php") });
            result.Add(new MenuItemDto { Menu = "Download to xls", Link = MapLink("stktoxls.php") });
        }

        return Ok(result);
    }

    private static string MapLink(string oldLink) =>
        LinkMap.TryGetValue(oldLink, out var mapped) ? mapped : "/" + oldLink.Replace(".php", "");

    private int GetUserType()
    {
        var roleClaim = User.FindFirstValue(ClaimTypes.Role);
        return int.TryParse(roleClaim, out var value) ? value : 0;
    }
}
