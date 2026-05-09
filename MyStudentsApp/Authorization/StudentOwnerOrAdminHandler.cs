using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

// This authorization handler enforces the ownership rule for User resources.
// It checks whether the current user is either:
// - An Admin (full access), OR
// - The owner of the User record being requested
public class StudentOwnerOrAdminHandler: AuthorizationHandler<StudentOwnerOrAdminRequirement,int>
{
    protected override Task HandleRequirementAsync
        (AuthorizationHandlerContext context,StudentOwnerOrAdminRequirement requirement,int authenticatedUsertId)
    {
        // Admin override
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // Ownership check
        var UserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (int.TryParse(UserId, out int currentUsertId) &&
            currentUsertId == authenticatedUsertId)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}