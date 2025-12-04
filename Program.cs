using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Data;

var builder = WebApplication.CreateBuilder(args);

// Configure EF Core DbContext
if (builder.Environment.IsProduction())
{
    // Production: Use SQL Server
    builder.Services.AddDbContext<MVCBookContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("MVCBookContext"),
        sqlServerOptionsAction: sqlOptions => sqlOptions.EnableRetryOnFailure()));
}
else
{
    // Development: Use SQLite
    var connectionString = builder.Configuration.GetConnectionString("MVCBookContext")
        ?? throw new InvalidOperationException("Connection string 'MVCBookContext' not found.");

    builder.Services.AddDbContext<MVCBookContext>(options =>
        options.UseSqlite(connectionString));
}

// Add Auth
builder.Services.AddAuthorization();

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<MVCBookContext>()
    .AddDefaultTokenProviders();

// Add this to make sure Controllers work
builder.Services.AddControllersWithViews();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    BookSeedData.Initialize(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Books}/{action=Index}/{id?}");

app.Run();