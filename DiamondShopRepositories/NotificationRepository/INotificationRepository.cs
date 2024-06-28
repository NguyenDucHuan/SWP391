using DiamondShopBOs;
using System.Collections.Generic;

namespace DiamondShopRepositories.NotificationRepository
{
    public interface INotificationRepository
    {
        void AddNotification(tblNotification notification);
        List<tblNotification> GetNotificationsByUserId(string userId);
        int GetUnreadNotificationCountByUserId(string userId);
        void MarkAllAsRead(string notificationID);
    }
}
