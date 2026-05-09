using Microsoft.AspNetCore.Authorization;

// This class represents the authorization rule itself.
// It does NOT contain logic.
// It simply defines the requirement:
// "Owner OR Admin can access the Student resource."
public class StudentOwnerOrAdminRequirement : IAuthorizationRequirement
{

   //Important concept:

   //This class is intentionally empty
   //It represents what rule exists, not how it is enforced
}