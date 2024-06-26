using DiamondShopServices.NotificationService;
using System.Linq;
using System.Web.Mvc;

namespace ProjectDiamondShop.Controllers
{
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationController()
        {
            _notificationService = new NotificationService();
        }

        public ActionResult Index()
        {
            var userId = User.Identity.Name; // Hoặc cách lấy user ID phù hợp khác
            var notifications = _notificationService.GetNotificationsByUserId(userId);
            return View(notifications);
        }

        [HttpPost]
        public ActionResult MarkAsRead(string notificationID)
        {
            _notificationService.MarkAsRead(notificationID);
            return RedirectToAction("Index");
        }
        public JsonResult GetNotifications()
        {
            string userId = Session["UserID"] as string;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "User not authenticated." }, JsonRequestBehavior.AllowGet);
            }

            var notifications = _notificationService.GetNotificationsByUserId(userId)
                                                    .OrderByDescending(n => n.date)
                                                    .Select(n => new
                                                    {
                                                        n.notificationID,
                                                        n.detail,
                                                        date = n.date.ToString("yyyy-MM-dd HH:mm:ss")
                                                    }).ToList();

            return Json(new { success = true, notifications }, JsonRequestBehavior.AllowGet);
        }
    }
}