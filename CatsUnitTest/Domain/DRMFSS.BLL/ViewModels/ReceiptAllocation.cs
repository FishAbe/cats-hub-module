﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace DRMFSS.BLL
{


    public partial class ReceiptAllocation
    {
        [NotMapped]
        public bool UserNotAllowedHub { set; get; }

        [NotMapped]
        public Decimal RemainingBalanceInUnit { get; set; }

        [NotMapped]
        public Decimal ReceivedQuantityInUnit
        {
            set { ; }
            get {

                if (this.QuantityInUnit == null)
                    return (0 - RemainingBalanceInUnit);
                else
                    return (this.QuantityInUnit.Value - RemainingBalanceInUnit);
               }

        }
        [NotMapped]
        public Decimal RemainingBalanceInMt { set; get; }
        [NotMapped]
        public Decimal ReceivedQuantityInMT
        {
            set { ; }
            get { return this.QuantityInMT - RemainingBalanceInMt; }
            
        } // { return GetReceivedAlready(this); } 
        [NotMapped]
        public string CommodityName { set;  get; }

        public decimal GetReceivedAlready(ReceiptAllocation receiptAllocation )
        {
            decimal sum = 0;
            if (receiptAllocation.Receives != null)
                foreach (Receive r in receiptAllocation.Receives)
                {
                    foreach (ReceiveDetail rd in r.ReceiveDetails)
                    {
                        sum = sum + rd.QuantityInMT;
                    }
                }
            return sum;
        }

        public decimal GetReceivedAlreadyInUnit(ReceiptAllocation receiptAllocation)
        {
            decimal sum = 0;
            if (receiptAllocation.Receives != null)
                foreach (Receive r in receiptAllocation.Receives)
                {
                    foreach (ReceiveDetail rd in r.ReceiveDetails)
                    {
                        sum = sum + rd.QuantityInUnit;
                    }
                }
            return sum;
        }
    }
}
