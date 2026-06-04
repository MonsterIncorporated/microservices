using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using NumberGenerator.Services;
using Serilog;
using Serilog.Sinks.Grafana.Loki;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

var AllowedOrigin = "originAllowed";
// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowedOrigin,
    builder =>
    {
        var originString = Environment.GetEnvironmentVariable("ALLOWED_ORIGIN_STRING");
        if (originString == null)
        {
            throw new InvalidOperationException("ALLOWED_ORIGIN_STRING environment variable is not set.");
        }
        builder.WithOrigins(originString).AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(new LoggerConfiguration()
                                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
                                .Enrich.FromLogContext()
                                .WriteTo.Console()
                                .CreateLogger());

builder.Host.UseSerilog((context, config) =>
{
    config
        .Enrich.FromLogContext()
        .Enrich.WithProperty("app", "numbergenerator-api")
        .WriteTo.Console()
        .WriteTo.GrafanaLoki(
            "http://loki:3100",
            labels: new[]
            {
                new LokiLabel { Key = "app", Value = "numbergenerator-api" },
            });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var authority = Environment.GetEnvironmentVariable("OPENID_AUTHORITY");
    var clientId = Environment.GetEnvironmentVariable("OPENID_CLIENT_ID");
    var clientSecret = Environment.GetEnvironmentVariable("OPENID_CLIENT_SECRET");
    options.AddSecurityDefinition("Keycloak", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                
                AuthorizationUrl = new Uri($"{authority}/protocol/openid-connect/auth"),
                TokenUrl = new Uri($"{authority}/protocol/openid-connect/token"),
                Scopes = new Dictionary<string, string>
                {
                    { "openid", "OpenID" },
                    { "profile", "Profile" }
                }
            }
        }
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Keycloak", document)] =
            new List<String>{ "openid", "profile" }
    });
});

builder.Services.AddSingleton<MqHelperService>();

builder.Services.AddScoped<HealthService>();
builder.Services.AddScoped<INumberService, NumberService>();

builder.Services.AddHostedService<RabbitMqService>();

builder.Services.AddDbContext<AppDbContext>((options) =>
{
    var dbConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING_NUMBERGENERATOR") ?? "Host=localhost;Port=5432;Database=numbergenerator;Username=postgres;Password=DEIN_PASSWORT";
    options.UseNpgsql(dbConnectionString);
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var authority = Environment.GetEnvironmentVariable("OPENID_AUTHORITY_PRIVATE");

        if (authority == null)
        {
            throw new InvalidOperationException("OPENID_AUTHORITY is not set.");
        }

        options.Authority = authority;
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters.ValidIssuer = Environment.GetEnvironmentVariable("OPENID_AUTHORITY");
        options.TokenValidationParameters.ValidAudiences = new [] { "numbergenerator-api-swagger", "numbergenerator-api-frontend" };
    });

var app = builder.Build();

app.UseCors(AllowedOrigin);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
        options.OAuthClientId(Environment.GetEnvironmentVariable("OPENID_CLIENT_ID"));
        options.OAuthUsePkce();
    });
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}
app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
