﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DRMFSS.BLL;
using DRMFSS.BLL.Services;
using DRMFSS.Web.Models;
using Newtonsoft.Json;
using Telerik.Web.Mvc;

namespace DRMFSS.Web.Controllers
{
    [Authorize]
    public class GiftCertificateController : BaseController
    {
        private readonly IGiftCertificateService _giftCertificateService;
        private readonly ICommodityService _commodityService;
        private readonly IUserProfileService _userProfileService;
        private readonly IReceiptAllocationService _receiptAllocationService;
        private readonly ICommodityTypeService _commodityTypeService;
        private readonly IDetailService _detailService;
        private readonly IDonorService _donorService;
        private readonly IProgramService _programService;
        private readonly IGiftCertificateDetailService _giftCertificateDetailService;

        public GiftCertificateController(
            IGiftCertificateService giftCertificateService,
            ICommodityService commodityService,
           IUserProfileService userProfileService,
            IReceiptAllocationService receiptAllocationService,
            IDetailService detailService,
            ICommodityTypeService commodityTypeService,
           IDonorService donorService,
            IProgramService programService,
            IGiftCertificateDetailService giftCertificateDetailService)
        {
            _giftCertificateService = giftCertificateService;
            _commodityService = commodityService;
            _userProfileService = userProfileService;
            _receiptAllocationService = receiptAllocationService;
            _detailService = detailService;
            _commodityTypeService = commodityTypeService;
            _donorService = donorService;
            _programService = programService;
            _giftCertificateDetailService = giftCertificateDetailService;

        }

        public virtual ActionResult NotUnique(string SINumber, int GiftCertificateID)
        {

            GiftCertificate gift = _giftCertificateService.FindBySINumber(SINumber);
            BLL.UserProfile user = _userProfileService.GetUser(User.Identity.Name);
            bool inReceiptAllocation = _receiptAllocationService.FindBySINumber(SINumber).Any(p => p.CommoditySourceID ==
                BLL.CommoditySource.Constants.LOCALPURCHASE);

            if ((gift == null || (gift.GiftCertificateID == GiftCertificateID)) && !(inReceiptAllocation))// new one or edit no problem 
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(string.Format("{0} is invalid, there is an existing record with the same SI Number ", SINumber),
                        JsonRequestBehavior.AllowGet);

            }
        }

        public ViewResult Index()
        {
            return View(_giftCertificateService.GetAllGiftCertificate().OrderByDescending(o => o.GiftCertificateID));
        }

        public ActionResult Print(int id)
        {
            return RedirectToAction("LetterPreview", "LetterTemplate", new { certificateId = id });
        }

        [GridAction]
        public ActionResult SelectGiftCertificateDetails(int? id)
        {
            if (!id.HasValue)
            {
                return View(new GridModel(new List<GiftCertificateDetailsViewModel>()));
            }
            else
            {
                var gc = _giftCertificateService.FindById(id.Value);
                if (gc != null)
                {
                    var gC = (from g in gc.GiftCertificateDetails
                              where g.GiftCertificateID == id
                              select new Models.GiftCertificateDetailsViewModel()
                                         {
                                             CommodityID = g.CommodityID,
                                             BillOfLoading = g.BillOfLoading,
                                             YearPurchased = g.YearPurchased,
                                             AccountNumber = g.AccountNumber,
                                             WeightInMT = g.WeightInMT,
                                             EstimatedPrice = g.EstimatedPrice,
                                             EstimatedTax = g.EstimatedTax,
                                             DCurrencyID = g.DCurrencyID,
                                             DFundSourceID = g.DFundSourceID,
                                             DBudgetTypeID = g.DBudgetTypeID,
                                             GiftCertificateDetailID = g.GiftCertificateDetailID,
                                             ExpiryDate = g.ExpiryDate,
                                             GiftCertificateID = g.GiftCertificateID,
                                         }).ToList();

                    return View(new GridModel(gC));
                }
                else
                {
                    return View(new GridModel(new List<GiftCertificateDetailsViewModel>()));
                }
            }
        }

        //
        // GET: /GiftCertificate/Details/5

        public ViewResult Details(int id)
        {
            return View(_giftCertificateService.FindById(id));
        }

        //
        // GET: /GiftCertificate/Create

        public ActionResult Create()
        {

            ViewBag.Commodities = _commodityService.GetAllCommodity().OrderBy(o => o.Name);
            ViewBag.CommodityTypes = new SelectList(_commodityTypeService.GetAllCommodityType().OrderBy(o => o.Name), "CommodityTypeID", "Name");
            ViewBag.DCurrencies = _detailService.GetAllDetail().Where(d => d.MasterID == BLL.Master.Constants.CURRENCY).OrderBy(o => o.SortOrder);
            ViewBag.DFundSources = _detailService.GetAllDetail().Where(d => d.MasterID == BLL.Master.Constants.FUND_SOURCE).OrderBy(o => o.SortOrder);
            ViewBag.DBudgetTypes = _detailService.GetAllDetail().Where(d => d.MasterID == BLL.Master.Constants.BUDGET_TYPE).OrderBy(o => o.SortOrder);
            ViewBag.Donors = new SelectList(_donorService.GetAllDonor().OrderBy(o => o.Name), "DonorID", "Name");
            ViewBag.Programs = new SelectList(_programService.GetAllProgram(), "ProgramID", "Name");
            ViewBag.DModeOfTransports = new SelectList(_detailService.GetAllDetail().Where(d => d.MasterID == BLL.Master.Constants.TRANSPORT_MODE).OrderBy(o => o.SortOrder), "DetailID", "Name");

            List<Models.GiftCertificateDetailsViewModel> GiftCertificateDetails = new List<Models.GiftCertificateDetailsViewModel>();
            ViewBag.GiftCertificateDetails = GiftCertificateDetails;

            return View(new GiftCertificateViewModel());
        }

        //
        // POST: /GiftCertificate/Create

        [HttpPost]
        public ActionResult Create(GiftCertificateViewModel giftcertificate)
        {
            if (ModelState.IsValid)
            {
                BLL.GiftCertificate giftCertificateModel = giftcertificate.GenerateGiftCertificate();

                InsertGiftCertificate(giftcertificate, giftCertificateModel);
                //repository.Add( giftCertificate );
                return RedirectToAction("Index");
            }

            ViewBag.Commodities = _commodityService.GetAllCommodity().OrderBy(o => o.Name);
            ViewBag.CommodityTypes = _commodityTypeService.GetAllCommodityType().OrderBy(o => o.Name);

            ViewBag.DCurrencies = _detailService.GetAllDetail().Where(d => d.MasterID == BLL.Master.Constants.CURRENCY).OrderBy(o => o.SortOrder);
            ViewBag.DFundSources = _detailService.GetAllDetail().Where(d => d.MasterID == BLL.Master.Constants.FUND_SOURCE).OrderBy(o => o.SortOrder);
            ViewBag.DBudgetTypes = _detailService.GetAllDetail().Where(d => d.MasterID == BLL.Master.Constants.BUDGET_TYPE).OrderBy(o => o.SortOrder);
            ViewBag.Donors = new SelectList(_donorService.GetAllDonor().OrderBy(o => o.Name), "DonorID", "Name");
            ViewBag.Programs = new SelectList(_programService.GetAllProgram(), "ProgramID", "Name");
            ViewBag.DModeOfTransports = new SelectList(_detailService.GetAllDetail().Where(d => d.MasterID == BLL.Master.Constants.TRANSPORT_MODE).OrderBy(o => o.SortOrder), "DetailID", "Name");

            //return the model with the values pre-populated
            return Create(); //GiftCertificateViewModel.GiftCertificateModel(giftcertificate));
        }

        private void InsertGiftCertificate(GiftCertificateViewModel giftcertificate, GiftCertificate giftCertificateModel)
        {
            List<Models.GiftCertificateDetailsViewModel> giftCertificateDetails = GetSelectedGiftCertificateDetails(giftcertificate.JSONInsertedGiftCertificateDetails);
            var giftDetails = GenerateGiftCertificate(giftCertificateDetails);
            foreach (BLL.GiftCertificateDetail giftDetail in giftDetails)
            {
                giftCertificateModel.GiftCertificateDetails.Add(giftDetail);
            }
            _giftCertificateService.AddGiftCertificate(giftCertificateModel);
        }

        //generate view models from the respective json array of GiftCertificateDetails json elements
        private List<Models.GiftCertificateDetailsViewModel> GetSelectedGiftCertificateDetails(string jsonArray)
        {
            List<Models.GiftCertificateDetailsViewModel> giftCertificateDetails = null;
            if (!string.IsNullOrEmpty(jsonArray))
            {
                // giftCertificateDetails = JsonConvert.DeserializeObject<List<Models.GiftCertificateDetailsViewModel>>(jsonArray);
            }
            return giftCertificateDetails;
        }

        //return the respective bll model's from the list of view models 
        private static List<GiftCertificateDetail> GenerateGiftCertificate(List<Models.GiftCertificateDetailsViewModel> giftCertificateDetails)
        {
            if (giftCertificateDetails != null)
            {
                var gifts = from g in giftCertificateDetails
                            where g != null
                            select new BLL.GiftCertificateDetail()
                            {
                                CommodityID = g.CommodityID,
                                BillOfLoading = g.BillOfLoading,
                                YearPurchased = g.YearPurchased,
                                AccountNumber = g.AccountNumber,
                                WeightInMT = g.WeightInMT,
                                EstimatedPrice = g.EstimatedPrice,
                                EstimatedTax = g.EstimatedTax,
                                DCurrencyID = g.DCurrencyID,
                                DFundSourceID = g.DFundSourceID,
                                DBudgetTypeID = g.DBudgetTypeID,
                                GiftCertificateDetailID = g.GiftCertificateDetailID,
                                GiftCertificateID = g.GiftCertificateID,
                                ExpiryDate = g.ExpiryDate
                            };
                return gifts.ToList();
            }
            else
            {
                return Enumerable.Empty<BLL.GiftCertificateDetail>().ToList();
            }
        }

        public ActionResult Edit(int id)
        {
            GiftCertificate giftcertificate = _giftCertificateService.FindById(id);
            ViewBag.Commodities = _commodityService.GetAllCommodity().OrderBy(o => o.Name);

            ViewBag.DCurrencies = _detailService.GetAllDetail().Where(d => d.MasterID == BLL.Master.Constants.CURRENCY).OrderBy(o => o.SortOrder);
            ViewBag.DFundSources = _detailService.GetAllDetail().Where(d => d.MasterID == BLL.Master.Constants.FUND_SOURCE).OrderBy(o => o.SortOrder);
            ViewBag.DBudgetTypes = _detailService.GetAllDetail().Where(d => d.MasterID == BLL.Master.Constants.BUDGET_TYPE).OrderBy(o => o.SortOrder);

            ViewBag.CommodityTypes = new SelectList(_commodityTypeService.GetAllCommodityType().OrderBy(o => o.Name), "CommodityTypeID", "Name", giftcertificate.GiftCertificateDetails.FirstOrDefault().Commodity.CommodityTypeID);
            ViewBag.Donors = new SelectList(_donorService.GetAllDonor().OrderBy(o => o.Name), "DonorID", "Name", giftcertificate.DonorID);
            ViewBag.Programs = new SelectList(_programService.GetAllProgram(), "ProgramID", "Name");
            ViewBag.DModeOfTransports = new SelectList(_detailService.GetAllDetail().Where(d => d.MasterID == BLL.Master.Constants.TRANSPORT_MODE).OrderBy(o => o.SortOrder), "DetailID", "Name");

            return View(GiftCertificateViewModel.GiftCertificateModel(giftcertificate));
        }

        [HttpPost]
        public ActionResult Edit(GiftCertificateViewModel giftcertificate)
        {
            //just incase the user meses with the the hidden GiftCertificateID field
            GiftCertificate giftcert = _giftCertificateService.FindById(giftcertificate.GiftCertificateID);

            if (ModelState.IsValid && giftcert != null)
            {

                GiftCertificate giftCertificateModel = giftcertificate.GenerateGiftCertificate();

                List<Models.GiftCertificateDetailsViewModel> insertCommodities = GetSelectedGiftCertificateDetails(giftcertificate.JSONInsertedGiftCertificateDetails);
                List<Models.GiftCertificateDetailsViewModel> deletedCommodities = GetSelectedGiftCertificateDetails(giftcertificate.JSONDeletedGiftCertificateDetails);
                List<Models.GiftCertificateDetailsViewModel> updateCommodities = GetSelectedGiftCertificateDetails(giftcertificate.JSONUpdatedGiftCertificateDetails);

                _giftCertificateService.Update(giftCertificateModel, GenerateGiftCertificate(insertCommodities),
                     GenerateGiftCertificate(updateCommodities),
                     GenerateGiftCertificate(deletedCommodities));

                return RedirectToAction("Index");
            }
            ViewBag.Commodities = _commodityService.GetAllCommodity().OrderBy(o => o.Name);


            ViewBag.DCurrencies = _detailService.GetAllDetail().Where(d => d.MasterID == BLL.Master.Constants.CURRENCY).OrderBy(o => o.SortOrder);
            ViewBag.DFundSources = _detailService.GetAllDetail().Where(d => d.MasterID == BLL.Master.Constants.FUND_SOURCE).OrderBy(o => o.SortOrder);
            ViewBag.DBudgetTypes = _detailService.GetAllDetail().Where(d => d.MasterID == BLL.Master.Constants.BUDGET_TYPE).OrderBy(o => o.SortOrder);

            ViewBag.CommodityTypes = new SelectList(_commodityTypeService.GetAllCommodityType().OrderBy(o => o.Name), "CommodityTypeID", "Name", giftcert.GiftCertificateDetails.FirstOrDefault().Commodity.CommodityTypeID);
            ViewBag.Donors = new SelectList(_donorService.GetAllDonor().OrderBy(o => o.Name), "DonorID", "Name", giftcertificate.DonorID);
            ViewBag.Programs = new SelectList(_programService.GetAllProgram(), "ProgramID", "Name", giftcertificate.ProgramID);
            ViewBag.DModeOfTransports = new SelectList(_detailService.GetAllDetail().Where(d => d.MasterID == BLL.Master.Constants.TRANSPORT_MODE).OrderBy(o => o.SortOrder), "DetailID", "Name", giftcertificate.DModeOfTransport);
            return View(giftcertificate);
        }

        public ActionResult Delete(int id)
        {
            GiftCertificate giftcertificate = _giftCertificateService.FindById(id);
            return View(giftcertificate);
        }


        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            _giftCertificateService.DeleteById(id);
            return RedirectToAction("Index");
        }


        public ActionResult MonthlySummary()
        {
            ViewBag.MonthlySummary = _giftCertificateService.GetMonthlySummary().ToList();
            ViewBag.MonthlySummaryETA = _giftCertificateService.GetMonthlySummaryETA().ToList();
            return View();
        }

        public ActionResult ChartView()
        {
            ViewBag.MonthlySummary = _giftCertificateService.GetMonthlySummary().ToList();
            ViewBag.MonthlySummaryETA = _giftCertificateService.GetMonthlySummaryETA().ToList();
            return View();
        }

        public ActionResult IsBillOfLoadingDuplicate(string BillOfLoading)
        {
            return Json(_giftCertificateDetailService.IsBillOfLoadingDuplicate(BillOfLoading), JsonRequestBehavior.AllowGet);
        }
    }
}