using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SharedLibrary.Data;
using SharedLibrary.Repositories;
using SharedLibrary.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// services
builder.Services.AddDbContext<ApplicationDbContext>(opts
    => opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        opts.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });


builder.Services.AddOpenApi();

builder.Services.AddSingleton<TokenGenerator>();
builder.Services.AddSingleton<IPomodoroTimerRepository, PomodoroTimerRepository>();

var app = builder.Build();


// middleware

/*            Seeding                */

//using (var scope = app.Services.CreateScope())
//{
//    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
//    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

//    if (!await roleMgr.RoleExistsAsync("Admin"))
//        await roleMgr.CreateAsync(new IdentityRole("Admin"));
//    if (!await roleMgr.RoleExistsAsync("User"))
//        await roleMgr.CreateAsync(new IdentityRole("User"));

//    var claim = new Claim(ClaimTypes.Name, "name");

//    var admin = await userMgr.FindByEmailAsync("admin@admin.com");
//    if (admin == null)
//    {
//        await userMgr.CreateAsync(new IdentityUser
//        {
//            UserName = "admin@admin.com",
//            Email = "admin@admin.com"
//        },
//        "Admin_123");
//        await userMgr.AddToRoleAsync(await userMgr.FindByEmailAsync("admin@admin.com"), "Admin");
//    }
//}


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
