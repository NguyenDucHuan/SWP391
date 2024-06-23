using DiamondShopBOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiamondShopDAOs
{
    public class ManagerDAO
    {
        private readonly DiamondShopManagementEntities _context;

        public ManagerDAO()
        {
            _context = new DiamondShopManagementEntities();
        }

        public List<tblUser> GetUsers()
        {
            return _context.tblUsers.Where(u => u.roleID == 1).ToList();
        }

        public List<tblOrder> GetOrders()
        {
            return _context.tblOrders.ToList();
        }

        public List<tblDiamond> GetDiamonds()
        {
            return _context.tblDiamonds.ToList();
        }

        public List<tblUser> GetUsersByRole(int roleID)
        {
            return _context.tblUsers.Where(u => u.roleID == roleID).ToList();
        }

        public List<RevenueData> GetRevenueData()
        {
            var data = _context.tblOrders
                .Where(o => o.saleDate != null)
                .GroupBy(o => System.Data.Entity.DbFunctions.TruncateTime(o.saleDate))
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(o => (double?)o.totalMoney) ?? 0
                })
                .OrderBy(r => r.Date)
                .ToList()
                .Select(x => new RevenueData
                {
                    Date = x.Date.Value.ToString("yyyy-MM-dd"),
                    Revenue = x.Revenue
                }).ToList();

            return data;
        }

        public List<RegistrationData> GetRegistrationData()
        {
            var rawData = _context.tblUsers
                .Where(u => u.roleID == 1)
                .GroupBy(u => new { u.createDate.Year, u.createDate.Month, u.createDate.Day })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Day = g.Key.Day,
                    Count = g.Count()
                })
                .OrderBy(r => r.Year)
                .ThenBy(r => r.Month)
                .ThenBy(r => r.Day)
                .ToList();

            var registrationData = rawData.Select(x => new RegistrationData
            {
                Date = $"{x.Year}-{x.Month.ToString().PadLeft(2, '0')}-{x.Day.ToString().PadLeft(2, '0')}",
                Registrations = x.Count
            }).ToList();

            return registrationData;
        }

        public void AddOrderStatusUpdate(tblOrderStatusUpdate statusUpdate)
        {
            _context.tblOrderStatusUpdates.Add(statusUpdate);
            _context.SaveChanges();
        }

        public tblOrder GetOrderById(string orderId)
        {
            return _context.tblOrders.FirstOrDefault(o => o.orderID == orderId);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public tblUser GetUserById(string userId)
        {
            return _context.tblUsers.FirstOrDefault(u => u.userID == userId);
        }
        public tblAccentStone GetAccentStoneById(int accentStoneId)
        {
            return _context.tblAccentStones.FirstOrDefault(a => a.accentStoneID == accentStoneId);
        }

        public tblSetting GetSettingById(int settingId)
        {
            return _context.tblSettings.FirstOrDefault(s => s.settingID == settingId);
        }

        public void AddUser(tblUser user)
        {
            // Check if the userName or email already exists
            bool userNameExists = _context.tblUsers.Any(u => u.userName == user.userName);
            bool emailExists = _context.tblUsers.Any(u => u.email == user.email);

            if (userNameExists)
            {
                throw new Exception($"UserName '{user.userName}' already exists.");
            }

            if (emailExists)
            {
                throw new Exception($"Email '{user.email}' already exists.");
            }

            // If no duplicates, add the user
            _context.tblUsers.Add(user);
            _context.SaveChanges();
        }
        public void AddVoucher(tblVoucher voucher)
        {
            _context.tblVouchers.Add(voucher);
            _context.SaveChanges();
        }

        public List<tblVoucher> GetVouchers()
        {
            return _context.tblVouchers.ToList();
        }

        public void SaveVoucherChanges()
        {
            _context.SaveChanges();
        }
        public List<tblAccentStone> GetAccentStones()
        {
            return _context.tblAccentStones.ToList();
        }

        public List<tblSetting> GetSettings()
        {
            return _context.tblSettings.ToList();
        }

    }
}
