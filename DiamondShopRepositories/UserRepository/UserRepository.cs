using DiamondShopBOs;
using DiamondShopDAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopRepositories.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDAO userDAO = null;

        public UserRepository()
        {
            userDAO = new UserDAO();
        }
        public void AddUser(tblUser user)
        {
            userDAO.AddUser(user);
        }

        public bool CheckDuplicateEmail(string email)
        {
            return userDAO.CheckDuplicateEmail(email);
        }

        public bool CheckDuplicateUserName(string userName)
        {
            return userDAO.CheckDuplicateUserName(userName);
        }

        public string GenerateResetCode(string email)
        {
           return userDAO.GenerateResetCode(email);
        }

        public List<tblUser> GetAllUser()
        {
            return userDAO.GetAllUser();
        }

        public string GetResetCodeByEmail(string email)
        {
            return userDAO.GetResetCodeByEmail(email);
        }

        public string GetTheLastestUserID()
        {
            return userDAO.GetTheLastestUserID();
        }

        public tblUser GetUserByEmail(string email)
        {
            return userDAO.GetUserByEmail(email);
        }

        public tblUser GetUserById(string id)
        {
            return userDAO.GetUserById(id);
        }

        public tblUser LoginUser(string userName, string passWord)
        {
            return userDAO.LoginUser(userName, passWord);
        }

        public void ResetPassword(string newPassword, string email, string resetCode)
        {
            userDAO.ResetPassword(newPassword, email, resetCode);
        }

        public void UpdateUser(string id, tblUser user)
        {
            userDAO.UpdateUser(id, user);
        }
        public List<tblUser> GetUsersByRole(List<int> roleIDs)
        {
            return userDAO.GetUsersByRole(roleIDs);
        }
    }
}
