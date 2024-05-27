using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectDiamondShop.Models
{
    public class Diamond
    {
        public int diamondID { get; set; }
        public string diamondName { get; set; }
        public decimal diamondPrice { get; set; }
        public string diamondDescription { get; set; }
        public float caratWeight { get; set; }
        public string clarityID { get; set; }
        public string cutID { get; set; }
        public string colorID { get; set; }
        public string shapeID { get; set; }
        public string diamondImagePath { get; set; }
        public bool status { get; set; }

    }
}