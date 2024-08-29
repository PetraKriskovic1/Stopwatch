using Ardalis.GuardClauses;
using FastEndpoints;
using FluentValidation;
using Stopwatch.Domain;

namespace Stopwatch.Features.UserRole;

public class AddRole
{
    public sealed record AddRoleRequest(Guid UserId, string RoleName);

    public sealed record AddRoleResponse(Guid UserId);

    public sealed class AddRoleEndpoint : Endpoint<AddRoleRequest, AddRoleResponse>
    {
        private readonly DatabaseContext _dbContext;

        public AddRoleEndpoint(DatabaseContext dbContext) => _dbContext = Guard.Against.Null(dbContext);

        public override void Configure()
        {
            Post("users/addRole");
            Roles(nameof(Administrator));
        }
        
        public override async Task HandleAsync(AddRoleRequest request, CancellationToken ct)
        {
            var user = await _dbContext.Users
                .FindAsync(request.UserId, ct);

            if (user == null) 
                ThrowError(ErrorKeys.UserNotFound);

            if (request.RoleName == "Administrator")
            {
                if (!user.HasRole<Administrator>())
                {
                    var admin = Administrator.Initialize();
                    _dbContext.UserRoles.Add(admin);
                    user.AddRole(admin);
                }
            }
            else if (request.RoleName == "Participant")
            {
                if (!user.HasRole<Participant>())
                {
                    var participant = Participant.Initialize();
                    _dbContext.UserRoles.Add(participant);
                    user.AddRole(participant);
                }
            }
            else
                ThrowError(ErrorKeys.InvalidUserRole);

            await _dbContext.SaveChangesAsync(ct);

            await SendAsync(new AddRoleResponse(request.UserId), cancellation: ct);
        }
    }
    
    public sealed class AddRoleValidator : Validator<AddRoleRequest>
    {
        public AddRoleValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage(ErrorKeys.Required);
            
            RuleFor(x => x.RoleName)
                .NotEmpty().WithMessage(ErrorKeys.Required);
        }
    }
}