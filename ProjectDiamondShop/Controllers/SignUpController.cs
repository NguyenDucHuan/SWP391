using DiamondShopBOs;
using DiamondShopServices.UserService;
using ProjectDiamondShop.Models;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace ProjectDiamondShop.Controllers
{
    public class SignUpController : Controller
    {
        private readonly IUserService userService;
        public SignUpController()
        {
            userService = new UserService();
        }
        // GET: Sign Up
        public ActionResult Index()
        {
            return View("SignUpPage");
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

            // Validate inputs
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
                // Validate password strength
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

            // Validate email format
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ModelState.AddModelError("InvalidEmail", "Email is not in a correct format.");
                hasErrors = true;
            }

            // Check if user name already exists
            string hashedUserName = HashString(userName);
            if (userService.CheckDuplicateUserName(hashedUserName))
            {
                ModelState.AddModelError("DuplicateUserName", "User name already exists.");
                hasErrors = true;
            }

            // Check if email already exists
            if (userService.CheckDuplicateEmail(email))
            {
                ModelState.AddModelError("DuplicateEmail", "Email already exists.");
                hasErrors = true;
            }

            // If any validation failed, return to the sign-up page with errors
            if (hasErrors)
            {
                ViewBag.UserName = userName;
                ViewBag.FullName = fullName;
                ViewBag.Email = email;

                // Only clear password fields if they have errors
                if (!ModelState.IsValidField("PasswordRequired") &&
                    !ModelState.IsValidField("PasswordMismatch") &&
                    !ModelState.IsValidField("PasswordLowercase") &&
                    !ModelState.IsValidField("PasswordUppercase") &&
                    !ModelState.IsValidField("PasswordSpecialChar"))
                {
                    ViewBag.Password = password;
                    ViewBag.ConfirmPassword = confirmPassword;
                }

                var prioritizedErrors = new[] { "RequiredFields", "PasswordRequired", "PasswordMismatch", "PasswordLowercase", "PasswordUppercase", "PasswordSpecialChar", "DuplicateUserName", "DuplicateEmail", "InvalidEmail" };
                foreach (var key in prioritizedErrors)
                {
                    if (ModelState.ContainsKey(key) && ModelState[key].Errors.Count > 0)
                    {
                        ModelState.AddModelError("", ModelState[key].Errors[0].ErrorMessage);
                        break;
                    }
                }
                return View("SignUpPage");
            }

            // Hash password
            string hashedPassword = HashString(password);
            // Create random userID
            string userId = GenerateNextUserId();

            // Create new user
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

            // Add user to database
            try
            {
                userService.AddUser(newUser);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.ToString());
                return View("SignUpPage");
            }

            // Redirect to login page after successful registration
            return RedirectToAction("Index", "Login");
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
