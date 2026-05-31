using Microsoft.EntityFrameworkCore;
using NumberGenerator.Services;
using Serilog;
using Serilog.Sinks.Grafana.Loki;

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
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<MqHelperService>();

builder.Services.AddScoped<HealthService>();
builder.Services.AddScoped<INumberService, NumberService>();

builder.Services.AddHostedService<RabbitMqService>();

builder.Services.AddDbContext<AppDbContext>((options) =>
{
    var dbConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING_NUMBERGENERATOR") ?? "Host=localhost;Port=5432;Database=numbergenerator;Username=postgres;Password=DEIN_PASSWORT";
    options.UseNpgsql(dbConnectionString);
});

var app = builder.Build();

app.UseCors(AllowedOrigin);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
