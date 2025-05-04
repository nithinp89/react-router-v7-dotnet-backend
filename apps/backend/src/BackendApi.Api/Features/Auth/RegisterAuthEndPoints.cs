namespace BackendApi.Api.Features;

using BackendApi.Api.DTOs;
using BackendApi.Api.Validators;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BackendApi.Api.Features.Auth;

/// <summary>
/// Contains extension methods for registering authentication-related endpoints.
/// </summary>
public static class AuthEndPoints
{
  /// <summary>
  /// Registers authentication endpoints with the application's endpoint routing system.
  /// </summary>
  /// <param name="app">The <see cref="WebApplication"/> to add endpoints to.</param>
  public static void RegisterAuthEndPoints(this WebApplication app)
  {

    var auth = app.MapGroup("/api/auth");

    auth.MapPost("/get-token", AuthHandlers.GetToken);
  }


}