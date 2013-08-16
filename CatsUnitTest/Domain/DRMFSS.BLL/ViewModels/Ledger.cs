﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRMFSS.BLL
{
    /// <summary>
    /// Leger Class
    /// </summary>
    public partial class Ledger
    {

        /// <summary>
        /// Constants in the Ledger 
        /// </summary>
        public class Constants
        {

            /// <summary>
            /// 
            /// </summary>
            public const int GOODS_ON_HAND_UNCOMMITED = 2;
            public const int GOODS_ON_HAND_COMMITED = 3;
            public const int GOODS_PROMISSED_PLEDGE = 4;
            public const int GOODS_PROMISSED_GIFT_CERTIFICATE_UNCOMMITED = 13;
            public const int GOODS_PROMISSED_GIFT_CERTIFICATE_COMMITED = 5;
            public const int GOODS_PROMISSED_AS_PART_OF_LOAN_UNCOMMITED = 7;
            public const int GOODS_PROMISSED_AS_PART_OF_LOAN_COMMITED = 8;
            public const int GOODS_DISPATCHED = 9;
            public const int GOODS_RECIEVABLE = 10;
            public const int LIABILITIES = 11;
            public const int GOODS_UNDER_CARE = 12;
        }

    }
}
