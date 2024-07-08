using DiamondShopBOs;
using DiamondShopDAOs.DAOs;
using System.Collections.Generic;

namespace DiamondShopRepositories.NotificationRepository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly NotificationDAO _notificationDAO;

        public NotificationRepository()
        {
            _notificationDAO = new NotificationDAO();
        }

        public void AddNotification(tblNotification notification)
        {
            _notificationDAO.AddNotification(notification);
        }

        public List<tblNotification> GetNotificationsByUserId(string userId)
        {
            return _notificationDAO.GetNotificationsByUserId(userId);
        }

        public int GetUnreadNotificationCountByUserId(string userId)
        {
            return _notificationDAO.GetUnreadNotificationCountByUserId(userId);
        }

        public void MarkAllAsRead(string userId)
        {
            _notificationDAO.MarkAllAsRead(userId);
        }

        public tblNotification GetNotificationById(int notificationID)
        {
            return _notificationDAO.GetNotificationById(notificationID);
        }

        public void UpdateNotification(tblNotification notification)
        {
            _notificationDAO.UpdateNotification(notification);
        }
        public IEnumerable<tblNotification> GetAll()
        {
            return _notificationDAO.GetAll();
        }
    }
}
