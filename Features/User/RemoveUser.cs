using Ardalis.GuardClauses;
using FastEndpoints;
using FluentValidation;
using Stopwatch.Domain;

namespace Stopwatch.Features.User;

public class RemoveUser
{
    public sealed record RemoveUserRequest(Guid Id);

    public sealed class RemoveUserEndpoint : Endpoint<RemoveUserRequest>
    {
        private readonly DatabaseContext _dbContext;

        public RemoveUserEndpoint(DatabaseContext dbContext) => _dbContext = Guard.Against.Null(dbContext);

        public override void Configure()
        {
            Post("users/delete/{id}");
            Roles(nameof(Administrator));
        }

        public override async Task HandleAsync(RemoveUserRequest request, CancellationToken ct)
        {
            var user = await _dbContext.Users
                .FindAsync(request.Id, ct);

            if (user == null) 
                ThrowError(ErrorKeys.UserNotFound);

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync(ct);

            await SendNoContentAsync(cancellation: ct);
        }
    }

    public sealed class RemoveUserValidator : Validator<RemoveUserRequest>
    {
        public RemoveUserValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(ErrorKeys.Required);
        }
    }
}