﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRMFSS.BLL.ViewModels.Common;
using System.ComponentModel.DataAnnotations;

namespace DRMFSS.BLL.ViewModels
{
    public class StackEventViewModel
    {
        [Required(ErrorMessage="Store is required")]
        [Display(Name="Store")]
        public List<StoreViewModel> Stores { get; set; }
        [Required(ErrorMessage="Stack is required")]
        [Display(Name="Stack")]
        public List<int> Stacks { get; set; }
        [Required(ErrorMessage = "Event Type is required")]
        [Display(Name="Event Type")]
        public List<StackEventType> StackEventTypes { get; set; }

        [Required(ErrorMessage = "Date is required")]
        [Display(Name="Date")]
        public DateTime EventDate { get; set; }
        
        public int StoreId { get; set; }

        public int StoreIdTwo { get; set; }
        public int StackEventTypeId { get; set; }
        
        public int StackNumber { get; set; }
        [Required(ErrorMessage="Stack is required filed")]
        public int StackNumberTwo { get; set; }

        [Required(ErrorMessage = "Follow Up Date is required")]
        [Display(Name="Follow Up Date")]
        public DateTime FollowUpDate { get; set; }
        
        [Required(ErrorMessage = "Description is required")]
        [Display(Name="Description")]
        [UIHint("AmharicTextArea")]
        public string Description { get; set; }
        
        [Required(ErrorMessage = "Recommendation is required")]
        [Display(Name="Recommendation")]
        [UIHint("AmharicTextArea")]
        public string Recommendation { get; set; }

        public int UserProfileId { get; set; }

        public StackEventViewModel()
        {
        }

        public StackEventViewModel(IUnitOfWork repository, UserProfile user)
        {
            this.EventDate = DateTime.Now;
            this.Stores = repository.Hub.GetAllStoreByUser(user);
            this.Stacks = new List<int>();
            this.StackEventTypes = repository.StackEventType.GetAll();
        }
    }
}
