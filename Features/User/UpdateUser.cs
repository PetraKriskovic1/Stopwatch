using Ardalis.GuardClauses;
using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Stopwatch.Domain;

namespace Stopwatch.Features.User;

public class UpdateUser
{
    public sealed record UpdateUserRequest(Guid Id, string FirstName, string LastName, string EmailAddress);

    public sealed record UpdateUserResponse(Guid Id);

    public sealed class UpdateUserEndpoint : Endpoint<UpdateUserRequest, UpdateUserResponse>
    {
        private readonly DatabaseContext _dbContext;

        public UpdateUserEndpoint(DatabaseContext dbContext) => _dbContext = Guard.Against.Null(dbContext);

        public override void Configure()
        {
            Put("users/{Id}");
            Roles(nameof(Administrator));
        }

        public override async Task HandleAsync(UpdateUserRequest request, CancellationToken ct)
        {
            var user = await _dbContext.Users
                .FindAsync(request.Id, ct);

            if (user == null) 
                ThrowError(ErrorKeys.UserNotFound);

            if (user.EmailAddress != request.EmailAddress &&
                await _dbContext.Users.AnyAsync(x => x.EmailAddress == request.EmailAddress, cancellationToken: ct))
                ThrowError(ErrorKeys.UserEmailAddressMustBeUnique);

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.EmailAddress = request.EmailAddress;
            user.UpdatedOn = DateTimeOffset.Now;

            await _dbContext.SaveChangesAsync(ct);

            await SendAsync(new UpdateUserResponse(user.Id), cancellation: ct);
        }
    }

    public sealed class UpdateUserValidator : Validator<UpdateUserRequest>
    {
        public UpdateUserValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage(ErrorKeys.Required);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage(ErrorKeys.Required);

            RuleFor(x => x.EmailAddress)
                .NotEmpty().WithMessage(ErrorKeys.Required)
                .EmailAddress().WithMessage(ErrorKeys.Invalid);
        }
    }
}