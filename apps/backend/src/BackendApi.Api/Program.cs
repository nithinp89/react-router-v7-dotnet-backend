using Microsoft.AspNetCore.Identity;
using BackendApi.Core.Common.Interfaces;
using BackendApi.Infrastructure.Identity;
using BackendApi.Infrastructure.Data;
using BackendApi.Api.Extensions;
using NSwag;
using Microsoft.EntityFrameworkCore;
using BackendApi.Api;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApiDocument(options => {
    options.PostProcess = document =>
    {
        document.Info = new OpenApiInfo
        {
            Version = "v1",
            Title = "Backend API",
            Description = "NiyaX AI Powered OpenSource CRM",
            TermsOfService = "https://github.com/nithinp89/niya-api#AGPL-3.0-1-ov-file",
            Contact = new OpenApiContact
            {
                Name = "Nithin Prathapan",
                Url = "https://github.com/nithinp89/niya-api"
            },
            License = new OpenApiLicense
            {
                Name = "AGPL-3.0 license",
                Url = "https://github.com/nithinp89/niya-api#AGPL-3.0-1-ov-file"
            }
        };
    };
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ApplicationDbContext")));

builder.Services.AddAuthorization();
builder.Services.AddAuthentication()
    .AddBearerToken(IdentityConstants.BearerScheme);

builder.Services.AddIdentityCore<ApplicationUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddApiEndpoints();

var app = builder.Build();

//app.UseHealthChecks("/health");
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseOpenApi();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(options => { });

app.MapGet("/", () => "Hello World!");

app.MapGroup("/identity").MapCustomIdentityApi<ApplicationUser>();

//app.MapEndpoints();

TestCases.Run();

app.Run();

