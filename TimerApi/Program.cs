using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SharedLibrary.Data;

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


builder.Services.AddOpenApi();

var app = builder.Build();


// middleware

using (var scope = app.Services.CreateScope())
{
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    if (!await roleMgr.RoleExistsAsync("Admin"))
        await roleMgr.CreateAsync(new IdentityRole("Admin"));
    if (!await roleMgr.RoleExistsAsync("User"))
        await roleMgr.CreateAsync(new IdentityRole("User"));

    var admin = await userMgr.FindByEmailAsync("admin@admin.com");
    if (admin == null)
    {
        await userMgr.CreateAsync(new IdentityUser
        {
            UserName = "admin@admin.com",
            Email = "admin@admin.com"
        },
        "Admin_123");
        await userMgr.AddToRoleAsync(await userMgr.FindByEmailAsync("admin@admin.com"), "Admin");
    }
}


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
