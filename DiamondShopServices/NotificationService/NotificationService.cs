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

        public void MarkAsRead(string notificationID)
        {
            _notificationRepository.MarkAsRead(notificationID);
        }

    }
}
