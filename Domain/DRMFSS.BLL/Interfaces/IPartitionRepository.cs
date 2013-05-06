﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRMFSS.BLL.Interfaces
{
    public interface IPartitionRepository : IRepository<Partition>
    {
        List<ViewModels.ReplicationViewModel> GetHubsSyncrtonizationDetails(int publication);
    }
}
