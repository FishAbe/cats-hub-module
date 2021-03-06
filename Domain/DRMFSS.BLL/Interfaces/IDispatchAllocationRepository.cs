using System;
using System.Collections.Generic;
using DRMFSS.BLL.ViewModels;
using DRMFSS.BLL.ViewModels.Common;
using DRMFSS.BLL.ViewModels.Dispatch;


namespace DRMFSS.BLL.Interfaces
{

    /// <summary>
    /// 
    /// </summary>
    public interface IDispatchAllocationRepository : IGenericRepository<DispatchAllocation>,IRepository<DispatchAllocation>
    {
        /// <summary>
        /// Gets the uncommited balance of an SI, commodity and hub combination.
        /// </summary>
        /// <param name="siNumber">The si number.</param>
        /// <param name="commodityId">The commodity id.</param>
        /// <param name="hubId">The hub id.</param>
        /// <returns></returns>
        decimal GetUncommitedBalance(int siNumber, int commodityId, int hubId);
        /// <summary>
        /// Gets the uncommited allocations by hub.
        /// </summary>
        /// <param name="hubId">The hub id.</param>
        /// <returns></returns>
        List<DispatchAllocation> GetUncommitedAllocationsByHub(int hubId);
        /// <summary>
        /// Gets the commited allocations by hub.
        /// </summary>
        /// <param name="hubId">The hub id.</param>
        /// <returns></returns>
        List<DispatchAllocation> GetCommitedAllocationsByHub(int hubId);
        /// <summary>
        /// Gets the uncomited allocations.
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <returns></returns>
        List<DispatchAllocation> GetUncomitedAllocations(Guid[] ids);
        /// <summary>
        /// Gets the available SI numbers.
        /// </summary>
        /// <param name="commodityID">The commodity ID.</param>
        /// <returns></returns>
        List<ShippingInstruction> GetAvailableSINumbersWithUncommitedBalance(int commodityID, int hubID);
        /// <summary>
        /// Gets the available commodities.
        /// </summary>
        /// <param name="requisitionNo">The requisition no.</param>
        /// <returns></returns>
        List<Commodity> GetAvailableCommodities(string requisitionNo);
        /// <summary>
        /// Gets the allocations.
        /// </summary>
        /// <param name="RequisitionNo">The requisition no.</param>
        /// <param name="CommodityID">The commodity ID.</param>
        /// <param name="hubId">The hub id.</param>
        /// <param name="UnComitted">if set to <c>true</c> [un comitted].</param>
        /// <returns></returns>
        List<DispatchAllocation> GetAllocations(string RequisitionNo, int CommodityID, int hubId, bool UnComitted, string PreferedWeightMeasurment);
        /// <summary>
        /// Gets the allocations.
        /// </summary>
        /// <param name="RequisitionNo">The requisition no.</param>
        /// <param name="hubId">The hub id.</param>
        /// <param name="UnComitted">if set to <c>true</c> [un comitted].</param>
        /// <returns></returns>
        List<DispatchAllocation> GetAllocations(string RequisitionNo, int hubId, bool UnComitted);
        /// <summary>
        /// Gets the uncommited allocation transaction.
        /// </summary>
        /// <param name="commodityID">The commodity ID.</param>
        /// <param name="shipingInstructionID">The shiping instruction ID.</param>
        /// <param name="hubID">The hub ID.</param>
        /// <returns></returns>
        Transaction GetUncommitedAllocationTransaction(int commodityID, int shipingInstructionID, int hubID);
        /// <summary>
        /// Attaches the transaction group.
        /// </summary>
        /// <param name="allocation">The allocation.</param>
        /// <param name="TransactionGroupID">The transaction group ID.</param>
        /// <returns></returns>
        bool AttachTransactionGroup(DispatchAllocation allocation, int TransactionGroupID);
        /// <summary>
        /// Gets the SI balances.
        /// </summary>
        /// <returns></returns>
        List<BLL.SIBalance> GetSIBalances(int hubID);
        /// <summary>
        /// Gets the available requision numbers.
        /// </summary>
        /// <param name="HubId">The hub id.</param>
        /// <param name="UnCommited">if set to <c>true</c> [un commited].</param>
        /// <returns></returns>
        List<string> GetAvailableRequisionNumbers(int HubId, bool UnCommited);
        /// <summary>
        /// Gets the SI balances grouped by commodity.
        /// </summary>
        /// <returns></returns>
        List<BLL.CommodityBalance> GetSIBalancesGroupedByCommodity(int hubID);

        /// <summary>
        /// Gets the allocations under a given requisition number.
        /// </summary>
        /// <param name="requisitonNumber">The requisiton number.</param>
        /// <returns></returns>
        List<DispatchAllocation> GetAllocations(string requisitonNumber);

        /// <summary>
        /// Commit allocations under a given si number and project code.
        /// </summary>
        /// <param name="AllocationId">The Allocation ID.</param>
        /// <param name="SIID">The SI Number ID.</param>
        /// <param name="ProjectCodeID">The Project Code ID.</param>
        /// <returns></returns>
        bool CommitDispatchAllocation(Guid AllocationId, int SIID, int ProjectCodeID);

        /// <summary>
        /// Commits the dispatch allocation.
        /// </summary>
        /// <param name="checkedRecords">The checked records.</param>
        /// <param name="SINumber">The SI number.</param>
        /// <param name="user">The user.</param>
        /// <param name="ProjectCode">The project code.</param>
        void CommitDispatchAllocation(string[] checkedRecords, int SINumber, UserProfile user, int ProjectCode);

        /// <summary>
        /// Gets the uncommited allocations.
        /// </summary>
        /// <param name="hubId">The hub id.</param>
        /// <returns></returns>
        List<RequisitionSummary> GetSummaryForUncommitedAllocations(int hubId);

        /// <summary>
        /// Gets the commited allocations.
        /// </summary>
        /// <param name="hubId">The hub id.</param>
        /// <returns></returns>
        List<RequisitionSummary> GetSummaryForCommitedAllocations(int hubId);

        /// <summary>
        /// Gets the uncommited SI balance.
        /// </summary>
        /// <param name="hubID">The hub ID.</param>
        /// <param name="commodityId">The commodity id.</param>
        /// <returns></returns>
        List<SIBalance> GetUncommitedSIBalance(int hubID, int commodityId, string PreferedWeightMeasurment);

        /// <summary>
        /// Closes the by id.
        /// </summary>
        /// <param name="id">The id.</param>
        void CloseById(Guid id);

        /// <summary>
        /// Gets the commited allocations by hub detached.
        /// </summary>
        /// <param name="hubId">The hub id.</param>
        /// <returns></returns>
        List<DispatchAllocationViewModelDto> GetCommitedAllocationsByHubDetached(int hubId, string PreferedWeightMeasurment, bool? closedToo, int? AdminUnitId, int? CommodityType);


        List<BidRefViewModel> GetAllBidRefsForReport();
    }
}
