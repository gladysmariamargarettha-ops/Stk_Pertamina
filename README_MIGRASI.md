# Migrasi STK Online (PHP → .NET) — Module 1: Fondasi & Login

Dokumen ini menjelaskan hasil migrasi **Module 1** dari project PHP lama (`web_kp.zip`)
ke struktur .NET (`Stk_Pertamina.zip`): **Backend** (ASP.NET Core Web API) +
**Frontend** (Blazor WebAssembly).

## 1. Peta modul PHP → .NET

| File/Fitur PHP lama | Pengganti di .NET |
|---|---|
| `inc/db.php` (koneksi MySQL + fungsi helper) | `Backend/Data/AppDbContext.cs`, `Backend/Common/CryptoHelper.cs` |
| Semua `CREATE TABLE` di `db_stk.sql` | `Backend/Models/*.cs` (12 entity, 1:1 dengan skema lama) |
| `vallogin.php` (bind LDAP + insert/update `employee`) | `Backend/Services/LdapAuthService.cs` + `Backend/Controllers/AuthController.cs` |
| `$_SESSION['USR_...']` | JWT — `Backend/Services/JwtTokenService.cs` (server) & `Frontend/Services/CustomAuthStateProvider.cs` (client) |
| `login.php` (form login) | `Frontend/Pages/Login.razor` |
| `logout.php` | `AuthController.Logout()` + `Frontend/Services/AuthService.LogoutAsync()` |
| `menu.php` (menu dinamis + admin) | `Backend/Controllers/MenuController.cs` + `Frontend/Layout/NavMenu.razor` |
| `head.php` + `foot.php` | `Frontend/Layout/MainLayout.razor` |
| `index.php` (banner) | `Frontend/Pages/Home.razor` (route `/`) |
| `home.php` (kerangka; isi tabel dokumen menyusul modul berikutnya) | `Frontend/Pages/Dashboard.razor` (route `/home`) |

Modul yang **belum** dimigrasikan (menyusul di sesi berikutnya, satu per satu):
`home.php` (tabel dokumen lengkap), `stkfungsi.php`, `stkgrafik.php`, `stkarsip.php`,
`stkpic.php`, `stktoxls.php`, `stkemail.php`, `update.php`, `notif.php`, `pdf.php`.

## 2. Persiapan sebelum menjalankan

### a. Database
Backend memakai **EF Core + Pomelo MySQL** dan **menyambung ke database MySQL/MariaDB yang sama**
(`db_stk`, dari file `db_stk.sql`). Tidak perlu migrasi data — cukup import `db_stk.sql` ke
server MySQL Anda seperti biasa, lalu sesuaikan connection string di:

`Backend/appsettings.json`
```json
"ConnectionStrings": {
  "DbStk": "Server=localhost;Port=3306;Database=db_stk;User=root;Password=;"
}
```

### b. Konfigurasi LDAP / Active Directory
Sesuaikan dengan server AD RU Dumai (di PHP lama: `ldap://ptplkpad1`):
```json
"Ldap": {
  "Server": "ptplkpad1",
  "Port": 389,
  "Domain": "pertamina",
  "BaseDn": "dc=PERTAMINA,dc=COM"
}
```

### c. Kunci JWT
**WAJIB diganti** sebelum dipakai sungguhan (minimal 32 karakter acak & rahasia):
```json
"Jwt": { "Key": "GANTI_DENGAN_KUNCI_RAHASIA_MINIMAL_32_KARAKTER!" }
```

### d. Alamat Backend di sisi Frontend
`Frontend/wwwroot/appsettings.json` → sesuaikan `ApiBaseUrl` dengan port Backend
saat dijalankan (lihat `Backend/Properties/launchSettings.json`, default `https://localhost:7250/`).

### e. CORS
`Backend/appsettings.json` bagian `Cors:FrontendOrigins` harus memuat alamat Frontend
(default sudah cocok dengan `Frontend/Properties/launchSettings.json`).

## 3. Cara menjalankan (setelah `dotnet restore` di masing-masing project)

```bash
# Terminal 1 - Backend
cd Backend
dotnet restore
dotnet run

# Terminal 2 - Frontend
cd Frontend
dotnet restore
dotnet run
```

Buka alamat Frontend (mis. `https://localhost:7118`) di browser.

## 4. Catatan penting

- **Password lama tidak dipakai untuk autentikasi.** Sama seperti versi PHP, login
  divalidasi lewat **LDAP bind ke Active Directory**, bukan membandingkan hash di
  tabel `employee`. Kolom `employee.password` tetap diisi (memakai algoritma
  `CryptoHelper.Encrypss`, port dari `encrypss()` PHP) hanya untuk kompatibilitas data lama.
- **Session digantikan JWT.** Frontend menyimpan token di `localStorage` browser dan
  mengirimkannya sebagai header `Authorization: Bearer ...` di setiap request ke Backend.
- **Package NuGet yang ditambahkan** di `Backend.csproj`:
  `Microsoft.EntityFrameworkCore`, `Pomelo.EntityFrameworkCore.MySql`,
  `Microsoft.EntityFrameworkCore.Design`, `Microsoft.AspNetCore.Authentication.JwtBearer`,
  `System.DirectoryServices.Protocols`. Jalankan `dotnet restore` untuk mengunduhnya
  (butuh koneksi internet saat build pertama kali).
- Jika ingin memakai **EF Core Migrations** dari skema yang sudah ada, jalankan
  (dari folder `Backend`):
  ```bash
  dotnet tool install --global dotnet-ef   # sekali saja
  dotnet ef dbcontext scaffold "Server=localhost;Database=db_stk;User=root;Password=;" \
      Pomelo.EntityFrameworkCore.MySql -o Models --force --no-onconfiguring
  ```
  Tapi ini **tidak wajib** — model yang sudah saya buat manual sudah cocok dengan skema di `db_stk.sql`.

## 5. Lanjutan

Beri tahu file PHP mana yang ingin dimigrasikan berikutnya (disarankan urut: `home.php`
lengkap → `stkfungsi.php` → `stkgrafik.php` → `stkarsip.php` → `stkpic.php` → `stktoxls.php`
→ `update.php` → `stkemail.php`/`notif.php`/`pdf.php`), supaya bisa dikerjakan satu per satu
seperti Module 1 ini.
