// ==========================================
// ANTES: Código acoplado en Program.cs
// ==========================================

// Program.cs (antes)
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Registro directo de Mongo (sin modularidad)
var mongoConnection = builder.Configuration["MongoDbSettings:ConnectionString"];
var mongoDatabaseName = builder.Configuration["ServiceSettings:ServiceName"];
var mongoClient = new MongoClient(mongoConnection);
builder.Services.AddSingleton(mongoClient.GetDatabase(mongoDatabaseName));

var app = builder.Build();
app.MapControllers();
app.Run();


// ==========================================
// DESPUÉS: Usando MÉTODO DE EXTENSIÓN
// ==========================================

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Catalog.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddMongoDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var mongoSettings = configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
            var serviceSettings = configuration.GetSection("ServiceSettings").Get<ServiceSettings>();

            // Validación básica
            if (string.IsNullOrEmpty(mongoSettings?.ConnectionString))
                throw new ArgumentException("Missing MongoDbSettings:ConnectionString");

            if (string.IsNullOrEmpty(serviceSettings?.ServiceName))
                throw new ArgumentException("Missing ServiceSettings:ServiceName");

            // Serializadores
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

            services.AddSingleton<IMongoDatabase>(serviceProvider =>
            {
                var client = new MongoClient(mongoSettings.ConnectionString);
                return client.GetDatabase(serviceSettings.ServiceName);
            });

            return services;
        }
    }
}

// Program.cs (después)
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddMongoDatabase(builder.Configuration);

var app = builder.Build();
app.MapControllers();
app.Run();
