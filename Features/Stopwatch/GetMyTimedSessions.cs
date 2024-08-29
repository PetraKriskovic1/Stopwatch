using Ardalis.GuardClauses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Stopwatch.Domain;

namespace Stopwatch.Features.Stopwatch;

public sealed class GetMyTimedSessions
{
    public sealed record TimedSessionsDto(TimeSpan? ElapsedTime);

    public sealed record GetMyTimedSessionsResponse(IEnumerable<TimedSessionsDto> TimedSessions, TimeSpan TotalElapsedTime);

    public sealed class GetMyTimedSessionsEndpoint : EndpointWithoutRequest<GetMyTimedSessionsResponse>
    {
        private readonly DatabaseContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetMyTimedSessionsEndpoint(DatabaseContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = Guard.Against.Null(dbContext);
            _httpContextAccessor = Guard.Against.Null(httpContextAccessor);
        }

        public override void Configure()
        {
            Get("stopwatch/my-timed-sessions");
            Roles(nameof(Participant));
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var userId = GetCurrentUserId();

            var sessions = await _dbContext.TimedSessions
                .Include(s => s.Participant)
                .Where(s => s.Participant.User.Id == userId)
                .Select(s => new TimedSessionsDto(s.ElapsedTime))
                .ToListAsync(ct);

            var totalElapsedTime = sessions
                .Where(s => s.ElapsedTime.HasValue)
                .Select(s => s.ElapsedTime!.Value)
                .Aggregate(TimeSpan.Zero, (acc, elapsed) => acc.Add(elapsed));

            await SendAsync(new GetMyTimedSessionsResponse(sessions, totalElapsedTime), cancellation: ct);
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("UserId")?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }
    }
}

