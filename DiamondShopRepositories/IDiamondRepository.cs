﻿using DiamondShopBOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopRepositories
{
    public interface IDiamondRepository
    {
        List<tblDiamond> GetAllDiamond();
        tblDiamond GetDiamondById(int id);
        tblDiamond UpdateDiamond(int id, tblDiamond diamond);
        void AddNewDiamond(tblDiamond newDiamond);
        List<tblDiamond> Filter(string searchTerm, string clarity, string cut, string color, string shape, decimal? minPrice, decimal? maxPrice, float? minCaratWeight, float? maxCaratWeight, string sortBy);
        void AddNewDiamond(tblDiamond newDiamond, tblCertificate newCertificate);

        List<tblCertificate> GetCertificatesByDiamondId(int diamondId);
        void UpdateDiamond(tblDiamond diamond);
        tblDiamond GetDiamondBySearchTerm(string searchTerm);
        tblDiamond GetDiamondByWarrantyCode(string warrantyCode, bool status, string detailStatus);
        tblDiamond GetDiamondByID(int id, bool status, string detailStatus);
    }
}
