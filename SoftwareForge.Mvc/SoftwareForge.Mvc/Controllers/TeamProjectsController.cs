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
using System.Web.Mvc;
using SoftwareForge.Common.Models;
using SoftwareForge.Mvc.WebApiClient;

namespace SoftwareForge.Mvc.Controllers
{
    public class TeamProjectsController : Controller
    {
        /// <summary>
        /// Create a new project view
        /// </summary>
        /// <param name="teamCollectionGuid">the guid of the parent teamCollection </param>
        /// <returns>A CreateProject View</returns>
        public ActionResult CreateProject(Guid teamCollectionGuid)
        {
            SelectListItem[] projectTypes = new[]
            {
                new SelectListItem { Value = ProjectType.Application.ToString(), Text = ProjectType.Application.ToString() },
                new SelectListItem { Value = ProjectType.Library.ToString(), Text =ProjectType.Library.ToString() },
                new SelectListItem { Value = ProjectType.Nonsoftware.ToString(), Text = ProjectType.Nonsoftware.ToString() },
            };
            ViewData["ProjectTypes"] = projectTypes;
            return View(new Project("", "", 0, new Guid(), teamCollectionGuid, ProjectType.Application));
        }

        /// <summary>
        /// Creates a new project
        /// </summary>
        /// <param name="project">project to create</param>
        /// <returns>redirects to overview page</returns>
        [HttpPostAttribute]
        [ValidateAntiForgeryTokenAttribute]
        public ActionResult Create(Project project)
        {
            if (ModelState.IsValid)
            {
                TeamCollectionsClient.CreateProject(project);
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index","Home");
        }

        /// <summary>
        /// Show the details page for a project
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public ActionResult ProjectDetailsPage(Guid guid)
        {
            Project teamCollections = TeamCollectionsClient.GetTeamProject(guid);
            return View(teamCollections);
        }

        /// <summary>
        /// Join a project
        /// </summary>
        /// <param name="guid">guid of project</param>
        /// <param name="username">username</param>
        /// <returns>Redirects to overview page</returns>
        public ActionResult JoinProject(Guid guid, String username)
        {

            TeamCollectionsClient.JoinProject(guid, username);
            return RedirectToAction("ProjectDetailsPage",new {guid});

        }

        /// <summary>
        /// Leave a project
        /// </summary>
        /// <param name="guid">guid of project</param>
        /// <param name="username">username</param>
        /// <returns>Redirects to overview page</returns>
        public ActionResult LeaveProject(Guid guid, String username)
        {
            TeamCollectionsClient.LeaveProject(guid, username);
            return RedirectToAction("ProjectDetailsPage", new { guid });
        }
    }
}