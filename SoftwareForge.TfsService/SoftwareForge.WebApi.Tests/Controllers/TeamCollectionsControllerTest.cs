using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftwareForge.Common.Models;
using SoftwareForge.WebApi.Controllers;

namespace SoftwareForge.WebApi.Tests.Controllers
{
    [TestClass]
    public class TeamCollectionsControllerTest
    {
        [TestMethod]
        public void GetTeamCollections()
        {
            // Anordnen
            TeamCollectionsController controller = new TeamCollectionsController();
            
            List<TeamCollection> collections = controller.GetTeamCollections();
            Assert.IsNotNull(collections);
            Assert.AreNotEqual(0, collections.Count, "Expected one or more teamcollections, but found none.");

            foreach (TeamCollection teamCollection in collections)
            {
                Assert.AreNotEqual(new Guid(), teamCollection.Guid);
                Assert.IsFalse(String.IsNullOrEmpty(teamCollection.Name));

                foreach (var project in teamCollection.Projects)
                {
                    Assert.AreNotEqual(new Guid(), project.Guid);
                    Assert.IsFalse(String.IsNullOrEmpty(project.Name));
                    Assert.IsTrue(project.Id > 0);
                }
            }
        }

        [TestMethod]
        public void GetTeamCollection()
        {
            // Anordnen
            TeamCollectionsController controller = new TeamCollectionsController();

            List<TeamCollection> result = controller.GetTeamCollections();
            Assert.IsNotNull(result);
            Assert.AreNotEqual(0, result.Count, "Expected one or more teamcollections, but found none.");

            foreach (TeamCollection teamCollection in result)
            {
                Assert.AreNotEqual(new Guid(), teamCollection.Guid);
                Assert.IsFalse(String.IsNullOrEmpty(teamCollection.Name));

                TeamCollection collection = controller.GetTeamCollection(teamCollection.Guid);
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
