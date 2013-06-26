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
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace SoftwareForge.TfsService.UnitTests
{
    /// <summary>
    /// Test the TfsController
    /// </summary>
    [TestClass]
    public class BugControllerUtc
    {
        private BugController _bugController;
        


        /// <summary>
        /// Init the connection to tfs
        /// </summary>
        [TestInitialize]
        public void TestInit()
        {
            _bugController = new BugController(new Uri(Properties.Settings.Default.TfsTestServerUri));
            Assert.IsNotNull(_bugController);
            
        }


        ///// <summary>
        ///// Test the GetWorkItems Method
        ///// </summary>
        //[TestMethod]
        //public void TestGetWorkItemsShouldReturnAllWorkItems()
        //{
        //    //TODO: Ensure a project exists and bugs exist!
        //    List<WorkItem> workItemCollection = _bugController.GetWorkItems(new Guid("ee0fe868-0c18-47f6-9e08-34de09c678a1"));
        //    Assert.IsNotNull(workItemCollection);
        //    Assert.AreNotEqual(0,workItemCollection.Count);
        //}

        

    }
}
