using APIBusinessLayer;
using APIDataAccessLayer;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyStudentsApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("CoreLimiter")] //Now EndPoints are protected.
    public class UsersAPIController : ControllerBase
    {
        private readonly ILogger<UsersAPIController> _logger;

        public UsersAPIController(ILogger<UsersAPIController> logger)
        {
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("All", Name = "GetAllUsers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUsers()
        {
            
            List<UserDTO> UsersList = await clsUser.GetAllUsersAsync();

            if (UsersList == null || UsersList.Count == 0)
            {
                return NotFound("No users found in the system.");
            }

            return Ok(UsersList);
        }

        [HttpGet("{id}", Name = "GetUserById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // Added 403 for security
        public async Task<ActionResult<UserDTO>> GetUserById(int id, [FromServices] IAuthorizationService authorizationService)
        {
            // 1. Basic validation for the ID
            if (id < 1) return BadRequest($"Invalid ID {id}");

            // 2. Search for the user in the database
            clsUser user = await clsUser.FindAsync(id);
            // 3. Check if the user exists in our system
            if (user == null) return NotFound("User not found.");

            //Policy-Based Authorization 
            var authResult = await authorizationService.AuthorizeAsync(User,id,"StudentOwnerOrAdmin");

            if (!authResult.Succeeded)
                return Forbid(); // 403
            // If everything is okay, return the data
            return Ok(user.UDTO);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost(Name = "AddNewUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDTO>> AddUser(CreateUserDTO createUserDto)
        {
            if (createUserDto == null || createUserDto.PersonID < 1)
                return BadRequest("Invalid data.");

            clsPerson person = await clsPerson.GetPersonByIdAsync(createUserDto.PersonID);

            if (person == null)
            {
                return NotFound($"Person with ID {createUserDto.PersonID} was not found.");
            }

           
            if (await clsUser.IsUserExistByPersonIDAsync(createUserDto.PersonID))
            {
                return BadRequest("A user account already exists for this person.");
            }

           
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(createUserDto.PasswordHash);

            clsUser user = new clsUser();
            user.PersonID = person.PersonId;
            user.UserName = createUserDto.UserName;
            user.PasswordHash = hashedPassword;
            user.Role = createUserDto.Role;

            if (await user.SaveAsync())
            {
               user.FullName = person.FirstName + " " + person.LastName;
                user.Email = person.Email;
                user.IsActive = person.IsActive;
                return CreatedAtRoute("GetUserById", new { id = user.UserID }, user.UDTO);
            }

            return StatusCode(500, "Internal server error while creating user.");
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}", Name = "UpdateUserField")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDTO>> UpdateUserField(int id, clsUser.enUpdateType updateType, [FromBody] string newValue)
        {
            // Capture IP and AdminId once for tracing (helps investigations later)
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "unknown";

            if (id < 1)
            {
                _logger.LogWarning(
                    "Admin action blocked (invalid id). AdminId={AdminId}, Action=UpdateUser, TargetId={TargetId}, IP={IP}",
                    adminId, id, ip
                );
                return BadRequest($"Invalid ID {id}");
            }

            clsUser user = await clsUser.FindAsync(id);

            if (user == null)
            {
                // Audit: admin attempted to update a non-existing user
                _logger.LogWarning(
                    "Admin action failed (target not found). AdminId={AdminId}, Action=UpdateUser, TargetId={TargetId}, IP={IP}",
                    adminId, id, ip
                );
                return NotFound($"User with ID {id} not found.");
            }

            // ==========================================
            // Audit BEFORE updating (Attempt started)
            // ==========================================
            _logger.LogInformation(
                "Admin action started. AdminId={AdminId}, Action=UpdateUser, TargetId={TargetId}, UpdateType={UpdateType}, IP={IP}",
                adminId, user.UserID, updateType.ToString(), ip
            );

            user.UpdateType = updateType;

            switch (updateType)
            {
                case clsUser.enUpdateType.UserName:
                    if (string.IsNullOrEmpty(newValue) || newValue == "string")
                        return BadRequest("UserName cannot be empty.");
                    user.UserName = newValue;
                    break;

                case clsUser.enUpdateType.Password:
                    if (string.IsNullOrEmpty(newValue) || newValue == "string")
                        return BadRequest("Password cannot be empty.");
                   
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newValue);
                    break;

                case clsUser.enUpdateType.Role:
                    if (string.IsNullOrEmpty(newValue) || newValue == "string")
                        return BadRequest("Role cannot be empty.");
                    user.Role = newValue;
                    break;

                default:
                    return BadRequest("Invalid Update Type.");
            }


            // Save changes to database
            if (!await user.SaveAsync())
            {
                // Audit: If database fails to save the modifications
                _logger.LogError(
                    "Admin action failed during save. AdminId={AdminId}, Action=UpdateUser, TargetId={TargetId}, IP={IP}",
                    adminId, id, ip
                );
                return StatusCode(500, "Database error: Could not update the user.");
            }

            // Audit: Success
            _logger.LogInformation(
                 "Admin action succeeded. AdminId={AdminId}, Action=UpdateUser, TargetId={TargetId}, IP={IP}",
                 adminId, id, ip
             );

            return Ok(user.UDTO);
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteUser(int id)
        {
            // Capture IP and AdminId once for tracing
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "unknown";

            if (id < 1)
            {
                _logger.LogWarning(
                    "Admin action blocked (invalid id). AdminId={AdminId}, Action=DeleteUser, TargetId={TargetId}, IP={IP}",
                    adminId, id, ip
                );
                return BadRequest($"Invalid ID {id}");
            }

            var user = await clsUser.FindAsync(id);

            if (user == null)
            {
                // Audit: admin attempted to delete a non-existing user
                _logger.LogWarning(
                    "Admin action failed (target not found). AdminId={AdminId}, Action=DeleteUser, TargetId={TargetId}, IP={IP}",
                    adminId, id, ip
                );
                return NotFound("User not found.");
            }

            // ===============================
            // Audit BEFORE deleting (recommended)
            // ===============================
            _logger.LogInformation(
                "Admin action started. AdminId={AdminId}, Action=DeleteUser, TargetId={TargetId}, TargetUserName={TargetUserName}, IP={IP}",
                adminId, user.UserID, user.UserName, ip
            );

            if (!await clsUser.DeleteAsync(id))
            {
                _logger.LogError(
                    "Admin action failed during delete. AdminId={AdminId}, Action=DeleteUser, TargetId={TargetId}, IP={IP}",
                    adminId, id, ip
                );
                return StatusCode(500, "Database error: Could not delete the user.");
            }

            // Audit: Success
            _logger.LogInformation(
                 "Admin action succeeded. AdminId={AdminId}, Action=DeleteUser, TargetId={TargetId}, IP={IP}",
                 adminId, id, ip
             );

            return Ok($"User {id} deleted.");
        }
    }
}