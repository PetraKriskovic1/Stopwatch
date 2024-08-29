using Ardalis.GuardClauses;
using FastEndpoints;
using FluentValidation;
using Stopwatch.Domain;

namespace Stopwatch.Features.User;

public class GetUser
{
    public sealed record GetUserRequest(Guid Id);
    
    public sealed record GetUserResponse(string FirstName, string LastName, string EmailAddress);

    public sealed class GetUserEndpoint : Endpoint<GetUserRequest, GetUserResponse>
    {
        private readonly DatabaseContext _dbContext;

        public GetUserEndpoint(DatabaseContext dbContext) => _dbContext = Guard.Against.Null(dbContext);

        public override void Configure()
        {
            Post("users/{id}");
            Roles(nameof(Administrator));
        }

        public override async Task HandleAsync(GetUserRequest request, CancellationToken ct)
        {
            var user = await _dbContext.Users
                .FindAsync(request.Id, ct);

            if (user == null) 
                ThrowError(ErrorKeys.UserNotFound);

            var response = new GetUserResponse(
                user.FirstName,
                user.LastName,
                user.EmailAddress
            );

            await SendAsync(response, cancellation: ct);
        }
    }
    
    public sealed class GetUserValidator : Validator<GetUserRequest>
    {
        public GetUserValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(ErrorKeys.Required);
        }
    }
}