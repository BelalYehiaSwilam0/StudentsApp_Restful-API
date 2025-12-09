
using System;
using System.Data;
using Microsoft.Data.SqlClient;
namespace StudentDataAccessLayer
{
    public class StudentDTO
    {
        public int StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public DateTime BirthDate { get; set; }
        public int Age { get; set; }

        public bool IsActive { get; set; }

        public StudentDTO(int studentId, string firstName, string lastName, string email, DateTime birthDate, int age, bool isActive)
        {
            StudentId = studentId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            BirthDate = birthDate;
            Age = age;
            IsActive = isActive;
        }
    }

    public class UpdateStudentDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool? IsActive { get; set; }
    }

    public class clsStudentData
    {
        
        public static List<StudentDTO> GetAllStudents()
        {
            var StudentsList = new List<StudentDTO>();
            try
            {
                using (SqlConnection conn = new SqlConnection(clsDatabaseAccessSettings._connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_GetAllStudents", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                StudentsList.Add(new StudentDTO
                                (
                                    reader.GetInt32(reader.GetOrdinal("StudentId")),
                                    reader.GetString(reader.GetOrdinal("FirstName")),
                                    reader.GetString(reader.GetOrdinal("LastName")),
                                    reader.GetString(reader.GetOrdinal("Email")),
                                    reader.GetDateTime(reader.GetOrdinal("BirthDate")),
                                    reader.GetInt32(reader.GetOrdinal("Age")),
                                    reader.GetBoolean(reader.GetOrdinal("IsActive"))
                                    
                                ));
                            }
                        }
                    }

                    return StudentsList;
                }
            }
            catch (Exception)
            {

                return new List<StudentDTO>();
            }
        }

        public static StudentDTO GetStudentById(int studentId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDatabaseAccessSettings._connectionString))
                {
                    using (var command = new SqlCommand("SP_GetStudentById",connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@StudentId", studentId);

                        connection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new StudentDTO
                                    (
                                    reader.GetInt32(reader.GetOrdinal("StudentId")),
                                    reader.GetString(reader.GetOrdinal("FirstName")),
                                    reader.GetString(reader.GetOrdinal("LastName")),
                                    reader.GetString(reader.GetOrdinal("Email")),
                                    reader.GetDateTime(reader.GetOrdinal("BirthDate")),
                                    reader.GetInt32(reader.GetOrdinal("Age")),
                                    reader.GetBoolean(reader.GetOrdinal("IsActive"))

                                    );
                                
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
                   
            }
            catch (Exception)
            {

                return null;
            }
        }

        public static int AddStudent(StudentDTO studentDTO)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDatabaseAccessSettings._connectionString))
                {
                    using (var command = new SqlCommand("SP_AddStudent", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@FirstName", studentDTO.FirstName);
                        command.Parameters.AddWithValue("@LastName", studentDTO.LastName);
                        command.Parameters.AddWithValue("@Email", studentDTO.Email);
                        command.Parameters.AddWithValue("@BirthDate", studentDTO.BirthDate);
                        var outputIdParam = new SqlParameter("@NewStudentId", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(outputIdParam);

                        connection.Open();
                        command.ExecuteNonQuery();

                        return (int)outputIdParam.Value;
                    }
                }
            }
            catch (Exception)
            {

                return -1;
            }
        }

        public static bool UpdateStudent(StudentDTO updateStudent)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDatabaseAccessSettings._connectionString))
                {
                    using (var command = new SqlCommand("SP_UpdateStudent", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@StudentId",updateStudent.StudentId);
                        command.Parameters.AddWithValue("@FirstName", updateStudent.FirstName);
                        command.Parameters.AddWithValue("@LastName", updateStudent.LastName);
                        command.Parameters.AddWithValue("@Email", updateStudent.Email);
                        command.Parameters.AddWithValue("@BirthDate", updateStudent.BirthDate);
                        command.Parameters.AddWithValue("@IsActive", updateStudent.IsActive);

                        connection.Open();
                        command.ExecuteNonQuery();
                        return true;
                    }
                }

            }
            catch (Exception)
            {

                return false;
            }
        }

        public static bool DeleteStudent(int studentId)
        {
            try
            {
                using (var connection = new SqlConnection(clsDatabaseAccessSettings._connectionString))
                {
                    using (var command = new SqlCommand("SP_DeleteStudent", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@StudentId", studentId);

                        connection.Open();

                        int rowsAffected = (int)command.ExecuteScalar();
                        return (rowsAffected == 1);


                    }
                }
            }
            catch (Exception)
            {

                return false;
            }
           
           
        }
    }
    
}
