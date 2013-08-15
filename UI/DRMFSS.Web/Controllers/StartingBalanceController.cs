﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DRMFSS.BLL;
using DRMFSS.BLL.ViewModels;
using Telerik.Web.Mvc;
using DRMFSS.BLL.Services;

namespace DRMFSS.Web.Controllers
{

   [Authorize]
    public class StartingBalanceController : BaseController
    {
        //
        // GET: /StartingBalance/
       private IUnitOfWork repository;
       private IUserProfileService _userProfileService;
       private ITransactionService _transactionService;
       private ICommodityService _commodityService;
       private IStoreService _storeService;
       private IDonorService _donorSerivce;
       private IProgramService _programService;
       private IUnitService _unitService;

       public StartingBalanceController(IUserProfileService userProfileService,
                                        ITransactionService transactionService,
                                        ICommodityService commoditySerivce,
                                        IStoreService storeService,
                                        IDonorService donorService,
                                        IProgramService programService,
                                        IUnitService unitService)
        {
            _userProfileService = userProfileService;
            _transactionService = transactionService;
            _commodityService = commoditySerivce;
            _storeService = storeService;
            _donorSerivce = donorService;
            _programService = programService;
            _unitService = unitService;
        }


        public ActionResult Index()
        {
            return View(new List<StartingBalanceViewModel>());
        }

        [GridAction]
        public ActionResult GetListOfStartingBalances()
        {
            var userProfile = _userProfileService.GetUser(User.Identity.Name);
            List<StartingBalanceViewModelDto> startBalanceDto = _transactionService.GetListOfStartingBalances(user.DefaultHub.HubID);
            return View(new GridModel(startBalnceDto)); 

           
        }

        //
        // GET: /StartingBalance/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /StartingBalance/Create

        public ActionResult Create()
        {

            List<Commodity> Commodities;
            List<Program> Programs;
            List<Store> Stores;
            List<Unit> Units;
            List<Donor> Donors;

            Commodities = _commodityService.GetAllCommodity().ToList();
            Programs = _programService.GetAllProgram().ToList();
            Stores =_storeService.GetAllByHUbs().Where(h=>h.HubID == user.DefaultHub.HubID).ToList();
            Units = _unitService.GetAllUnit().ToList();
            Donors = _donorSerivce.GetAllDonor().ToList();

            var user = _userProfileService.GetUser(User.Identity.Name);

            StartingBalanceViewModel startingBalanceViewModel = new StartingBalanceViewModel(Commodities,Stores,Units,Programs,Donors, user);
            return PartialView(startingBalanceViewModel);

          
        } 

        //
        // POST: /StartingBalance/Create

        [HttpPost]
        public ActionResult Create(StartingBalanceViewModel startingBalance)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    var user = _userProfileService.GetUser(User.Identity.Name);
                    _transactionService.SaveStartingBalanceTransaction(startingBalance, user);
                    return Json(true, JsonRequestBehavior.AllowGet);
                   
                }
            }
            catch
            {
                return View();
            }
        }
        
        //
        // GET: /StartingBalance/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /StartingBalance/Edit/5

        [HttpPost]
        public ActionResult Edit(StartingBalanceViewModel startingBalance)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /StartingBalance/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /StartingBalance/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, StartingBalanceViewModel startingBalance)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
