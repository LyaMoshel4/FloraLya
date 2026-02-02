using LyaShop.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies; // הוספתי את זה

var builder = WebApplication.CreateBuilder(args);

// הוספת החיבור למסד הנתונים
builder.Services.AddDbContext<LyaFlowerShopContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();

// --- 1. הוספת שירות האימות (Cookies) ---
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Login"; // לאן להפנות אם מישהו לא מורשה
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // זמן חיבור
    });
// ---------------------------------------

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// --- 2. הסדר כאן קריטי! קודם בדיקת זהות ואז בדיקת הרשאות ---
app.UseAuthentication();
app.UseAuthorization();
// ---------------------------------------------------------

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();