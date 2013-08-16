﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DRMFSS.BLL.Services
{
    public interface IAdjustmentReasonService:IDisposable
    {

         bool AddAdjustmentReason(AdjustmentReason adjustmentReason);

        bool EditAdjustmentReason(AdjustmentReason adjustmentReason);

         bool DeleteAdjustmentReason(AdjustmentReason adjustmentReason);

        bool DeleteById(int id);

        List<AdjustmentReason> GetAllAdjustmentReason();

        AdjustmentReason FindById(int id);

        List<AdjustmentReason> FindBy(Expression<Func<AdjustmentReason, bool>> predicate);
   
    }
}
