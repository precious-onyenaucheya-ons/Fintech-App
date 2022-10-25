using FluentValidation.AspNetCore;
using Payment.API.Extensions;
using Payment.Core;
using Payment.Core.Interfaces;
using Payment.Infrastructure;
using Serilog;

// Add Serilog setup
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var isDevelopment = environment == Environments.Development;

IConfiguration config = ConfigurationSetup.GetConfig(isDevelopment);
LogSettings.SetUpSerilog(config);

try
{
    Log.Information("Application is starting...");
    var builder = WebApplication.CreateBuilder(args);
    var configuration = builder.Configuration;

    // Add services to the container.

    builder.Services.AddControllers();
    builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerExtension();
    builder.Services.AddSeriLogExtension();
    builder.Services.AddCors();
    builder.Services.AddPaymentInfrastructure(config);
    builder.Services.AddApplicationLayer(config);

    var app = builder.Build();

    // Configure Seeder in the pipeline
   /* using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ISeedPaymentInitialData>();
    db.Seed().GetAwaiter().GetResult();*/

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    if (app.Environment.IsProduction())
    {
        var port = Environment.GetEnvironmentVariable("PORT");
        app.Urls.Add($"http://*:{port}");
    }

    app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

    app.UseGlobalErrorHandlerMiddleWare();

    app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());//.WithOrigins("http://localhost:3000")

    app.UseSwaggerExtension();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e.Message, e.StackTrace, "The application failed to start correctly");
}
finally
{
    Log.CloseAndFlush();
}
