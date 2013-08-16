using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using DRMFSS.BLL.Services;
using DRMFSS.BLL.ViewModels.Report;
using DRMFSS.Web.Controllers.Reports;
using DRMFSS.Web.Models;
using DRMFSS.Web.Reports;
using Moq;
using NUnit.Framework;

namespace DRMFSS.Web.Test
{
    [TestFixture]
    public class ReportControllerTests
    {

        #region SetUp / TearDown

        private ReportsController _reportsController;
        [SetUp]
        public void Init()
        {
            var dispatchService = new Mock<IDispatchService>();
            var receiveService = new Mock<IReceiveService>();
            var userProfileService = new Mock<IUserProfileService>();
            var hubService = new Mock<IHubService>();
            var transactionService = new Mock<ITransactionService>();
            _reportsController = new ReportsController(dispatchService.Object, receiveService.Object,
                                                       userProfileService.Object, hubService.Object,
                                                       transactionService.Object);
        }

        [TearDown]
        public void Dispose()
        { _reportsController.Dispose();}

        #endregion

        #region Tests

        [Test]
        public void ShouldDisplaySIReport()
        {
            //Act
            var result = _reportsController.SIReport("SI-01");
            //Assert
            Assert.IsInstanceOf<SIReportModel>(((ViewResult)result).Model);
        }

        [Test]
        public void ShouldDisplayFreeStock()
        {
            //Act
            var result = _reportsController.FreeStock();

            //Assert
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public void ShoulDisplayFreeStockPartial()
        {
            //Act
            var filter = new FreeStockFilterViewModel();
            var result = _reportsController.FreeStockPartial(filter);
           
            //Assert

            Assert.IsInstanceOf<PartialViewResult>(result);


        }
        [Test]
        public void ShouldDisplayOffloadingReport()
        {
            //Act

            var result = _reportsController.OffloadingReport();

            //Assert
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public void ShouldDisplayOffloadingReportPartial()
        {
            //Act
            var filter=new DispatchesViewModel();
            var result = _reportsController.OffloadingReportPartial(filter);
            
            //Assert

            Assert.IsInstanceOf<PartialViewResult>(result);
        }

        [Test]
        public void ShouldDisplayReceiveReport()
        {
            //Act
            var result = _reportsController.Receive();

            //Assert

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public void ShouldDisplayReceivePartial()
        {
            //Act
            var result = _reportsController.ReceivePartial(new ReceiptsViewModel());
            //Assert
            Assert.IsInstanceOf<PartialViewResult>(result);
        }

        [Test]
        public void ShouldDisplayDistributionReport()
        {
            //Act

            var result = _reportsController.DistributionReport();

            //Assert
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public void ShouldDisplayDistributionReportPartial()
        {
            //Act

            var result = _reportsController.DistributionReportPartial(new DistributionViewModel());

            //Assert
            Assert.IsInstanceOf<PartialViewResult>(result);
        }

        [Test]
        public void ShouldDisplayDonationReport()
        {
            //Act

            var result = _reportsController.DonationReport();

            //Assert
            Assert.IsInstanceOf<ViewResult>(result);
            
        }

        [Test]
        public void ShouldDisplayDonationReportPartial()
        {
            //Act
            var result = _reportsController.DonationReportPartial();

            //Assert

            Assert.IsInstanceOf<PartialViewResult>(result);
        }


        #endregion
    }
}
