using DiamondShopBOs;
using DiamondShopServices;
using DiamondShopServices.UserService;
using ProjectDiamondShop.Models;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;

namespace ProjectDiamondShop.Controllers
{
    public class LoginController : Controller
    {

        private readonly IUserService service = null;
        public LoginController()
        {
            service = new UserService();
        }
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
            string hashedPassword = HashString(password);
            string hashedUserName = HashString(userName);

            try
            {
                tblUser loginUser = service.LoginUser(hashedUserName, hashedPassword);
                Session["UserID"] = loginUser.userID;
                Session["UserName"] = loginUser.userName;
                Session["RoleID"] = loginUser.roleID;
                Session["IsAuthenticated"] = true;

                // Chuyển hướng người dùng đến trang danh sách đơn hàng của họ
                if (loginUser.roleID == 5)
                {
                    return RedirectToAction("Index", "SaleStaff");
                }
                else if (loginUser.roleID == 4)
                {
                    return RedirectToAction("Index", "DeliveryStaff");
                }
                else if (loginUser.roleID == 3)
                {
                    return RedirectToAction("Index", "Manager");
                }
                else if (loginUser.roleID == 2)
                {
                    return RedirectToAction("Index", "Manager");
                }
                else
                {
                    TempData["SuccessMessage"] = "Login successful!";
                    TempData["UserName"] = userName;
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.UserName = userName;
                ViewBag.Password = password;
                return View("LoginPage");
            }

        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        private string HashString(string password)
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
