// ==========================================
// ANTES: Configuración con "magic strings"
// ==========================================

var builder = WebApplication.CreateBuilder(args);

var connection = builder.Configuration["MongoDbSettings:ConnectionString"];
Console.WriteLine($"Mongo connection: {connection}");

// Rígido, sin tipado, difícil de testear.


// ==========================================
// DESPUÉS: Usando OPTIONS PATTERN
// ==========================================

using Microsoft.Extensions.Options;

namespace Catalog.API.Settings
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = null!;
    }

    public class ServiceSettings
    {
        public string ServiceName { get; set; } = null!;
    }
}

// Program.cs (después)
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.Configure<ServiceSettings>(
    builder.Configuration.GetSection("ServiceSettings"));

// Uso en una clase
public class MongoRepository
{
    private readonly MongoDbSettings _settings;

    public MongoRepository(IOptions<MongoDbSettings> options)
    {
        _settings = options.Value;
    }

    public void Connect()
    {
        Console.WriteLine($"Connecting to MongoDB: {_settings.ConnectionString}");
    }
}
