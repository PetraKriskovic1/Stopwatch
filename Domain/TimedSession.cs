namespace Stopwatch.Domain;

public class TimedSession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Participant Participant { get; set; } = null!;
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset? EndTime { get; set; }
    public TimeSpan? ElapsedTime { get; set; }
    public bool IsRunning { get; set; }
    public bool IsReset { get; set; } = false;
}