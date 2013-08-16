﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using DRMFSS.BLL.ViewModels.Report;
using DRMFSS.BLL.ViewModels.Common;

namespace DRMFSS.BLL.Services
{
    public interface IProgramService
    {

        bool AddProgram(Program program);
        bool DeleteProgram(Program program);
        bool DeleteById(int id);
        bool EditProgram(Program program);
        Program FindById(int id);
        List<Program> GetAllProgram();
        List<Program> FindBy(Expression<Func<Program, bool>> predicate);
        List<ProgramViewModel> GetAllProgramsForReport();

    }
}


