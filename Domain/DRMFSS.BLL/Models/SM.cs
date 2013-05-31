using System;
using System.Collections.Generic;

namespace DRMFSS.BLL
{
    public partial class SM
    {
        public int SMSID { get; set; }
        public string InOutInd { get; set; }
        public string MobileNumber { get; set; }
        public string Text { get; set; }
        public Nullable<System.DateTime> RequestDate { get; set; }
        public Nullable<System.DateTime> SendAfterDate { get; set; }
        public Nullable<System.DateTime> QueuedDate { get; set; }
        public Nullable<System.DateTime> SentDate { get; set; }
        public string Status { get; set; }
        public System.DateTime StatusDate { get; set; }
        public int Attempts { get; set; }
        public Nullable<System.DateTime> LastAttemptDate { get; set; }
        public string EventTag { get; set; }
    }
}
