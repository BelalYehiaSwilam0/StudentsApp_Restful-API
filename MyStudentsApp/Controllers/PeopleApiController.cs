using APIBusinessLayer;
using APIDataAccessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace MyStudentsApp.Controllers
{
    [Authorize]
    [Route("api/People")]
    [ApiController]
    public class PeopleApiController : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpGet ("All", Name = "GetPeople")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
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


        //I will finish the "GetPersonById" endpoint later. I need to implement Ownership Rules first.
        [HttpGet("{id}", Name = "GetPersonById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeletePersonById(int id)
        {
            if(id < 1)
            {
                return BadRequest($"Not accepted ID {id}");
            }

            if(APIBusinessLayer.clsPerson.DeletePerson(id))
            {
                return Ok($"Person with Id {id} has been deleted");
            }
            else
            {
                return NotFound($"Person With id {id} not found,no rows deleted!");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}", Name = "UpdatePersonById")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<PersonDTO> UpdatePersonById(int id, UpdatePersonDTO updatedPerson)
        {
            if (id < 1 || updatedPerson == null)
                return BadRequest("Invalid student data.");

            clsPerson person = clsPerson.GetPersonById(id);
            if (person == null)
                return NotFound($"Student with ID {id} not found.");

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

            person.Save();
                
            return Ok(person.PDTO);
        }


    }
}
