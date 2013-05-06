﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using DRMFSS.BLL.ViewModels;

namespace DRMFSS.Web.Controllers.Reports
{
     [Authorize]
    public class TransportationReportController : BaseController
    {
        //
        // GET: /TransportationReport/
         BLL.IUnitOfWork repository = new BLL.UnitOfWork();

        public ActionResult Index()
        {
            PopulateDateFormat();
            return View();
        }

        public ActionResult ReceiveTrend(int Operation, string from, string to)
        {

            DateTime fromDate;
            DateTime toDate;

            bool fromResult = DateTime.TryParse(from, Thread.CurrentThread.CurrentCulture, DateTimeStyles.None, out fromDate);
            bool toResult = DateTime.TryParse(to, Thread.CurrentThread.CurrentCulture, DateTimeStyles.None, out toDate);

            OperationMode mode = (OperationMode)Operation;
            DateTime? f = (fromResult)? fromDate : (DateTime?) null;
            DateTime? t = (toResult) ? toDate : (DateTime?)null;
            var list = repository.Transaction.GetTransportationReports(mode, f, t);
          
            
            return PartialView("PartialGrid", list);
        }

         private void PopulateDateFormat()
         {
             if (Thread.CurrentThread.CurrentCulture.Name == "en-US")
             {
                 ViewBag.DateFormat = "dd-M-yy";
             }
             else if (Thread.CurrentThread.CurrentCulture.Name == "en-GB")
             {
                 ViewBag.DateFormat = "dd-M-yy";
             }
             else
             {
                 ViewBag.DateFormat = "dd-M-yy";
             }
         }
    }
}
