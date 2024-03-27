using API_SO_tag_analyzer.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// logger
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Host.UseSerilog(Log.Logger);

// Add services to the container.
var currentDirectory = Directory.GetCurrentDirectory();
var jsonFilePath = Path.Combine(currentDirectory, "so-response.json");
JsonFileService jsonFileService = new JsonFileService(jsonFilePath);
builder.Services.AddSingleton(jsonFileService);
builder.Services.AddSingleton(new StackOverflowApiService(jsonFileService));

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Add support to logging request with SERILOG
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
