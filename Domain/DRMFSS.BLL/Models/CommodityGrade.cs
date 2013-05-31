using System;
using System.Collections.Generic;

namespace DRMFSS.BLL
{
    public partial class CommodityGrade
    {
        public CommodityGrade()
        {
            this.Transactions = new List<Transaction>();
        }

        public int CommodityGradeID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int SortOrder { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
