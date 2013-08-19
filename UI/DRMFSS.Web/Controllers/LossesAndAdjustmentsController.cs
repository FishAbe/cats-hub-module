﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DRMFSS.BLL;
using DRMFSS.BLL.ViewModels;
using Telerik.Web.Mvc;
using DRMFSS.BLL.ViewModels.Common;
using DRMFSS.BLL.Services;

namespace DRMFSS.Web.Controllers
{
   
    [Authorize]
    public class LossesAndAdjustmentsController : BaseController
    {
        
        private readonly IUserProfileService _userProfileService;
        private readonly ICommodityService _commodityService;
        private readonly IStoreService _storeService;
        private readonly IProgramService _programService;
        private readonly IHubService _hubService;
        private readonly IUnitService _unitService;
        private readonly IAdjustmentReasonService _adjustmentReasonService;
        private readonly IAdjustmentService _adjustmentService;
        private readonly ITransactionService _TransactionService;
        private readonly IProjectCodeService _projectCodeService;
        private readonly IShippingInstructionService _shippingInstructionService;

        public LossesAndAdjustmentsController(IUserProfileService userProfileSerice,
                                                ICommodityService commodityService,
                                                IStoreService storeService,
                                                IProgramService programService,
                                                IHubService hubService,
                                                IUnitService unitService,
                                                IAdjustmentReasonService adjustmentReasonService,
                                                IAdjustmentService adjustmentService,
                                                ITransactionService transactionService,
                                                IProjectCodeService projectCodeService,
                                                IShippingInstructionService shippingInstructionService)
        {
            _userProfileService = userProfileSerice;
            _commodityService = commodityService;
            _storeService = storeService;
            _programService = programService;
            _hubService = hubService;
            _unitService = unitService;
            _adjustmentReasonService = adjustmentReasonService;
            _adjustmentService = adjustmentService;
            _TransactionService = transactionService;
            _projectCodeService = projectCodeService;
            _shippingInstructionService = shippingInstructionService;

        }
                
        [Authorize]
        public ActionResult Index()
        {

            return View(_adjustmentService.GetAllLossAndAdjustmentLog(UserProfile.DefaultHub.HubID).OrderByDescending(c => c.Date));
        }

        public ActionResult CreateLoss()
        {
            List<Commodity> commodity;
            List<Store> stores;
            List<AdjustmentReason> adjustmentReasonMinus;
            List<AdjustmentReason> adjustmentReasonPlus;
            List<uint> units;
            List<Program> programs;

            commodity = _commodityService.GetAllParents();
            stores = _hubService.GetAllStoreByUser(user);
            adjustmentReasonMinus = _adjustmentReasonService.GetAllAdjustmentReason().Where(c => c.Direction == "-").ToList();
            adjustmentReasonPlus = _adjustmentReasonService.GetAllAdjustmentReason().Where(c => c.Direction == "+").ToList();
            units = _unitService.GetAllUnit().ToList();
            programs = _programService.GetAllProgramsForReport();


            BLL.UserProfile user = _userProfileService.GetUser(User.Identity.Name);
            LossesAndAdjustmentsViewModel viewModel = new LossesAndAdjustmentsViewModel(commodity, stores, adjustmentReasonMinus, adjustmentReasonPlus,units, programs, user, 1);

            return View(viewModel);
        }

        public ActionResult CreateAdjustment()
        {
            List<Commodity> commodity;
            List<Store> stores;
            List<AdjustmentReason> adjustmentReasonMinus;
            List<AdjustmentReason> adjustmentReasonPlus;
            List<uint> units;
            List<Program> programs;

            commodity = _commodityService.GetAllParents();
            stores = _hubService.GetAllStoreByUser(user);
            adjustmentReasonMinus = _adjustmentReasonService.GetAllAdjustmentReason().Where(c => c.Direction == "-").ToList();
            adjustmentReasonPlus = _adjustmentReasonService.GetAllAdjustmentReason().Where(c => c.Direction == "+").ToList();
            units = _unitService.GetAllUnit().ToList();
            programs = _programService.GetAllProgramsForReport();


            BLL.UserProfile user = _userProfileService.GetUser(User.Identity.Name);
            LossesAndAdjustmentsViewModel viewModel = new LossesAndAdjustmentsViewModel(commodity, stores, adjustmentReasonMinus, adjustmentReasonPlus, units, programs, user, 2);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult CreateLoss(LossesAndAdjustmentsViewModel viewModel)
        {
            List<Commodity> commodity;
            List<Store> stores;
            List<AdjustmentReason> adjustmentReasonMinus;
            List<AdjustmentReason> adjustmentReasonPlus;
            List<uint> units;
            List<Program> programs;

            commodity = _commodityService.GetAllParents();
            stores = _hubService.GetAllStoreByUser(user);
            adjustmentReasonMinus = _adjustmentReasonService.GetAllAdjustmentReason().Where(c => c.Direction == "-").ToList();
            adjustmentReasonPlus = _adjustmentReasonService.GetAllAdjustmentReason().Where(c => c.Direction == "+").ToList();
            units = _unitService.GetAllUnit().ToList();
            programs = _programService.GetAllProgramsForReport();


            BLL.UserProfile user = _userProfileService.GetUser(User.Identity.Name);
            LossesAndAdjustmentsViewModel viewModel = new LossesAndAdjustmentsViewModel(commodity, stores, adjustmentReasonMinus, adjustmentReasonPlus, units, programs, user, 1);

           
            
                           
            if (viewModel.QuantityInMt > _TransactionService.GetCommodityBalanceForStore(viewModel.StoreId, viewModel.CommodityId, viewModel.ShippingInstructionId, viewModel.ProjectCodeId))
            {
                ModelState.AddModelError("QuantityInMT", "You have nothing to loss");
                return View(newViewModel);
            }

            if (viewModel.QuantityInMt <= 0)
            {
                ModelState.AddModelError("QuantityInMT", "You have nothing to loss");

                return View(newViewModel);
            }
            viewModel.IsLoss = true;
            _adjustmentService.AddNewLossAndAdjustment(viewModel, user);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult CreateAdjustment(LossesAndAdjustmentsViewModel viewModel)
        {
            LossesAndAdjustmentsViewModel newViewModel = new LossesAndAdjustmentsViewModel();
            BLL.UserProfile user = _userProfileService.GetUser(User.Identity.Name);

            viewModel.IsLoss = false;
            _adjustmentService.AddNewLossAndAdjustment(viewModel, user);
            return RedirectToAction("Index");
        }


        [HttpPost]
        public JsonResult GetStoreMan(int? storeId)
        {
            string storeMan = String.Empty;
            if (storeId != null)
            {
                storeMan = _storeService.FindById(storeId.Value).StoreManName;
            }
            return Json(storeMan, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Log()
        {

            return View(_adjustmentService.GetAllLossAndAdjustmentLog(UserProfile.DefaultHub.HubID));
        }

        public ActionResult Filter()
        {
            return PartialView();
        }



        public ActionResult GetStacksForToStore(int? ToStoreId)
        {
            return Json(new SelectList(_storeService.GetStacksByStoreId(ToStoreId), JsonRequestBehavior.AllowGet));
        }

        public ActionResult GetProjecCodetForCommodity(int? CommodityId)
        {
            var projectCodes = _projectCodeService.GetProjectCodesForCommodity(UserProfile.DefaultHub.HubID, CommodityId.Value);
            return Json(new SelectList(projectCodes, "ProjectCodeId", "ProjectName"), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSINumberForProjectCode(int? ProjectCodeId)
        {
            if (ProjectCodeId.HasValue)
            {
                BLL.UserProfile user = _userProfileService.GetUser(User.Identity.Name);
                return Json(new SelectList(_shippingInstructionService.GetShippingInstructionsForProjectCode(user.DefaultHub.HubID, ProjectCodeId.Value), "ShippingInstructionId", "ShippingInstructionName"), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new SelectList(new List<ShippingInstructionViewModel>()));
            }
        }

        public ActionResult ViewDetial(string TransactionId)
        {
            var lossAndAdjustment = _adjustmentService.GetAllLossAndAdjustmentLog(UserProfile.DefaultHub.HubID).FirstOrDefault(c => c.TransactionId == Guid.Parse(TransactionId));
            return PartialView(lossAndAdjustment);
        }
        [HttpPost]
        public ActionResult GetFilters()
        {
            var filters = new List<SelectListItem>();
            filters.Add(new SelectListItem { Value = "L", Text = "Loss"});
            filters.Add(new SelectListItem { Value ="A", Text ="Adjustment"});
            return Json(new SelectList(filters, "Value", "Text"));
        }
        [GridAction]
        public ActionResult FilteredGrid(string filterId)
        {
            
            if (filterId != null && filterId != string.Empty)
            {
                var lossAndAdjustmentLogViewModel = _adjustmentService.GetAllLossAndAdjustmentLog(UserProfile.DefaultHub.HubID).Where(c => c.Type == filterId).OrderByDescending(c => c.Date);
                return View(new GridModel(lossAndAdjustmentLogViewModel));
            }
            return View(new GridModel(_adjustmentService.GetAllLossAndAdjustmentLog(UserProfile.DefaultHub.HubID).OrderByDescending(c => c.Date)));
        }

        
        public ActionResult GetStoreForParentCommodity(int? commodityParentId, int? SINumber)
        {
            if (commodityParentId.HasValue && SINumber.HasValue)
            {
                BLL.UserProfile user = _userProfileService.GetUser(User.Identity.Name);
                return Json(new SelectList(ConvertStoreToStoreViewModel(_storeService.GetStoresWithBalanceOfCommodityAndSINumber(commodityParentId.Value, SINumber.Value, user.DefaultHub.HubID)), "StoreId", "StoreName"));
            }
            else
            {
                return Json(new SelectList(new List<StoreViewModel>()));
            }
        }


        public ActionResult SINumberBalance(int? parentCommodityId, int? projectcode, int? SINumber, int? StoreId, int? StackId)
        {
            StoreBalanceViewModel viewModel = new StoreBalanceViewModel();
            BLL.UserProfile user = repository.UserProfile.GetUser(User.Identity.Name);
            if (!StoreId.HasValue && !StackId.HasValue && parentCommodityId.HasValue && projectcode.HasValue && SINumber.HasValue)
            {
                viewModel.ParentCommodityNameB = _commodityService.FindById(parentCommodityId.Value).Name;
                viewModel.ProjectCodeNameB =_projectCodeService.FindById(projectcode.Value).Value;
                viewModel.ShppingInstructionNumberB = _shippingInstructionService.FindById(SINumber.Value).Value;
                viewModel.QtBalance = _TransactionService.GetCommodityBalanceForHub(user.DefaultHub.HubID, parentCommodityId.Value, SINumber.Value, projectcode.Value);
            }
            else if (StoreId.HasValue && !StackId.HasValue && parentCommodityId.HasValue && projectcode.HasValue && SINumber.HasValue)
            {
                viewModel.ParentCommodityNameB = _commodityService.FindById(parentCommodityId.Value).Name;
                viewModel.ProjectCodeNameB = _projectCodeService.FindById(projectcode.Value).Value;
                viewModel.ShppingInstructionNumberB = _shippingInstructionService.FindById(SINumber.Value).Value;
                viewModel.QtBalance = _TransactionService.GetCommodityBalanceForStore(StoreId.Value, parentCommodityId.Value, SINumber.Value, projectcode.Value);
                var store = _storeService.FindById(StoreId.Value);
                viewModel.StoreNameB = string.Format("{0} - {1}", store.Name, store.StoreManName);
            }

            else if (StoreId.HasValue && StackId.HasValue && parentCommodityId.HasValue && projectcode.HasValue && SINumber.HasValue)
            {
                viewModel.ParentCommodityNameB = _commodityService.FindById(parentCommodityId.Value).Name;
                viewModel.ProjectCodeNameB = _projectCodeService.FindById(projectcode.Value).Value;
                viewModel.ShppingInstructionNumberB = _shippingInstructionService.FindById(SINumber.Value).Value;
                viewModel.QtBalance = _TransactionService.GetCommodityBalanceForStack(StoreId.Value, StackId.Value, parentCommodityId.Value, SINumber.Value, projectcode.Value);
                var store = _storeService.FindById(StoreId.Value);
                viewModel.StoreNameB = string.Format("{0} - {1}", store.Name, store.StoreManName);
                viewModel.StackNumberB = StackId.Value.ToString();
            }

            return PartialView(viewModel);
        }

        List<StoreViewModel> ConvertStoreToStoreViewModel(List<Store> Stores)
        {
            List<StoreViewModel> viewModel = new List<StoreViewModel>();
            foreach (var store in Stores)
            {
                viewModel.Add(new StoreViewModel { StoreId = store.StoreID, StoreName = string.Format("{0} - {1} ", store.Name, store.StoreManName) });
            }

            return viewModel;
        }
         
    }
}
