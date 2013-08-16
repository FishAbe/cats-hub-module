﻿

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;



namespace DRMFSS.BLL.Services
{

    public class UserHubService : IUserHubService
    {
        private readonly IUnitOfWork _unitOfWork;


        public UserHubService()
        {
            this._unitOfWork = new UnitOfWork();
        }
        #region Default Service Implementation
        public bool AddUserHub(UserHub entity)
        {
            _unitOfWork.UserHubRepository.Add(entity);
            _unitOfWork.Save();
            return true;

        }
        public bool EditUserHub(UserHub entity)
        {
            _unitOfWork.UserHubRepository.Edit(entity);
            _unitOfWork.Save();
            return true;

        }
        public bool DeleteUserHub(UserHub entity)
        {
            if (entity == null) return false;
            _unitOfWork.UserHubRepository.Delete(entity);
            _unitOfWork.Save();
            return true;
        }
        public bool DeleteById(int id)
        {
            var entity = _unitOfWork.UserHubRepository.FindById(id);
            if (entity == null) return false;
            _unitOfWork.UserHubRepository.Delete(entity);
            _unitOfWork.Save();
            return true;
        }
        public List<UserHub> GetAllUserHub()
        {
            return _unitOfWork.UserHubRepository.GetAll();
        }
        public UserHub FindById(int id)
        {
            return _unitOfWork.UserHubRepository.FindById(id);
        }
        public List<UserHub> FindBy(Expression<Func<UserHub, bool>> predicate)
        {
            return _unitOfWork.UserHubRepository.FindBy(predicate);
        }
        #endregion

        public void Dispose()
        {
            _unitOfWork.Dispose();

        }

       
           

       

        public void AddUserHub(int warehouseID, int userID)
        {
            UserProfile uProfile = _unitOfWork.UserProfileRepository.Get(t=>t.UserProfileID==userID,null,"UserHubs").FirstOrDefault();
            if (uProfile != null)
            {
                var associations = from v in uProfile.UserHubs
                                   where v.HubID == warehouseID
                                   select v;
                if (!associations.Any())
                {
                    var userHub = new UserHub
                                      {
                                          UserProfileID = uProfile.UserProfileID,
                                          HubID = warehouseID
                                      };
                    AddUserHub(userHub);
                }
            }
        }
    }
}


