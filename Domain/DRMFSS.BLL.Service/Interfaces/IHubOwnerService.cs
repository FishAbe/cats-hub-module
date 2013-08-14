﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DRMFSS.BLL.Services
{
    public interface IHubOwnerService
    {

        bool AddHubOwner(HubOwner hubOwner);
        bool DeleteHubOwner(HubOwner hubOwner);
        bool DeleteById(int id);
        bool EditHubOwner(HubOwner hubOwner);
        HubOwner FindById(int id);
        List<HubOwner> GetAllHubOwner();
        List<HubOwner> FindBy(Expression<Func<HubOwner, bool>> predicate);


    }
}


