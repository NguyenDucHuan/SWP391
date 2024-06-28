using DiamondShopBOs;
using DiamondShopRepositories.NotificationRepository;
using System.Collections.Generic;

namespace DiamondShopServices.NotificationService
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService()
        {
            _notificationRepository = new NotificationRepository();
        }

        public void AddNotification(tblNotification notification)
        {
            _notificationRepository.AddNotification(notification);
        }

        public List<tblNotification> GetNotificationsByUserId(string userId)
        {
            return _notificationRepository.GetNotificationsByUserId(userId);
        }

        public int GetUnreadNotificationCountByUserId(string userId)
        {
            return _notificationRepository.GetUnreadNotificationCountByUserId(userId);
        }


        public void MarkAllAsRead(string userId)
        {
            _notificationRepository.MarkAllAsRead(userId);
        }

    }
}
