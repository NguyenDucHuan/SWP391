﻿using DiamondShopBOs;
using DiamondShopDAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopRepositories.ManagerRepositories
{
    public interface IManagerRepository
    {
        List<tblUser> GetUsers();
        List<tblOrder> GetOrders();
        List<tblDiamond> GetDiamonds();
        List<tblUser> GetUsersByRole(int roleID);
        List<RevenueData> GetRevenueData();
        List<RegistrationData> GetRegistrationData();
        void AddOrderStatusUpdate(tblOrderStatusUpdate statusUpdate);
        tblOrder GetOrderById(string orderId);
        void SaveChanges();
        tblUser GetUserById(string userId);
        tblAccentStone GetAccentStoneById(int accentStoneId);
        tblSetting GetSettingById(int settingId);
        void AddUser(tblUser user);
        void AddVoucher(tblVoucher voucher);
        void SaveVoucherChanges();
        List<tblVoucher> GetVouchers();
        List<tblAccentStone> GetAccentStones();
        List<tblSetting> GetSettings();
    }
}
