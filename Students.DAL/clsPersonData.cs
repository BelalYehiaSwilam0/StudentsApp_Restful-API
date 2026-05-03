
using System;
using System.Data;
using Microsoft.Data.SqlClient;
namespace APIDataAccessLayer
{
    public class PersonDTO
    {
        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public DateTime BirthDate { get; set; }
        public int Age { get; set; }

        public bool IsActive { get; set; }

        public PersonDTO(int personId, string firstName, string lastName, string email, DateTime birthDate, int age, bool isActive)
        {
            PersonId = personId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            BirthDate = birthDate;
            Age = age;
            IsActive = isActive;
        }
    }

    public class UpdatePersonDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool? IsActive { get; set; }
    }

    public class clsPersonData
    {
        
        public static List<PersonDTO> GetPeople()
        {
            var StudentsList = new List<PersonDTO>();
            try
            {
                using (SqlConnection conn = new SqlConnection(clsDatabaseAccessSettings._connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_GetPeople", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                StudentsList.Add(new PersonDTO
                                (
                                    reader.GetInt32(reader.GetOrdinal("PersonId")),
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

                return new List<PersonDTO>();
            }
        }

        public static PersonDTO GetPersonById(int personId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDatabaseAccessSettings._connectionString))
                {
                    using (var command = new SqlCommand("SP_GetPersonById", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@PersonId", personId);

                        connection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new PersonDTO
                                    (
                                    reader.GetInt32(reader.GetOrdinal("PersonId")),
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

        public static int AddPerson(PersonDTO personDTO)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDatabaseAccessSettings._connectionString))
                {
                    using (var command = new SqlCommand("SP_AddPerson", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@FirstName", personDTO.FirstName);
                        command.Parameters.AddWithValue("@LastName", personDTO.LastName);
                        command.Parameters.AddWithValue("@Email", personDTO.Email);
                        command.Parameters.AddWithValue("@BirthDate", personDTO.BirthDate);
                        var outputIdParam = new SqlParameter("@NewPersonId", SqlDbType.Int)
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

        public static bool UpdatePerson(PersonDTO updatePerson)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDatabaseAccessSettings._connectionString))
                {
                    using (var command = new SqlCommand("SP_UpdatePerson", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@PersonId",updatePerson.PersonId);
                        command.Parameters.AddWithValue("@FirstName", updatePerson.FirstName);
                        command.Parameters.AddWithValue("@LastName", updatePerson.LastName);
                        command.Parameters.AddWithValue("@Email", updatePerson.Email);
                        command.Parameters.AddWithValue("@BirthDate", updatePerson.BirthDate);
                        command.Parameters.AddWithValue("@IsActive", updatePerson.IsActive);

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

        public static bool DeletePerson(int personId)
        {
            try
            {
                using (var connection = new SqlConnection(clsDatabaseAccessSettings._connectionString))
                {
                    using (var command = new SqlCommand("SP_DeletePerson", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@PersonId", personId);

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

        public static bool IsPersonExist(int PersonID)
        {
            bool isFound = false;
            using (SqlConnection connection = new SqlConnection(clsDatabaseAccessSettings._connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_IsPersonExist", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PersonID", PersonID);
                    connection.Open();
                    isFound = (int)command.ExecuteScalar() == 1;
                }
            }
            return isFound;
        }
    }
    
}
