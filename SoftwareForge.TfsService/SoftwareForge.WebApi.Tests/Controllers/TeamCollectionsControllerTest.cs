/*
 * Copyright (c) 2013 by Denis Bach, Konstantin Tsysin, Taner Tunc, Marvin Kampf, Florian Wittmann
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
using SoftwareForge.WebApi.Controllers;

namespace SoftwareForge.WebApi.Tests.Controllers
{
    [TestClass]
    public class TeamCollectionsControllerTest
    {

        private TeamCollectionsController _controller;

        /// <summary>
        /// Init the connection to tfs
        /// </summary>
        [TestInitialize]
        public void TestInit()
        {
            _controller = new TeamCollectionsController();
        }


        [TestMethod]
        public void GetTeamCollections()
        {
            List<TeamCollection> collections = _controller.GetTeamCollections();
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
            List<TeamCollection> result = _controller.GetTeamCollections();
            Assert.IsNotNull(result);
            Assert.AreNotEqual(0, result.Count, "Expected one or more teamcollections, but found none.");

            foreach (TeamCollection teamCollection in result)
            {
                Assert.AreNotEqual(new Guid(), teamCollection.Guid);
                Assert.IsFalse(String.IsNullOrEmpty(teamCollection.Name));

                TeamCollection collection = _controller.GetTeamCollection(teamCollection.Guid);
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

            List<TeamCollection> collections = _controller.GetTeamCollections();
            Assert.AreNotEqual(0, collections.Count, "Expected one or more teamcollections, but found none.");


            TeamCollection teamCollection = _controller.CreateTeamCollection("newTestCollection");
            Assert.IsNotNull(teamCollection);
            Assert.IsFalse(String.IsNullOrEmpty(teamCollection.Name));
            Assert.AreNotEqual(new Guid(), teamCollection.Guid);
            Assert.AreEqual(teamCollection.Projects.Count, 0);

            int expectedCollectionsCount = collections.Count + 1;
            collections = _controller.GetTeamCollections();
            Assert.AreEqual(expectedCollectionsCount, collections.Count);

            //Assert.IsTrue(collections.Contains(teamCollection));
            _controller.RemoveTeamCollection(teamCollection.Guid);
            //Assert.IsFalse(collections.Contains(teamCollection));

            expectedCollectionsCount = expectedCollectionsCount - 1;
            collections = _controller.GetTeamCollections();
            Assert.AreEqual(collections.Count, expectedCollectionsCount);
        }


    }
}
