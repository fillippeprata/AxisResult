var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapControllers();
app.MapGet("/", () => "Welcome!");
app.MapHealthChecks("/health");

app.Run();
