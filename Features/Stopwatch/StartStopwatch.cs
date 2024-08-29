using Ardalis.GuardClauses;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Stopwatch.Domain;

namespace Stopwatch.Features.Stopwatch;

public sealed class StartStopwatch
{
    public sealed record StartStopwatchResponse(Guid SessionId);

    public sealed class StartStopwatchEndpoint : EndpointWithoutRequest<StartStopwatchResponse>
    {
        private readonly DatabaseContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StartStopwatchEndpoint(DatabaseContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = Guard.Against.Null(dbContext);
            _httpContextAccessor = Guard.Against.Null(httpContextAccessor);
        }

        public override void Configure()
        {
            Post("stopwatch/start");
            Roles(nameof(Participant));
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var userId = GetCurrentUserId();

            var activeSession = await _dbContext.TimedSessions
                .Include(s => s.Participant)
                .Where(s => s.Participant.User.Id == userId && s.IsRunning)
                .OrderByDescending(s => s.StartTime)
                .FirstOrDefaultAsync(ct);

            if (activeSession != null)
            {
                ThrowError(ErrorKeys.AlreadyRunningSession);
            }

            var session = await _dbContext.TimedSessions
                .Include(s => s.Participant)
                .Where(s => s.Participant.User.Id == userId && !s.IsReset)
                .OrderByDescending(s => s.StartTime)
                .FirstOrDefaultAsync(ct);

            if (session != null)
            {
                session.IsRunning = true;
            }
            else
            {
                var userRole = await _dbContext.UserRoles
                    .Include(ur => ur.User)
                    .FirstOrDefaultAsync(ur => ur.User.Id == userId && ur.RoleName == "Participant", ct);

                if (userRole == null)
                {
                    ThrowError(ErrorKeys.UnauthorizedRole);
                }

                session = new TimedSession
                {
                    Participant = (Participant)userRole, // Assign the Participant entity
                    StartTime = DateTimeOffset.UtcNow,
                    IsRunning = true,
                    IsReset = false
                };

                _dbContext.TimedSessions.Add(session);
            }

            await _dbContext.SaveChangesAsync(ct);

            await SendAsync(new StartStopwatchResponse(session.Id), cancellation: ct);
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("UserId")?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }
    }
}