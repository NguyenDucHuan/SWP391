﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectDiamondShop.Models
{
    public class Users
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Fullname { get; set; }
        public bool IsActive { get; set; }
    }
}