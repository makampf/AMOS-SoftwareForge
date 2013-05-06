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
using System.Net.Http;
using System.Web.Mvc;
using SoftwareForge.Mvc.WebApiClient;

namespace SoftwareForge.Mvc.Controllers
{
    /// <summary>
    /// Controller for Teamcollections
    /// </summary>
    public class TeamCollectionsController : Controller
    {
        /// <summary>
        /// GET: /CreateTeamCollection.
        /// </summary>
        /// <returns>Teamcollection view for the browser</returns>
        public ActionResult CreateTeamCollection()
        {
            return View();
        }
        /// <summary>
        /// POST: /CreateTeamCollection with a message body.
        /// </summary>
        /// <param name="collection">Message body <example> name1=value1&name2=value2 </example></param>
        /// <returns>Either view with new collection, returns to the start page or view where 
        /// the user have to type the name of the colection. </returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateTeamCollection(FormCollection collection)
        {
            try
            {

                ValueProviderResult name = collection.GetValue("Name");
                if (name == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                try
                {
                    TeamCollectionsClient.CreateTeamCollection(name.AttemptedValue);
                }
                catch (HttpRequestException)
                {
                    //TODO error
                    throw;
                }
        
                return RedirectToAction("Index","Home");
            }
            catch
            {
                return View();
            }
        }
    }
}
