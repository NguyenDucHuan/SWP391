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
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        [HttpGet]
        public ActionResult EditProfile()
        {
            if (Session["IsAuthenticated"] == null || !(bool)Session["IsAuthenticated"])
            {
                return RedirectToAction("Index", "Login");
            }

            string userId = Session["UserID"] as string;
            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            User user = GetUserById(userId);
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
        public ActionResult EditProfile(User model, string oldPassword, string newPassword)
        {
            if (Session["IsAuthenticated"] == null || !(bool)Session["IsAuthenticated"])
            {
                return RedirectToAction("Index", "Login");
            }

            string userId = Session["UserID"] as string;
            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            User user = GetUserById(userId);
            if (user == null)
            {
                return HttpNotFound("User not found");
            }

            // Check old password if new password is provided
            if (!string.IsNullOrEmpty(newPassword))
            {
                if (string.IsNullOrEmpty(oldPassword) || !ValidateOldPassword(userId, oldPassword))
                {
                    ModelState.AddModelError("OldPassword", "Old password is incorrect.");
                    return View(model);
                }
                user.Password = HashPassword(newPassword);
            }

            // Update fields if they have changed
            if (!string.IsNullOrEmpty(model.FullName) && model.FullName != user.FullName)
            {
                user.FullName = model.FullName;
            }

            if (!string.IsNullOrEmpty(model.Email) && model.Email != user.Email)
            {
                user.Email = model.Email;
            }

            UpdateUser(user);
            ViewBag.SuccessMessage = "Profile updated successfully!";
            ViewBag.UserName = Session["UserName"] as string; // Lấy UserName gốc từ Session
            return View(user);
        }

        private User GetUserById(string userId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand getUser = new SqlCommand("SELECT * FROM tblUsers WHERE userID = @UserID", conn);
                getUser.Parameters.AddWithValue("@UserID", userId);
                using (SqlDataReader reader = getUser.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            UserID = reader["userID"].ToString(),
                            UserName = reader["userName"].ToString(),
                            FullName = reader["fullName"].ToString(),
                            Email = reader["email"].ToString(),
                            Password = reader["password"].ToString(),
                            RoleID = int.Parse(reader["roleID"].ToString()),
                            Status = bool.Parse(reader["status"].ToString()),
                            BonusPoint = reader["bonusPoint"] as int?
                        };
                    }
                }
            }
            return null;
        }

        private void UpdateUser(User user)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand updateUser = new SqlCommand("UPDATE tblUsers SET fullName = @FullName, email = @Email, password = @Password, roleID = @RoleID, status = @Status, bonusPoint = @BonusPoint WHERE userID = @UserID", conn);
                updateUser.Parameters.AddWithValue("@UserID", user.UserID);
                updateUser.Parameters.AddWithValue("@FullName", user.FullName);
                updateUser.Parameters.AddWithValue("@Email", user.Email);
                updateUser.Parameters.AddWithValue("@Password", user.Password);
                updateUser.Parameters.AddWithValue("@RoleID", user.RoleID);
                updateUser.Parameters.AddWithValue("@Status", user.Status);
                updateUser.Parameters.AddWithValue("@BonusPoint", user.BonusPoint.HasValue ? (object)user.BonusPoint.Value : DBNull.Value);
                updateUser.ExecuteNonQuery();
            }
        }

        private bool ValidateOldPassword(string userId, string oldPassword)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand getUser = new SqlCommand("SELECT password FROM tblUsers WHERE userID = @UserID", conn);
                getUser.Parameters.AddWithValue("@UserID", userId);
                string storedPassword = (string)getUser.ExecuteScalar();
                return storedPassword == HashPassword(oldPassword);
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
