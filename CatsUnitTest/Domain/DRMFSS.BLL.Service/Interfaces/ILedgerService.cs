﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DRMFSS.BLL.Services
{
    public interface ILedgerService
    {

        bool AddLedger(Ledger entity);
        bool DeleteLedger(Ledger entity);
        bool DeleteById(int id);
        bool EditLedger(Ledger entity);
        Ledger FindById(int id);
        List<Ledger> GetAllLedger();
        List<Ledger> FindBy(Expression<Func<Ledger, bool>> predicate);


    }
}


