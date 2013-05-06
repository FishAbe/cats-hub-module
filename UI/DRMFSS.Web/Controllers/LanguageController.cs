﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DRMFSS.Web.Helpers;

namespace DRMFSS.Web.Controllers
{
    public partial class LanguageController : BaseController
    {
        //
        // GET: /Language/

        public  ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Language/SetLanguage/en

        public virtual ActionResult SetLanguage(string lang)
        {

            string Lang = null;

            Lang = CultureHelper.GetNeutralCulture(CultureHelper.GetImplementedCulture(lang).ToString());

            if (User.Identity.IsAuthenticated) {
                
                this.GetCurrentUserProfile().ChangeLanguage(Lang);
            }
            else {
                HttpCookie cultureCookie = Response.Cookies["_culture"];
                cultureCookie.Value = Lang;
                Response.SetCookie(cultureCookie);
            }
            return Redirect(this.Request.UrlReferrer.PathAndQuery);
        }
    }
}
