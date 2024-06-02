using System;

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

        // Certificate details
        public string CertificateNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public string CertifyingAuthority { get; set; }
    }
}
