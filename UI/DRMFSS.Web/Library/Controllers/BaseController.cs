﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Web.Security;
using DRMFSS.BLL;
using DRMFSS.Web.Helpers;
using MembershipProvider = DRMFSS.Web.Helpers.MembershipProvider;

namespace DRMFSS.Web
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseController : Controller
    {
        private UserProfile userProfile = null;
        /// <summary>
        /// Gets the user profile.
        /// </summary>
        public UserProfile UserProfile { get
            {
                if(userProfile == null)
                {
                    userProfile = GetCurrentUserProfile();
                }
                return userProfile;
            }  
        }
        protected IUnitOfWork repository = new UnitOfWork();


        /// <summary>
        /// Gets the current user profile.
        /// </summary>
        /// <returns></returns>
        protected UserProfile GetCurrentUserProfile()
        {
                
                MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true);
                MembershipProvider mem = new MembershipProvider();
                return mem.getUser(currentUser,repository);
        }

        /// <summary>
        /// Invokes the action in the current controller context.
        /// </summary>
        protected override void ExecuteCore()
        {
            
            string cultureName = null;
            
            //get the current user and chek if she/he set the LanguageCode
            if (User.Identity.IsAuthenticated)
            {
                try
                {

                    cultureName = this.GetCurrentUserProfile().LanguageCode;
                }
                catch (Exception e)
                {
                    cultureName = "en";
                }
            }
            else
            {
                // Attempt to read the culture cookie from Request
                HttpCookie cultureCookie = Request.Cookies["_culture"];
                if (cultureCookie != null)
                    cultureName = cultureCookie.Value;
                else
                    cultureName = "en";//Request.UserLanguages[0];  obtain it from HTTP header AcceptLanguages
            }
            // Validate culture name
            cultureName = CultureHelper.GetImplementedCulture(cultureName); // This is safe


            // Modify current thread's cultures            
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern = "dd-MMM-yyyy";

           base.ExecuteCore();
        }   
      
    }

}

