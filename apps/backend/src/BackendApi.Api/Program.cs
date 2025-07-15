using BackendApi.Api.DTOs;
using BackendApi.Api.DTOs.Auth;
using BackendApi.Api.Extensions;
using BackendApi.Api.Features;
using BackendApi.Api.Validators;
using BackendApi.Core.Common.Interfaces;
using BackendApi.Core.Constants;
using BackendApi.Infrastructure.Data;
using BackendApi.Infrastructure.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using Serilog;
using System.Reflection;
using System.Text;
using BackendApi.Application.Services.Identity;
using BackendApi.Core.Interfaces.Services.Identity;

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
    .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true, reloadOnChange: true)
    .Build();

var logFile = $"{configuration["LogFilePath"]}log_{Environment.MachineName}_{configuration["ApplicationName"]}_.txt";

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithThreadId()
    .Enrich.WithThreadName()
    .Enrich.WithClientIp()
    .Enrich.WithCorrelationId(headerName: "X-Correlation-Id", addValueIfHeaderAbsence: true)
    .Enrich.WithMachineName()
    .Enrich.WithProperty("Application", configuration["ApplicationName"])
    .Enrich.WithProperty("Environment", environment)
    .ReadFrom.Configuration(configuration)
    .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{Level:u4}] [{Environment}] [{Application}] [{MachineName}] [{ClientIp}] [{CorrelationId}] [{SourceContext}] [{RequestPath}] [{ThreadId}] [{Message} {Exception}]{NewLine}")
    .WriteTo.File(logFile, rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true, outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{Level:u4}] [{Environment}] [{Application}] [{MachineName}] [{ClientIp}] [{CorrelationId}] [{SourceContext}] [{RequestPath}] [{ThreadId}] [{Message} {Exception}]{NewLine}")
    //.WriteTo.Sink(new S3Sink(configuration, outputTemplate))
    .CreateBootstrapLogger();

Log.Information("Starting up");


var builder = WebApplication.CreateBuilder(args);

// HTTP Loging Config
builder.Services.AddHttpLogging(logging =>
{
  logging.LoggingFields = HttpLoggingFields.All;
  logging.RequestHeaders.Add("x-correlation-id");
  logging.RequestHeaders.Add("X-Forwarded-For");
  logging.RequestHeaders.Add("X-Forwarded-Proto");
  logging.RequestHeaders.Add("X-Forwarded-Port");
  logging.RequestHeaders.Add("X-Forwarded-Host");
  logging.RequestHeaders.Add("X-Forwarded-Server");
  logging.RequestHeaders.Add("X-Amzn-Trace-Id");
  logging.RequestHeaders.Add("Upgrade-Insecure-Requests");
  logging.RequestHeaders.Add("sec-ch-ua");
  logging.RequestHeaders.Add("sec-ch-ua-mobile");

  logging.ResponseHeaders.Add("x-correlation-id");
  logging.ResponseHeaders.Add("Pragma");
  logging.ResponseHeaders.Add("Cache-Control");
  logging.ResponseHeaders.Add("max-age");
});

builder.Services.AddControllers();

builder.Services.AddOpenApiDocument(options =>
{
  options.PostProcess = document =>
  {
    document.Info = new OpenApiInfo
    {
      Version = "v1",
      Title = "Backend API",
      Description = "Backend API Powered By Dotnet",
      TermsOfService = "https://path-to-licence-file.com",
      Contact = new OpenApiContact
      {
        Name = "Your Name",
        Url = "https://your-company.com"
      },
      License = new OpenApiLicense
      {
        Name = "MIT License",
        Url = "https://path-to-licence-file.com"
      }
    };
  };
});

builder.Services.AddOpenApi(); // Document name is v1

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ApplicationDbContext"))
          .EnableSensitiveDataLogging()
          .EnableDetailedErrors());

builder.Services.AddAuthorization();
builder.Services.AddAuthentication();
//.AddBearerToken(IdentityConstants.BearerScheme);

// Configure both cookie and JWT auth
builder.Services.AddAuthentication(options =>
{
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
//.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
//{
//    options.LoginPath = "/auth/login";
//    options.AccessDeniedPath = "/auth/denied";
//    options.Cookie.HttpOnly = true;
//    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
//    //options.Cookie.SameSite = SameSiteMode.None; // Allow cross-site cookies for development
//})
.AddJwtBearer(options =>
{
  var key = Encoding.UTF8.GetBytes("FGwuftblrSZdxWmCE!2=JlU7OdL7BJRMKdxPUGbNa-R-WBANhSgo4G=Yx=eGVnbz0fFw0bMd6-2bH6Mp1R?35P-b!HOtbVebxTQlbA/OL1jWZ85y?HukTSKblaUMmHfnBKat!a861Y1nXAvwHkZJ?UyLQbTcDMq/s7RnVL790ZP-f5A36qLj68kHXhn-w/v7LokJsWSZik8cMjO1rjba4Cf=GJqZ2e6XpWtyKcWt8Lemu7rj/Tqc4qJ8GVWCIF-g");
  options.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(key)
  };
});

// Add CORS services
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowFrontend", builder =>
  {
    builder
          .WithOrigins("http://localhost:3000", "http://localhost:8080", "http://localhost:5173")
          .AllowAnyMethod()
          .AllowAnyHeader()
          .AllowCredentials();
  });
});

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>(ApplicationIdentityConstants.SESSION_LOGIN_PROVIDER)
    .AddApiEndpoints();

// Register DummyUserCreator as a service
builder.Services.AddScoped<BackendApi.Api.Features.Auth.DummyUserCreator>();

// Register TokenStoreService for managing authentication tokens
builder.Services.AddScoped<ITokenStoreService, TokenStoreService>();

// Register JWT token service
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// For http request context accessing
builder.Services.AddHttpContextAccessor();

// Register FluentValidation validators
builder.Services.AddScoped<IValidator<GetTokenRequest>, GetTokenRequestValidator>();

// Log
builder.Host.UseSerilog();


var app = builder.Build();

// Create dummy user account at startup
using (var scope = app.Services.CreateScope())
{
    var dummyUserCreator = scope.ServiceProvider.GetRequiredService<BackendApi.Api.Features.Auth.DummyUserCreator>();
    await dummyUserCreator.CreateDummyUserAsync();
}

//app.UseHealthChecks("/health");
//app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
  app.MapOpenApi();
  app.UseOpenApi(settings =>
  {
      settings.PostProcess = (document, request) =>
      {
          document.Servers.Clear();
          document.Servers.Add(new NSwag.OpenApiServer { Url = "/api" });
      };
  });
  app.UseSwaggerUI();
}

// Log all requests and response.
app.UseHttpLogging();

// Streamlines framework logs into a single message per request, including path, method, timings, status code, and exception.
app.UseSerilogRequestLogging();


//app.UseExceptionHandler(options => {  });

app.UsePathBase("/api");

// Use CORS before authentication
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => "Hello World!");
app.RegisterAuthEndPoints();

//TestCases.Run();

app.Run();
