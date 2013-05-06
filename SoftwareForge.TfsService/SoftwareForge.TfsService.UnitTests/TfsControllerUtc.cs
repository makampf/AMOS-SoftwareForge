/*
 * Copyright (c) 2013 by Denis Bach, Marvin Kampf, Konstantin Tsysin, Taner Tunc, Florian Wittmann
 *
 * This file is part of the Software Forge Overlay rating application.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as
 * published by the Free Software Foundation, either version 3 of the
 * License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public
 * License along with this program. If not, see
 * <http://www.gnu.org/licenses/>.
 */
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
            _tfsController = new TfsController(new Uri(Properties.Settings.Default.TfsTestServerUri), Properties.Settings.Default.DbTestConnectionString);
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


        /// <summary>
        /// Test the CreateTeamCollection method.
        /// </summary>
        [TestMethod]
        public void TestCreateAndRemoveTeamCollection()
        {
            //Hint: If something goes wrong, remove TeamCollection manually:
            //C:\Program Files\Microsoft Team Foundation Server 11.0\Tools\TFSConfig.exe Collection /delete /CollectionName:"newTestCollection"

            List<TeamCollection> collections = _tfsController.GetTeamCollections();
            Assert.AreNotEqual(0, collections.Count, "Expected one or more teamcollections, but found none.");

            
            TeamCollection teamCollection = _tfsController.CreateTeamCollection("newTestCollection");
            Assert.IsNotNull(teamCollection);
            Assert.IsFalse(String.IsNullOrEmpty(teamCollection.Name));
            Assert.AreNotEqual(new Guid(), teamCollection.Guid);
            Assert.AreEqual(teamCollection.Projects.Count, 0);

            int expectedCollectionsCount = collections.Count + 1;
            collections = _tfsController.GetTeamCollections();
            Assert.AreEqual(expectedCollectionsCount, collections.Count);

            //Assert.IsTrue(collections.Contains(teamCollection));
            _tfsController.RemoveTeamCollection(teamCollection.Guid);
            //Assert.IsFalse(collections.Contains(teamCollection));

            expectedCollectionsCount = expectedCollectionsCount - 1;
            collections = _tfsController.GetTeamCollections();
            Assert.AreEqual(collections.Count, expectedCollectionsCount);
        }

    }
}
