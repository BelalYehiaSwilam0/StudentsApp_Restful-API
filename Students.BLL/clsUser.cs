using System;
using System.Collections.Generic;
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

            // Load refresh token data
            this.RefreshTokenInfo =
                clsUserRefreshToken.FindByUserID(this.UserID);
            this.Mode = enMode.Update;
        }

        public static List<UserDTO> GetAllUsers() => clsUserData.GetAllUsers();

        public static clsUser Find(int userID)
        {
            UserDTO uDto = clsUserData.GetUserById(userID);
            return (uDto != null) ? new clsUser(uDto) : null;
        }

        public static clsUser FindUserByUserName(string userName)
        {
            // Call DAL to get the user data
             UserDTO uDto = clsUserData.GetUserByUserName(userName);
            return (uDto != null) ? new clsUser(uDto) : null;
        }

        private bool _AddNewUser()
        {
            this.UserID = clsUserData.AddNewUser(new CreateUserDTO
            {
                PersonID = this.PersonID,
                UserName = this.UserName,
                PasswordHash = this.PasswordHash,
                Role = this.Role
            });

            return (this.UserID != -1);
        }

        private bool _UpdateUser()
        {
            switch (UpdateType)
            {
                case enUpdateType.UserName:
                    return clsUserData.UpdateUserName(new UpdateUserNameDTO
                    {
                        UserID = this.UserID,
                        NewUserName = this.UserName
                    });

                case enUpdateType.Password:
                   
                    return clsUserData.UpdateUserPassword(new UpdateUserPasswordDTO
                    {
                        UserID = this.UserID,
                        NewPasswordHash = this.PasswordHash
                    });

                case enUpdateType.Role:
                    return clsUserData.UpdateUserRole(new UpdateUserRoleDTO
                    {
                        UserID = this.UserID,
                        NewRole = this.Role
                    });

                default:
                    return false;
            }
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewUser()) 
                    { 
                        Mode = enMode.Update;
                        return true;
                    }
                    return false;
                case enMode.Update:
                    return _UpdateUser();
            }
            return false;
        }

        public static bool Delete(int userID) => clsUserData.DeleteUser(userID);

        public static bool IsUserExistByPersonID(int PersonID)
        {
            return clsUserData.IsUserExistByPersonID(PersonID);
        }
    }
}