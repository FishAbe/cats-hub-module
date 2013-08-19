using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using DRMFSS.BLL;
using DRMFSS.BLL.Services;
using NUnit.Framework;
using DRMFSS.Web.Controllers;
using Moq;

namespace DRMFSS.Web.Test
{
    [TestFixture]
    public class ErrorControllerTests 
    {
        #region SetUp / TearDown

        private ErrorController _errorController;
        [SetUp]
        public void Init()
        {
            var errors = new List<ErrorLog>
                {
                    new ErrorLog {Application = "CATS", Host = "http://123.234.214",Message="Message1",ErrorLogID=Guid.NewGuid(),Sequence=12,Source="",PartitionID=1,StatusCode=404,TimeUtc=DateTime.Now,Type="",User="Admin"},
                    new ErrorLog {Application = "CATS", Host = "http://123.234.214",Message="Message2",ErrorLogID=Guid.NewGuid(),Sequence=11,Source="",PartitionID=1,StatusCode=500,TimeUtc=DateTime.Now,Type="",User="User"},
                    new ErrorLog {Application = "HUBS", Host = "http://111.234.214",Message="Message3",ErrorLogID=Guid.NewGuid(),Sequence=14,Source="",PartitionID=1,StatusCode=500,TimeUtc=DateTime.Now,Type="",User="Admin"},
                    new ErrorLog {Application = "HUBS", Host = "http://111.234.214",Message="Message4",ErrorLogID=Guid.NewGuid(),Sequence=10,Source="",PartitionID=1,StatusCode=404,TimeUtc=DateTime.Now,Type="",User="Admin"},
                };
            var errorService = new Mock<IErrorLogService>();
            errorService.Setup(t => t.GetAllErrorLog()).Returns(errors);
            _errorController = new ErrorController(errorService.Object);

        }

        [TearDown]
        public void Dispose()
        {
            _errorController.Dispose();
        }

        #endregion

        #region Tests

        [Test]
        public void CanViewErrors()
        {
            //ACT
            var result = _errorController.ViewErrors();
            var model = ((ViewResult)result).Model;
            //Assert

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<IEnumerable<Unit>>(model);
            Assert.AreEqual(2, ((IEnumerable<Unit>)model).Count());
        }

        [Test]
        public void CanViewNotFound()
        {
            //ACT
            var result = _errorController.NotFound("/none_existing/item");
            var model = ((ViewResult)result).Model;
            //Assert

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<ErrorLog>(model);
           
        }

        

        #endregion
    }
}
