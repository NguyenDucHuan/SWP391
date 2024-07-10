using DiamondShopBOs;
using DiamondShopDAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopRepositories.ManagerRepositories
{
    public class ManagerRepository : IManagerRepository
    {
        private readonly ManagerDAO _managerDAO;

        public ManagerRepository()
        {
            _managerDAO = new ManagerDAO();
        }

        public List<tblUser> GetUsers()
        {
            return _managerDAO.GetUsers();
        }

        public List<tblOrder> GetOrders()
        {
            return _managerDAO.GetOrders();
        }

        public List<tblDiamond> GetDiamonds()
        {
            return _managerDAO.GetDiamonds();
        }

        public List<tblUser> GetUsersByRole(int roleID)
        {
            return _managerDAO.GetUsersByRole(roleID);
        }

        public List<RevenueData> GetRevenueData()
        {
            return _managerDAO.GetRevenueData();
        }

        public List<RegistrationData> GetRegistrationData()
        {
            return _managerDAO.GetRegistrationData();
        }

        public void AddOrderStatusUpdate(tblOrderStatusUpdate statusUpdate)
        {
            _managerDAO.AddOrderStatusUpdate(statusUpdate);
        }

        public tblOrder GetOrderById(string orderId)
        {
            return _managerDAO.GetOrderById(orderId);
        }

        public void SaveChanges()
        {
            _managerDAO.SaveChanges();
        }

        public tblUser GetUserById(string userId)
        {
            return _managerDAO.GetUserById(userId);
        }
        public tblAccentStone GetAccentStoneById(int accentStoneId)
        {
            return _managerDAO.GetAccentStoneById(accentStoneId);
        }

        public tblSetting GetSettingById(int settingId)
        {
            return _managerDAO.GetSettingById(settingId);
        }

        public void AddUser(tblUser user)
        {
            _managerDAO.AddUser(user);
        }
        public void AddVoucher(tblVoucher voucher)
        {
            _managerDAO.AddVoucher(voucher);
        }

        public void SaveVoucherChanges()
        {
            _managerDAO.SaveVoucherChanges();
        }
        public List<tblVoucher> GetVouchers()
        {
            return _managerDAO.GetVouchers();
        }
        public List<tblAccentStone> GetAccentStones()
        {
            return _managerDAO.GetAccentStones();
        }

        public List<tblSetting> GetSettings()
        {
            return _managerDAO.GetSettings();
        }
        public void AddSetting(tblSetting setting)
        {
            _managerDAO.AddSetting(setting);
        }
        public void AddAccentStone(tblAccentStone accentStone)
        {
            _managerDAO.AddAccentStone(accentStone);
        }
        public List<RevenueData> GetChartData(int month, int year) => _managerDAO.GetChartData(month, year);
        public tblDiamond GetDiamondById(int diamondID)
        {
            return _managerDAO.GetDiamondById(diamondID);
        }
        public void ToggleVoucherStatus(int voucherId, bool status)
        {
            _managerDAO.ToggleVoucherStatus(voucherId, status);
        }
        public void UpdateVoucherQuantity(int voucherId, int? newQuantity)
        {
            _managerDAO.UpdateVoucherQuantity(voucherId, newQuantity);
        }
        public void UpdateVoucher(tblVoucher voucher)
        {
            _managerDAO.UpdateVoucher(voucher);
        }

        public List<tblVoucher> GetAllVouchers()
        {
            return _managerDAO.GetAllVouchers();
        }
        public void UpdateOrderDeliveryStaffName(string orderId, string deliveryStaffId)
        {
            _managerDAO.UpdateOrderDeliveryStaffName(orderId, deliveryStaffId);
        }

    }
}
