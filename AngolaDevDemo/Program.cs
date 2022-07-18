using AngolaDevDemo.Extensions;
using AngolaDevDemo.Models;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AngolaDevContext>(x => x.UseSqlServer(connectionString));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

builder.Services.AddOpenTelemetryTracing(traceProvider =>
{
    traceProvider
        .AddSource(OpenTelemetryConfig.ServiceName)
        .SetResourceBuilder(
            ResourceBuilder.CreateDefault()
                .AddService(serviceName: OpenTelemetryConfig.ServiceName,
                    serviceVersion: OpenTelemetryConfig.ServiceVersion))
        .AddHttpClientInstrumentation()
        .AddOtlpExporter(opts => { opts.Endpoint = new Uri("http://localhost:4317/"); })
        .AddAspNetCoreInstrumentation()
        .AddSqlClientInstrumentation()
        .AddConsoleExporter(); 
});



var app = builder.Build();
ActivitySource _activitySource = OpenTelemetryConfig.CreateActivitySource();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/Palestrantes", (AngolaDevContext db) =>
{
    using var activity = _activitySource.StartActivity("Retornar todos palestrantes");

    return db.Palestrante.ToList();
});

app.MapGet("/PalestrantesById", (AngolaDevContext db, Guid id) =>
{
    using var activity = _activitySource.StartActivity("Retornar palestrante por Id");

    var palestrante = db.Palestrante.Find(id);
    return Results.Ok(palestrante);
});

app.MapPost("/AddPalestrantes", (AngolaDevContext db, Palestrante palestrante) =>
{
    using var activity = _activitySource.StartActivity("Adicionar palestrante");

    db.Palestrante.Add(palestrante);
    db.SaveChanges();
    return Results.Created($"/PalestrantesById/{ palestrante.Id}", palestrante);
});

app.MapPut("/UpdatePalestrante/", (AngolaDevContext db, Palestrante palestrante) =>
{
    using var activity = _activitySource.StartActivity("Actualizar palestrante");

    var p = db.Palestrante.AsNoTracking().FirstOrDefault(x => x.Id == palestrante.Id);
    p.Nome = palestrante.Nome;
    p.Tema = palestrante.Tema;
    db.Palestrante.Update(palestrante);
    db.SaveChanges();
    return Results.NoContent();
});

app.MapDelete("/DeletePalestrante/", (AngolaDevContext db, Guid id) =>
{
    using var activity = _activitySource.StartActivity("Remover palestrante");

    var palestrante = db.Palestrante.Find(id);
    db.Palestrante.Remove(palestrante);
    db.SaveChanges();
    return Results.NoContent();
});

app.Run();
