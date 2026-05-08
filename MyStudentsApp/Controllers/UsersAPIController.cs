using APIBusinessLayer;
using APIDataAccessLayer;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MyStudentsApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersAPIController : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpGet("All", Name = "GetAllUsers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<UserDTO>> GetAllUsers()
        {
            
            List<UserDTO> UsersList = clsUser.GetAllUsers();

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
        public ActionResult<UserDTO> GetUserById(int id)
        {
            // 1. Basic validation for the ID
            if (id < 1) return BadRequest($"Invalid ID {id}");

            // 2. Search for the user in the database
            APIBusinessLayer.clsUser user = clsUser.Find(id);

            // 3. Check if the user exists in our system
            if (user == null) return NotFound("User not found.");

            // --- Ownership Logic Starts Here ---

            // 4. Get User ID and Role from the JWT Token
            // Note: User.FindFirst is a built-in way to read claims from the token
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Convert the authenticated user ID from string to integer.
            int currentUsertId = int.Parse(authenticatedUserId);

            // Extract the authenticated user's role from the JWT.
            // Typical values are "Student" or "Admin".
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            // Determine whether the current user is an Admin.
            // Admins are allowed to access any student record.
            bool isAdmin = currentUserRole == "Admin";

            // Ownership check:
            // If the user is NOT an admin and is trying to access
            // a student record that does not belong to them,
            // the request is forbidden.
            if (!isAdmin && currentUsertId != id)
                return Forbid(); // Returns HTTP 403 Forbidden
            
            // 7. If everything is okay, return the data
            return Ok(user.UDTO);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost(Name = "AddNewUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<UserDTO> AddUser(CreateUserDTO createUserDto)
        {
            if (createUserDto == null || createUserDto.PersonID < 1)
                return BadRequest("Invalid data.");

            clsPerson person = clsPerson.GetPersonById(createUserDto.PersonID);

            if (person == null)
            {
                return NotFound($"Person with ID {createUserDto.PersonID} was not found.");
            }

           
            if (clsUser.IsUserExistByPersonID(createUserDto.PersonID))
            {
                return BadRequest("A user account already exists for this person.");
            }

           
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(createUserDto.PasswordHash);

            clsUser user = new clsUser();
            user.PersonID = person.PersonId;
            user.UserName = createUserDto.UserName;
            user.PasswordHash = hashedPassword;
            user.Role = createUserDto.Role;

            if (user.Save())
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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<UserDTO> UpdateUserField(int id, clsUser.enUpdateType updateType, [FromBody] string newValue)
        {
            
            clsUser user = clsUser.Find(id);

            if (user == null) return NotFound($"User with ID {id} not found.");

           
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

            
            if (user.Save())
            {
               
                return Ok(user.UDTO);
            }

            return StatusCode(500, "Update failed.");
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteUser(int id)
        {
            if (clsUser.Delete(id)) return Ok($"User {id} deleted.");
            return NotFound("User not found.");
        }
    }
}