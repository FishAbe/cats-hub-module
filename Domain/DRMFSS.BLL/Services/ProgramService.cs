﻿

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DRMFSS.BLL.ViewModels.Report;
using DRMFSS.BLL.ViewModels.Common;
using System.Linq;
    
namespace DRMFSS.BLL.Services
{

    public class ProgramService : IProgramService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProgramService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        #region Default Service Implementation
        public bool AddProgram(Program program)
        {
            _unitOfWork.ProgramRepository.Add(program);
            _unitOfWork.Save();
            return true;

        }
        public bool EditProgram(Program program)
        {
            _unitOfWork.ProgramRepository.Edit(program);
            _unitOfWork.Save();
            return true;

        }
        public bool DeleteProgram(Program program)
        {
            if (program == null) return false;
            _unitOfWork.ProgramRepository.Delete(program);
            _unitOfWork.Save();
            return true;
        }
        public bool DeleteById(int id)
        {
            var entity = _unitOfWork.ProgramRepository.FindById(id);
            if (entity == null) return false;
            _unitOfWork.ProgramRepository.Delete(entity);
            _unitOfWork.Save();
            return true;
        }
        public List<Program> GetAllProgram()
        {
            return _unitOfWork.ProgramRepository.GetAll();
        }
        public Program FindById(int id)
        {
            return _unitOfWork.ProgramRepository.FindById(id);
        }
        public List<Program> FindBy(Expression<Func<Program, bool>> predicate)
        {
            return _unitOfWork.ProgramRepository.FindBy(predicate);
        }
        #endregion

        public void Dispose()
        {
            _unitOfWork.Dispose();

        }


        public List<ProgramViewModel> GetAllProgramsForReport()
        {

            var prog = _unitOfWork.ProgramRepository.GetAll();
            var programs = (from c in prog select new ProgramViewModel() { ProgramId = c.ProgramID, ProgramName = c.Name }).ToList();
            programs.Insert(0, new ProgramViewModel { ProgramName = "All Programs" });
            return programs;
        }

    }
}


