using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

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
        
        public static List<UserDTO> GetAllUsers()
        {
            var usersList = new List<UserDTO>();
            try
            {
                using (SqlConnection conn = new SqlConnection(clsDatabaseAccessSettings._connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_GetAllUsers", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
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

       
        public static UserDTO GetUserById(int userID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(clsDatabaseAccessSettings._connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_GetUserById", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
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

        public static UserDTO GetUserByUserName(string userName)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(clsDatabaseAccessSettings._connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_GetUserByUserName", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserName", userName);
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
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


        public static int AddNewUser(CreateUserDTO userDTO)
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

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        return (int)outputParam.Value;
                    }
                }
            }
            catch (Exception) { return -1; }
        }

        
        public static bool UpdateUserName(UpdateUserNameDTO dto)
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
                        conn.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch { return false; }
        }

        
        public static bool UpdateUserPassword(UpdateUserPasswordDTO dto)
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
                        conn.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch { return false; }
        }

       
        public static bool UpdateUserRole(UpdateUserRoleDTO dto)
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
                        conn.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch { return false; }
        }

        
        public static bool DeleteUser(int userID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(clsDatabaseAccessSettings._connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_DeleteUser", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        conn.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch { return false; }
        }

        public static bool IsUserExistByPersonID(int PersonID)
        {
            bool isFound = false;
            using (SqlConnection connection = new SqlConnection(clsDatabaseAccessSettings._connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_IsUserExistByPersonID", connection))
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