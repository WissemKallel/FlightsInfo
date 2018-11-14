using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlightsInfo.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FlightsInfo.Controllers.Tests
{
    [TestClass()]
    public class HomeControllerTests
    {
        private readonly HomeController sut = new HomeController();

        [TestMethod]
        public void Index()
        {
            // Arrange
            HomeController sut = new HomeController();
            // Act
            ViewResult result = sut.Index() as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void About()
        {
            // Arrange
            HomeController controller = new HomeController();
            // Act
            ViewResult result = controller.About() as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Contact()
        {
            // Arrange
            HomeController controller = new HomeController();
            // Act
            ViewResult result = controller.Contact() as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }
    }
}