﻿using DiamondShopBOs;
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

        public void AddUser(tblUser user)
        {
            _managerDAO.AddUser(user);
        }
    }
}
