using Ardalis.GuardClauses;
using FastEndpoints;
using FastEndpoints.Security;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Stopwatch.Authorization;

public static class Login
{
    public sealed record LoginRequest(string EmailAddress, string Password);

    public sealed record LoginResponse(string Token);

    public class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
    {
        private readonly DatabaseContext _dbContext;
        private readonly JwtConfiguration _jwtConfiguration;

        public LoginEndpoint(DatabaseContext dbContext, IOptions<JwtConfiguration> jwtConfiguration)
        {
            _dbContext = Guard.Against.Null(dbContext);
            _jwtConfiguration = Guard.Against.Null(jwtConfiguration.Value);
        }

        public override void Configure()
        {
            Post("login");
            AllowAnonymous();
        }

        public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
        {
            var user = await _dbContext.Users.Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.EmailAddress == req.EmailAddress, ct);

            if (user == null)
                ThrowError(ErrorKeys.InvalidEmailAddress);

            if (!user.IsCorrectPassword(req.Password)) ThrowError(ErrorKeys.InvalidPassword);

            var jwtToken = JwtBearer.CreateToken(
                o =>
                {
                    o.SigningKey = _jwtConfiguration.SigningKey;
                    o.ExpireAt = DateTime.UtcNow.AddDays(1);
                    o.User.Roles.AddRange(user.Roles.Select(r => r.RoleName));
                    o.User.Claims.Add(("UserId", user.Id.ToString()));
                    o.User.Claims.Add(("FullName", $"{user.FirstName} {user.LastName}"));
                });

            await SendAsync(new LoginResponse(jwtToken));
        }
    }

    public sealed class LoginValidator : Validator<LoginRequest>
    {
        public LoginValidator()
        {
            RuleFor(x => x.EmailAddress)
                .NotEmpty().WithMessage(ErrorKeys.Required)
                .EmailAddress().WithMessage(ErrorKeys.Invalid);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(ErrorKeys.Required)
                .MinimumLength(8).WithMessage(ErrorKeys.Invalid);
        }
    }
}