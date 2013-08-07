﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRMFSS.BLL.Services
{
    public class HubService : IHubService
    {

        private readonly IUnitOfWork _unitOfWork;


        public HubService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        #region Default Service Implementation
        public bool AddHub(Hub hub)
        {
            _unitOfWork.HubRepository.Add(hub);
            _unitOfWork.Save();
            return true;
        }

        public bool DeleteHub(Hub hub)
        {
            if (hub == null) return false;
            _unitOfWork.HubOwnerRepository.Delete(hub);
            _unitOfWork.Save();
            return true;
        }

        public bool DeleteById(int id)
        {
            var entity = _unitOfWork.HubRepository.FindById(id);
            if (entity == null) return false;
            _unitOfWork.HubRepository.Delete(entity);
            _unitOfWork.Save();
            return true;
        }

        public bool EditHub(Hub hub)
        {
            _unitOfWork.HubRepository.Edit(hub);
            _unitOfWork.Save();
            return true;
        }

        public Hub FindById(int id)
        {
            return _unitOfWork.HubRepository.FindById(id);
        }

        public List<Hub> GetAllHub()
        {
            return _unitOfWork.HubRepository.GetAll();
        }

        public List<Hub> FindBy(System.Linq.Expressions.Expression<Func<Hub, bool>> predicate)
        {
            return _unitOfWork.HubRepository.FindBy(predicate);
        }
        #endregion

        public void Dispose()
        {
            _unitOfWork.Dispose();

        }
    }
}
