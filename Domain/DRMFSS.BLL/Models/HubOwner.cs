using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DRMFSS.BLL
{
    public partial class HubOwner
    {
        public HubOwner()
        {
            this.Hubs = new List<Hub>();
            this.Transactions = new List<Transaction>();
        }
        [Key]
        public int HubOwnerID { get; set; }
        public string Name { get; set; }
        public string LongName { get; set; }
        public virtual ICollection<Hub> Hubs { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
