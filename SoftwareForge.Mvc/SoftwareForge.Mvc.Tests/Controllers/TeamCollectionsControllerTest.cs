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
using System.Collections.Specialized;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftwareForge.Mvc.Controllers;

namespace SoftwareForge.Mvc.Tests.Controllers
{
    [TestClass]
    public class TeamCollectionControllerTest
    {
        private TeamCollectionsController _controller;

        /// <summary>
        /// Init the TeamCollectionController.
        /// </summary>
        [TestInitialize]
        public void TestInit()
        {
            _controller = new TeamCollectionsController();
        }

        /// <summary>
        /// Test the CreateTeamCollection() method.
        /// </summary>
        [TestMethod]
        public void CreateTeamCollectionTest()
        {
            //Without parameters
            ActionResult result = _controller.CreateTeamCollection();
            Assert.IsNotNull(result);
            
            //With Parameters
            const string key = "Name";
            const string value = "testCollection";
            NameValueCollection nameValuecollection = new NameValueCollection();
            nameValuecollection.Add(key, value);
            FormCollection formCollection = new FormCollection(nameValuecollection);
            result = _controller.CreateTeamCollection(formCollection);
            Assert.IsNotNull(result);

            //TODO: remove testCollection
        }
    }
}
