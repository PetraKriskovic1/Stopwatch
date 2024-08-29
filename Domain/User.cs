using System.Security.Cryptography;
using System.Text;
using Ardalis.GuardClauses;

namespace Stopwatch.Domain;

public class User
{
    private readonly List<UserRole> _roles = new List<UserRole>();

    public Guid Id { get; set; } = Guid.NewGuid();
    
    public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTimeOffset UpdatedOn { get; set; }
    
    public IReadOnlyCollection<UserRole> Roles => _roles.AsReadOnly();

    public void AddRole(UserRole role)
    {
        Guard.Against.Null(role, nameof(role));
        _roles.Add(role);
    }

    public void RemoveRole(UserRole role)
    {
        Guard.Against.Null(role, nameof(role));
        _roles.Remove(role);
    }
    
    public static User Initialize(
        string firstName,
        string lastName,
        string emailAddress,
        string password
    ) =>
        new User
        {
            FirstName = Guard.Against.NullOrWhiteSpace(firstName),
            LastName = Guard.Against.NullOrWhiteSpace(lastName),
            EmailAddress = Guard.Against.NullOrWhiteSpace(emailAddress),
            Password = HashPassword(Guard.Against.StringTooShort(password, 8))
        };
    
    public bool HasRole<T>() where T : UserRole => _roles.OfType<T>().Any();
    
    public bool IsCorrectPassword(string password) => HashPassword(password) == Password;

    private static string HashPassword(string password) =>
        BitConverter.ToString(SHA512.HashData(Encoding.UTF8.GetBytes(password))).Replace("-", string.Empty);
}