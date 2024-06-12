using DiamondShopBOs;
using DiamondShopRepositories;
using DiamondShopRepositories.UserRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopServices.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository = null;
        public UserService()
        {
            userRepository = new UserRepository();
        }

        public void AddUser(tblUser user)
        {
            userRepository.AddUser(user);
        }

        public bool CheckDuplicateEmail(string email)
        {
            return userRepository.CheckDuplicateEmail(email);
        }

        public bool CheckDuplicateUserName(string userName)
        {
            return userRepository.CheckDuplicateUserName(userName);
        }

        public string GenerateResetCode(string email)
        {
           return userRepository.GenerateResetCode(email);
        }

        public List<tblUser> GetAllUser()
        {
            return userRepository.GetAllUser();
        }

        public string GetResetCodeByEmail(string email)
        {
            return userRepository.GetResetCodeByEmail(email);
        }

        public string GetTheLastestUserID()
        {
            return userRepository.GetTheLastestUserID();
        }

        public tblUser GetUserByEmail(string email)
        {
            return userRepository.GetUserByEmail(email);
        }

        public tblUser GetUserById(string id)
        {
            return userRepository.GetUserById(id);
        }

        public tblUser LoginUser(string userName, string passWord)
        {
            return userRepository.LoginUser(userName, passWord);
        }

        public void ResetPassword(string newPassword, string email, string resetCode)
        {
            userRepository.ResetPassword(newPassword, email, resetCode);
        }

        public void UpdateUser(string id, tblUser user)
        {
            userRepository.UpdateUser(id, user);
        }

    }
}
