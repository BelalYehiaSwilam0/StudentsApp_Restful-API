using APIBusinessLayer;
using APIDataAccessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
namespace MyStudentsApp.Controllers
{
    [Authorize]
    [Route("api/People")]
    [ApiController]
    [EnableRateLimiting("CoreLimiter")] //Now EndPoints are protected.
    public class PeopleApiController : ControllerBase
    {
        private readonly ILogger<PeopleApiController> _logger;

        public PeopleApiController(ILogger<PeopleApiController> logger)
        {
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet ("All", Name = "GetPeople")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<PersonDTO>> GetPeople()
        {
            List<PersonDTO> PeopleList = APIBusinessLayer.clsPerson.GetPeople();

            if(PeopleList.Count == 0)
            {
                return NotFound("No People found!");
            }
            return Ok(PeopleList);
        }


        [Authorize(Roles = "Admin")]// Only Admins can access this
        [HttpGet("{id}", Name = "GetPersonById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<PersonDTO> GetPersonById(int id)
        {
            if (id < 1)
            {
                return BadRequest($"Not accepted ID {id}");
            }

            APIBusinessLayer.clsPerson person = APIBusinessLayer.clsPerson.GetPersonById(id);

            if(person == null)
            {
                return NotFound("Person is not found");
            }

            PersonDTO PDTO = person.PDTO;

            return Ok(PDTO);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost(Name = "AddPerson")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<PersonDTO>AddPerson([FromBody] PersonDTO newPersonDTO)
        {
            if (newPersonDTO == null || string.IsNullOrEmpty(newPersonDTO.FirstName) || string.IsNullOrEmpty(newPersonDTO.LastName) || newPersonDTO.BirthDate > DateTime.Now)
            {
                return BadRequest("Invalid Person data.");
            }

            APIBusinessLayer.clsPerson person = new clsPerson();
            person.FirstName = newPersonDTO.FirstName;
            person.LastName = newPersonDTO.LastName;
            person.Email = newPersonDTO.Email;
            person.BirthDate = newPersonDTO.BirthDate;
            person.Age = newPersonDTO.Age;
            person.IsActive = newPersonDTO.IsActive;

            if (person.Save())
            {
                newPersonDTO.PersonId = person.PersonId;
                return CreatedAtRoute("GetPersonById", new { id = newPersonDTO.PersonId }, newPersonDTO);
            }
            else
            {
                return StatusCode(500, "Database error: Could not save the person.");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}" , Name = "DeletePersonById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeletePersonById(int id)
        {
            // Capture IP once for tracing (helps investigations later)
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            // Identify the admin who is performing the action
            // ClaimTypes.NameIdentifier is what you put in JWT during login.
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "unknown";

            if (id < 1)
            {
                // ✅ Audit attempt (invalid input) - still useful signal
                _logger.LogWarning(
                    "Admin action blocked (invalid id). AdminId={AdminId}, Action=DeletePerson, TargetId={TargetId}, IP={IP}",
                    adminId,
                    id,
                    ip
                );
                return BadRequest($"Not accepted ID {id}");
            }

            var person = clsPerson.GetPersonById(id);

            if (person == null)
            {
                // ✅ Audit: admin attempted to delete a non-existing person
                _logger.LogWarning(
                    "Admin action failed (target not found). AdminId={AdminId}, Action=DeletePerson, TargetId={TargetId}, IP={IP}",
                    adminId,
                    id,
                    ip
                );

                return NotFound($"Person with ID {id} not found.");
            }

            // ===============================
            // Audit BEFORE deleting (recommended)
            // ===============================
            // ✅ Why before?
            // If delete throws or fails later, you still have the audit record of the attempt.
            _logger.LogInformation(
                "Admin action started. AdminId={AdminId}, Action=DeleteStudent, TargetId={TargetId}, TargetEmail={TargetEmail}, IP={IP}",
                adminId,
                person.PersonId,
                person.Email,
                ip
            );

            if (!APIBusinessLayer.clsPerson.DeletePerson(id))
            {
                _logger.LogInformation(
                    "Admin action not succeeded. AdminId={AdminId}, Action=DeletePerson, TargetId={TargetId}, IP={IP}",
                    adminId,
                    id,
                    ip
             );
                return NotFound($"Person With id {id} not found,no rows deleted!");
                
            }

            _logger.LogInformation(
                 "Admin action succeeded. AdminId={AdminId}, Action=DeletePerson, TargetId={TargetId}, IP={IP}",
                 adminId,
                 id,
                 ip
             );

            return Ok($"Person with Id {id} has been deleted");
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}", Name = "UpdatePersonById")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<PersonDTO> UpdatePersonById(int id, UpdatePersonDTO updatedPerson)
        {
            // Capture IP once for tracing (helps investigations later)
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            // Identify the admin who is performing the action
            // ClaimTypes.NameIdentifier is what you put in JWT during login.
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "unknown";

            if (id < 1 || updatedPerson == null)
            {
                _logger.LogWarning(
                   "Admin action blocked (invalid id). AdminId={AdminId}, Action=UpdatePerson, TargetId={TargetId}, IP={IP}",
                   adminId,
                   id,
                   ip
               );

                return BadRequest("Invalid person data.");
            }
               

            clsPerson person = clsPerson.GetPersonById(id);
            if (person == null)
            {
                // ✅ Audit: admin attempted to update a non-existing person
                _logger.LogWarning(
                    "Admin action failed (target not found). AdminId={AdminId}, Action=UpdatePerson, TargetId={TargetId}, IP={IP}",
                    adminId,
                    id,
                    ip
                );

                return NotFound($"Person with ID {id} not found.");
            }

            // ==========================================
            // Audit BEFORE updating (Attempt started)
            // ==========================================
            _logger.LogInformation(
                "Admin action started. AdminId={AdminId}, Action=UpdatePerson, TargetId={TargetId}, TargetEmail={TargetEmail}, IP={IP}",
                adminId,
                person.PersonId,
                person.Email,
                ip
            );

            if (!string.IsNullOrEmpty(updatedPerson.FirstName) && updatedPerson.FirstName != "string")
                person.FirstName = updatedPerson.FirstName;
            if (!string.IsNullOrEmpty(updatedPerson.LastName) && updatedPerson.LastName != "string")
                person.LastName = updatedPerson.LastName;
            if (!string.IsNullOrEmpty(updatedPerson.Email) && updatedPerson.Email != "string")
                person.Email = updatedPerson.Email;
            if (updatedPerson.BirthDate.HasValue && updatedPerson.BirthDate.Value <= DateTime.Now)
                person.BirthDate = updatedPerson.BirthDate.Value;
            if (updatedPerson.IsActive.HasValue)
                person.IsActive = updatedPerson.IsActive.Value;

            // Save changes to database
            if (!person.Save())
            {
                // ✅ Audit: If database fails to save the modifications
                _logger.LogError(
                    "Admin action failed during save. AdminId={AdminId}, Action=UpdatePerson, TargetId={TargetId}, IP={IP}",
                    adminId,
                    id,
                    ip
                );
                return StatusCode(500, "Database error: Could not update the person.");
            }

            // ✅ Audit: Success
            _logger.LogInformation(
                 "Admin action succeeded. AdminId={AdminId}, Action=UpdatePerson, TargetId={TargetId}, IP={IP}",
                 adminId,
                 id,
                 ip
             );

            return Ok(person.PDTO);
        }


    }
}
