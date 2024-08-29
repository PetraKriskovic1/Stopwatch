using Ardalis.GuardClauses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Stopwatch.Domain;

namespace Stopwatch.Features.Stopwatch;

public sealed class ResetStopwatch
{
    public sealed record ResetStopwatchResponse(Guid SessionId);

    public sealed class ResetStopwatchEndpoint : EndpointWithoutRequest<ResetStopwatchResponse>
    {
        private readonly DatabaseContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ResetStopwatchEndpoint(DatabaseContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = Guard.Against.Null(dbContext);
            _httpContextAccessor = Guard.Against.Null(httpContextAccessor);
        }

        public override void Configure()
        {
            Post("stopwatch/reset");
            Roles(nameof(Participant));
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var userId = GetCurrentUserId();

            var session = await _dbContext.TimedSessions
                .Where(s => s.Participant.User.Id == userId && !s.IsRunning && !s.IsReset)
                .OrderByDescending(s => s.StartTime)
                .FirstOrDefaultAsync(ct);

            if (session == null)
            {
                ThrowError(ErrorKeys.NoActiveSession);
            }

            session.IsReset = true;
            
            _dbContext.TimedSessions.Update(session);
            await _dbContext.SaveChangesAsync(ct);

            await SendAsync(new ResetStopwatchResponse(session.Id), cancellation: ct);
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("UserId")?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }
    }
}