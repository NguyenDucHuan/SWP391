using ProjectDiamondShop.Models;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;

namespace ProjectDiamondShop.Controllers
{
    public class LoginController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // GET: Login
        public ActionResult Index()
        {
            return View("LoginPage");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Authenticate()
        {
            string userName = Request.Form["username"];
            string password = Request.Form["password"];

            // Hash the password and username
            string hashedPassword = HashPassword(password);
            string hashedUserName = HashUserName(userName);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT userID, roleID FROM tblUsers WHERE userName = @UserName AND password = @Password", conn);
                cmd.Parameters.AddWithValue("@UserName", hashedUserName);
                cmd.Parameters.AddWithValue("@Password", hashedPassword);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    string userId = reader["userID"].ToString();
                    int roleId = Convert.ToInt32(reader["roleID"]);

                    Session["UserID"] = userId;
                    Session["UserName"] = userName;
                    Session["RoleID"] = roleId;
                    Session["IsAuthenticated"] = true;

                    if (roleId == 5)
                    {
                        return RedirectToAction("Index", "SaleStaff");
                    }
                    else if (roleId == 4)
                    {
                        return RedirectToAction("Index", "DeliveryStaff");
                    }
                    else
                    {
                        TempData["SuccessMessage"] = "Login successful!";
                        TempData["UserName"] = userName;
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid user name or password.");
                    ViewBag.UserName = userName;
                    ViewBag.Password = password;
                    return View("LoginPage");
                }
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
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

        private string HashUserName(string userName)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(userName));
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
