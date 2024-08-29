using Ardalis.GuardClauses;
using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Stopwatch.Domain;

namespace Stopwatch.Features.UserRole;

public class RemoveRole
{
    public sealed record RemoveRoleRequest(Guid UserId, string RoleName);

    public sealed record RemoveRoleResponse(Guid UserId);

    public sealed class RemoveRoleEndpoint : Endpoint<RemoveRoleRequest, RemoveRoleResponse>
    {
        private readonly DatabaseContext _dbContext;

        public RemoveRoleEndpoint(DatabaseContext dbContext) => _dbContext = Guard.Against.Null(dbContext);

        public override void Configure()
        {
            Post("users/removeRole");
            Roles(nameof(Administrator));
        }
        
        public override async Task HandleAsync(RemoveRoleRequest request, CancellationToken ct)
        {
            var user = await _dbContext.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == request.UserId, ct);

            if (user == null) 
                ThrowError(ErrorKeys.UserNotFound);

            var roleToRemove = user.Roles.FirstOrDefault(r => r.RoleName == request.RoleName);
            if (roleToRemove != null)
            {
                _dbContext.UserRoles.Remove(roleToRemove);
                user.RemoveRole(roleToRemove);
            }

            await _dbContext.SaveChangesAsync(ct);

            await SendAsync(new RemoveRoleResponse(request.UserId), cancellation: ct);
        }
    }
    
    public sealed class RemoveRoleValidator : Validator<RemoveRoleRequest>
    {
        public RemoveRoleValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage(ErrorKeys.Required);

            RuleFor(x => x.RoleName)
                .NotEmpty().WithMessage(ErrorKeys.Required);
        }
    }
}
