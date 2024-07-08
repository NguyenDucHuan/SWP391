using DiamondShopBOs;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DiamondShopDAOs.DAOs
{
    public class NotificationDAO
    {
        private readonly DiamondShopManagementEntities _context;

        public NotificationDAO()
        {
            _context = new DiamondShopManagementEntities();
        }

        public void AddNotification(tblNotification notification)
        {
            _context.tblNotifications.Add(notification);
            _context.SaveChanges();
        }

        public List<tblNotification> GetNotificationsByUserId(string userId)
        {
            return _context.tblNotifications.Where(n => n.userID == userId).ToList();
        }

        public int GetUnreadNotificationCountByUserId(string userId)
        {
            return _context.tblNotifications.Count(n => n.userID == userId && n.status == true);
        }


        public void MarkAllAsRead(string userId)
        {
            var unreadNotifications = _context.tblNotifications.Where(n => n.userID == userId && n.status == true).ToList();
            foreach (var notification in unreadNotifications)
            {
                notification.status = false;
            }
            _context.SaveChanges();
        }
        public tblNotification GetNotificationById(int notificationID)
        {
            return _context.tblNotifications.SingleOrDefault(n => n.notificationID == notificationID);
        }

        public void UpdateNotification(tblNotification notification)
        {
            _context.Entry(notification).State = EntityState.Modified;
            _context.SaveChanges();
        }
        public IEnumerable<tblNotification> GetAll()
        {
            return _context.tblNotifications.ToList();
        }
    }
}
