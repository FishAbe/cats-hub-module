﻿using System.ComponentModel.DataAnnotations;
using System.Linq;
using DRMFSS.BLL.MetaModels;

namespace DRMFSS.BLL
{
    
    partial class Setting
    {        
        DRMFSSEntities1 context = new DRMFSSEntities1();

        public Setting GetSetting(string key)
        {            
            Setting set = context.Settings.FirstOrDefault(m => m.Key == key);

            //if (set != null)
            //{
            //    // success
            //    s.ID = set.ID;
            //    s.Value = set.Value;
            //    s.Key = key;
            //    s.Option = set.Option;
            //    s.Type = set.Type;
            //    return 1;
            //}
            return set;
            
            //return 0;
        }

        public void EditSetting(Setting s)
        {
            Setting set = context.Settings.FirstOrDefault(m => m.Key == s.Key);

            if (set != null)
            {
                set.Category = s.Category;
                set.Value = s.Value;
                set.Option = s.Option;
                set.Type = s.Type;
                context.SaveChanges();
            }
            else
            {
                context.Settings.AddObject(s);

                context.SaveChanges();
            }
            //else
            //{
            //    set = new Setting();

            //    set.Category = s.Category;
            //    set.Key  = s.Key;
            //    set.Value = s.Value;
            //    set.Option = s.Option;
            //    set.Type = s.Type;

            //    context.Settings.AddObject(set);
            //}
     
        }

        public void AddSetting(Setting s)
        {
            context.Settings.AddObject(s);
            
            context.SaveChanges();
        }
    }


}