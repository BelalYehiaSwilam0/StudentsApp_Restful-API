using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace APIDataAccessLayer
{
    public class UserDTO
    {
        public int UserID { get; set; }
        public int PersonID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }

        public UserDTO(int userID, int personID, string fullName, string email,
                       string userName, string passwordHash, string role, bool isActive)
        {
            UserID = userID;
            PersonID = personID;
            FullName = fullName;
            Email = email;
            UserName = userName;
            PasswordHash = passwordHash;
            Role = role;
            IsActive = isActive;
        }
    }
    public class CreateUserDTO
    {
        public int PersonID { get; set; } 
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
    }

    // Classes for Specialized Updates
    public class UpdateUserNameDTO
    {
        public int UserID { get; set; }
        public string NewUserName { get; set; }
    }
    public class UpdateUserPasswordDTO
    {
        public int UserID { get; set; }
        public string NewPasswordHash { get; set; }
    }
    public class UpdateUserRoleDTO
    {
        public int UserID { get; set; }
        public string NewRole { get; set; }
    }

    public class clsUserData
    {
        
        public static async Task<List<UserDTO>> GetAllUsersAsync()
        {
            var usersList = new List<UserDTO>();
            try
            {
                using (SqlConnection conn = new SqlConnection(clsDatabaseAccessSettings._connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_GetAllUsers", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        await conn.OpenAsync();

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                usersList.Add(new UserDTO(
                                    reader.GetInt32(reader.GetOrdinal("UserID")),
                                    reader.GetInt32(reader.GetOrdinal("PersonID")),
                                    reader.GetString(reader.GetOrdinal("FullName")),
                                    reader.GetString(reader.GetOrdinal("Email")),
                                    reader.GetString(reader.GetOrdinal("UserName")),
                                    reader.GetString(reader.GetOrdinal("PasswordHash")),
                                    reader.GetString(reader.GetOrdinal("Role")),
                                    reader.GetBoolean(reader.GetOrdinal("IsActive"))
                                ));
                            }
                        }
                    }
                }
            }
            catch (Exception) { }
            return usersList;
        }

       
        public static async Task<UserDTO> GetUserByIdAsync(int userID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(clsDatabaseAccessSettings._connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_GetUserById", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserID", userID);

                        await conn.OpenAsync();

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new UserDTO(
                                    reader.GetInt32(reader.GetOrdinal("UserID")),
                                    reader.GetInt32(reader.GetOrdinal("PersonID")),
                                    reader.GetString(reader.GetOrdinal("FullName")),
                                    reader.GetString(reader.GetOrdinal("Email")),
                                    reader.GetString(reader.GetOrdinal("UserName")),
                                    reader.GetString(reader.GetOrdinal("PasswordHash")),
                                    reader.GetString(reader.GetOrdinal("Role")),
                                    reader.GetBoolean(reader.GetOrdinal("IsActive"))
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception) { }
            return null;
        }

        public static async Task<UserDTO> GetUserByUserNameAsync(string userName)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(clsDatabaseAccessSettings._connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_GetUserByUserName", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserName", userName);

                        await conn.OpenAsync();

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new UserDTO(
                                    reader.GetInt32(reader.GetOrdinal("UserID")),
                                    reader.GetInt32(reader.GetOrdinal("PersonID")),
                                    reader.GetString(reader.GetOrdinal("FullName")),
                                    reader.GetString(reader.GetOrdinal("Email")),
                                    reader.GetString(reader.GetOrdinal("UserName")),
                                    reader.GetString(reader.GetOrdinal("PasswordHash")),
                                    reader.GetString(reader.GetOrdinal("Role")),
                                    reader.GetBoolean(reader.GetOrdinal("IsActive"))
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception) { }
            return null;
        }


        public static async Task<int> AddNewUserAsync(CreateUserDTO userDTO)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(clsDatabaseAccessSettings._connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_AddNewUser", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@PersonID", userDTO.PersonID);
                        cmd.Parameters.AddWithValue("@UserName", userDTO.UserName);
                        cmd.Parameters.AddWithValue("@PasswordHash", userDTO.PasswordHash);
                        cmd.Parameters.AddWithValue("@Role", userDTO.Role);

                        SqlParameter outputParam = new SqlParameter("@NewUserID", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        cmd.Parameters.Add(outputParam);

                        await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                        return (int)outputParam.Value;
                    }
                }
            }
            catch (Exception) { return -1; }
        }

        
        public static async Task<bool> UpdateUserNameAsync(UpdateUserNameDTO dto)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(clsDatabaseAccessSettings._connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_UpdateUserName", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserID", dto.UserID);
                        cmd.Parameters.AddWithValue("@NewUserName", dto.NewUserName);

                        await conn.OpenAsync();
                        return await cmd.ExecuteNonQueryAsync() > 0;
                    }
                }
            }
            catch { return false; }
        }

        
        public static async Task<bool> UpdateUserPasswordAsync(UpdateUserPasswordDTO dto)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(clsDatabaseAccessSettings._connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_UpdateUserPassword", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserID", dto.UserID);
                        cmd.Parameters.AddWithValue("@NewPasswordHash", dto.NewPasswordHash);

                        await conn.OpenAsync();
                        return await cmd.ExecuteNonQueryAsync() > 0;
                    }
                }
            }
            catch { return false; }
        }

       
        public static async Task<bool> UpdateUserRoleAsync(UpdateUserRoleDTO dto)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(clsDatabaseAccessSettings._connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_UpdateUserRole", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserID", dto.UserID);
                        cmd.Parameters.AddWithValue("@NewRole", dto.NewRole);

                        await conn.OpenAsync();
                        return await cmd.ExecuteNonQueryAsync() > 0;
                    }
                }
            }
            catch { return false; }
        }

        
        public static async Task<bool> DeleteUserAsync(int userID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(clsDatabaseAccessSettings._connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_DeleteUser", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserID", userID);

                        await conn.OpenAsync();
                        return await cmd.ExecuteNonQueryAsync() > 0;
                    }
                }
            }
            catch { return false; }
        }

        public static async Task<bool> IsUserExistByPersonIDAsync(int PersonID)
        {
            bool isFound = false;
            using (SqlConnection connection = new SqlConnection(clsDatabaseAccessSettings._connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_IsUserExistByPersonID", connection))
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