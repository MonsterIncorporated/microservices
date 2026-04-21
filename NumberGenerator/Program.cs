using NumberGenerator.Services;

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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<NumberService>();
builder.Services.AddSingleton<MqHelperService>();

builder.Services.AddTransient<HealthService>();

builder.Services.AddHostedService<RabbitMqService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors(AllowedOrigin);
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseAuthorization();

app.MapControllers();

app.Run();
