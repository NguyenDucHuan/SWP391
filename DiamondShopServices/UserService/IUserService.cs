﻿using DiamondShopBOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopServices.UserService
{
    public interface IUserService
    {
        List<tblUser> GetAllUser();
        tblUser GetUserById(String id);
        tblUser LoginUser(String userName, String passWord);
        void AddUser(tblUser user);
        void UpdateUser(String id, tblUser user);
        tblUser GetUserByEmail(String email);
        bool CheckDuplicateUserName(String userName);
        bool CheckDuplicateEmail(String email);
        String GetTheLastestUserID();
        void GenerateResetCode(String email);
        String GetResetCodeByEmail(String email);
        void ResetPassword(String newPassword, String email, String resetCode);
    }
}
