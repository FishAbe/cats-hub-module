﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRMFSS.BLL.Repository;
using DRMFSS.BLL.ViewModels.Common;

namespace DRMFSS.BLL.ViewModels.Report
{
    /// <summary>
    /// view model for Receipts view and Wrapping  filtering criteria objects
    /// </summary>
    public class ReceiptsViewModel
    {
        public List<ProgramViewModel> Programs { get; set; }
        public List<CodesViewModel> Cods { get; set; }
        public List<PortViewModel> Ports { get; set; }
        public List<CommoditySourceViewModel> CommoditySources { get; set; }
        public List<CommodityTypeViewModel> CommodityTypes { get; set; }
        public List<PeriodViewModel> Periods { get; set; }
        public List<StoreViewModel> Stores { get; set; }
        



        public int? ProgramId { get; set; }
        public int? CodesId { get; set; }
        public int? PortId { get; set; }
        public int? CommoditySourceId { get; set; }
        public int? CommodityTypeId { get; set; }
        public int? PeriodId { get; set; }
        public int? StoreId { get; set; }
        public int? ProjectCodeId { get; set; }
        public int? ShippingInstructionId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ReceiptsViewModel()
        {
        }

        public ReceiptsViewModel(IUnitOfWork Repository, UserProfile user)
        {
            this.Periods = GetAllPeriod();
            this.CommoditySources = Repository.CommoditySource.GetAllCommoditySourceForReport();
            this.Ports = Repository.Receive.GetALlPorts();
            this.Cods = ConstantsRepository.GetAllCodes();
            this.CommodityTypes = Repository.CommodityType.GetAllCommodityTypeForReprot();
            this.Programs = Repository.Program.GetAllProgramsForReport();
            this.Stores = Repository.Hub.GetAllStoreByUser(user);
        }


        public List<PeriodViewModel> GetAllPeriod()
        {
            List<PeriodViewModel> periodes = new List<PeriodViewModel>();
            //periodes.Add(new PeriodViewModel { PeriodId = 1, PeriodName = "Today" });
            periodes.Add(new PeriodViewModel { PeriodId = 6, PeriodName = "Current Week" });
            //periodes.Add(new PeriodViewModel { PeriodId = 7, PeriodName = "Month to Date" });
            periodes.Add(new PeriodViewModel { PeriodId = 8, PeriodName = "Custome Date Range" });
            return periodes;
        }

    }
}
