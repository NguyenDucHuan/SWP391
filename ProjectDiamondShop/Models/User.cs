using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectDiamondShop.Models
{
    public class User
    {
        [Key]
        public string UserID { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 6)]
        public string Password { get; set; }

        public int RoleID { get; set; } = 1;
        public bool Status { get; set; } = true;
        public int? BonusPoint { get; set; }
    }
}
