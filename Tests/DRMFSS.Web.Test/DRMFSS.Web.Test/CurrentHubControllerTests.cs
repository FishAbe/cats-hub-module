using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using DRMFSS.BLL;
using DRMFSS.BLL.Services;
using DRMFSS.Web.Controllers;
using DRMFSS.Web.Models;
using Moq;
using NUnit.Framework;

namespace DRMFSS.Web.Test
{
    [TestFixture]
    public class CurrentHubControllerTests
    {
        #region SetUp / TearDown

        private CurrentHubController _currentHubController;
        [SetUp]
        public void Init()
        {
            var userProfiles = new List<UserProfile>
                {
                    new UserProfile {UserProfileID = 1, UserName = "Nathnael", Password = "passWord", Email = "123@edge.com"},
                    new UserProfile {UserProfileID = 2, UserName = "Banty", Password="passWord", Email = "321@edge.com"},

                };
            var userProfileService = new Mock<IUserProfileService>();
            userProfileService.Setup(t => t.GetAllUserProfile()).Returns(userProfiles);

            _currentHubController = new CurrentHubController(userProfileService.Object);
        }

        [TearDown]
        public void Dispose()
        {
            _currentHubController.Dispose();
        }

        #endregion

        #region Tests

        [Test]
        public void CanViewDisplayCurrentHub()
        {
            //ACT
            var viewResult = _currentHubController.DisplayCurrentHub() as ViewResult;
            Assert.NotNull(viewResult);
            var model = viewResult.Model;

            //ASSERT
            Assert.IsInstanceOf<CurrentUserModel>(model);
        }

        #endregion
    }
}
