﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DRMFSS.BLL.Services
{
    public interface IRoleService:IDisposable
    {

        bool AddRole(Role role);
        bool DeleteRole(Role role);
        bool DeleteById(int id);
        bool EditRole(Role role);
        Role FindById(int id);
        List<Role> GetAllRole();
        List<Role> FindBy(Expression<Func<Role, bool>> predicate);


    }
}


      
