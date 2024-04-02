using API_SO_tag_analyzer.Helpers;
using API_SO_tag_analyzer.Services;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Serilog;
using System.Reflection;
using System.Text.Json.Serialization;

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
JsonFileService jsonFileService = new JsonFileService(jsonFilePath, Log.Logger);
builder.Services.AddSingleton(jsonFileService);
builder.Services.AddSingleton(new TagOperationService(jsonFileService, Log.Logger));
builder.Services.AddSingleton(new StackOverflowApiService(jsonFileService, Log.Logger));
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.Converters.Add(new StringEnumConverter
        {
            CamelCaseText = true
        });
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
     options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "SO API tags",
        Description = "StackOverflow tags analyzer",
    });
    options.SchemaFilter<EnumSchemaFilter>();
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddSwaggerGenNewtonsoftSupport();
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Stack Overflow tags analyzer"); });
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/swagger/index.html");
        return;
    }

    await next();
});

// Add support to logging request with SERILOG
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
