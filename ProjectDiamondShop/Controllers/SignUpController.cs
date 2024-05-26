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
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

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

            // Check if user name already exists
            string hashedUserName = HashUserName(userName);
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand checkUserName = new SqlCommand("SELECT COUNT(*) FROM tblUsers WHERE userName = @UserName", conn);
                checkUserName.Parameters.AddWithValue("@UserName", hashedUserName);
                int userNameExists = (int)checkUserName.ExecuteScalar();

                if (userNameExists > 0)
                {
                    ModelState.AddModelError("DuplicateUserName", "User name already exists.");
                    hasErrors = true;
                }
            }

            // Check if email already exists
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand checkUser = new SqlCommand("SELECT COUNT(*) FROM tblUsers WHERE email = @Email", conn);
                checkUser.Parameters.AddWithValue("@Email", email);
                int emailExists = (int)checkUser.ExecuteScalar();

                if (emailExists > 0)
                {
                    ModelState.AddModelError("DuplicateEmail", "Email already exists.");
                    hasErrors = true;
                }
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

                var prioritizedErrors = new[] { "RequiredFields", "PasswordRequired", "PasswordMismatch", "PasswordLowercase", "PasswordUppercase", "PasswordSpecialChar", "DuplicateUserName", "DuplicateEmail" };
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
            string hashedPassword = HashPassword(password);

            // Create random userID
            string userId = GenerateRandomUserId();

            // Create new user
            User newUser = new User
            {
                UserID = userId,
                UserName = hashedUserName,
                FullName = fullName,
                Email = email,
                Password = hashedPassword,
                RoleID = 1,
                Status = true
            };

            // Add user to database
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand insertUser = new SqlCommand("INSERT INTO tblUsers (userID, userName, fullName, email, password, roleID, status, bonusPoint) VALUES (@UserID, @UserName, @FullName, @Email, @Password, @RoleID, @Status, @BonusPoint)", conn);
                    insertUser.Parameters.AddWithValue("@UserID", newUser.UserID);
                    insertUser.Parameters.AddWithValue("@UserName", newUser.UserName);
                    insertUser.Parameters.AddWithValue("@FullName", newUser.FullName);
                    insertUser.Parameters.AddWithValue("@Email", newUser.Email);
                    insertUser.Parameters.AddWithValue("@Password", newUser.Password);
                    insertUser.Parameters.AddWithValue("@RoleID", newUser.RoleID);
                    insertUser.Parameters.AddWithValue("@Status", newUser.Status);
                    insertUser.Parameters.AddWithValue("@BonusPoint", newUser.BonusPoint.HasValue ? (object)newUser.BonusPoint.Value : DBNull.Value);

                    insertUser.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    ModelState.AddModelError("", "User ID or Email already exists.");
                }
                else
                {
                    ModelState.AddModelError("", "An error occurred while creating the account. Please try again.");
                }
                return View("SignUpPage");
            }

            // Redirect to login page after successful registration
            return RedirectToAction("Index", "Login");
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

        private string GenerateRandomUserId()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
