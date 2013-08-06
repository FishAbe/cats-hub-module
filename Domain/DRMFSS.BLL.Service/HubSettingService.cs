﻿

using System;
using System.Collections.Generic;
using System.Linq.Expressions;



namespace DRMFSS.BLL.Services
{

    public class HubSettingService : IHubSettingService
    {
        private readonly IUnitOfWork _unitOfWork;


        public HubSettingService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        #region Default Service Implementation
        public bool AddHubSetting(HubSetting hubSetting)
        {
            _unitOfWork.HubSettingRepository.Add(hubSetting);
            _unitOfWork.Save();
            return true;

        }
        public bool EditHubSetting(HubSetting hubSetting)
        {
            _unitOfWork.HubSettingRepository.Edit(hubSetting);
            _unitOfWork.Save();
            return true;

        }
        public bool DeleteHubSetting(HubSetting hubSetting)
        {
            if (hubSetting == null) return false;
            _unitOfWork.HubSettingRepository.Delete(hubSetting);
            _unitOfWork.Save();
            return true;
        }
        public bool DeleteById(int id)
        {
            var entity = _unitOfWork.HubSettingRepository.FindById(id);
            if (entity == null) return false;
            _unitOfWork.HubSettingRepository.Delete(entity);
            _unitOfWork.Save();
            return true;
        }
        public List<HubSetting> GetAllHubSetting()
        {
            return _unitOfWork.HubSettingRepository.GetAll();
        }
        public HubSetting FindById(int id)
        {
            return _unitOfWork.HubSettingRepository.FindById(id);
        }
        public List<HubSetting> FindBy(Expression<Func<HubSetting, bool>> predicate)
        {
            return _unitOfWork.HubSettingRepository.FindBy(predicate);
        }
        #endregion

        public void Dispose()
        {
            _unitOfWork.Dispose();

        }

    }
}


