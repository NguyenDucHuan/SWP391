using DiamondShopBOs;
using System.Collections.Generic;
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

        public void MarkAsRead(string notificationID)
        {
            var notification = _context.tblNotifications.Find(notificationID);
            if (notification != null)
            {
                notification.status = false; // Đánh dấu là đã đọc
                _context.SaveChanges();
            }
        }
    }
}
