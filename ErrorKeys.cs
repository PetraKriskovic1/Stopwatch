using FluentValidation.Results;

namespace Stopwatch;

public static class ErrorKeys
{
    public const string Required = nameof(Required);
    public const string Invalid = nameof(Invalid);
    public const string UserEmailAddressMustBeUnique = nameof(UserEmailAddressMustBeUnique);
    public const string InvalidEmailAddress = nameof(InvalidEmailAddress);
    public const string InvalidPassword = nameof(InvalidPassword);
    public const string UserNotFound = nameof(UserNotFound);
    public const string InvalidUserRole = nameof(InvalidUserRole);
    public const string UnauthorizedRole = nameof(UnauthorizedRole);
    public const string AlreadyRunningSession = nameof(AlreadyRunningSession);
    public const string NoActiveSession = nameof(NoActiveSession);
}