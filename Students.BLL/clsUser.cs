using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using APIDataAccessLayer;

namespace APIBusinessLayer
{
    public class clsUser
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode = enMode.AddNew;

        public enum enUpdateType { None = 0, UserName = 1, Password = 2, Role = 3 }
        public enUpdateType UpdateType = enUpdateType.None;

        public int UserID { get; set; }
        public int PersonID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }

        public clsUserRefreshToken RefreshTokenInfo { get; set; }

        public UserDTO UDTO
        {
            get { return new UserDTO(UserID, PersonID, FullName, Email, UserName, PasswordHash, Role, IsActive); }
        }

        public clsUser()
        {
            this.UserID = -1;
            this.PersonID = -1;
            this.UserName = string.Empty;
            this.PasswordHash = string.Empty;
            this.Role = string.Empty;
            this.Mode = enMode.AddNew;
        }
        private clsUser(UserDTO uDto)
        {
            this.UserID = uDto.UserID;
            this.PersonID = uDto.PersonID;
            this.FullName = uDto.FullName;
            this.Email = uDto.Email;
            this.UserName = uDto.UserName;
            this.PasswordHash = uDto.PasswordHash;
            this.Role = uDto.Role;
            this.IsActive = uDto.IsActive;
            // Note: We cannot use 'await' inside constructors in C#.
            // To make the code async and fast, we moved 'RefreshTokenInfo' out of here.
            // It is now loaded using async methods like FindAsync() and FindUserByUserNameAsync().
            this.Mode = enMode.Update;
        }

        public static async Task<List<UserDTO>> GetAllUsersAsync() => await clsUserData.GetAllUsersAsync();

        public static async Task<clsUser> FindAsync(int userID)
        {
            UserDTO uDto = await clsUserData.GetUserByIdAsync(userID);
            if (uDto != null)
            {
                clsUser user = new clsUser(uDto);

                user.RefreshTokenInfo = await clsUserRefreshToken.FindByUserIDAsync(userID);

                return user;
            }
            return null;
        }

        public static async Task<clsUser> FindUserByUserNameAsync(string userName)
        {
            // Call DAL to get the user data
             UserDTO uDto = await clsUserData.GetUserByUserNameAsync(userName);
            if (uDto != null)
            {
                clsUser user = new clsUser(uDto);
                user.RefreshTokenInfo = await clsUserRefreshToken.FindByUserIDAsync(user.UserID);
                return user;
            }
            return null;
        }

        private async Task<bool> _AddNewUserAsync()
        {
            this.UserID = await clsUserData.AddNewUserAsync(new CreateUserDTO
            {
                PersonID = this.PersonID,
                UserName = this.UserName,
                PasswordHash = this.PasswordHash,
                Role = this.Role
            });

            return (this.UserID != -1);
        }

        private async Task<bool> _UpdateUserAsync()
        {
            switch (UpdateType)
            {
                case enUpdateType.UserName:
                    return await clsUserData.UpdateUserNameAsync(new UpdateUserNameDTO
                    {
                        UserID = this.UserID,
                        NewUserName = this.UserName
                    });

                case enUpdateType.Password:
                   
                    return await clsUserData.UpdateUserPasswordAsync(new UpdateUserPasswordDTO
                    {
                        UserID = this.UserID,
                        NewPasswordHash = this.PasswordHash
                    });

                case enUpdateType.Role:
                    return await clsUserData.UpdateUserRoleAsync(new UpdateUserRoleDTO
                    {
                        UserID = this.UserID,
                        NewRole = this.Role
                    });

                default:
                    return false;
            }
        }

        public async Task<bool> SaveAsync()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (await _AddNewUserAsync()) 
                    { 
                        Mode = enMode.Update;
                        return true;
                    }
                    return false;
                case enMode.Update:
                    return await _UpdateUserAsync();
            }
            return false;
        }

        public static async Task<bool> DeleteAsync(int userID) => await clsUserData.DeleteUserAsync(userID);

        public static async Task<bool> IsUserExistByPersonIDAsync(int PersonID)
        {
            return await clsUserData.IsUserExistByPersonIDAsync(PersonID);
        }
    }
}