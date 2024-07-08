using DiamondShopBOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopServices.ManagerServices
{
    public interface IManagerService
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
        void AddSetting(tblSetting setting);
        void AddAccentStone(tblAccentStone accentStone);
        List<RevenueData> GetChartData(int month, int year);
        tblDiamond GetDiamondById(int diamondID);
        void ToggleVoucherStatus(int voucherId, bool status);
        void UpdateVoucherQuantity(int voucherId, int? newQuantity);
        void UpdateVoucher(tblVoucher voucher);
        List<tblVoucher> GetAllVouchers();
    }
}
