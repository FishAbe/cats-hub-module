﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Data;
using System.Data.Objects;
using System.Web;
using System.Reflection;
using System.Data.Objects.DataClasses;
using System.Runtime.Serialization;
using System.IO;
using System.Data.Entity;
using System.Data.SqlClient;


namespace DRMFSS.BLL
{


    partial class CTSContext
    {
        /// <summary>
        /// 
        /// </summary>
        public enum AuditActions
        {
            I,
            U,
            D
        }

        public class ProcParam
        {

            public object Value { get; set; }
            public string ParmName { get; set; }
        }

        //   private string userName;


        private List<BLL.Audit> auditTrailList = new List<BLL.Audit>();
        #region  Old Audit Trial Implementation Code
        /// <summary>
        /// Called when [context created].
        /// </summary>
        /*  partial void OnContextCreated()
          {
              HttpContext context = HttpContext.Current;
              if (context != null && context.Request.IsAuthenticated && context.User != null)
              {
                  userName = context.User.Identity.Name;
              }
              else
              {
                  userName = "Anonymous";
              }
              this.SavingChanges += new EventHandler(DRMFSSEntities_SavingChanges);
          }*/

        /// <summary>
        /// Handles the SavingChanges event of the DRMFSSEntities control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /*  private void DRMFSSEntities_SavingChanges(object sender, EventArgs e)
          {
              IEnumerable<ObjectStateEntry> changes =
                  this.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Deleted |
                                                                EntityState.Modified);
              foreach (ObjectStateEntry stateEntryEntity in changes)
              {
                  if (!stateEntryEntity.IsRelationship &&
                      stateEntryEntity.Entity != null &&
                      !(stateEntryEntity.Entity is BLL.Audit) && !(stateEntryEntity.Entity is BLL.Role) &&
                      !(stateEntryEntity.Entity is BLL.UserProfile) &&
                      !(stateEntryEntity.Entity is BLL.ForgetPasswordRequest))
                  {
  //is a normal entry, not a relationship
                      auditTrailList = this.AuditTrailFactory(stateEntryEntity, userName);
                  }
              }

              if (auditTrailList.Count > 0)
              {
                  foreach (var audit in auditTrailList)
                  {
                      //add all audits 
                      //TODO: Remove this try catch
                      audit.AuditID = Guid.NewGuid();
                      this.AddToAudits(audit);
                  }
              }
          }*/

        //TODO: Revise this method and make it work again.
        /// <summary>
        /// Audits the trail factory.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="UserName">Name of the user.</param>
        /// <returns></returns>
        /// 
        /*  private List<BLL.Audit> AuditTrailFactory(ObjectStateEntry entry, string UserName)
          {

              //return null;
              IUnitOfWork repository = new UnitOfWork();
              List<BLL.Audit> audits = new List<BLL.Audit>();

              try
              {
                  int UserId = 0;
                  BLL.UserProfile cuUser = repository.UserProfile.GetUser(UserName);
                  if (cuUser != null)
                  {
                      UserId = cuUser.UserProfileID;
                  }

                  if (entry.State == EntityState.Added)
                  {
                      //entry is Added 
                      foreach (var propName in entry.EntitySet.ElementType.Members)
                      {

                          BLL.Audit audit = new Audit();
                          audit.DateTime = DateTime.Now;
                          audit.TableName = entry.Entity.GetType().Name;
                          audit.LoginID = UserId;
                          audit.HubID = cuUser.DefaultHub.HubID;
                          //TODO: fix this partion id
                          audit.PartitionID = 0;
                          // this means the value is changed
                          audit.OldValue = null;
                          audit.NewValue = entry.CurrentValues[propName.Name].ToString();
                          //Dispose the second context 
                          audit.Action = AuditActions.I.ToString();
                          audit.PrimaryKey = entry.EntityKey.EntityKeyValues[0].Value.ToString();
                          //assing collection of mismatched Columns name as serialized string 
                          audit.ColumnName = propName.Name;
                          audits.Add(audit);


                      }
                  }
                  else if (entry.State == EntityState.Deleted)
                  {
                      BLL.Audit audit = new Audit();
                      audit.DateTime = DateTime.Now;
                      audit.TableName = entry.Entity.GetType().Name;
                      audit.LoginID = UserId;
                      audit.HubID = cuUser.DefaultHub.HubID;
                      //TODO: fix this partion id
                      audit.PartitionID = 0;
                      //Dispose the second context 
                      audit.Action = AuditActions.D.ToString();
                      audit.PrimaryKey = entry.EntityKey.EntityKeyValues[0].Value.ToString();
                      //assing collection of mismatched Columns name as serialized string 
                      audit.ColumnName = null;
                      audits.Add(audit);
                  }
                  else
                  {
                      //initially commented
                      //entry is modified
                      CTSContext entities = new CTSContext();
                      object orgentry = entities.GetObjectByKey(entry.EntityKey);
                      if (orgentry != null)
                      {
                          EntityObject obj = (orgentry as EntityObject);
                          ObjectStateEntry oldEntry =
                              entities.ObjectStateManager.GetObjectStateEntry(((IEntityWithKey) obj).EntityKey);

                          // ChangedValues values = GetChangedValues(stateEntry, entry);
                          foreach (string propName in entry.GetModifiedProperties())
                          {
                              if (
                                  !oldEntry.CurrentValues[propName].ToString().Equals(
                                      entry.CurrentValues[propName].ToString()))
                              {
                                  BLL.Audit audit = new Audit();
                                  audit.DateTime = DateTime.Now;
                                  audit.TableName = entry.Entity.GetType().Name;
                                  audit.LoginID = UserId;
                                  audit.HubID = cuUser.DefaultHub.HubID;
                                  //TODO: fix this partion id
                                  audit.PartitionID = 0;
                                  // this means the value is changed
                                  audit.OldValue = oldEntry.CurrentValues[propName].ToString();
                                  audit.NewValue = entry.CurrentValues[propName].ToString();
                                  //Dispose the second context 
                                  audit.Action = AuditActions.U.ToString();
                                  audit.PrimaryKey = entry.EntityKey.EntityKeyValues[0].Value.ToString();
                                  //assing collection of mismatched Columns name as serialized string 
                                  audit.ColumnName = propName;
                                  audits.Add(audit);
                              }

                          }
                          entities.Dispose(true);

                      }
                  }

                  return audits;
              }
              catch
              {
                  return audits;
              }
          }
          */
        #endregion

        private string CurrentUserName
        {
            get
            {
                string userName = string.Empty;
                HttpContext context = HttpContext.Current;
                if (context != null && context.Request.IsAuthenticated && context.User != null)
                {
                    userName = context.User.Identity.Name;
                }
                else
                {
                    userName = "Anonymous";
                }
                return userName;
            }
        }
        // This is overridden to prevent someone from calling SaveChanges without specifying the user making the change
        public override int SaveChanges()
        {
            //TODO:Refactor Required when doing service layer
            int UserId = 0;
            BLL.UserProfile cuUser = this.UserProfiles.FirstOrDefault(t => t.UserName == CurrentUserName);
            if (cuUser != null)
            {
                UserId = cuUser.UserProfileID;
            }

            return SaveChanges(cuUser);
            //throw new InvalidOperationException("User ID must be provided");
        }

        public int SaveChanges(BLL.UserProfile userId)
        {
            // Get all Added/Deleted/Modified entities (not Unmodified or Detached)
            foreach (var ent in this.ChangeTracker.Entries().Where(p => p.State == System.Data.EntityState.Added || p.State == System.Data.EntityState.Deleted || p.State == System.Data.EntityState.Modified))
            {
                // For each changed record, get the audit record entries and add them
                foreach (Audit x in GetAuditRecordsForChange(ent, userId))
                {
                    this.Audits.Add(x);
                }
            }

            // Call the original SaveChanges(), which will save both the changes made and the audit records
            return base.SaveChanges();
        }
        /// <summary>
        /// GetAuditRecordsForChange
        /// </summary>
        /// <param name="dbEntry"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private List<Audit> GetAuditRecordsForChange(DbEntityEntry dbEntry, BLL.UserProfile userId)
        {
            List<Audit> result = new List<Audit>();

            DateTime changeTime = DateTime.UtcNow;

            // Get the Table() attribute, if one exists
            TableAttribute tableAttr = dbEntry.Entity.GetType().GetCustomAttributes(typeof(TableAttribute), false).SingleOrDefault() as TableAttribute;

            // Get table name (if it has a Table attribute, use that, otherwise get the pluralized name)
            string tableName = tableAttr != null ? tableAttr.Name : dbEntry.Entity.GetType().Name;

            // Get primary key value (If you have more than one key column, this will need to be adjusted)
            string keyName = dbEntry.Entity.GetType().GetProperties().Single(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Count() > 0).Name;

            if (dbEntry.State == System.Data.EntityState.Added)
            {
                // For Inserts, just add the whole record
                // If the entity implements IDescribableEntity, use the description from Describe(), otherwise use ToString()
                result.Add(new Audit()
                {
                    AuditID = Guid.NewGuid(),
                    LoginID = userId.UserProfileID,
                    DateTime = changeTime,
                    Action = "A", // Added
                    TableName = tableName,
                    PrimaryKey = dbEntry.CurrentValues.GetValue<object>(keyName).ToString(),  // Again, adjust this if you have a multi-column key
                    ColumnName = "*ALL",    // Or make it nullable, whatever you want
                    NewValue = dbEntry.CurrentValues.ToObject().ToString(),

                    HubID = userId.DefaultHub.HubID,
                    //TODO: fix this partion id
                    PartitionID = 0
                }
                    );
            }
            else if (dbEntry.State == System.Data.EntityState.Deleted)
            {
                // Same with deletes, do the whole record, and use either the description from Describe() or ToString()
                result.Add(new Audit()
                {
                    AuditID = Guid.NewGuid(),
                    LoginID = userId.UserProfileID,
                    DateTime = changeTime,
                    Action = "D", // Deleted
                    TableName = tableName,
                    PrimaryKey = dbEntry.OriginalValues.GetValue<object>(keyName).ToString(),
                    ColumnName = "*ALL",
                    NewValue = dbEntry.OriginalValues.ToObject().ToString(),
                    HubID = userId.DefaultHub.HubID,
                    //TODO: fix this partion id
                    PartitionID = 0
                }
                    );
            }
            else if (dbEntry.State == System.Data.EntityState.Modified)
            {
                foreach (string propertyName in dbEntry.OriginalValues.PropertyNames)
                {
                    // For updates, we only want to capture the columns that actually changed
                    if (!object.Equals(dbEntry.OriginalValues.GetValue<object>(propertyName), dbEntry.CurrentValues.GetValue<object>(propertyName)))
                    {
                        result.Add(new Audit()
                        {
                            AuditID = Guid.NewGuid(),
                            LoginID = userId.UserProfileID,
                            DateTime = changeTime,
                            Action = "M",    // Modified
                            TableName = tableName,
                            PrimaryKey = dbEntry.OriginalValues.GetValue<object>(keyName).ToString(),
                            ColumnName = propertyName,
                            OldValue = dbEntry.OriginalValues.GetValue<object>(propertyName) == null ? null : dbEntry.OriginalValues.GetValue<object>(propertyName).ToString(),
                            NewValue = dbEntry.CurrentValues.GetValue<object>(propertyName) == null ? null : dbEntry.CurrentValues.GetValue<object>(propertyName).ToString(),
                            HubID = userId.DefaultHub.HubID,
                            //TODO: fix this partion id
                            PartitionID = 0
                        }
                            );
                    }
                }
            }
            // Otherwise, don't do anything, we don't care about Unchanged or Detached entities

            return result;
        }

        private ObjectResult<T> ExecProcedure<T>(string procedureName, string entitySetName, params ProcParam[] param) where T : class
        {

            // If using Code First we need to make sure the model is built before we open the connection
            // This isn't required for models created with the EF Designer
            Database.Initialize(force: false);

            // Create a SQL command to execute the sproc
            var cmd = Database.Connection.CreateCommand();
            cmd.CommandText = procedureName;

            foreach (var procParam in param)
            {
                var dbParam = new SqlParameter(procParam.ParmName, procParam.Value);

                cmd.Parameters.Add(dbParam);
            }

            try
            {

                Database.Connection.Open();
                // Run the sproc 
                var reader = cmd.ExecuteReader();

                // Read Blogs from the first result set
                var result = ((IObjectContextAdapter)this)
                    .ObjectContext
                    .Translate<T>(reader, entitySetName, MergeOption.AppendOnly);

                return result;

            }
            finally
            {
                Database.Connection.Close();
            }

        }

        public ObjectResult<RPT_MonthGiftSummary_Result> GetMonthlySummary()
        {
            return ExecProcedure<RPT_MonthGiftSummary_Result>("GetMonthlyGiftSummary", "RPT_MonthGiftSummary_Results");
        }
        public ObjectResult<RPT_Distribution_Result> RPT_Distribution(int hubId)
        {
            return ExecProcedure<RPT_Distribution_Result>("RPT_Distribution", "RPT_Distributions", new ProcParam() { ParmName = "hubId", Value = hubId });
        }
        public ObjectResult<RPT_Distribution_Result> RPT_ReceiptReport(int hubID, DateTime sTime, DateTime eTime)
        {
            return ExecProcedure<RPT_Distribution_Result>("RPT_Distribution", "RPT_Distributions",
                new ProcParam() { ParmName = "hubId", Value = hubID },
                new ProcParam() { ParmName = "sTime", Value = sTime },
                new ProcParam() { ParmName = "eTime", Value = eTime }
                );
        }

        public ObjectResult<RPT_Distribution_Result> RPT_Offloading(int hubID, DateTime sTime, DateTime eTime)
        {
            return ExecProcedure<RPT_Distribution_Result>("RPT_Offloading", "RPT_Offloadings",
                new ProcParam() { ParmName = "hubId", Value = hubID },
                new ProcParam() { ParmName = "sTime", Value = sTime },
                new ProcParam() { ParmName = "eTime", Value = eTime }
                );
        }
        public ObjectResult<RPT_Distribution_Result> util_GetDispatchedAllocationFromSI(int hubId, int sis)
        {
            return ExecProcedure<RPT_Distribution_Result>("util_GetDispatchedAllocationFromSI_Result", "util_GetDispatchedAllocationFromSI_Results",
                new ProcParam() { ParmName = "hubId", Value = hubId },
                new ProcParam() { ParmName = "sis", Value = sis }
                );
        }
        public ObjectResult<BinCardReport> RPT_BinCardNonFood(int hubID, int? StoreID, int? CommodityID, string ProjectID)
        {
            return ExecProcedure<BinCardReport>("RPT_BinCardNonFood", "RPT_BinCardNonFoods",
                new ProcParam() { ParmName = "hubId", Value = hubID },
                new ProcParam() { ParmName = "StoreID", Value = StoreID },
                  new ProcParam() { ParmName = "CommodityID", Value = CommodityID },
                new ProcParam() { ParmName = "ProjectID", Value = ProjectID }
                );
        }
        public ObjectResult<BinCardReport> RPT_BinCard(int hubID, int? StoreID, int? CommodityID, string ProjectID)
        {
            return ExecProcedure<BinCardReport>("RPT_BinCard", "RPT_BinCardNonFoods",
                new ProcParam() { ParmName = "hubId", Value = hubID },
                new ProcParam() { ParmName = "StoreID", Value = StoreID },
                  new ProcParam() { ParmName = "CommodityID", Value = CommodityID },
                new ProcParam() { ParmName = "ProjectID", Value = ProjectID }
                );
        }

        public ObjectResult<RPT_MonthlyGiftSummary_Result> GetMonthlyGiftSummaryETA()
        {
            return ExecProcedure<RPT_MonthlyGiftSummary_Result>("GetMonthlyGiftSummaryETA", "RPT_MonthlyGiftSummary_Results");
        }
        public ObjectResult<RPT_MonthlyGiftSummary_Result> GetMonthlyGiftSummary()
        {
            return ExecProcedure<RPT_MonthlyGiftSummary_Result>("RPT_MonthlyGiftSummary", "RPT_MonthlyGiftSummary_Results");
        }
        public ObjectResult<StockStatusReport> RPT_StockStatus(int hubID, int commodityID)
        {
            return ExecProcedure<StockStatusReport>("RPT_StockStatus", "StockStatusReports",
                new ProcParam() { ParmName = "hubId", Value = hubID },
                  new ProcParam() { ParmName = "CommodityID", Value = commodityID });
        }
        public ObjectResult<StockStatusReport> RPT_StockStatusNonFood(int? hubID, int? commodityID)
        {
            return ExecProcedure<StockStatusReport>("RPT_StockStatusNonFood", "StockStatusReports",
                new ProcParam() { ParmName = "hubId", Value = hubID },
                  new ProcParam() { ParmName = "CommodityID", Value = commodityID });
        }
        public ObjectResult<StatusReportBySI_Result> GetStatusReportBySI(int? hubID)
        {
            return ExecProcedure<StatusReportBySI_Result>("GetStatusReportBySI", "StatusReportBySI_Results",
                new ProcParam() { ParmName = "hubId", Value = hubID });
        }
        public ObjectResult<DispatchFulfillmentStatus_Result> GetDispatchFulfillmentStatus(int? hubID)
        {
            return ExecProcedure<DispatchFulfillmentStatus_Result>("GetDispatchFulfillmentStatus", "DispatchFulfillmentStatus_Results",
                new ProcParam() { ParmName = "hubId", Value = hubID });
        }

        public ObjectResult<DispatchFulfillmentStatus_Result> GetAllLossAndAdjustmentLog()
        {
            return ExecProcedure<DispatchFulfillmentStatus_Result>("GetDispatchFulfillmentStatus", "DispatchFulfillmentStatus_Results");
        }
        /**
         * 
         * 
         */

    }
}
