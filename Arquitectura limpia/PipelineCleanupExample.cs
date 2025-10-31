// ==========================================
// ANTES: Pipeline mezclado en Program.cs
// ==========================================

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();


// ==========================================
// DESPUÉS: Usando PIPELINE CLEANUP
// ==========================================

namespace Catalog.API.Extensions
{
    public static class PipelineExtensions
    {
        public static WebApplication UseApiPipeline(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            return app;
        }
    }
}

// Program.cs (después)
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddSwaggerGen();

var app = builder.Build();
app.UseApiPipeline();
app.Run();
