using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using LyaShop.Data;
using LyaShop.Models;
using Microsoft.AspNetCore.Authentication.Cookies; // הוספנו את זה בשביל האבטחה

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 1. הגדרת זיכרון (Session)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 2. הגדרת גישה לנתוני משתמש (מה שהיה חסר קודם)
builder.Services.AddHttpContextAccessor();

// 3. הגדרת מנגנון הזדהות (התיקון לשגיאה הנוכחית)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });

// חיבור לדאטה-בייס
builder.Services.AddDbContext<LyaFlowerShopContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LyaFlowerShopContext") ?? throw new InvalidOperationException("Connection string 'LyaFlowerShopContext' not found.")));

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

// --- סדר הפעולות כאן קריטי! ---
app.UseSession();        // קודם סשן
app.UseAuthentication(); // אז בדיקת זהות (מי אתה?)
app.UseAuthorization();  // ולבסוף הרשאות (מה מותר לך?)
// -----------------------------

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();