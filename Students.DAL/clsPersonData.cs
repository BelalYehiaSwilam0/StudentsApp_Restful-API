
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        
        public static async Task<List<PersonDTO>> GetPeopleAsync()
        {
            var StudentsList = new List<PersonDTO>();
            try
            {
                using (SqlConnection conn = new SqlConnection(clsDatabaseAccessSettings._connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_GetPeople", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Open connection asynchronously
                        await conn.OpenAsync();

                        // Execute reader asynchronously
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            // Read rows asynchronously
                            while (await reader.ReadAsync())
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

        public static async Task<PersonDTO> GetPersonByIdAsync(int personId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDatabaseAccessSettings._connectionString))
                {
                    using (var command = new SqlCommand("SP_GetPersonById", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@PersonId", personId);

                        await connection.OpenAsync();

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
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

        public static async Task<int> AddPersonAsync(PersonDTO personDTO)
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

                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();

                        return (int)outputIdParam.Value;
                    }
                }
            }
            catch (Exception)
            {

                return -1;
            }
        }

        public static async Task<bool> UpdatePersonAsync(PersonDTO updatePerson)
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

                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();
                        return true;
                    }
                }

            }
            catch (Exception)
            {

                return false;
            }
        }

        public static async Task<bool> DeletePersonAsync(int personId)
        {
            try
            {
                using (var connection = new SqlConnection(clsDatabaseAccessSettings._connectionString))
                {
                    using (var command = new SqlCommand("SP_DeletePerson", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@PersonId", personId);

                        await connection.OpenAsync();

                        var result = await command.ExecuteScalarAsync();
                        int rowsAffected = result != null ? (int)result : 0;
                        return (rowsAffected == 1);


                    }
                }
            }
            catch (Exception)
            {

                return false;
            }
           
           
        }

        public static async Task<bool> IsPersonExistAsync(int PersonID)
        {
            bool isFound = false;
            using (SqlConnection connection = new SqlConnection(clsDatabaseAccessSettings._connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_IsPersonExist", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PersonID", PersonID);

                    await connection.OpenAsync();

                    var result = await command.ExecuteScalarAsync();
                    isFound = result != null && (int)result == 1;
                }
            }
            return isFound;
        }
    }
    
}
