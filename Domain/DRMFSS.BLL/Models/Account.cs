using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DRMFSS.BLL
{
    public partial class Account
    {
        public Account()
        {
            this.Transactions = new List<Transaction>();
        }
        [Key]
        public int AccountID { get; set; }
        public string EntityType { get; set; }
        public int EntityID { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
