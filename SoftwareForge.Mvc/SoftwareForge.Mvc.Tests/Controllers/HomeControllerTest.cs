using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftwareForge.Mvc;
using SoftwareForge.Mvc.Controllers;

namespace SoftwareForge.Mvc.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {

        private HomeController _controller;

        /// <summary>
        /// Init the HomeController.
        /// </summary>
        [TestInitialize]
        public void TestInit()
        {
            _controller = new HomeController();
        }


        [TestMethod]
        public void Index()
        {
            ViewResult result = _controller.Index() as ViewResult;
            Assert.IsNotNull(result);
        }
    }
}
