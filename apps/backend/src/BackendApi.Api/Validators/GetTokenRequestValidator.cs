using FluentValidation;
using BackendApi.Api.DTOs;
using BackendApi.Api.DTOs.Auth;

namespace BackendApi.Api.Validators
{
    /// <summary>
    /// Validator for <see cref="GetTokenRequest"/> DTO. Defines validation rules for the login request.
    /// </summary>
    public class GetTokenRequestValidator : AbstractValidator<GetTokenRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetTokenRequestValidator"/> class and defines validation rules for email and password.
        /// </summary>
        public GetTokenRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(9).WithMessage("Password must be at least 9 characters long.");
        }
    }
}
