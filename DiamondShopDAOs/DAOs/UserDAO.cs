using DiamondShopBOs;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiamondShopDAOs
{
    public class UserDAO
    {
        private readonly DiamondShopManagementEntities diamondShopManagementEntities = null;
        public UserDAO()
        {
            if (diamondShopManagementEntities == null)
            {
                diamondShopManagementEntities = new DiamondShopManagementEntities();
            }
        }
        public List<tblUser> GetAllUser()
        {
            return diamondShopManagementEntities.tblUsers.ToList();
        }
        public tblUser GetUserById(String id)
        {
            return diamondShopManagementEntities.tblUsers.Where(d => d.userID.Contains(id)).FirstOrDefault();
        }
        public tblUser LoginUser(string userName, string passWord)
        {
            tblUser user = diamondShopManagementEntities.tblUsers.FirstOrDefault(d => d.userName.Contains(userName));
            if (user != null)
            {
                if (user.password == passWord)
                {
                    return user;
                }
                else
                {
                    throw new ArgumentException("Password Incorrect!!");
                }
            }
            else
            {
                throw new ArgumentException("UserID Not Found!!");
            }
        }
        public void AddUser(tblUser user)
        {
            try
            {
                using (var diamondShop = new DiamondShopManagementEntities())
                {
                    diamondShop.tblUsers.Add(user);
                    diamondShop.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the user.", ex);
            }
        }
        public void UpdateUser(String id, tblUser user)
        {
            try
            {
                var diamondShop = new DiamondShopManagementEntities();
                var outUser = diamondShop.tblUsers.Where(d => d.userID.Contains(user.userID)).FirstOrDefault();
                if (outUser != null)
                {
                    outUser.fullName = user.fullName;
                    outUser.password = user.password;
                    outUser.email = user.email;
                    diamondShop.SaveChanges();
                }
                else
                {
                    throw new ArgumentException("Not Exisis User");
                }

            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the user.", ex);
            }
        }
        public tblUser GetUserByEmail(String email)
        {
            tblUser user = diamondShopManagementEntities.tblUsers.FirstOrDefault(d => d.email.Contains(email));
            if (user != null)
            {
                return user;
            }
            return null;
        }
        public bool CheckDuplicateUserName(String userName)
        {
            var user = diamondShopManagementEntities.tblUsers.FirstOrDefault(d => d.userName.Contains(userName));
            if (user != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckDuplicateEmail(string email)
        {
            var user = diamondShopManagementEntities.tblUsers.FirstOrDefault(d => d.email.Contains(email));
            if (user != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public String GetTheLastestUserID()
        {
            var allUsers = diamondShopManagementEntities.tblUsers.ToList();
            var usersWithFormattedId = allUsers.Where(d => Regex.IsMatch(d.userID, @"^ID\d+$")).ToList();
            if (!usersWithFormattedId.Any())
            {
                return null;
            }
            var latestUser = usersWithFormattedId
                .OrderByDescending(d => int.Parse(Regex.Match(d.userID, @"\d+$").Value))
                .FirstOrDefault();

            return latestUser?.userID;
        }
        public void UpdatePassWord(String newPass, String userID)
        {
            try
            {
                tblUser user = diamondShopManagementEntities.tblUsers.Where(d => d.userID.Contains(userID)).FirstOrDefault();
                if (user != null)
                {
                    user.password = newPass;
                    user.resetCode = "";
                    diamondShopManagementEntities.SaveChanges();
                }
                else
                {
                    throw new ArgumentException("Not Exisis User");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the user.", ex);
            }
        }
        public string GenerateResetCode(string email)
        {
            string resetCode = "";
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] data = new byte[6];
                rng.GetBytes(data);
                resetCode = Convert.ToBase64String(data).Substring(0, 6).Replace("/", "").Replace("+", "");
            }
            tblUser user = GetUserByEmail(email);
            if (user != null)
            {
                user.resetCode = resetCode;
                diamondShopManagementEntities.SaveChanges();
            }
            else
            {
                throw new Exception("Email not found.");
            }
            return resetCode;
        }

        public String GetResetCodeByEmail(String email)
        {
            String resetCode = diamondShopManagementEntities.tblUsers.Where(d => d.email.Contains(email)).FirstOrDefault().resetCode;
            if (resetCode == null)
            {
                try
                {
                    GenerateResetCode(email);
                    return resetCode = diamondShopManagementEntities.tblUsers.Where(d => d.email.Contains(email)).FirstOrDefault().resetCode;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

            }
            else
            {
                return resetCode;
            }
        }
        public void ResetPassword(String newPassword, String email, String resetCode)
        {
            tblUser user = diamondShopManagementEntities.tblUsers.Where(d => (d.email.Contains(email) && d.resetCode.Contains(resetCode))).FirstOrDefault();
            try
            {
                if (user != null)
                {
                    user.resetCode = "";
                    user.password = newPassword;
                    diamondShopManagementEntities.SaveChanges();
                }
                else { throw new Exception("Invalid reset code."); }
            }
            catch (Exception ex)
            {
                throw new Exception("Error resetPassword");

            }
        }
    }
}
