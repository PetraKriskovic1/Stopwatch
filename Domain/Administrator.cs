namespace Stopwatch.Domain;

public class Administrator : UserRole
{
    public static Administrator Initialize() => new Administrator { RoleName = "Administrator" };
}