using Feedr.Components;

using Feedr.DBAccess;
using Feedr.State;
using Feedr.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Standardfejlsvar fra ASP.NET Core, så vi ikke behøver en separat fejlside.
builder.Services.AddProblemDetails();

// 2.1: Samme server tilbyder både Blazor-GUI og et controllerbaseret API.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSingleton<FileService>();

// Klienten kan flyttes til en anden server med en adresse fra konfigurationen.
var clientOrigins = builder.Configuration.GetSection("FileClient:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options => options.AddPolicy("FileClient", policy =>
{
    if (clientOrigins.Length > 0)
        policy.WithOrigins(clientOrigins).WithMethods("GET", "POST").AllowAnyHeader()
            .WithExposedHeaders("Content-Disposition");
}));

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

// Opret databasen og eksempeldata ved første start. Eksisterende data bevares.
await using (var db = await app.Services
    .GetRequiredService<IDbContextFactory<FeedrDBContext>>().CreateDbContextAsync())
{
    await DatabaseInitializer.InitializeAsync(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Feedr fil-API"));
}

app.UseAntiforgery();

app.MapControllers();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
