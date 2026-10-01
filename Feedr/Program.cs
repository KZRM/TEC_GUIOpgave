using Feedr.Components;

using Feedr.DBAccess;
using Feedr.State;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Standardfejlsvar fra ASP.NET Core, så vi ikke behøver en separat fejlside.
builder.Services.AddProblemDetails();

// Registrerer en factory, som kan oprette FeedrDBContext til SQL Server.
// Forbindelsen findes under "DefaultConnection" i konfigurationen.
builder.Services.AddDbContextFactory<FeedrDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Gør Repository tilgængelig via @inject.
// Samme Repository-objekt genbruges inden for brugerens Blazor-forbindelse.
builder.Services.AddScoped<Repository>();

// 2.2: Hver Blazor-forbindelse får sit eget UiState, så brugerne ikke deler sprogvalg.
builder.Services.AddScoped<UiState>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
