﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using DRMFSS.BLL.ViewModels.Report;


namespace DRMFSS.BLL.Services
{
    public interface IReceiveService
    {

        bool AddReceive(Receive entity);
        bool DeleteReceive(Receive entity);
        bool DeleteById(int id);
        bool EditReceive(Receive entity);
        Receive FindById(int id);
        List<Receive> GetAllReceive();
        List<Receive> FindBy(Expression<Func<Receive, bool>> predicate);
        List<Receive> ByHubId(int hubId);
        public List<PortViewModel> GetALlPorts();


    }
}


