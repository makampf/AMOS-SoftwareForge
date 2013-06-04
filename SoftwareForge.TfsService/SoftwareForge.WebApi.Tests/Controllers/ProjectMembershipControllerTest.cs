﻿/*
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftwareForge.Common.Models;
using SoftwareForge.WebApi.Controllers;

namespace SoftwareForge.WebApi.Tests.Controllers
{
    [TestClass]
    public class ProjectMembershipControllerTest
    {
        private ProjectMembershipController _controller;

        /// <summary>
        /// Init the TeamCollectionsController.
        /// </summary>
        [TestInitialize]
        public void TestInit()
        {
            _controller = new ProjectMembershipController();
            Assert.IsNotNull(_controller);
        }

        /// <summary>
        /// Test the GetTeamCollections method (simple).
        /// </summary>
        [TestMethod]
        public void TestGetProjectOwnerProjects()
        {
            var windowsIdentity = System.Security.Principal.WindowsIdentity.GetCurrent();
            if (windowsIdentity != null)
            {
                var res = _controller.GetProjectOwnerProjects(new User {Username = windowsIdentity.Name});
                foreach (var project in res)
                {
                    Assert.IsNotNull(project);
                }
                Assert.IsNotNull(res);
            }
        }
    }
}