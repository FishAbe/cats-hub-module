﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRMFSS.BLL.Repository;
using DRMFSS.BLL.ViewModels.Common;

namespace DRMFSS.BLL.ViewModels.Report
{
    /// <summary>
    /// view for DeliveryAgainstDispatch view and Wrapping  filtering criteria objects
    /// </summary>
    public class DeliveryAgainstDispatchViewModel
    {
        public List<ProgramViewModel> Programs { get; set; }
        public List<CodesViewModel> Cods { get; set; }
        public List<CommodityTypeViewModel> CommodityTypes { get; set; }
        public List<PeriodViewModel> Periods { get; set; }
        public List<StoreViewModel> Stores { get; set; }
        public List<AreaViewModel> Areas { get; set; }
        public List<TypeViewModel> Types { get; set; }



        public int? ProgramId { get; set; }
        public int? CodesId { get; set; }
        public int? CommodityTypeId { get; set; }
        public int? PeriodId { get; set; }
        public int? StoreId { get; set; }
        public int? AreaId { get; set; }
        public int? TypeId { get; set; }
        public DateTime SelectedDate { get; set; }

        public DeliveryAgainstDispatchViewModel()
        {
        }
        public DeliveryAgainstDispatchViewModel(IUnitOfWork Repository, UserProfile user)
        {
            this.Cods = ConstantsRepository.GetAllCodes();
            this.CommodityTypes = Repository.CommodityType.GetAllCommodityTypeForReprot();
            this.Programs = Repository.Program.GetAllProgramsForReport();
            this.Stores = Repository.Hub.GetAllStoreByUser(user);
            this.Areas = Repository.AdminUnit.GetAllAreasForReport();
            this.Types = ConstantsRepository.GetAllTypes();
        }
    }
}
