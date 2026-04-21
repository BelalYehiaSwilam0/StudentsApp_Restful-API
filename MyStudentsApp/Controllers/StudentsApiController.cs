using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentAPIBusinessLayer;
using StudentDataAccessLayer;
namespace MyStudentsApp.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/Students")]
    [ApiController]
    public class StudentsApiController : ControllerBase
    {
        [HttpGet ("All", Name = "GetAllStudents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<IEnumerable<StudentDTO>> GetAllStudents()
        {
            List<StudentDTO> studentsList = StudentAPIBusinessLayer.Student.GetAllStudents();

            if(studentsList.Count == 0)
            {
                return NotFound("No Studens found!");
            }
            return Ok(studentsList);
        }

        [HttpGet("{id}", Name = "GetStudentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<StudentDTO> GetStudentById(int id)
        {
            if (id < 1)
            {
                return BadRequest($"Not accepted ID {id}");
            }

            StudentAPIBusinessLayer.Student student = StudentAPIBusinessLayer.Student.GetStudentById(id);

            if(student == null)
            {
                return NotFound("Student is not found");
            }

            StudentDTO SDTO = student.SDTO;

            return Ok(SDTO);
        }



        [HttpPost(Name = "AddStudent")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<StudentDTO>AddStudent(StudentDTO newStudentDTO)
        {
            if (newStudentDTO == null || string.IsNullOrEmpty(newStudentDTO.FirstName) || string.IsNullOrEmpty(newStudentDTO.LastName) || newStudentDTO.BirthDate > DateTime.Now)
            {
               return BadRequest("Invalid student data.");
            }

            StudentAPIBusinessLayer.Student student = new(newStudentDTO);
            student.Save();

            newStudentDTO.StudentId = student.StudentId;

            return CreatedAtRoute("GetStudentById", new { id = newStudentDTO.StudentId }, newStudentDTO);
        }

        
        [HttpDelete("{id}" , Name = "DeleteStudent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult DeleteStudentById(int id)
        {
            if(id < 1)
            {
                return BadRequest($"Not accepted ID {id}");
            }

            if(StudentAPIBusinessLayer.Student.DeleteStudent(id))
            {
                return Ok($"Student with Id {id} has been deleted");
            }
            else
            {
                return NotFound($"Student With id {id} not found,no rows deleted!");
            }
        }


        [HttpPut("{id}", Name = "UpdateStudentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<StudentDTO> UpdateStudentById(int id, UpdateStudentDTO updatedStudent)
        {
            if (id < 1 || updatedStudent == null)
                return BadRequest("Invalid student data.");

            Student student = Student.GetStudentById(id);
            if (student == null)
                return NotFound($"Student with ID {id} not found.");

            if (!string.IsNullOrEmpty(updatedStudent.FirstName))
                student.FirstName = updatedStudent.FirstName;
            if (!string.IsNullOrEmpty(updatedStudent.LastName))
                student.LastName = updatedStudent.LastName;
            if (!string.IsNullOrEmpty(updatedStudent.Email))
                student.Email = updatedStudent.Email;
            if (updatedStudent.BirthDate.HasValue && updatedStudent.BirthDate.Value <= DateTime.Now)
                student.BirthDate = updatedStudent.BirthDate.Value;
            if (updatedStudent.IsActive.HasValue)
                student.IsActive = updatedStudent.IsActive.Value;

            student.Save();

            return Ok(student.SDTO);
        }


    }
}
