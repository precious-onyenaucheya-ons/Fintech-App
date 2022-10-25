using FluentValidation;
using Microsoft.Extensions.Options;
using NotificationServices.API;
using NotificationServices.API.Extensions;
using NotificationServices.API.Middlewares;
using NotificationServices.Core.AppSettings;
using NotificationServices.Core.Interfaces;
using NotificationServices.Core.Services;
using NotificationServices.Infrastructure.ExternalServices;
using NotificationServices.Infrastructure.Repository;
using NotificationServicesAPI.Core.Interfaces;
using Serilog;


try
{
    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    var isDevelopment = environment == Environments.Development;
    IConfiguration config = ConfigurationSetUp.GetConfig(isDevelopment);
    LogSettings.SetUpSerilog(config);
    Log.Logger.Information("start notification services");

    //builder class information

    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddCors(options => {
        options.AddPolicy("AllowAll", builder => builder.AllowAnyMethod().AllowAnyHeader().AllowCredentials().SetIsOriginAllowed((hosts) => true));
    });
    var configuration = builder.Configuration;
  

    // Add services to the container.  
    builder.Services.AddSingleton(Log.Logger);
    builder.Services.AddSwaggerExtension();
    
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddCors();
    builder.Services.AddValidatorsFromAssemblyContaining<Program>();
    builder.Services.AddDbContextAndConfigurations(builder.Environment, builder.Configuration);
    builder.Logging.ClearProviders();
    builder.Logging.AddSerilog();

    builder.Services.AddSingleton(Log.Logger);

    builder.Services.Configure<NotificationSettings>(configuration.GetSection(nameof(NotificationSettings)));
    builder.Services.Configure<RavenDbConfigurations>(configuration.GetSection(nameof(RavenDbConfigurations)));
    builder.Services.AddScoped<INotificationService, NotificationService>();
    builder.Services.AddScoped<IEmailService, EmailService>();
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    builder.Services.AddScoped<IPushNotificationServices, PushNotificationServices>();
    builder.Services.AddSignalR();

    builder.Services.AddScoped(v => v.GetRequiredService<IOptions<NotificationSettings>>().Value);
    builder.Services.AddScoped(v => v.GetRequiredService<IOptions<RavenDbConfigurations>>().Value);


    var app = builder.Build();
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "NotificationServices API");
        });
    }
    app.UseCors("AllowAll");
    app.UseHttpsRedirection();

    app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

    app.UseMiddleware<ExceptionMiddleware>();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.MapHub<MessageHub>("/notification");

    app.Run();
}
catch (Exception e)
{
    Log.Logger.Fatal(e.StackTrace, "The application failed to start correctly");
}
finally
{
    Log.CloseAndFlush();
}



