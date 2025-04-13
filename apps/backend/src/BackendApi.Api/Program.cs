using Microsoft.AspNetCore.Identity;
using BackendApi.Core.Common.Interfaces;
using BackendApi.Infrastructure.Identity;
using BackendApi.Infrastructure.Data;
using BackendApi.Api.Extensions;
using NSwag;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using BackendApi.Api;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApiDocument(options => {
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

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ApplicationDbContext")));

//builder.Services.AddAuthorization();
//builder.Services.AddAuthentication()
//.AddBearerToken(IdentityConstants.BearerScheme);

// Configure both cookie and JWT auth
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/auth/login";
    options.AccessDeniedPath = "/auth/denied";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    //options.Cookie.SameSite = SameSiteMode.None; // Allow cross-site cookies for development
})
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
/*builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder
            .WithOrigins("http://localhost:3000", "http://localhost:8080", "http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});*/

builder.Services.AddIdentityCore<ApplicationUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddApiEndpoints();

var app = builder.Build();

//app.UseHealthChecks("/health");
//app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseOpenApi();
    app.UseSwaggerUI();
}

//app.UseExceptionHandler(options => {  });

// Use CORS before authentication
//app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => "Hello World!");

app.MapGroup("/identity").MapCustomIdentityApi<ApplicationUser>();

//app.MapEndpoints();

//TestCases.Run();

app.Run();
