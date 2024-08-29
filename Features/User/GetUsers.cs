using Ardalis.GuardClauses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Stopwatch.Domain;

namespace Stopwatch.Features.User;

public class GetUsers
{
    public sealed record GetUsersResponse(string FirstName, string LastName, string EmailAddress);

    public sealed class GetUsersEndpoint : EndpointWithoutRequest<List<GetUsersResponse>>
    {
        private readonly DatabaseContext _dbContext;

        public GetUsersEndpoint(DatabaseContext dbContext) => _dbContext = Guard.Against.Null(dbContext);

        public override void Configure()
        {
            Get("users");
            Roles(nameof(Administrator));
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var response = await _dbContext.Users
                .Select(u => new GetUsersResponse(
                    u.FirstName,
                    u.LastName,
                    u.EmailAddress
                ))
                .ToListAsync(ct);

            await SendAsync(response, cancellation: ct);
        }
    }
}