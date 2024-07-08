﻿using DiamondShopBOs;
using System.Collections.Generic;

namespace DiamondShopServices.NotificationService
{
    public interface INotificationService
    {
        void AddNotification(tblNotification notification);
        List<tblNotification> GetNotificationsByUserId(string userId);
        int GetUnreadNotificationCountByUserId(string userId);
        void MarkAllAsRead(string notificationID);
        tblNotification GetNotificationById(int notificationID);
        void UpdateNotification(tblNotification notification);
        List<tblNotification> GetAllNotifications();
    }
}
