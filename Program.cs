using Microsoft.EntityFrameworkCore;
using GreTutor.DbContext;
using GreTutor.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using GreTutor.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure; // Thêm namespace cho UseMySql

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Đăng ký DbContext với MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

// Cấu hình Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // Thiết lập Password
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 3;
    options.Password.RequiredUniqueChars = 1;

    // Cấu hình Lockout
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // Cấu hình User
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;

    // Cấu hình SignIn
    options.SignIn.RequireConfirmedEmail = true;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Cấu hình cookie authentication
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Đường dẫn đăng nhập
    options.LogoutPath = "/Account/Logout"; // Đường dẫn đăng xuất
    options.AccessDeniedPath = "/Account/AccessDenied"; // Đường dẫn khi bị từ chối truy cập
});

// Cấu hình gửi email
var mailSettings = builder.Configuration.GetSection("MailSettings");
builder.Services.Configure<MailSettings>(mailSettings);
builder.Services.AddTransient<IEmailSender, SendMailService>();

// Thêm Razor Pages
builder.Services.AddRazorPages();

// Cấu hình Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Allow", policy => policy.RequireAuthenticatedUser());
    options.AddPolicy("ShowStaffMenu", policy => policy.RequireRole("Staff"));
});

var app = builder.Build();

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Đảm bảo Authentication trước Authorization
app.UseAuthentication();
app.UseAuthorization();

// Định nghĩa routes
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.MapRazorPages();

// Khởi tạo roles
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roleNames = { "Tutor", "Staff", "Student" };

    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // (Tùy chọn) Kiểm tra kết nối MySQL
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        await dbContext.Database.EnsureCreatedAsync();
        Console.WriteLine("Kết nối MySQL thành công!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Lỗi kết nối MySQL: {ex.Message}");
    }
}

app.Run();