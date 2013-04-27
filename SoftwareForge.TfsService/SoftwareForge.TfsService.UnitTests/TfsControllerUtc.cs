using System;
using System.Collections.Generic;
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
            _tfsController = new TfsController(new Uri(Properties.Settings.Default.TfsServerUri));
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

            foreach (var teamCollection in collections)
            {
                Assert.AreNotEqual(new Guid(), teamCollection.Guid);
                Assert.IsFalse(String.IsNullOrEmpty(teamCollection.Name));
            }
        }

        /// <summary>
        /// Test the GetProjectsOfTeamCollectionGuid method.
        /// </summary>
        [TestMethod]
        public void TestGetProjectsOfTeamCollectionGuid()
        {
            List<TeamCollection> collections = _tfsController.GetTeamCollections();
            Assert.AreNotEqual(0, collections.Count, "Expected one or more teamcollections, but found none.");

            foreach (var teamCollection in collections)
            {
                Assert.AreNotEqual(new Guid(), teamCollection.Guid);
                Assert.IsFalse(String.IsNullOrEmpty(teamCollection.Name));

                List<Project> list = _tfsController.GetProjectsOfTeamCollectionGuid(teamCollection.Guid);
                Assert.IsNotNull(list);

                foreach (var project in list)
                {
                    Assert.AreNotEqual(new Guid(), project.Guid);
                    Assert.IsFalse(String.IsNullOrEmpty(project.Name));
                    Assert.IsTrue(project.Id > 0);
                }
            }
        }

        /// <summary>
        /// Test the GetTeamCollection method.
        /// </summary>
        [TestMethod]
        public void TestGetTeamCollection()
        {
            List<TeamCollection> collections = _tfsController.GetTeamCollections();
            Assert.AreNotEqual(0, collections.Count, "Expected one or more teamcollections, but found none.");

            foreach (var teamCollection in collections)
            {
                TeamCollection collection = _tfsController.GetTeamCollection(teamCollection.Guid);
                Assert.AreNotEqual(new Guid(), collection.Guid);
                Assert.IsFalse(String.IsNullOrEmpty(collection.Name));
                Assert.IsNotNull(collection.Projects);
                Assert.AreEqual(teamCollection.Guid, collection.Guid);
                Assert.AreEqual(teamCollection.Name, collection.Name);
                Assert.AreEqual(teamCollection.Projects.Count, collection.Projects.Count);

                for (int i = 0; i < teamCollection.Projects.Count; i++)
                {
                    Assert.AreEqual(teamCollection.Projects[i].Guid, collection.Projects[i].Guid);
                    Assert.AreEqual(teamCollection.Projects[i].Id, collection.Projects[i].Id);
                    Assert.AreEqual(teamCollection.Projects[i].Name, collection.Projects[i].Name);
                }
            }
        }




    }
}
