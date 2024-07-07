using DiamondShopBOs;
using DiamondShopServices.UserService;
using ProjectDiamondShop.Models;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace ProjectDiamondShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService userService;

        public AccountController()
        {
            userService = new UserService();
        }

        // GET: Login/SignUp
        public ActionResult Index()
        {
            return View("AccountPage");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login()
        {
            string userName = Request.Form["username"];
            string password = Request.Form["password"];
            string hashedPassword = HashString(password);
            string hashedUserName = HashString(userName);

            try
            {
                tblUser loginUser = userService.LoginUser(hashedUserName, hashedPassword);
                Session["UserID"] = loginUser.userID;
                Session["UserName"] = loginUser.userName;
                Session["RoleID"] = loginUser.roleID;
                Session["IsAuthenticated"] = true;

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
                if (ex.Message == "User account is inactive.")
                {
                    ViewBag.Message = "Your account has been disabled, please contact customer service for more details!!!";
                }
                else
                {
                    ViewBag.Message = ex.Message;
                }
                ViewBag.UserName = userName;
                ViewBag.Password = password;
                return View("AccountPage");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register()
        {
            string userName = Request.Form["txtUname"];
            string password = Request.Form["txtPass"];
            string confirmPassword = Request.Form["txtCPass"];
            string fullName = Request.Form["txtName"];
            string email = Request.Form["txtEmail"];
            bool hasErrors = false;

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("RequiredFields", "All fields are required.");
                hasErrors = true;
            }

            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                ModelState.AddModelError("PasswordRequired", "Password and Confirm Password are required.");
                hasErrors = true;
            }
            else if (password != confirmPassword)
            {
                ModelState.AddModelError("PasswordMismatch", "Passwords do not match.");
                hasErrors = true;
            }
            else
            {
                if (!Regex.IsMatch(password, @"[a-z]"))
                {
                    ModelState.AddModelError("PasswordLowercase", "Password must contain at least one lowercase letter.");
                    hasErrors = true;
                }

                if (!Regex.IsMatch(password, @"[A-Z]"))
                {
                    ModelState.AddModelError("PasswordUppercase", "Password must contain at least one uppercase letter.");
                    hasErrors = true;
                }

                if (!Regex.IsMatch(password, @"[\W_]"))
                {
                    ModelState.AddModelError("PasswordSpecialChar", "Password must contain at least one special character.");
                    hasErrors = true;
                }
            }

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ModelState.AddModelError("InvalidEmail", "Email is not in a correct format.");
                hasErrors = true;
            }

            string hashedUserName = HashString(userName);
            if (userService.CheckDuplicateUserName(hashedUserName))
            {
                ModelState.AddModelError("DuplicateUserName", "User name already exists.");
                hasErrors = true;
            }

            if (userService.CheckDuplicateEmail(email))
            {
                ModelState.AddModelError("DuplicateEmail", "Email already exists.");
                hasErrors = true;
            }

            if (hasErrors)
            {
                ViewBag.UserName = userName;
                ViewBag.FullName = fullName;
                ViewBag.Email = email;

                var prioritizedErrors = new[] { "RequiredFields", "PasswordRequired", "PasswordMismatch", "PasswordLowercase", "PasswordUppercase", "PasswordSpecialChar", "DuplicateUserName", "DuplicateEmail", "InvalidEmail" };
                foreach (var key in prioritizedErrors)
                {
                    if (ModelState.ContainsKey(key) && ModelState[key].Errors.Count > 0)
                    {
                        ModelState.AddModelError("", ModelState[key].Errors[0].ErrorMessage);
                        break;
                    }
                }
                return View("AccountPage");
            }

            string hashedPassword = HashString(password);
            string userId = GenerateNextUserId();

            tblUser newUser = new tblUser
            {
                userID = userId,
                userName = hashedUserName,
                fullName = fullName,
                email = email,
                password = hashedPassword,
                roleID = 1,
                status = true,
                resetCode = ""
            };

            try
            {
                userService.AddUser(newUser);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.ToString());
                return View("AccountPage");
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        private string HashString(string str)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(str));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString().Substring(0, 32);
            }
        }

        public string GenerateNextUserId()
        {
            string currentId = userService.GetTheLastestUserID();
            if (string.IsNullOrEmpty(currentId))
            {
                return "ID0000001";
            }

            string numericPart = currentId.Substring(2);
            if (!int.TryParse(numericPart, out int numericValue))
            {
                throw new ArgumentException("Invalid numeric part in ID");
            }
            numericValue++;
            string newNumericPart = numericValue.ToString().PadLeft(numericPart.Length, '0');
            return "ID" + newNumericPart;
        }
    }
}
