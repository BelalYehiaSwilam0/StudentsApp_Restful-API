using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace APIDataAccessLayer
{
    // This class holds refresh token data
    public class UserRefreshTokenDTO
    {
        public int RefreshTokenID { get; set; }

        public int UserID { get; set; }

        public string RefreshTokenHash { get; set; }

        public DateTime RefreshTokenExpiresAt { get; set; }

        public DateTime? RefreshTokenRevokedAt { get; set; }

        public UserRefreshTokenDTO(int refreshTokenID,int userID,string refreshTokenHash,DateTime refreshTokenExpiresAt,
            DateTime? refreshTokenRevokedAt)
        {
            RefreshTokenID = refreshTokenID;

            UserID = userID;

            RefreshTokenHash = refreshTokenHash;

            RefreshTokenExpiresAt = refreshTokenExpiresAt;

            RefreshTokenRevokedAt = refreshTokenRevokedAt;
        }
    }

    public class clsUserRefreshTokenData
    {
        // This method gets refresh token by user id
        public static UserRefreshTokenDTO GetRefreshTokenByUserID(int userID)
        {
            try
            {
                using (SqlConnection conn =
                       new SqlConnection(clsDatabaseAccessSettings._connectionString))
                {
                    using (SqlCommand cmd =
                           new SqlCommand("SP_GetRefreshTokenByUserID", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@UserID", userID);

                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new UserRefreshTokenDTO(
                                    reader.GetInt32(reader.GetOrdinal("RefreshTokenID")),

                                    reader.GetInt32(reader.GetOrdinal("UserID")),

                                    reader.GetString(reader.GetOrdinal("RefreshTokenHash")),

                                    reader.GetDateTime(reader.GetOrdinal("RefreshTokenExpiresAt")),

                                    reader.IsDBNull(reader.GetOrdinal("RefreshTokenRevokedAt"))
                                    ? null
                                    : reader.GetDateTime(reader.GetOrdinal("RefreshTokenRevokedAt"))
                                );
                            }
                        }
                    }
                }
            }
            catch
            {

            }

            return null;
        }

        // This method adds refresh token
        public static int AddRefreshToken(int userID,string refreshTokenHash,DateTime refreshTokenExpiresAt)
        {
            try
            {
                using (SqlConnection conn =
                       new SqlConnection(clsDatabaseAccessSettings._connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_AddRefreshToken", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@UserID", userID);

                        cmd.Parameters.AddWithValue("@RefreshTokenHash",
                            refreshTokenHash);

                        cmd.Parameters.AddWithValue("@RefreshTokenExpiresAt",
                            refreshTokenExpiresAt);

                        SqlParameter outputParam = new SqlParameter("@NewRefreshTokenId", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        cmd.Parameters.Add(outputParam);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        return (int)outputParam.Value;
                    }
                }
            }
            catch
            {
                return -1;
            }
        }

        // This method updates refresh token
        public static bool UpdateRefreshToken(int userID,string refreshTokenHash,DateTime refreshTokenExpiresAt,DateTime? refreshTokenRevokedAt)
        {
            try
            {
                using (SqlConnection conn =
                       new SqlConnection(clsDatabaseAccessSettings._connectionString))
                {
                    using (SqlCommand cmd =
                           new SqlCommand("SP_UpdateRefreshToken", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@UserID", userID);

                        cmd.Parameters.AddWithValue("@RefreshTokenHash",refreshTokenHash);

                        cmd.Parameters.AddWithValue("@RefreshTokenExpiresAt",refreshTokenExpiresAt);

                        cmd.Parameters.AddWithValue("@RefreshTokenRevokedAt", (object)refreshTokenRevokedAt ?? DBNull.Value);

                        conn.Open();

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
      
       
    }
}