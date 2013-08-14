﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DRMFSS.BLL.Services
{
    public interface IHubSettingService
    {

        bool AddHubSetting(HubSetting hubSetting);
        bool DeleteHubSetting(HubSetting hubSetting);
        bool DeleteById(int id);
        bool EditHubSetting(HubSetting hubSetting);
        HubSetting FindById(int id);
        List<HubSetting> GetAllHubSetting();
        List<HubSetting> FindBy(Expression<Func<HubSetting, bool>> predicate);


    }
}


