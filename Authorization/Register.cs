using Ardalis.GuardClauses;
using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Stopwatch.Authorization;

public static class Register
{
    public sealed record RegisterRequest(string FirstName, string LastName, string EmailAddress, string Password);

    public sealed record RegisterResponse(Guid UserId);

    public class RegisterEndpoint : Endpoint<RegisterRequest, RegisterResponse>
    {
        private readonly DatabaseContext _dbContext;

        public RegisterEndpoint(DatabaseContext dbContext)
        {
            _dbContext = Guard.Against.Null(dbContext);
        }

        public override void Configure()
        {
            Post("register");
            AllowAnonymous();
        }

        public override async Task HandleAsync(RegisterRequest request, CancellationToken ct)
        {
            if (await _dbContext.Users.AnyAsync(x => x.EmailAddress == request.EmailAddress, cancellationToken: ct))
                ThrowError(ErrorKeys.UserEmailAddressMustBeUnique);

            var user = Domain.User.Initialize(
                request.FirstName,
                request.LastName,
                request.EmailAddress,
                request.Password
            );

            _dbContext.Add(user);
            await _dbContext.SaveChangesAsync(ct);

            await SendAsync(new RegisterResponse(user.Id), cancellation: ct);
        }
    }

    public sealed class RegisterValidator : Validator<RegisterRequest>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage(ErrorKeys.Required);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage(ErrorKeys.Required);

            RuleFor(x => x.EmailAddress)
                .NotEmpty().WithMessage(ErrorKeys.Required)
                .EmailAddress().WithMessage(ErrorKeys.Invalid);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(ErrorKeys.Required)
                .MinimumLength(8).WithMessage(ErrorKeys.Invalid);
        }
    }
}
