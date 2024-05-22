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

            // Hash the UserID and Password
            string hashedUserID = HashUserID(userName);
            string hashedPassword = HashPassword(password);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM tblUsers WHERE userID = @UserID AND password = @Password", conn);
                cmd.Parameters.AddWithValue("@UserID", hashedUserID);
                cmd.Parameters.AddWithValue("@Password", hashedPassword);

                int userExists = (int)cmd.ExecuteScalar();

                if (userExists > 0)
                {
                    // Authentication successful
                    Session["UserID"] = hashedUserID;
                    Session["UserName"] = userName;
                    Session["IsAuthenticated"] = true;
                    TempData["SuccessMessage"] = "Login successful!";
                    TempData["UserName"] = userName;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Authentication failed
                    ModelState.AddModelError("", "Invalid user name or password.");
                    ViewBag.UserName = userName;
                    ViewBag.Password = password;
                    return View("LoginPage");
                }
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

        private string HashUserID(string userID)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(userID));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString().Substring(0, 32); 
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
