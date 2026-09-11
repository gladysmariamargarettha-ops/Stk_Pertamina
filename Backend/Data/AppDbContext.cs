using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

/// <summary>
/// DbContext yang memetakan seluruh tabel milik database lama "db_stk" (MySQL/MariaDB).
/// Struktur tabel TIDAK diubah supaya data lama tetap bisa dipakai langsung tanpa migrasi data.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Bagian> Bagians => Set<Bagian>();
    public DbSet<Dokumen> Dokumens => Set<Dokumen>();
    public DbSet<DokumenLalu> DokumenLalus => Set<DokumenLalu>();
    public DbSet<Fungsi> Fungsis => Set<Fungsi>();
    public DbSet<FungsiBaru> FungsiBarus => Set<FungsiBaru>();
    public DbSet<LogImel> LogImels => Set<LogImel>();
    public DbSet<LogImelFungsi> LogImelFungsis => Set<LogImelFungsi>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<Notif> Notifs => Set<Notif>();
    public DbSet<TUser> TUsers => Set<TUser>();
    public DbSet<AppUser> AppUsers => Set<AppUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Simpan enum JenisDokumen sebagai string di kolom ENUM MySQL, bukan sebagai angka.
        modelBuilder.Entity<Dokumen>()
            .Property(d => d.Jenis)
            .HasConversion<string>();

        modelBuilder.Entity<DokumenLalu>()
            .Property(d => d.Jenis)
            .HasConversion<string>();

        // Tabel log_imelfungsi tidak punya primary key tunggal pada skema asli;
        // kita definisikan composite key supaya EF Core bisa memetakannya.
        modelBuilder.Entity<LogImelFungsi>()
            .HasKey(l => new { l.IdLog, l.IdFungsi });
    }
}
