﻿

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DRMFSS.BLL.ViewModels.Common;
using System.Linq;

namespace DRMFSS.BLL.Services
{

    public class ProjectCodeService : IProjectCodeService
    {
        private readonly IUnitOfWork _unitOfWork;


        public ProjectCodeService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        #region Default Service Implementation
        public bool AddProjectCode(ProjectCode projectCode)
        {
            _unitOfWork.ProjectCodeRepository.Add(projectCode);
            _unitOfWork.Save();
            return true;

        }
        public bool EditProjectCode(ProjectCode projectCode)
        {
            _unitOfWork.ProjectCodeRepository.Edit(projectCode);
            _unitOfWork.Save();
            return true;

        }
        public bool DeleteProjectCode(ProjectCode projectCode)
        {
            if (projectCode == null) return false;
            _unitOfWork.ProjectCodeRepository.Delete(projectCode);
            _unitOfWork.Save();
            return true;
        }
        public bool DeleteById(int id)
        {
            var entity = _unitOfWork.ProjectCodeRepository.FindById(id);
            if (entity == null) return false;
            _unitOfWork.ProjectCodeRepository.Delete(entity);
            _unitOfWork.Save();
            return true;
        }
        public List<ProjectCode> GetAllProjectCode()
        {
            return _unitOfWork.ProjectCodeRepository.GetAll();
        }
        public ProjectCode FindById(int id)
        {
            return _unitOfWork.ProjectCodeRepository.FindById(id);
        }
        public List<ProjectCode> FindBy(Expression<Func<ProjectCode, bool>> predicate)
        {
            return _unitOfWork.ProjectCodeRepository.FindBy(predicate);
        }
        #endregion

        public void Dispose()
        {
            _unitOfWork.Dispose();

        }


        public int GetProjectCodeId(string projectCode)
        {
            var projCode = _unitOfWork.ProjectCodeRepository.FindBy(i => i.Value.ToUpper() == projectCode.ToUpper()).SingleOrDefault();

            if (projCode == null)
            {
                return 0;
            }
            else
            {
                return projCode.ProjectCodeID;
            }
        }

        /// <summary>
        /// Gets the project code id W ith create.
        /// </summary>
        /// <param name="projectNumber">The project number.</param>
        /// <returns></returns>
        public ProjectCode GetProjectCodeIdWIthCreate(string projectNumber)
        {
            var projCode = _unitOfWork.ProjectCodeRepository.FindBy(i => i.Value.ToUpper() == projectNumber.ToUpper()).SingleOrDefault();


            if (projCode != null)
            {
                return projCode;
            }
            else
            {
                ProjectCode newProjectCode = new ProjectCode()
                {
                    Value = projectNumber.ToUpperInvariant()
                };
                _unitOfWork.ProjectCodeRepository.Add(newProjectCode);
                _unitOfWork.Save();

               
                return newProjectCode;
            }

        }

        /// <summary>
        /// Gets all the project code in ProejctCodeViewModel 
        /// </summary>
        /// <returns></returns>
        public List<ViewModels.Common.ProjectCodeViewModel> GetAllProjectCodeForReport()
        {
            var projCodes = _unitOfWork.ProjectCodeRepository.Get();
            var projectCodes = (from c in projCodes select new ViewModels.Common.ProjectCodeViewModel() { ProjectCodeId = c.ProjectCodeID, ProjectName = c.Value }).ToList();
            return projectCodes;
        }

        public List<ProjectCodeViewModel> GetProjectCodesForCommodity(int hubID, int parentCommodityId)
        {
            var projeCodes = _unitOfWork.ProjectCodeRepository.Get();

           // projeCodes doesnt  have fields 'ParentCommodityID','HubsID' defined



            //var projectCodes = (from v in projeCodes
            //                    where v.ParentCommodityID == parentCommodityId && v.HubID == hubID
            //                    select
            //                        new ProjectCodeViewModel { ProjectCodeId = v.ProjectCodeID, ProjectName = v.ProjectCode.Value }).Distinct()
            //    .ToList();
            return null;
        }



    }
}


