using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using DRMFSS.BLL;
using DRMFSS.BLL.Services;
using DRMFSS.Web.Controllers;
using Moq;
using NUnit.Framework;

namespace DRMFSS.Web.Test
{
    [TestFixture]
    public class UserWarehouseControllerTests
    {
        #region SetUp / TearDown

        private UserWarehouseController _userWarehouseController;
        [SetUp]
        public void Init()
        {
            var userHubServie = new Mock<IUserHubService>();
            userHubServie.Setup(t => t.GetAllUserHub()).Returns(new List<UserHub>()
                                                                    {
                                                                        new UserHub
                                                                            {
                                                                                HubID = 1,
                                                                                UserHubID = 1,
                                                                                UserProfileID = 1
                                                                            }
                                                                    });
            _userWarehouseController=new UserWarehouseController(userHubServie.Object);
        }

        [TearDown]
        public void Dispose()
        {
            _userWarehouseController.Dispose();
        }

        #endregion

        #region Tests

        [Test]
        public void CanDisplayUserWarhouses()
        {
            //Act
            var result = _userWarehouseController.Index();

            //Assert
            Assert.IsInstanceOf<List<UserHub>>(result.Model);
        }

        #endregion
    }
}
