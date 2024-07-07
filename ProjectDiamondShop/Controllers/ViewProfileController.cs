using DiamondShopBOs;
using DiamondShopServices.NotificationService;
using DiamondShopServices.UserService;
using ProjectDiamondShop.Models;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;

namespace ProjectDiamondShop.Controllers
{
    public class ViewProfileController : Controller
    {
        private readonly IUserService service = null;
        private readonly INotificationService _notificationService;

        private bool IsAdmin()
        {
            return Session["RoleID"] != null && (int)Session["RoleID"] == 2;
        }
        private bool IsSaleStaff()
        {
            return Session["RoleID"] != null && (int)Session["RoleID"] == 5;
        }
        private bool IsDelivery()
        {
            return Session["RoleID"] != null && (int)Session["RoleID"] == 4;
        }
        private bool IsManager()
        {
            return Session["RoleID"] != null && (int)Session["RoleID"] == 3;
        }
        public ViewProfileController()
        {
            service = new UserService();
            _notificationService = new NotificationService();
        }
        [HttpGet]
        public ActionResult EditProfile()
        {
            if (Session["IsAuthenticated"] == null || !(bool)Session["IsAuthenticated"])
            {
                return RedirectToAction("Index", "Account");
            }

            string userId = Session["UserID"] as string;
            if (userId == null)
            {
                return RedirectToAction("Index", "Account");  
            }

            tblUser user = service.GetUserById(userId);
            if (user == null)
            {
                return HttpNotFound("User not found");
            }

            // Lấy UserID và UserName từ Session để truyền vào View
            ViewBag.OriginalUserID = Session["UserID"] as string;
            ViewBag.UserName = Session["UserName"] as string; // Lấy UserName gốc từ Session

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(tblUser model, string oldPassword, string newPassword)
        {
            if (Session["IsAuthenticated"] == null || !(bool)Session["IsAuthenticated"])
            {
                return RedirectToAction("Index", "Account");
            }

            string userId = Session["UserID"] as string;
            if (userId == null)
            {
                return RedirectToAction("Index", "Account");
            }

            tblUser user = service.GetUserById(userId);
            if (user == null)
            {
                return HttpNotFound("User not found");
            }

            bool passwordChanged = false;

            if (!string.IsNullOrEmpty(newPassword))
            {
                if (string.IsNullOrEmpty(oldPassword) || !ValidateOldPassword(userId, oldPassword))
                {
                    ModelState.AddModelError("OldPassword", "Old password is incorrect.");
                    return View(model);
                }
                user.password = HashPassword(newPassword);
                passwordChanged = true;
            }

            if (!string.IsNullOrEmpty(model.fullName) && model.fullName != user.fullName)
            {
                user.fullName = model.fullName;
            }

            if (!string.IsNullOrEmpty(model.email) && model.email != user.email)
            {
                user.email = model.email;
            }

            try
            {
                service.UpdateUser(userId, user);
                ViewBag.SuccessMessage = "Profile updated successfully!";
                ViewBag.UserName = Session["UserName"] as string;

                if (passwordChanged)
                {
                    _notificationService.AddNotification(new tblNotification
                    {
                        userID = userId,
                        date = DateTime.Now,
                        detail = "Your password has been changed successfully.",
                        status = true
                    });
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while updating the profile.";
            }

            return View(user);
        }


        private void UpdateUser(tblUser user)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private bool ValidateOldPassword(string userId, string NewPassword)
        {
            try
            {
                tblUser user = service.GetUserById(userId);
                if (user.password.Contains(NewPassword))
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return true;
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString().Substring(0, 32);
            }
        }
    }
}
