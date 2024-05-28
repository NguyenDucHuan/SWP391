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
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        [HttpGet]
        [Route("ForgotPassword")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("ForgotPassword/SendResetCode")]
        public ActionResult SendResetCode(string email)
        {
            // Generate reset code
            string resetCode = GenerateResetCode();

            // Save reset code in the database
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE tblUsers SET resetCode = @ResetCode WHERE email = @Email", conn);
                cmd.Parameters.AddWithValue("@ResetCode", resetCode);
                cmd.Parameters.AddWithValue("@Email", email);
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    // Send reset code email
                    SendResetEmail(email, resetCode);
                    TempData["Email"] = email;
                    TempData["Message"] = "A password reset code has been sent to your email.";
                    return RedirectToAction("EnterResetCode");
                }
                else
                {
                    ViewBag.Message = "Email not found.";
                }
            }

            return View("Index");
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
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT userID FROM tblUsers WHERE email = @Email AND resetCode = @ResetCode", conn);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@ResetCode", resetCode);
                string userId = cmd.ExecuteScalar() as string;

                if (!string.IsNullOrEmpty(userId))
                {
                    TempData["ResetCode"] = resetCode;
                    return RedirectToAction("ResetPassword", new { code = resetCode });
                }
                else
                {
                    ViewBag.Message = "Invalid reset code.";
                }
            }

            ViewBag.Email = email;
            return View();
        }

        [HttpGet]
        [Route("ForgotPassword/ResetPassword")]
        public ActionResult ResetPassword(string code)
        {
            ViewBag.ResetCode = code;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("ForgotPassword/ResetPassword")]
        public ActionResult ResetPassword(string code, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ViewBag.Message = "Passwords do not match.";
                ViewBag.ResetCode = code;
                return View();
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT userID FROM tblUsers WHERE resetCode = @ResetCode", conn);
                cmd.Parameters.AddWithValue("@ResetCode", code);
                string userId = cmd.ExecuteScalar() as string;

                if (!string.IsNullOrEmpty(userId))
                {
                    // Hash new password
                    string hashedPassword = HashPassword(newPassword);

                    // Update password in the database
                    cmd = new SqlCommand("UPDATE tblUsers SET password = @Password, resetCode = NULL WHERE userID = @UserID", conn);
                    cmd.Parameters.AddWithValue("@Password", hashedPassword);
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.ExecuteNonQuery();

                    // Redirect to login page after successful password reset
                    TempData["Message"] = "Password has been reset successfully. You can now log in with your new password.";
                    return RedirectToAction("Index", "Login");
                }
                else
                {
                    ViewBag.Message = "Invalid reset code.";
                }
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

        private string GenerateResetCode()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] data = new byte[6];
                rng.GetBytes(data);
                return Convert.ToBase64String(data).Substring(0, 6).Replace("/", "").Replace("+", "");
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
