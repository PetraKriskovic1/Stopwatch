using Ardalis.GuardClauses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Stopwatch.Domain;

namespace Stopwatch.Features.Stopwatch;

public sealed class GetAllTimedSessions
{
    public sealed record UserDto(string FirstName, string LastName);

    public sealed record TimedSessionDto(TimeSpan? ElapsedTime);

    public sealed record UserTimedSessionsDto(UserDto User, IEnumerable<TimedSessionDto> TimedSessions);

    public sealed record GetAllTimedSessionsResponse(IEnumerable<UserTimedSessionsDto> UsersWithTimedSessions);

    public sealed class GetAllTimedSessionsEndpoint : EndpointWithoutRequest<GetAllTimedSessionsResponse>
    {
        private readonly DatabaseContext _dbContext;

        public GetAllTimedSessionsEndpoint(DatabaseContext dbContext)
        {
            _dbContext = Guard.Against.Null(dbContext);
        }

        public override void Configure()
        {
            Get("stopwatch/all-timed-sessions");
            Roles(nameof(Administrator));
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var sessions = await _dbContext.TimedSessions
                .Include(s => s.Participant)
                .ThenInclude(p => p.User)
                .Select(s => new
                {
                    s.Id,
                    s.ElapsedTime,
                    User = new UserDto(
                        s.Participant.User.FirstName,
                        s.Participant.User.LastName
                    )
                })
                .ToListAsync(ct);

            var usersWithTimedSessions = sessions
                .GroupBy(s => s.User)
                .Select(g => new UserTimedSessionsDto(
                    g.Key,
                    g.Select(s => new TimedSessionDto(s.ElapsedTime))
                ))
                .ToList();

            await SendAsync(new GetAllTimedSessionsResponse(usersWithTimedSessions), cancellation: ct);
        }
    }
}
