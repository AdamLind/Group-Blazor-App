using Microsoft.EntityFrameworkCore;
using MvcMovie.Data;

var builder = WebApplication.CreateBuilder(args);
// ..

// Add services to the container.
builder.Services.AddControllersWithViews();

var environmentName = builder.Environment.EnvironmentName;

if (string.Equals(environmentName, "Production", StringComparison.OrdinalIgnoreCase))
{
    // Production: Use SQL Server
    builder.Services.AddDbContext<MVCBookContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("MVCBookContext"),
            sqlServerOptionsAction: sqlOptions => sqlOptions.EnableRetryOnFailure()));
}
else
{
    // Development: Use SQLite
    builder.Services.AddDbContext<MVCBookContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("MVCBookContext")));
}

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