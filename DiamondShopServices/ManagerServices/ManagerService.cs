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

        public void AddUser(tblUser user)
        {
            _managerRepository.AddUser(user);
        }
    }
}
