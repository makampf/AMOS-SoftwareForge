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

using SoftwareForge.Common.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using SoftwareForge.Mvc.Facade;

namespace SoftwareForge.Mvc.Controllers
{
    /// <summary>
    /// Controller for home view.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// GET: /Home.
        /// </summary>
        /// <returns>Home view with teamcollections.</returns>
        public ActionResult Index()
        {
            IEnumerable<TeamCollection> teamCollections = SoftwareForgeFacade.Client.GetTeamCollections();
            return View(teamCollections);
        }





        
    }
}
