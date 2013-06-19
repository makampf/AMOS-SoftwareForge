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
using System.Linq;
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
            Assert.IsNotNull(_tfsController);
            Assert.IsTrue(_tfsController.HasAuthenticated);
        }

        [TestMethod]
        public void TestRemoveProject()
        {
            _tfsController.RemoveTeamCollection(new Guid(""));
        }

        /// <summary>
        /// Test the GetTeamCollections method.
        /// </summary>
        [TestMethod]
        public void TestGetTeamCollections()
        {
            List<TeamCollection> collections = _tfsController.GetTeamCollections();
            Assert.IsNotNull(collections);

            foreach (var teamCollection in collections)
            {
                Assert.AreNotEqual(new Guid(), teamCollection.Guid);
                Assert.IsFalse(String.IsNullOrEmpty(teamCollection.Name));
                Assert.IsNotNull(teamCollection.Projects);

                foreach (var project in teamCollection.Projects)
                {
                    Assert.AreNotEqual(new Guid(), project.Guid);
                    Assert.IsFalse(String.IsNullOrEmpty(project.TfsName));
                    Assert.IsNotNull(project.Name);
                    Assert.IsTrue(project.Id > 0);
                    Assert.IsNotNull(project.ProjectType);
                    Assert.AreEqual((int)project.ProjectType, project.ProjectTypeValue);
                }
            }
        }


        /// <summary>
        /// Test the GetTeamProjectsOfTeamCollection method.
        /// </summary>
        [TestMethod, ExpectedException(typeof(Exception))]
        public void TestGetProjectsOfTeamCollectionGuidWithInvalidGuid()
        {
            _tfsController.GetTeamProjectsOfTeamCollection(new Guid());
        }

        /// <summary>
        /// Test the GetTeamProjectsOfTeamCollection method.
        /// </summary>
        [TestMethod]
        public void TestGetTeamProjectsOfTeamCollection()
        {
            List<TeamCollection> collections = _tfsController.GetTeamCollections();
            Assert.IsNotNull(collections);

            foreach (var teamCollection in collections)
            {
                Assert.AreNotEqual(new Guid(), teamCollection.Guid);
                Assert.IsFalse(String.IsNullOrEmpty(teamCollection.Name));

                List<Project> list = _tfsController.GetTeamProjectsOfTeamCollection(teamCollection.Guid);
                Assert.IsNotNull(list);

                foreach (var project in list)
                {
                    Assert.AreNotEqual(new Guid(), project.Guid);
                    Assert.IsFalse(String.IsNullOrEmpty(project.TfsName));
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
            Assert.IsNotNull(collections);

            foreach (var teamCollection in collections)
            {
                Assert.AreNotEqual(new Guid(), teamCollection.Guid);
                Assert.IsFalse(String.IsNullOrEmpty(teamCollection.Name));

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
                    Assert.AreEqual(teamCollection.Projects[i].TfsName, collection.Projects[i].TfsName);
                    Assert.AreEqual(teamCollection.Projects[i].ProjectType, collection.Projects[i].ProjectType);
                    Assert.AreEqual(teamCollection.Projects[i].ProjectTypeValue, collection.Projects[i].ProjectTypeValue);
                    Assert.AreEqual(teamCollection.Projects[i].TeamCollectionGuid, collection.Projects[i].TeamCollectionGuid);

                    Assert.IsTrue(teamCollection.Projects[i].Users.SequenceEqual(collection.Projects[i].Users));
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
            Assert.IsNotNull(collections);

            TeamCollection teamCollection = _tfsController.CreateTeamCollection(Properties.Settings.Default.TestCollectionName);
            Assert.IsNotNull(teamCollection);
            Assert.IsFalse(String.IsNullOrEmpty(teamCollection.Name));
            Assert.AreNotEqual(new Guid(), teamCollection.Guid);
            Assert.IsNotNull(teamCollection.Projects);
            Assert.AreEqual(0, teamCollection.Projects.Count);

            int expectedCollectionsCount = collections.Count + 1;
            collections = _tfsController.GetTeamCollections();
            Assert.AreEqual(expectedCollectionsCount, collections.Count);
            
            _tfsController.RemoveTeamCollection(teamCollection.Guid);
          
            expectedCollectionsCount = expectedCollectionsCount - 1;
            collections = _tfsController.GetTeamCollections();
            Assert.AreEqual(expectedCollectionsCount, collections.Count);
        }


        /// <summary>
        /// Test the GetTemplatesInCollection method.
        /// </summary>
        [TestMethod]
        public void TestGetTemplatesInCollection()
        {
            List<TeamCollection> collections = _tfsController.GetTeamCollections();
            
            foreach (var teamCollection in collections)
            {
                List<String> results = _tfsController.GetTemplatesInCollection(teamCollection.Guid);
                Assert.IsNotNull(results);
            }
        }


        /// <summary>
        /// Test the CreateTeamProjectInTeamCollection method.
        /// </summary>
        [TestMethod]
        public void TestCreateTeamProjectInTeamCollection()
        {
            List<TeamCollection> collections = _tfsController.GetTeamCollections();
            Assert.IsNotNull(collections);

            String testCollectionName = Properties.Settings.Default.TestCollectionName;
            String testProjectName = Properties.Settings.Default.TestProjectName;

            TeamCollection teamCollection;
            if (collections.Any(a => a.Name == testCollectionName))
                teamCollection = collections.Find(a => a.Name == testCollectionName);
            else
                teamCollection = _tfsController.CreateTeamCollection(testCollectionName);


            Assert.IsNotNull(teamCollection);
            Assert.IsFalse(String.IsNullOrEmpty(teamCollection.Name));
            Assert.AreNotEqual(new Guid(), teamCollection.Guid);
            Assert.IsNotNull(teamCollection.Projects);

            List<String> templates = _tfsController.GetTemplatesInCollection(teamCollection.Guid);
            Assert.IsNotNull(templates);
            Assert.IsTrue(templates.Count > 0);
            _tfsController.CreateTeamProjectInTeamCollection(teamCollection.Guid, testProjectName, testProjectName, "Description", ProjectType.Application, templates[0]);
            
            _tfsController.RemoveTeamCollection(teamCollection.Guid);
        }


        //ToDo: Better Testing
        [TestMethod]
        public void TestGetTfsProjectUserList()
        {
            List<ProjectUser> list = _tfsController.GetTfsProjectUserList();
            Assert.IsNotNull(list);
        }

        

    }
}
