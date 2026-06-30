using System;
using System.Threading.Tasks;
using APIDataAccessLayer;

namespace APIBusinessLayer
{
    public class clsUserRefreshToken
    {

        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode = enMode.AddNew;

        public int RefreshTokenID { get; set; }

        public int UserID { get; set; }

        public string RefreshTokenHash { get; set; }

        public DateTime RefreshTokenExpiresAt { get; set; }

        public DateTime? RefreshTokenRevokedAt { get; set; }

        public clsUserRefreshToken()
        {
            this.RefreshTokenID = -1;
            this.UserID = -1;
            this.RefreshTokenHash = string.Empty;
            this.RefreshTokenExpiresAt = DateTime.MinValue;
            this.RefreshTokenRevokedAt = null;
        }

        private clsUserRefreshToken(UserRefreshTokenDTO dto)
        {
            RefreshTokenID = dto.RefreshTokenID;

            UserID = dto.UserID;

            RefreshTokenHash = dto.RefreshTokenHash;

            RefreshTokenExpiresAt = dto.RefreshTokenExpiresAt;

            RefreshTokenRevokedAt = null;

            this.Mode = enMode.Update;
        }

        // This method finds token by user id
        public static async Task<clsUserRefreshToken> FindByUserIDAsync(int userID)
        {
            UserRefreshTokenDTO dto = await clsUserRefreshTokenData.GetRefreshTokenByUserIDAsync(userID);

            if (dto != null)
            {
                return new clsUserRefreshToken(dto);
            }

            return null;
        }

        private async Task<bool> _AddRefreshTokenAsync()
        {
            this.RefreshTokenID = await clsUserRefreshTokenData.AddRefreshTokenAsync(
                   this.UserID,
                   this.RefreshTokenHash,
                   this.RefreshTokenExpiresAt
               );
            return (this.UserID != -1);
        }

        private async Task<bool> _UpdateRefreshTokenInfoAsync()
        {
            return await clsUserRefreshTokenData.UpdateRefreshTokenAsync(this.UserID,this.RefreshTokenHash,
                this.RefreshTokenExpiresAt,this.RefreshTokenRevokedAt);
        }

        // This method saves refresh token
        public async Task<bool> SaveAsync()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (await _AddRefreshTokenAsync())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:
                    return await _UpdateRefreshTokenInfoAsync();

            }
            return false;
        }

        

      
    }
}