﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using DRMFSS.BLL.ViewModels;
using DRMFSS.BLL.ViewModels.Report;
using DRMFSS.BLL.ViewModels.Report.Data;
using DRMFSS.Web.Models;

namespace DRMFSS.BLL.Services
{
    public class TransactionService : ITransactionService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountService _accountService;
        private readonly IShippingInstructionService _shippingInstructionService;

        public TransactionService(IUnitOfWork unitOfWork,IAccountService accountService)
        {
            this._unitOfWork = unitOfWork;
            this._accountService = accountService;
        }

       


        /// <summary>
        /// Gets the active accounts for ledger.
        /// </summary>
        /// <param name="LedgerID">The ledger ID.</param>
        /// <returns></returns>
        /// 

        public List<Account> GetActiveAccountsForLedger(int LedgerID)
        {
            var accounts =
                _unitOfWork.TransactionRepository.FindBy(t => t.LedgerID == LedgerID).Select(t => t.Account).ToList();

            return accounts;
        }


        /// <summary>
        /// Gets the transactions for ledger.
        /// </summary>
        /// <param name="LedgerID">The ledger ID.</param>
        /// <param name="AccountID">The account ID.</param>
        /// <param name="Commodity">The commodity.</param>
        /// <returns></returns>
        public List<Transaction> GetTransactionsForLedger(int LedgerID, int AccountID, int Commodity)
        {

            var transactions =
                _unitOfWork.TransactionRepository.FindBy(
                    t =>
                    (t.LedgerID == LedgerID && t.AccountID == AccountID) &&
                    (t.CommodityID == Commodity || t.ParentCommodityID == Commodity));

            return transactions;
        }


        /// <summary>
        /// Gets the total receipt allocation.
        /// </summary>
        /// <param name="siNumber">The SI number.</param>
        /// <param name="commodityId"></param>
        /// <param name="hubId">The hub id.</param>
        /// <returns></returns>
        public decimal GetTotalReceiptAllocation(int siNumber, int commodityId, int hubId)
        {
            decimal totalAllocation = 0;
            var commodity = _unitOfWork.CommodityRepository.FindById(commodityId);
            if (commodity.ParentID != null)
            {
                commodityId = commodity.ParentID.Value;
            }
            var allocationSum = _unitOfWork.TransactionRepository.FindBy(
                t => t.ShippingInstructionID == siNumber
                     && t.HubID == hubId
                     && (t.ParentCommodityID == commodityId || t.CommodityID == commodityId)
                     && t.LedgerID == Ledger.Constants.GOODS_PROMISSED_GIFT_CERTIFICATE_COMMITED
                     && t.QuantityInMT > 0
                ).Select(t => t.QuantityInMT).ToList();



            if (allocationSum.Any())
            {
                totalAllocation = allocationSum.Sum();
            }
            return totalAllocation;
        }

        /// <summary>
        /// Gets the total received from receipt allocation.
        /// </summary>
        /// <param name="siNumber">The SI number.</param>
        /// <param name="hubId">The hub id.</param>
        /// <returns></returns>
        public decimal GetTotalReceivedFromReceiptAllocation(int siNumber, int commodityId, int hubId)
        {
            decimal totalAllocation = 0;
            var commodity = _unitOfWork.CommodityRepository.FindById(commodityId);
            if (commodity.ParentID != null)
            {
                commodityId = commodity.ParentID.Value;
            }
            var allocationSum = _unitOfWork.TransactionRepository.FindBy(t => t.ShippingInstructionID == siNumber
                                         && t.HubID == hubId
                                       && t.ParentCommodityID == commodityId
                                       && t.LedgerID == Ledger.Constants.GOODS_ON_HAND_UNCOMMITED
                                       && t.QuantityInMT > 0).Select(t => t.QuantityInMT).ToList();


            if (allocationSum.Any())
            {
                totalAllocation = allocationSum.Sum();
            }
            return totalAllocation;
        }

        /// <summary>
        /// Gets the commodity balance for store.
        /// </summary>
        /// <param name="storeId">The store id.</param>
        /// <param name="parentCommodityId">The parent commodity id.</param>
        /// <param name="si">The SI.</param>
        /// <param name="project">The project.</param>
        /// <returns></returns>
        public decimal GetCommodityBalanceForStore(int storeId, int parentCommodityId, int si, int project)
        {
            var balance = _unitOfWork.TransactionRepository.FindBy(t =>
                                                                   t.StoreID == storeId &&
                                                                   t.ParentCommodityID == parentCommodityId &&
                                                                   t.ShippingInstructionID == si &&
                                                                   t.ProjectCodeID == project &&
                                                                   t.LedgerID ==
                                                                   Ledger.Constants.GOODS_ON_HAND_UNCOMMITED

                ).Select(t => t.QuantityInMT).ToList();

            if (balance.Any())
            {
                return balance.Sum();
            }
            return 0;
        }


        /// <summary>
        /// Gets the commodity balance for stack.
        /// </summary>
        /// <param name="storeId">The store id.</param>
        /// <param name="stack">The stack.</param>
        /// <param name="parentCommodityId">The parent commodity id.</param>
        /// <param name="si">The SI.</param>
        /// <param name="project">The project.</param>
        /// <returns></returns>
        public decimal GetCommodityBalanceForStack(int storeId, int stack, int parentCommodityId, int si, int project)
        {
            var balance = _unitOfWork.TransactionRepository.FindBy(t =>
                                                                   t.StoreID == storeId &&
                                                                   t.ParentCommodityID == parentCommodityId &&
                                                                   t.ShippingInstructionID == si &&
                                                                   t.ProjectCodeID == project && t.Stack == stack &&
                                                                   t.LedgerID ==
                                                                   Ledger.Constants.GOODS_ON_HAND_UNCOMMITED

                ).Select(t => t.QuantityInMT).ToList();
          
            if (balance.Any())
            {
                return balance.Sum();
            }

            return 0;
        }




        /// <summary>
        /// Saves the receipt transaction.
        /// </summary>
        /// <param name="receiveModels">The receive models.</param>
        /// <param name="user">The user.</param>
        public void SaveReceiptTransaction(ReceiveViewModel receiveModels, UserProfile user)
        {
            // Populate more details of the reciept object 
            // Save it when you are done.

            Receive receive = receiveModels.GenerateReceive();
            receive.CreatedDate = DateTime.Now;
            receive.HubID = user.DefaultHub.HubID;
            receive.UserProfileID = user.UserProfileID;
            var commType = _unitOfWork.CommodityTypeRepository.FindById(receiveModels.CommodityTypeID);

            // var comms = GenerateReceiveDetail(commodities);




            foreach (ReceiveDetailViewModel c in receiveModels.ReceiveDetails)
            {
                if (commType.CommodityTypeID == 2)//if it's a non food
                {
                    c.ReceivedQuantityInMT = 0;
                    c.SentQuantityInMT = 0;
                }
                TransactionGroup tgroup = new TransactionGroup();

                var receiveDetail = new ReceiveDetail()
                {
                    CommodityID = c.CommodityID,
                    Description = c.Description,
                    SentQuantityInMT = c.SentQuantityInMT.Value,
                    SentQuantityInUnit = c.SentQuantityInUnit.Value,
                    UnitID = c.UnitID,
                    ReceiveID = receive.ReceiveID
                };
                if (c.ReceiveDetailID.HasValue)
                {
                    receiveDetail.ReceiveDetailID = c.ReceiveDetailID.Value;
                }

                // receiveDetail.TransactionGroupID = tgroup.TransactionGroupID;
                receiveDetail.TransactionGroup = tgroup;
                receive.ReceiveDetails.Add(receiveDetail);

                Transaction transaction = new Transaction();
                transaction.TransactionDate = DateTime.Now;
                transaction.ParentCommodityID = _unitOfWork.CommodityRepository.FindById(c.CommodityID).ParentID ?? c.CommodityID;
                transaction.CommodityID = c.CommodityID;
                transaction.LedgerID = Ledger.Constants.GOODS_ON_HAND_UNCOMMITED;
                transaction.HubOwnerID = user.DefaultHub.HubOwnerID;

                transaction.AccountID = _accountService.GetAccountIdWithCreate(Account.Constants.HUB, receive.HubID);
                transaction.ShippingInstructionID =_shippingInstructionService.GetSINumberIdWithCreate(receiveModels.SINumber).ShippingInstructionID;
                //TODO:After Implementing ProjectCodeService Please Return Here
                //transaction.ProjectCodeID = repository.ProjectCode.GetProjectCodeIdWIthCreate(receiveModels.ProjectNumber).ProjectCodeID;
                transaction.HubID = user.DefaultHub.HubID;
                transaction.UnitID = c.UnitID;
                if (c.ReceivedQuantityInMT != null) transaction.QuantityInMT = c.ReceivedQuantityInMT.Value;
                if (c.ReceivedQuantityInUnit != null) transaction.QuantityInUnit = c.ReceivedQuantityInUnit.Value;
                if (c.CommodityGradeID != null) transaction.CommodityGradeID = c.CommodityGradeID.Value;

                transaction.ProgramID = receiveModels.ProgramID;
                transaction.StoreID = receiveModels.StoreID;
                transaction.Stack = receiveModels.StackNumber;
                transaction.TransactionGroupID = tgroup.TransactionGroupID;
                tgroup.Transactions.Add(transaction);

                // do the second half of the transaction here.

                var transaction2 = new Transaction();
                transaction2.TransactionDate = DateTime.Now;
                //TAKEs the PARENT FROM THE FIRST TRANSACTION
                transaction2.ParentCommodityID = transaction.ParentCommodityID;
                transaction2.CommodityID = c.CommodityID;
                transaction2.HubOwnerID = user.DefaultHub.HubOwnerID;
                //Decide from where the -ve side of the transaction comes from
                //it is either from the allocated stock
                // or it is from goods under care.

                // this means that this receipt is done without having gone through the gift certificate process.



                if (receiveModels.CommoditySourceID == BLL.CommoditySource.Constants.DONATION || receiveModels.CommoditySourceID == BLL.CommoditySource.Constants.LOCALPURCHASE)
                {
                    transaction2.LedgerID = Ledger.Constants.GOODS_UNDER_CARE;
                    transaction2.AccountID = _accountService.GetAccountIdWithCreate(Account.Constants.DONOR, receive.ResponsibleDonorID.Value);
                }
                else if (receiveModels.CommoditySourceID == BLL.CommoditySource.Constants.REPAYMENT)
                {
                    transaction2.LedgerID = Ledger.Constants.GOODS_RECIEVABLE;
                    transaction2.AccountID = _accountService.GetAccountIdWithCreate(Account.Constants.HUB, receiveModels.SourceHubID.Value);
                }
                else
                {
                    transaction2.LedgerID = Ledger.Constants.LIABILITIES;
                    transaction2.AccountID = _accountService.GetAccountIdWithCreate(Account.Constants.HUB, receiveModels.SourceHubID.Value);
                }

                transaction2.ShippingInstructionID = _shippingInstructionService.GetSINumberIdWithCreate(receiveModels.SINumber).ShippingInstructionID;
                //TODO:After Implementing ProjectCodeService Please return here
                //transaction2.ProjectCodeID = repository.ProjectCode.GetProjectCodeIdWIthCreate(receiveModels.ProjectNumber).ProjectCodeID;
                transaction2.HubID = user.DefaultHub.HubID;
                transaction2.UnitID = c.UnitID;
                // this is the credit part, so make it Negative
                if (c.ReceivedQuantityInMT != null) transaction2.QuantityInMT = -c.ReceivedQuantityInMT.Value;
                if (c.ReceivedQuantityInUnit != null) transaction2.QuantityInUnit = -c.ReceivedQuantityInUnit.Value;
                if (c.CommodityGradeID != null) transaction2.CommodityGradeID = c.CommodityGradeID.Value;

                transaction2.ProgramID = receiveModels.ProgramID;
                transaction2.StoreID = receiveModels.StoreID;
                transaction2.Stack = receiveModels.StackNumber;
                transaction2.TransactionGroupID = tgroup.TransactionGroupID;
                // hack to get past same key object in context error
                //repository.Transaction = new TransactionRepository();
                tgroup.Transactions.Add(transaction2);

            }
            // Try to save this transaction
            db.Database.Connection.Open();
            DbTransaction dbTransaction = db.Database.Connection.BeginTransaction();
            try
            {
                repository.Receive.Add(receive);
                dbTransaction.Commit();
            }
            catch (Exception exp)
            {
                dbTransaction.Rollback();
                //TODO: Save the exception somewhere
                throw new Exception("The Receipt Transaction Cannot be saved. <br />Detail Message :" + exp.StackTrace);

            }

        }


        /// <summary>
        /// Gets the transportation reports.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">To date.</param>
        /// <returns></returns>
        public List<TransporationReport> GetTransportationReports(OperationMode mode, DateTime? fromDate, DateTime? toDate)
        {
            int ledgerId = (mode == OperationMode.Dispatch) ? Ledger.Constants.GOODS_DISPATCHED : Ledger.Constants.GOODS_ON_HAND_UNCOMMITED;
            var list = from item in db.Transactions
                       where (item.LedgerID == ledgerId && (item.QuantityInMT > 0 || item.QuantityInUnit > 0))
                              &&
                              (item.TransactionGroup.DispatchDetails.Any() || item.TransactionGroup.ReceiveDetails.Any())
                              &&
                              (!item.TransactionGroup.InternalMovements.Any() || !item.TransactionGroup.Adjustments.Any())
                       select item;

            if (fromDate.HasValue)
            {
                list = list.Where(p => p.TransactionDate >= fromDate.Value);
            }
            if (toDate.HasValue)
            {
                list = list.Where(p => p.TransactionDate <= toDate.Value);
            }

            return (from t in list
                    group t by new { t.Commodity, t.Program } into tgroup
                    select new TransporationReport()
                    {
                        Commodity = tgroup.Key.Commodity.Name,
                        Program = tgroup.Key.Program.Name,
                        NoOfTrucks = tgroup.Count(),
                        QuantityInMT = tgroup.Sum(p => p.QuantityInMT),
                        QuantityInUnit = tgroup.Sum(p => p.QuantityInUnit)
                    }).ToList();

        }

        /// <summary>
        /// Gets the grouped transportation reports.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">To date.</param>
        /// <returns></returns>
        public List<GroupedTransportation> GetGroupedTransportationReports(OperationMode mode, DateTime? fromDate, DateTime? toDate)
        {
            var list = (from tr in GetTransportationReports(mode, fromDate, toDate)
                        group tr by tr.Program into trg
                        select new GroupedTransportation()
                        {
                            Program = trg.Key,
                            Transportations = trg.ToList()
                        });
            return list.ToList(); ;
        }


        /// <summary>
        /// Gets the available allocations.
        /// </summary>
        /// <param name="hubId">The hub id.</param>
        /// <returns></returns>
        public List<ReceiptAllocation> GetAvailableAllocations(int hubId)
        {

            var avaliableRAll = (from rAll in db.ReceiptAllocations
                                 where rAll.QuantityInMT >= (from v in db.Transactions
                                                             where v.ShippingInstruction.Value == rAll.SINumber
                                                                   && v.HubID == hubId
                                                                   && v.LedgerID == Ledger.Constants.GOODS_ON_HAND_UNCOMMITED
                                                                   && v.QuantityInMT > 0
                                                             select v.QuantityInMT).Sum()
                                 select rAll);

            return avaliableRAll.ToList();
        }


        /// <summary>
        /// Saves the dispatch transaction.
        /// </summary>
        /// <param name="dispatchModel">The dispatch model.</param>
        /// <param name="user">The user.</param>
        public void SaveDispatchTransaction(DispatchModel dispatchModel, UserProfile user)
        {
            Dispatch dispatch = dispatchModel.GenerateDipatch();

            dispatch.HubID = user.DefaultHub.HubID;
            dispatch.UserProfileID = user.UserProfileID;
            dispatch.DispatchAllocationID = dispatchModel.DispatchAllocationID;
            dispatch.OtherDispatchAllocationID = dispatchModel.OtherDispatchAllocationID;
            CommodityType commType = repository.CommodityType.FindById(dispatchModel.CommodityTypeID);

            foreach (DispatchDetailModel detail in dispatchModel.DispatchDetails)
            {

                if (commType.CommodityTypeID == 2)//if it's a non food
                {
                    detail.DispatchedQuantityMT = 0;
                    detail.RequestedQuantityMT = 0;
                }

                TransactionGroup group = new TransactionGroup();

                if (dispatchModel.Type == 1)
                {
                    Transaction transaction2 = GetPositiveFDPTransaction(dispatchModel, dispatch, detail);
                    group.Transactions.Add(transaction2);

                    Transaction transaction = GetNegativeFDPTransaction(dispatchModel, dispatch, detail);
                    group.Transactions.Add(transaction);
                }
                else
                {
                    Transaction transaction2 = GetPositiveHUBTransaction(dispatchModel, dispatch, detail);
                    group.Transactions.Add(transaction2);

                    Transaction transaction = GetNegativeHUBTransaction(dispatchModel, dispatch, detail);
                    group.Transactions.Add(transaction);
                }

                DispatchDetail dispatchDetail = GenerateDispatchDetail(detail);
                dispatchDetail.TransactionGroup = group;


                dispatch.DispatchDetails.Add(dispatchDetail);

            }
            // Try to save this transaction
            db.Database.Connection.Open();
            DbTransaction dbTransaction = db.Database.Connection.BeginTransaction();
            try
            {
                repository.Dispatch.Add(dispatch);
                dbTransaction.Commit();
            }
            catch (Exception exp)
            {
                dbTransaction.Rollback();
                //TODO: Save the detail of this exception somewhere
                throw new Exception("The Dispatch Transaction Cannot be saved. <br />Detail Message :" + exp.Message);
            }

            if (dispatch.Type == 1)
            {
                string sms = dispatch.GetSMSText();
                SMS.SendSMS(dispatch.FDPID.Value, sms);
            }
        }

        #region dispatch transaction helpers
        //TODO: this section has to be cleaned
        /// <summary>
        /// Gets the positive FDP transaction.
        /// </summary>
        /// <param name="dispatchModel">The dispatch model.</param>
        /// <param name="dispatch">The dispatch.</param>
        /// <param name="detail">The detail.</param>
        /// <returns></returns>
        private Transaction GetPositiveFDPTransaction(DispatchModel dispatchModel, Dispatch dispatch, DispatchDetailModel detail)
        {
            Transaction transaction2 = new Transaction();
            transaction2.AccountID = repository.Account.GetAccountIDWithCreate(Account.Constants.FDP, dispatchModel.FDPID.Value);
            transaction2.ProgramID = dispatchModel.ProgramID;
            transaction2.ParentCommodityID = detail.CommodityID;
            transaction2.CommodityID = detail.CommodityID;
            transaction2.HubID = dispatch.HubID;
            transaction2.HubOwnerID = repository.Hub.FindById(dispatch.HubID).HubOwnerID;
            transaction2.LedgerID = Ledger.Constants.GOODS_DISPATCHED;
            transaction2.QuantityInMT = +detail.DispatchedQuantityMT.Value;
            transaction2.QuantityInUnit = +detail.DispatchedQuantity.Value;
            transaction2.ShippingInstructionID = repository.ShippingInstruction.GetShipingInstructionId(dispatchModel.SINumber);
            transaction2.ProjectCodeID = repository.ProjectCode.GetProjectCodeId(dispatchModel.ProjectNumber);
            transaction2.Stack = dispatchModel.StackNumber;
            transaction2.StoreID = dispatchModel.StoreID;
            transaction2.TransactionDate = DateTime.Now;
            transaction2.UnitID = detail.Unit;
            return transaction2;
        }

        /// <summary>
        /// Gets the negative FDP transaction.
        /// </summary>
        /// <param name="dispatchModel">The dispatch model.</param>
        /// <param name="dispatch">The dispatch.</param>
        /// <param name="detail">The detail.</param>
        /// <returns></returns>
        private Transaction GetNegativeFDPTransaction(DispatchModel dispatchModel, Dispatch dispatch, DispatchDetailModel detail)
        {
            Transaction transaction = new Transaction();
            transaction.AccountID = repository.Account.GetAccountIDWithCreate(Account.Constants.FDP, dispatch.FDPID.Value);
            transaction.ProgramID = dispatchModel.ProgramID;
            transaction.ParentCommodityID = detail.CommodityID;
            transaction.CommodityID = detail.CommodityID;
            transaction.HubID = dispatch.HubID;
            transaction.HubOwnerID = repository.Hub.FindById(dispatch.HubID).HubOwnerID;
            transaction.LedgerID = Ledger.Constants.GOODS_ON_HAND_UNCOMMITED;
            transaction.QuantityInMT = -detail.DispatchedQuantityMT.Value;
            transaction.QuantityInUnit = -detail.DispatchedQuantity.Value;
            transaction.ShippingInstructionID = repository.ShippingInstruction.GetShipingInstructionId(dispatchModel.SINumber);
            transaction.ProjectCodeID = repository.ProjectCode.GetProjectCodeId(dispatchModel.ProjectNumber);
            transaction.Stack = dispatchModel.StackNumber;
            transaction.StoreID = dispatchModel.StoreID;
            transaction.TransactionDate = DateTime.Now;
            transaction.UnitID = detail.Unit;
            return transaction;
        }

        /// <summary>
        /// Gets the positive HUB transaction.
        /// </summary>
        /// <param name="dispatchModel">The dispatch model.</param>
        /// <param name="dispatch">The dispatch.</param>
        /// <param name="detail">The detail.</param>
        /// <returns></returns>
        private Transaction GetPositiveHUBTransaction(DispatchModel dispatchModel, Dispatch dispatch, DispatchDetailModel detail)
        {
            Transaction transaction2 = new Transaction();
            transaction2.AccountID = repository.Account.GetAccountIDWithCreate(Account.Constants.HUB, dispatchModel.ToHubID.Value);
            transaction2.ProgramID = dispatchModel.ProgramID;
            transaction2.ParentCommodityID = detail.CommodityID;
            transaction2.CommodityID = detail.CommodityID;
            transaction2.HubID = dispatch.HubID;
            transaction2.HubOwnerID = repository.Hub.FindById(dispatch.HubID).HubOwnerID;
            transaction2.LedgerID = Ledger.Constants.GOODS_DISPATCHED;
            transaction2.QuantityInMT = +detail.DispatchedQuantityMT.Value;
            transaction2.QuantityInUnit = +detail.DispatchedQuantity.Value;
            transaction2.ShippingInstructionID = repository.ShippingInstruction.GetShipingInstructionId(dispatchModel.SINumber);
            transaction2.ProjectCodeID = repository.ProjectCode.GetProjectCodeId(dispatchModel.ProjectNumber);
            transaction2.Stack = dispatchModel.StackNumber;
            transaction2.StoreID = dispatchModel.StoreID;
            transaction2.TransactionDate = DateTime.Now;
            transaction2.UnitID = detail.Unit;
            return transaction2;
        }

        /// <summary>
        /// Gets the negative HUB Transaction.
        /// </summary>
        /// <param name="dispatchModel">The dispatch model.</param>
        /// <param name="dispatch">The dispatch.</param>
        /// <param name="detail">The detail.</param>
        /// <returns></returns>
        private Transaction GetNegativeHUBTransaction(DispatchModel dispatchModel, Dispatch dispatch, DispatchDetailModel detail)
        {
            Transaction transaction = new Transaction();
            transaction.AccountID = repository.Account.GetAccountIDWithCreate(Account.Constants.HUB, dispatch.HubID);
            transaction.ProgramID = dispatchModel.ProgramID;
            transaction.ParentCommodityID = detail.CommodityID;
            transaction.CommodityID = detail.CommodityID;
            transaction.HubID = dispatch.HubID;
            transaction.HubOwnerID = repository.Hub.FindById(dispatch.HubID).HubOwnerID;
            transaction.LedgerID = Ledger.Constants.GOODS_ON_HAND_UNCOMMITED;
            transaction.QuantityInMT = -detail.DispatchedQuantityMT.Value;
            transaction.QuantityInUnit = -detail.DispatchedQuantity.Value;
            transaction.ShippingInstructionID = repository.ShippingInstruction.GetShipingInstructionId(dispatchModel.SINumber);
            transaction.ProjectCodeID = repository.ProjectCode.GetProjectCodeId(dispatchModel.ProjectNumber);
            transaction.Stack = dispatchModel.StackNumber;
            transaction.StoreID = dispatchModel.StoreID;
            transaction.TransactionDate = DateTime.Now;
            transaction.UnitID = detail.Unit;
            return transaction;
        }

        /// <summary>
        /// Generates the dispatch detail.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns></returns>
        private DispatchDetail GenerateDispatchDetail(DispatchDetailModel c)
        {
            if (c != null)
            {
                DispatchDetail dispatchDetail = new BLL.DispatchDetail()
                {
                    CommodityID = c.CommodityID,
                    Description = c.Description,
                    // DispatchDetailID = c.Id,
                    RequestedQuantityInMT = c.RequestedQuantityMT.Value,
                    //DispatchedQuantityInMT = c.DispatchedQuantityMT,
                    //DispatchedQuantityInUnit = c.DispatchedQuantity,
                    RequestedQunatityInUnit = c.RequestedQuantity.Value,
                    UnitID = c.Unit
                };
                if (c.Id.HasValue)
                {
                    dispatchDetail.DispatchDetailID = c.Id.Value;
                }

                return dispatchDetail;
            }
            else
            {
                return null;
            }
        }

        #endregion


        /// <summary>
        /// Finds the by transaction group ID.
        /// </summary>
        /// <param name="partition">The partition.</param>
        /// <param name="transactionGroupID">The transaction group ID.</param>
        /// <returns></returns>
        public Transaction FindByTransactionGroupID(Guid transactionGroupID)
        {
            return (from tr in db.Transactions
                    where tr.TransactionGroupID == transactionGroupID
                    select tr).FirstOrDefault();
        }



        /// <summary>
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="user"></param>
        /// <exception cref="System.Exception"></exception>
        public void SaveInternalMovementTrasnsaction(InternalMovementViewModel viewModel, UserProfile user)
        {
            InternalMovement internalMovement = new InternalMovement();
            TransactionGroup transactionGroup = new TransactionGroup();
            Transaction transactionFromStore = new Transaction();

            Commodity commodity = repository.Commodity.FindById(viewModel.CommodityId);


            //transaction.TransactionGroupID = transactionGroupId;
            transactionFromStore.LedgerID = 2;
            transactionFromStore.HubOwnerID = user.DefaultHub.HubOwner.HubOwnerID;
            //trasaction.AccountID
            transactionFromStore.AccountID = repository.Account.GetAccountIDWithCreate(Account.Constants.HUB, user.DefaultHub.HubID); // 
            transactionFromStore.HubID = user.DefaultHub.HubID;
            transactionFromStore.StoreID = viewModel.FromStoreId;  //
            transactionFromStore.Stack = viewModel.FromStackId; //
            transactionFromStore.ProjectCodeID = viewModel.ProjectCodeId;
            transactionFromStore.ShippingInstructionID = viewModel.ShippingInstructionId;
            transactionFromStore.ProgramID = viewModel.ProgramId;
            transactionFromStore.ParentCommodityID = (commodity.ParentID == null)
                                                       ? commodity.CommodityID
                                                       : commodity.ParentID.Value;
            transactionFromStore.CommodityID = viewModel.CommodityId;
            transactionFromStore.CommodityGradeID = null; // How did I get this value ? 
            transactionFromStore.QuantityInMT = 0 - viewModel.QuantityInMt;
            transactionFromStore.QuantityInUnit = 0 - viewModel.QuantityInUnit;
            transactionFromStore.UnitID = viewModel.UnitId;
            transactionFromStore.TransactionDate = DateTime.Now;



            Transaction transactionToStore = new Transaction();

            //transactionToStore.TransactionGroupID = transactionGroupId;
            transactionToStore.LedgerID = 2;
            transactionToStore.HubOwnerID = user.DefaultHub.HubOwner.HubOwnerID;
            //transactionToStore.AccountID
            transactionToStore.AccountID = repository.Account.GetAccountIDWithCreate(Account.Constants.HUB, user.DefaultHub.HubID); // 
            transactionToStore.HubID = user.DefaultHub.HubID;
            transactionToStore.StoreID = viewModel.ToStoreId;  //
            transactionToStore.Stack = viewModel.ToStackId; //
            transactionToStore.ProjectCodeID = viewModel.ProjectCodeId;
            transactionToStore.ShippingInstructionID = viewModel.ShippingInstructionId;
            transactionToStore.ProgramID = viewModel.ProgramId;

            transactionToStore.ParentCommodityID = (commodity.ParentID == null)
                                                       ? commodity.CommodityID
                                                       : commodity.ParentID.Value;
            transactionToStore.CommodityID = viewModel.CommodityId;
            transactionToStore.CommodityGradeID = null; // How did I get this value ? 
            transactionToStore.QuantityInMT = viewModel.QuantityInMt;
            transactionToStore.QuantityInUnit = viewModel.QuantityInUnit;
            transactionToStore.UnitID = viewModel.UnitId;
            transactionToStore.TransactionDate = DateTime.Now;

            transactionGroup.Transactions.Add(transactionFromStore);
            transactionGroup.Transactions.Add(transactionToStore);
            transactionGroup.PartitionID = 0;

            internalMovement.PartitionID = 0;
            internalMovement.TransactionGroup = transactionGroup;
            internalMovement.TransferDate = viewModel.SelectedDate;
            internalMovement.DReason = viewModel.ReasonId;
            internalMovement.Notes = viewModel.Note;
            internalMovement.ApprovedBy = viewModel.ApprovedBy;
            internalMovement.ReferenceNumber = viewModel.ReferenceNumber;



            // Try to save this transaction
            db.Database.Connection.Open();
            DbTransaction dbTransaction = db.Database.Connection.BeginTransaction();
            try
            {
                repository.InternalMovement.Add(internalMovement);
                dbTransaction.Commit();
            }
            catch (Exception exp)
            {
                dbTransaction.Rollback();
                //TODO: Save the detail of this exception somewhere
                throw new Exception("The Internal Movement Transaction Cannot be saved. <br />Detail Message :" + exp.Message);
            }

        }

        /// <summary>
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="user"></param>
        /// <exception cref="System.Exception"></exception>
        public void SaveLossTrasnsaction(LossesAndAdjustmentsViewModel viewModel, UserProfile user)
        {
            Commodity commodity = repository.Commodity.FindById(viewModel.CommodityId);



            Adjustment lossAndAdjustment = new Adjustment();
            TransactionGroup transactionGroup = new TransactionGroup();
            Transaction transactionOne = new Transaction();


            //transaction.TransactionGroupID = transactionGroupId;
            transactionOne.LedgerID = 2;
            transactionOne.HubOwnerID = user.DefaultHub.HubOwner.HubOwnerID;
            transactionOne.AccountID = repository.Account.GetAccountIDWithCreate(Account.Constants.HUB, user.DefaultHub.HubID); // 
            transactionOne.HubID = user.DefaultHub.HubID;
            transactionOne.StoreID = viewModel.StoreId;  //
            transactionOne.ProjectCodeID = viewModel.ProjectCodeId;
            transactionOne.ShippingInstructionID = viewModel.ShippingInstructionId;

            transactionOne.ParentCommodityID = (commodity.ParentID == null)
                                                       ? commodity.CommodityID
                                                       : commodity.ParentID.Value;
            transactionOne.CommodityID = viewModel.CommodityId;
            transactionOne.ProgramID = viewModel.ProgramId;
            transactionOne.CommodityGradeID = null; // How did I get this value ? 
            transactionOne.QuantityInMT = 0 - viewModel.QuantityInMt;
            transactionOne.QuantityInUnit = 0 - viewModel.QuantityInUint;
            transactionOne.UnitID = viewModel.UnitId;
            transactionOne.TransactionDate = DateTime.Now;



            Transaction transactionTwo = new Transaction();

            //transactionToStore.TransactionGroupID = transactionGroupId;
            transactionTwo.LedgerID = 14;
            transactionTwo.HubOwnerID = user.DefaultHub.HubOwnerID;
            transactionTwo.AccountID = repository.Account.GetAccountIDWithCreate(Account.Constants.HUB, user.DefaultHub.HubID); // 
            transactionTwo.HubID = user.DefaultHub.HubID;
            transactionTwo.StoreID = viewModel.StoreId;  //
            transactionTwo.ProjectCodeID = viewModel.ProjectCodeId;
            transactionTwo.ShippingInstructionID = viewModel.ShippingInstructionId;
            transactionTwo.ParentCommodityID = (commodity.ParentID == null)
                                                       ? commodity.CommodityID
                                                       : commodity.ParentID.Value;
            transactionTwo.CommodityID = viewModel.CommodityId;
            transactionTwo.ProgramID = viewModel.ProgramId;
            transactionTwo.CommodityGradeID = null; // How did I get this value ? 
            transactionTwo.QuantityInMT = viewModel.QuantityInMt;
            transactionTwo.QuantityInUnit = viewModel.QuantityInUint;
            transactionTwo.UnitID = viewModel.UnitId;
            transactionTwo.TransactionDate = DateTime.Now;

            transactionGroup.Transactions.Add(transactionOne);
            transactionGroup.Transactions.Add(transactionTwo);


            lossAndAdjustment.PartitionID = 0;
            lossAndAdjustment.TransactionGroup = transactionGroup;
            lossAndAdjustment.HubID = user.DefaultHub.HubID;
            lossAndAdjustment.AdjustmentReasonID = viewModel.ReasonId;
            lossAndAdjustment.AdjustmentDirection = "L";
            lossAndAdjustment.AdjustmentDate = viewModel.SelectedDate;
            lossAndAdjustment.ApprovedBy = viewModel.ApprovedBy;
            lossAndAdjustment.Remarks = viewModel.Description;
            lossAndAdjustment.UserProfileID = user.UserProfileID;
            lossAndAdjustment.ReferenceNumber = viewModel.MemoNumber;
            lossAndAdjustment.StoreManName = viewModel.StoreMan;



            // Try to save this transaction
            db.Database.Connection.Open();
            DbTransaction dbTransaction = db.Database.Connection.BeginTransaction();
            try
            {
                repository.Adjustment.Add(lossAndAdjustment);
                dbTransaction.Commit();
            }
            catch (Exception exp)
            {
                dbTransaction.Rollback();
                //TODO: Save the detail of this exception somewhere
                throw new Exception("The Internal Movement Transaction Cannot be saved. <br />Detail Message :" + exp.Message);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="user"></param>
        public void SaveAdjustmentTrasnsaction(LossesAndAdjustmentsViewModel viewModel, UserProfile user)
        {


            Adjustment lossAndAdjustment = new Adjustment();
            TransactionGroup transactionGroup = new TransactionGroup();
            Transaction transactionOne = new Transaction();

            Commodity commodity = repository.Commodity.FindById(viewModel.CommodityId);

            transactionOne.LedgerID = 14;
            transactionOne.HubOwnerID = user.DefaultHub.HubOwner.HubOwnerID;
            transactionOne.AccountID = repository.Account.GetAccountIDWithCreate(Account.Constants.HUB, user.DefaultHub.HubID); // 
            transactionOne.HubID = user.DefaultHub.HubID;
            transactionOne.StoreID = viewModel.StoreId;  //
            transactionOne.ProjectCodeID = viewModel.ProjectCodeId;
            transactionOne.ShippingInstructionID = viewModel.ShippingInstructionId;
            transactionOne.ParentCommodityID = (commodity.ParentID == null)
                                                       ? commodity.CommodityID
                                                       : commodity.ParentID.Value;
            transactionOne.CommodityID = viewModel.CommodityId;
            transactionOne.ProgramID = viewModel.ProgramId;
            transactionOne.CommodityGradeID = null; // How did I get this value ? 
            transactionOne.QuantityInMT = 0 - viewModel.QuantityInMt;
            transactionOne.QuantityInUnit = 0 - viewModel.QuantityInUint;
            transactionOne.UnitID = viewModel.UnitId;
            transactionOne.TransactionDate = DateTime.Now;



            Transaction transactionTwo = new Transaction();

            transactionTwo.LedgerID = 2;
            transactionTwo.HubOwnerID = user.DefaultHub.HubOwnerID;
            transactionTwo.AccountID = repository.Account.GetAccountIDWithCreate(Account.Constants.HUB, user.DefaultHub.HubID); // 
            transactionTwo.HubID = user.DefaultHub.HubID;
            transactionTwo.StoreID = viewModel.StoreId;  //
            transactionTwo.ProjectCodeID = viewModel.ProjectCodeId;
            transactionTwo.ShippingInstructionID = viewModel.ShippingInstructionId;
            transactionTwo.ParentCommodityID = (commodity.ParentID == null)
                                                       ? commodity.CommodityID
                                                       : commodity.ParentID.Value;
            transactionTwo.CommodityID = viewModel.CommodityId;
            transactionTwo.ProgramID = viewModel.ProgramId;
            transactionTwo.CommodityGradeID = null; // How did I get this value ? 
            transactionTwo.QuantityInMT = viewModel.QuantityInMt;
            transactionTwo.QuantityInUnit = viewModel.QuantityInUint;
            transactionTwo.UnitID = viewModel.UnitId;
            transactionTwo.TransactionDate = DateTime.Now;


            transactionGroup.Transactions.Add(transactionOne);
            transactionGroup.Transactions.Add(transactionTwo);

            lossAndAdjustment.PartitionID = 0;
            lossAndAdjustment.TransactionGroup = transactionGroup;
            lossAndAdjustment.HubID = user.DefaultHub.HubID;
            lossAndAdjustment.AdjustmentReasonID = viewModel.ReasonId;
            lossAndAdjustment.AdjustmentDirection = "A";
            lossAndAdjustment.AdjustmentDate = viewModel.SelectedDate;
            lossAndAdjustment.ApprovedBy = viewModel.ApprovedBy;
            lossAndAdjustment.Remarks = viewModel.Description;
            lossAndAdjustment.UserProfileID = user.UserProfileID;
            lossAndAdjustment.ReferenceNumber = viewModel.MemoNumber;
            lossAndAdjustment.StoreManName = viewModel.StoreMan;

            // Try to save this transaction
            db.Database.Connection.Open();
            DbTransaction dbTransaction = db.Database.Connection.BeginTransaction();
            try
            {
                repository.Adjustment.Add(lossAndAdjustment);
                dbTransaction.Commit();
            }
            catch (Exception exp)
            {
                dbTransaction.Rollback();
                //TODO: Save the detail of this exception somewhere
                throw new Exception("The Loss / Adjustment Transaction Cannot be saved. <br />Detail Message :" + exp.Message);
            }
        }

        /// <summary>
        /// Saves the loss adjustment transaction.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="user">The user.</param>
        public void SaveLossAdjustmentTransaction(LossesAndAdjustmentsViewModel viewModel, UserProfile user)
        {
            if (viewModel.IsLoss == true)
            {
                SaveLossTrasnsaction(viewModel, user);
            }
            else
            {
                SaveAdjustmentTrasnsaction(viewModel, user);
            }
        }


        /// <summary>
        /// Gets the total received from receipt allocation.
        /// </summary>
        /// <param name="siNumber">The si number.</param>
        /// <param name="commodityId">The commodity id.</param>
        /// <param name="hubId">The hub id.</param>
        /// <returns></returns>
        public decimal GetTotalReceivedFromReceiptAllocation(string siNumber, int commodityId, int hubId)
        {
            return GetTotalReceivedFromReceiptAllocation(
                repository.ShippingInstruction.GetShipingInstructionId(siNumber), commodityId, hubId);
        }


        /// <summary>
        /// Gets the commodity balance for hub.
        /// </summary>
        /// <param name="HubId">The hub id.</param>
        /// <param name="parentCommodityId">The parent commodity id.</param>
        /// <param name="si">The si.</param>
        /// <param name="project">The project.</param>
        /// <returns></returns>
        public decimal GetCommodityBalanceForHub(int HubId, int parentCommodityId, int si, int project)
        {
            var balance = (from v in db.Transactions
                           where
                               v.HubID == HubId && v.ParentCommodityID == parentCommodityId &&
                               v.ShippingInstructionID == si && v.ProjectCodeID == project && v.LedgerID == Ledger.Constants.GOODS_ON_HAND_UNCOMMITED
                           select v.QuantityInMT);
            if (balance.Any())
            {
                return balance.Sum();
            }
            return 0;
        }


        /// <summary>
        /// Saves the starting balance transaction.
        /// </summary>
        /// <param name="startingBalance">The starting balance.</param>
        /// <param name="user">The user.</param>
        /// <exception cref="System.Exception"></exception>
        public void SaveStartingBalanceTransaction(StartingBalanceViewModel startingBalance, UserProfile user)
        {
            int repositoryAccountGetAccountIDWithCreateNegative = repository.Account.GetAccountIDWithCreate(Account.Constants.DONOR, startingBalance.DonorID); ;
            int repositoryProjectCodeGetProjectCodeIdWIthCreateProjectCodeID = repository.ProjectCode.GetProjectCodeIdWIthCreate(startingBalance.ProjectNumber).ProjectCodeID; ;
            int repositoryShippingInstructionGetSINumberIdWithCreateShippingInstructionID = repository.ShippingInstruction.GetSINumberIdWithCreate(startingBalance.SINumber).ShippingInstructionID; ;
            int repositoryAccountGetAccountIDWithCreatePosetive = repository.Account.GetAccountIDWithCreate(Account.Constants.HUB, user.DefaultHub.HubID); ;

            TransactionGroup transactionGroup = new TransactionGroup();

            Transaction transactionOne = new Transaction();

            transactionOne.PartitionID = 0;
            transactionOne.LedgerID = Ledger.Constants.GOODS_UNDER_CARE;
            transactionOne.HubOwnerID = user.DefaultHub.HubOwner.HubOwnerID;
            transactionOne.AccountID = repositoryAccountGetAccountIDWithCreateNegative;
            transactionOne.HubID = user.DefaultHub.HubID;
            transactionOne.StoreID = startingBalance.StoreID;
            transactionOne.Stack = startingBalance.StackNumber;
            transactionOne.ProjectCodeID = repositoryProjectCodeGetProjectCodeIdWIthCreateProjectCodeID;
            transactionOne.ShippingInstructionID = repositoryShippingInstructionGetSINumberIdWithCreateShippingInstructionID;
            transactionOne.ProgramID = startingBalance.ProgramID;
            var comm = repository.Commodity.FindById(startingBalance.CommodityID);
            transactionOne.ParentCommodityID = (comm.ParentID != null)
                                                       ? comm.ParentID.Value
                                                       : comm.CommodityID;
            transactionOne.CommodityID = startingBalance.CommodityID;
            transactionOne.CommodityGradeID = null;
            transactionOne.QuantityInMT = 0 - startingBalance.QuantityInMT;
            transactionOne.QuantityInUnit = 0 - startingBalance.QuantityInUnit;
            transactionOne.UnitID = startingBalance.UnitID;
            transactionOne.TransactionDate = DateTime.Now;

            Transaction transactionTwo = new Transaction();

            transactionTwo.PartitionID = 0;
            transactionTwo.LedgerID = Ledger.Constants.GOODS_ON_HAND_UNCOMMITED;
            transactionTwo.HubOwnerID = user.DefaultHub.HubOwnerID;
            transactionTwo.AccountID = repositoryAccountGetAccountIDWithCreatePosetive;
            transactionTwo.HubID = user.DefaultHub.HubID;
            transactionTwo.StoreID = startingBalance.StoreID;
            transactionTwo.Stack = startingBalance.StackNumber;
            transactionTwo.ProjectCodeID = repositoryProjectCodeGetProjectCodeIdWIthCreateProjectCodeID;
            transactionTwo.ShippingInstructionID = repositoryShippingInstructionGetSINumberIdWithCreateShippingInstructionID;
            transactionTwo.ProgramID = startingBalance.ProgramID;
            transactionTwo.ParentCommodityID = (comm.ParentID != null)
                                                       ? comm.ParentID.Value
                                                       : comm.CommodityID;
            transactionTwo.CommodityID = startingBalance.CommodityID;
            transactionTwo.CommodityGradeID = null; // How did I get this value ? 
            transactionTwo.QuantityInMT = startingBalance.QuantityInMT;
            transactionTwo.QuantityInUnit = startingBalance.QuantityInUnit;
            transactionTwo.UnitID = startingBalance.UnitID;
            transactionTwo.TransactionDate = DateTime.Now;

            transactionGroup.PartitionID = 0;
            //transactionGroup.Transactions.Add(transactionOne);
            //transactionGroup.Transactions.Add(transactionTwo);
            //db.SaveChanges();
            // Try to save this transaction
            db.Database.Connection.Open();
            DbTransaction dbTransaction = db.Database.Connection.BeginTransaction();
            try
            {
                transactionGroup.Transactions.Add(transactionOne);
                transactionGroup.Transactions.Add(transactionTwo);
                repository.TransactionGroup.Add(transactionGroup);
                dbTransaction.Commit();
            }
            catch (Exception exp)
            {
                dbTransaction.Rollback();
                //TODO: Save the detail of this exception somewhere
                throw new Exception("The Starting Balance Transaction Cannot be saved. <br />Detail Message :" + exp.Message);
            }
            //throw new NotImplementedException();
        }


        /// <summary>
        /// Gets the list of starting balances.
        /// </summary>
        /// <param name="hubID">The hub ID.</param>
        /// <returns></returns>
        public List<StartingBalanceViewModelDto> GetListOfStartingBalances(int hubID)
        {
            return (from t in db.Transactions
                    where
                    !t.TransactionGroup.ReceiveDetails.Any()
                    &&
                    !t.TransactionGroup.DispatchDetails.Any()
                    &&
                    !t.TransactionGroup.InternalMovements.Any()
                    &&
                    !t.TransactionGroup.Adjustments.Any()
                    &&
                    t.HubID == hubID
                    join d in db.Donors on t.Account.EntityID equals d.DonorID
                    where t.Account.EntityType == "Donor"
                    select new StartingBalanceViewModelDto()
                    {
                        CommodityName = t.Commodity.Name,
                        SINumber = t.ShippingInstruction.Value,
                        ProgramName = t.Program.Name,
                        ProjectCode = t.ProjectCode.Value,
                        QuantityInMT = Math.Abs(t.QuantityInMT),
                        QuantityInUnit = Math.Abs(t.QuantityInUnit),
                        StackNumber = (t.Stack.HasValue) ? t.Stack.Value : 0,
                        StoreName = t.Store.Name,
                        UnitName = t.Unit.Name,
                        DonorName = d.Name,
                    }).ToList();
        }


        /// <summary>
        /// Gets the offloading report.
        /// </summary>
        /// <param name="hubID">The hub ID.</param>
        /// <param name="dispatchesViewModel">The dispatches view model.</param>
        /// <returns></returns>
        public List<ViewModels.Report.Data.OffloadingReport> GetOffloadingReport(int hubID, DispatchesViewModel dispatchesViewModel)
        {
            DateTime sTime = DateTime.Now;
            DateTime eTime = DateTime.Now;

            if (!dispatchesViewModel.PeriodId.HasValue || dispatchesViewModel.PeriodId == 6)
            {
                //filter it to only the current week
                //sTime = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
                eTime = sTime.AddDays(7).Date;
            }
            else
            {
                //start end date filters
                if (dispatchesViewModel.PeriodId == 8)
                {
                    sTime = dispatchesViewModel.StartDate ?? sTime;
                    eTime = dispatchesViewModel.EndDate ?? eTime;
                }
                //allocation round
                else if (dispatchesViewModel.PeriodId == 9)
                {
                }
                //allocation year + month
                else if (dispatchesViewModel.PeriodId == 9)
                {
                }
            }

            string StartTime = sTime.ToShortDateString();
            string EndTime = eTime.ToShortDateString();
            // string HUbName = repository.Hub.FindById(hubID).HubNameWithOwner;
            var dbGetOffloadingReport = db.RPT_Offloading(hubID, sTime, eTime).ToList();

            if (dispatchesViewModel.ProgramId.HasValue && dispatchesViewModel.ProgramId != 0)
            {
                dbGetOffloadingReport = dbGetOffloadingReport.Where(p => p.ProgramID == dispatchesViewModel.ProgramId).ToList();
            }
            if (dispatchesViewModel.AreaId.HasValue && dispatchesViewModel.AreaId != 0)
            {
                dbGetOffloadingReport = dbGetOffloadingReport.Where(p => p.RegionID == dispatchesViewModel.AreaId).ToList();
            }
            if (dispatchesViewModel.bidRefId != null)
            {
                dbGetOffloadingReport = dbGetOffloadingReport.Where(p => p.BidRefNo == dispatchesViewModel.bidRefId).ToList();
            }


            return (from t in dbGetOffloadingReport
                    group t by new { t.BidRefNo, t.ProgramName, t.Round, t.PeriodMonth, t.PeriodYear, t.RegionName }
                        into b
                        select new OffloadingReport()
                        {
                            ContractNumber = b.Key.BidRefNo,
                            EndDate = EndTime,
                            StartDate = StartTime,
                            Month = Convert.ToString(b.Key.PeriodMonth),
                            Round = Convert.ToString(b.Key.Round),
                            Year = b.Key.PeriodYear,//??0, modified Banty 23_5_13
                            Region = b.Key.RegionName,
                            Program = b.Key.ProgramName,
                            OffloadingDetails = b.Select(t1 => new OffloadingDetail()
                            {
                                RequisitionNumber = t1.RequisitionNo,
                                Product = t1.CommodityName,
                                Zone = t1.ZoneName,
                                Woreda = t1.WoredaName,
                                Destination = t1.FDPName,
                                Allocation = t1.AllocatedInMT ?? 0,
                                Dispatched = t1.DispatchedQuantity ?? 0,
                                Remaining = t1.RemainingAmount ?? 0,
                                Transporter = t1.TransaporterName,
                                Donor = t1.DonorName
                            }).ToList()

                        }).ToList();
            //return (from t in db.Dispatches.Where(p=>p.DispatchAllocationID != null)
            //                 .Select(q=>q.DispatchDetails.FirstOrDefault().TransactionGroup.Transactions.FirstOrDefault())
            //                 .Where(p => p.HubID == hubID
            //                    && p.LedgerID == Ledger.Constants.GOODS_DISPATCHED
            //                    && p.QuantityInMT > 0
            //                    ).ToList()

            //        group t by new {t.Program,
            //                        t.Hub,
            //                        t.TransactionGroup.DispatchDetails.FirstOrDefault().Dispatch,
            //                        t.TransactionGroup.DispatchDetails.FirstOrDefault().Dispatch.DispatchAllocation}
            //        into b
            //        select new OffloadingReport()
            //                   {
            //                       ContractNumber = b.Key.DispatchAllocation.BidRefNo,
            //                       EndDate = "today",
            //                       HubName = b.Key.Hub.HubNameWithOwner, 
            //                       StartDate = "yesterday",
            //                       Month = Convert.ToString(b.Key.Dispatch.PeriodMonth),
            //                       Round = Convert.ToString(b.Key.DispatchAllocation.Round),
            //                       Year = (b.Key.Dispatch.PeriodYear),
            //                       OffloadingDetails = b.Select(t1 => new OffloadingDetail()
            //                       {
            //                           RequisitionNumber = b.Key.DispatchAllocation.BidRefNo,
            //                           Product = t1.Commodity.Name,
            //                           Zone = b.Key.DispatchAllocation.FDP.AdminUnit.AdminUnit2.Name,
            //                           Woreda = b.Key.DispatchAllocation.FDP.AdminUnit.Name,
            //                           Destination = b.Key.DispatchAllocation.FDP.Name,
            //                           Allocation = b.Key.DispatchAllocation.Amount*10,
            //                           Dispatched = b.Key.DispatchAllocation.DispatchedAmount*10,
            //                           Remaining = b.Key.DispatchAllocation.RemainingQuantityInQuintals * 10,
            //                           Transporter = b.Key.Dispatch.Transporter.Name,
            //                           Donor = "DOn",//b.Key.DispatchAllocation.DonorID

            //                       }).ToList(),
            //                       Region = "afar",
            //                       Program = b.Key.Program.Name,

            //                   }).ToList();

        }


        /// <summary>
        /// Gets the receive report.
        /// </summary>
        /// <param name="hubID">The hub ID.</param>
        /// <param name="receiptsViewModel">The receipts view model.</param>
        /// <returns></returns>
        public List<ReceiveReport> GetReceiveReport(int hubID, ReceiptsViewModel receiptsViewModel)
        {
            DateTime sTime = DateTime.Now;
            DateTime eTime = DateTime.Now;

            if (!receiptsViewModel.PeriodId.HasValue)
            {
                sTime = new DateTime(1979, 01, 01, 00, 00, 00, 000);
            }
            else
            {
                //start end date filters
                if (receiptsViewModel.PeriodId == 8)
                {
                    sTime = receiptsViewModel.StartDate ?? sTime;
                    eTime = receiptsViewModel.EndDate ?? eTime;
                }
                //allocation round
                else if (receiptsViewModel.PeriodId == 6)
                {
                    //filter it to only the current week
                    // sTime = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
                    eTime = sTime.AddDays(7).Date;
                }
                //allocation year + month
                else if (receiptsViewModel.PeriodId == 9)
                {
                }
            }

            string StartTime = sTime.ToShortDateString();
            string EndTime = eTime.ToShortDateString();
            // string HUbName = repository.Hub.FindById(hubID).HubNameWithOwner;
            var dbGetReceiptReport = db.RPT_ReceiptReport(hubID, sTime, eTime).ToList();

            if (receiptsViewModel.ProgramId.HasValue && receiptsViewModel.ProgramId != 0)
            {
                dbGetReceiptReport = dbGetReceiptReport.Where(p => p.ProgramID == receiptsViewModel.ProgramId).ToList();
            }
            if (receiptsViewModel.CommodityTypeId.HasValue && receiptsViewModel.CommodityTypeId != 0)
            {
                dbGetReceiptReport = dbGetReceiptReport.Where(p => p.CommodityTypeID == receiptsViewModel.CommodityTypeId).ToList();
            }

            return (from t in dbGetReceiptReport
                    group t by new { t.BudgetYear }
                        into b
                        select new ReceiveReport()
                        {
                            BudgetYear = b.Key.BudgetYear.Value,
                            rows = b.Select(t1 => new ReceiveRow()
                            {
                                Product = t1.CommodityName,
                                Program = t1.ProgramName,
                                Quantity = t1.BalanceInMt.Value,
                                Quarter = t1.Quarter.Value
                                /*MeasurementUnit = t1.BalanceInUnit.Value*/

                            }).ToList()

                        }).ToList();
        }


        /// <summary>
        /// Gets the distribution report.
        /// </summary>
        /// <param name="hubID">The hub ID.</param>
        /// <param name="distributionViewModel">The distribution view model.</param>
        /// <returns></returns>
        public List<DistributionRows> GetDistributionReport(int hubID, DistributionViewModel distributionViewModel)
        {

            var dbDistributionReport = db.RPT_Distribution(hubID).ToList();

            if (distributionViewModel.PeriodId.HasValue && distributionViewModel.PeriodId != 0)
            {
                dbDistributionReport = dbDistributionReport.Where(p => p.Quarter == distributionViewModel.PeriodId.Value).ToList();
            }
            if (distributionViewModel.ProgramId.HasValue && distributionViewModel.ProgramId != 0)
            {
                dbDistributionReport = dbDistributionReport.Where(p => p.ProgramID == distributionViewModel.ProgramId.Value).ToList();
            }
            if (distributionViewModel.AreaId.HasValue && distributionViewModel.AreaId != 0)
            {
                dbDistributionReport = dbDistributionReport.Where(p => p.RegionID == distributionViewModel.AreaId.Value).ToList();
            }

            return (from t in dbDistributionReport
                    //  group t by new { t.BudgetYear }
                    //      into b
                    select new DistributionRows()
                    {
                        BudgetYear = t.PeriodYear,
                        Program = t.ProgramName,
                        DistributedAmount = t.DispatchedQuantity.Value,
                        Month = Convert.ToString(t.PeriodMonth),
                        Quarter = t.Quarter.Value,
                        Region = t.RegionName
                    }).ToList();
        }

        public bool DeleteById(int id)
        {
            var original = FindById(id);
            if (original == null) return false;
            db.Transactions.Remove(original);

            return true;
        }





        public Transaction FindById(System.Guid id)
        {
            return db.Transactions.FirstOrDefault(t => t.TransactionID == id);

        }









    }
}
