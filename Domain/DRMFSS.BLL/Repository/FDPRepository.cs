﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRMFSS.BLL.Interfaces;

namespace DRMFSS.BLL.Repository
{
    /// <summary>
    /// 
    /// </summary>
    public partial class FDPRepository : IFDPRepository 
    {

        /// <summary>
        /// Gets the FDPs by region.
        /// </summary>
        /// <param name="regionId">The region id.</param>
        /// <returns></returns>
        public List<FDP> GetFDPsByRegion(int regionId)
        {
            return db.FDPs.Include("AdminUnit").Where(p => p.AdminUnit.AdminUnit2.ParentID == regionId).OrderBy(o => o.Name).ToList();
        }

        /// <summary>
        /// Gets the FDPs by woreda.
        /// </summary>
        /// <param name="woredaId">The woreda id.</param>
        /// <returns></returns>
        public List<FDP> GetFDPsByWoreda(int woredaId)
        {
            return db.FDPs.Include("AdminUnit").Where(p => p.AdminUnitID == woredaId).OrderBy(o => o.Name).ToList();
        }

        /// <summary>
        /// Gets the FDPs by zone.
        /// </summary>
        /// <param name="zoneId">The zone id.</param>
        /// <returns></returns>
        public List<FDP> GetFDPsByZone(int zoneId)
        {
            return db.FDPs.Include("AdminUnit").Where(p => p.AdminUnit.ParentID == zoneId).OrderBy(o => o.Name).ToList();
        }


        List<ViewModels.Report.AreaViewModel> IFDPRepository.GetAllFDPForReport()
        {
            throw new NotImplementedException();
        }

    }
}
