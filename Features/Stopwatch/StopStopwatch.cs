using Ardalis.GuardClauses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Stopwatch.Domain;

namespace Stopwatch.Features.Stopwatch;

public sealed class StopStopwatch
{
    public sealed record StopStopwatchResponse(Guid SessionId, TimeSpan ElapsedTime);

    public sealed class StopStopwatchEndpoint : EndpointWithoutRequest<StopStopwatchResponse>
    {
        private readonly DatabaseContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StopStopwatchEndpoint(DatabaseContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = Guard.Against.Null(dbContext);
            _httpContextAccessor = Guard.Against.Null(httpContextAccessor);
        }

        public override void Configure()
        {
            Post("stopwatch/stop");
            Roles(nameof(Participant));
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var userId = GetCurrentUserId();

            var session = await _dbContext.TimedSessions
                .Where(s => s.Participant.User.Id == userId && s.IsRunning)
                .OrderByDescending(s => s.StartTime)
                .FirstOrDefaultAsync(ct);

            if (session == null)
            {
                ThrowError(ErrorKeys.NoActiveSession);
            }

            session.EndTime = DateTimeOffset.UtcNow;
            session.IsRunning = false;

            // Calculate elapsed time
            var elapsedTime = session.EndTime.Value - session.StartTime;

            session.ElapsedTime = elapsedTime;

            _dbContext.TimedSessions.Update(session);
            await _dbContext.SaveChangesAsync(ct);

            await SendAsync(new StopStopwatchResponse(session.Id, elapsedTime), cancellation: ct);
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("UserId")?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }
    }
}