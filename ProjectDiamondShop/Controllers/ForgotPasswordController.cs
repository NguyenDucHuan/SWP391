using DiamondShopServices.UserService;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;

namespace ProjectDiamondShop.Controllers
{
    public class ForgotPasswordController : Controller
    {
        private readonly IUserService userService = null;
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
        public ForgotPasswordController()
        {
            userService = new UserService();
        }
        [HttpGet]
        [Route("ForgotPassword")]
        public ActionResult Index()
        {
            if (IsAdmin())
            {
                return RedirectToAction("Index", "Manager");
            }
            if (IsManager())
            {
                return RedirectToAction("Index", "Manager");
            }
            if (IsSaleStaff())
            {
                return RedirectToAction("Index", "SaleStaff");
            }
            if (IsDelivery())
            {
                return RedirectToAction("Index", "DeliveryStaff");
            }
            return View("ForgotPassword");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("ForgotPassword/SendResetCode")]
        public ActionResult SendResetCode(string email)
        {
            if (IsAdmin())
            {
                return RedirectToAction("Index", "Manager");
            }
            if (IsManager())
            {
                return RedirectToAction("Index", "Manager");
            }
            if (IsSaleStaff())
            {
                return RedirectToAction("Index", "SaleStaff");
            }
            if (IsDelivery())
            {
                return RedirectToAction("Index", "DeliveryStaff");
            }
            try
            {
                string resetCode = userService.GenerateResetCode(email);
                SendResetEmail(email, resetCode);
                TempData["Email"] = email;
                TempData["Message"] = "A password reset code has been sent to your email.";
                return RedirectToAction("EnterResetCode");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }

            return View("ForgotPassword");
        }

        [HttpGet]
        [Route("ForgotPassword/EnterResetCode")]
        public ActionResult EnterResetCode()
        {
            ViewBag.Email = TempData["Email"];
            ViewBag.Message = TempData["Message"];
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("ForgotPassword/EnterResetCode")]
        public ActionResult EnterResetCode(string email, string resetCode)
        {
            if (resetCode.Contains(userService.GetResetCodeByEmail(email)))
            {
                TempData["ResetCode"] = resetCode;
                return RedirectToAction("ResetPassword", new { code = resetCode, email = email });
            }
            else
            {
                ViewBag.Message = "Invalid reset code.";
            }
            ViewBag.Email = email;
            return View();
        }
        [HttpGet]
        [Route("ForgotPassword/ResetPassword")]
        public ActionResult ResetPassword(string code, string email)
        {
            if (IsAdmin())
            {
                return RedirectToAction("Index", "Manager");
            }
            if (IsManager())
            {
                return RedirectToAction("Index", "Manager");
            }
            if (IsSaleStaff())
            {
                return RedirectToAction("Index", "SaleStaff");
            }
            if (IsDelivery())
            {
                return RedirectToAction("Index", "DeliveryStaff");
            }
            ViewBag.Email = email;
            ViewBag.ResetCode = code;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("ForgotPassword/ResetPassword")]
        public ActionResult ResetPassword(string code, String email, string newPassword, string confirmPassword)
        {
            if (IsAdmin())
            {
                return RedirectToAction("Index", "Manager");
            }
            if (IsManager())
            {
                return RedirectToAction("Index", "Manager");
            }
            if (IsSaleStaff())
            {
                return RedirectToAction("Index", "SaleStaff");
            }
            if (IsDelivery())
            {
                return RedirectToAction("Index", "DeliveryStaff");
            }
            if (newPassword != confirmPassword)
            {
                ViewBag.Message = "Passwords do not match.";
                ViewBag.ResetCode = code;
                return View();
            }

            try
            {
                userService.ResetPassword(HashPassword(newPassword), email, code);
                TempData["Message"] = "Password has been reset successfully. You can now log in with your new password.";
                return RedirectToAction("Index", "Account");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            ViewBag.ResetCode = code;
            return View();
        }
        private void SendResetEmail(string email, string resetCode)
        {
            var message = new MailMessage();
            message.To.Add(new MailAddress(email));
            message.From = new MailAddress(ConfigurationManager.AppSettings["smtp:from"]);
            message.Subject = "Password Reset Code";
            message.Body = $"Your password reset code is: {resetCode}";
            message.IsBodyHtml = false;

            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = ConfigurationManager.AppSettings["smtp:username"],
                    Password = ConfigurationManager.AppSettings["smtp:password"]
                };
                smtp.Credentials = credential;
                smtp.Host = ConfigurationManager.AppSettings["smtp:host"];
                smtp.Port = int.Parse(ConfigurationManager.AppSettings["smtp:port"]);
                smtp.EnableSsl = true;
                smtp.Send(message);
            }
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
