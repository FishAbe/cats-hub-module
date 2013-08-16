﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRMFSS.BLL.Services
{
    public interface ICommonService:IDisposable
    {
        List<AdminUnit> GetRegions();
        List<int?> GetYears();
        List<Program> GetAllProgram();
        List<Unit> GetAllUnit();
        List<Donor> GetAllDonors();
        List<Commodity> GetAllCommodity();
        List<Commodity> GetAllParents();
        List<int?> GetMonths(int year);
            List<CommodityType> GetAllCommodityType();
    }
}
