using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftwareForge.Common.Models;

namespace SoftwareForge.TfsService.UnitTests
{
    /// <summary>
    /// Test the TfsController
    /// </summary>
    [TestClass]
    public class TfsControllerUtc
    {
        private TfsController _tfsController;

        /// <summary>
        /// Init the connection to tfs
        /// </summary>
        [TestInitialize]
        public void TestInit()
        {
            _tfsController = new TfsController(new Uri("http://localhost:8080/tfs"));
        }

        /// <summary>
        /// Test if init was successful
        /// </summary>
        [TestMethod]
        public void TestInitTfsConnection()
        {
            Assert.IsNotNull(_tfsController);
            Assert.IsTrue(_tfsController.HasAuthenticated);
        }

        /// <summary>
        /// Test the GetTeamCollections method.
        /// </summary>
        [TestMethod]
        public void TestGetTeamCollections()
        {
            List<TeamCollection> collections = _tfsController.GetTeamCollections();
            Assert.AreNotEqual(0, collections.Count,"Expected one or more teamcollections, but found none.");
        }
    }
}
