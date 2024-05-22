using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectDiamondShop.Models
{
    public class User
    {
        [Key]
        public string UserID { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 6)]
        public string Password { get; set; }

        public int RoleID { get; set; } = 1;
        public bool Status { get; set; } = true;
        public int? BonusPoint { get; set; }
    }
}