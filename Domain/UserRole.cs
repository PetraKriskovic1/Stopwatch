namespace Stopwatch.Domain;

public abstract class UserRole
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;
    public string RoleName { get; set; } = null!;
    
    public User User { get; private set; } = null!;
}