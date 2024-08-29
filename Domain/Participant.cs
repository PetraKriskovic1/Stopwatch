namespace Stopwatch.Domain;

public class Participant : UserRole
{
    public List<TimedSession> TimedSessions { get; set; } = new List<TimedSession>();
    
    public static Participant Initialize() => new Participant { RoleName = "Participant" };
    
}