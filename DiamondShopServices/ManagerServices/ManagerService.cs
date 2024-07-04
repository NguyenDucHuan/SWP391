using DiamondShopBOs;
using DiamondShopRepositories.ManagerRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopServices.ManagerServices
{
    public class ManagerService : IManagerService
    {
        private readonly IManagerRepository _managerRepository;

        public ManagerService()
        {
            _managerRepository = new ManagerRepository();
        }

        public List<tblUser> GetUsers()
        {
            return _managerRepository.GetUsers();
        }

        public List<tblOrder> GetOrders()
        {
            return _managerRepository.GetOrders();
        }

        public List<tblDiamond> GetDiamonds()
        {
            return _managerRepository.GetDiamonds();
        }

        public List<tblUser> GetUsersByRole(int roleID)
        {
            return _managerRepository.GetUsersByRole(roleID);
        }

        public List<RevenueData> GetRevenueData()
        {
            return _managerRepository.GetRevenueData();
        }

        public List<RegistrationData> GetRegistrationData()
        {
            return _managerRepository.GetRegistrationData();
        }

        public void AddOrderStatusUpdate(tblOrderStatusUpdate statusUpdate)
        {
            _managerRepository.AddOrderStatusUpdate(statusUpdate);
        }

        public tblOrder GetOrderById(string orderId)
        {
            return _managerRepository.GetOrderById(orderId);
        }

        public void SaveChanges()
        {
            _managerRepository.SaveChanges();
        }

        public tblUser GetUserById(string userId)
        {
            return _managerRepository.GetUserById(userId);
        }
        public tblAccentStone GetAccentStoneById(int accentStoneId)
        {
            return _managerRepository.GetAccentStoneById(accentStoneId);
        }

        public tblSetting GetSettingById(int settingId)
        {
            return _managerRepository.GetSettingById(settingId);
        }

        public void AddUser(tblUser user)
        {
            _managerRepository.AddUser(user);
        }
        public void AddVoucher(tblVoucher voucher)
        {
            _managerRepository.AddVoucher(voucher);
        }

        public void SaveVoucherChanges()
        {
            _managerRepository.SaveVoucherChanges();
        }
        public List<tblVoucher> GetVouchers()
        {
            return _managerRepository.GetVouchers();
        }
        public List<tblAccentStone> GetAccentStones()
        {
            return _managerRepository.GetAccentStones();
        }

        public List<tblSetting> GetSettings()
        {
            return _managerRepository.GetSettings();
        }
        public void AddSetting(tblSetting setting)
        {
            _managerRepository.AddSetting(setting);
        }
        public void AddAccentStone(tblAccentStone accentStone)
        {
            _managerRepository.AddAccentStone(accentStone);
        }
        public List<RevenueData> GetChartData(int month, int year) => _managerRepository.GetChartData(month, year); // New method
        public tblDiamond GetDiamondById(int diamondID)
        {
            return _managerRepository.GetDiamondById(diamondID);
        }
        public void ToggleVoucherStatus(int voucherId, bool status)
        {
            _managerRepository.ToggleVoucherStatus(voucherId, status);
        }
        public void UpdateVoucherQuantity(int voucherId, int? newQuantity)
        {
            _managerRepository.UpdateVoucherQuantity(voucherId, newQuantity);
        }
    }
}
