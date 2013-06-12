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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
                new SelectListItem { Value = ProjectType.Nonsoftware.ToString(), Text = ProjectType.Nonsoftware.ToString() }
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
            }

            return RedirectToAction("Index","Home");
        }

        /// <summary>
        /// Show the details page for a project
        /// </summary>
        /// <param name="guid">The guid of the project</param>
        /// <returns>The details page view</returns>
        public ActionResult ProjectDetailsPage(Guid guid)
        {
            Project project = TeamCollectionsClient.GetTeamProject(guid);
            return View(project);
        }

        /// <summary>
        /// Watch a project
        /// </summary>
        /// <param name="guid">The guid of project</param>
        /// <param name="username">The username</param>
        /// <returns>Redirects to overview page</returns>
        public ActionResult WatchProject(Guid guid, String username)
        {
            TeamCollectionsClient.WatchProject(guid, username);
            return RedirectToAction("ProjectDetailsPage",new {guid});

        }

        /// <summary>
        /// Unwatch a project
        /// </summary>
        /// <param name="guid">guid of project</param>
        /// <param name="username">username</param>
        /// <returns>Redirects to overview page</returns>
        public ActionResult UnwatchProject(Guid guid, String username)
        {
            TeamCollectionsClient.UnwatchProject(guid, username);
            return RedirectToAction("ProjectDetailsPage", new { guid });
        }


        /// <summary>
        /// Join a project
        /// </summary>
        /// <param name="guid">The guid of project</param>
        /// <returns>The JoinRequest to the CreateProjectJoinRequest View.</returns>
        public ActionResult CreateProjectJoinRequest(Guid guid)
        {
            ViewData["UserRoles"] = new List<SelectListItem>{ 
                new SelectListItem {Text = UserRole.Contributor.ToString(), Value = ((int) UserRole.Contributor).ToString(CultureInfo.InvariantCulture)}, 
                new SelectListItem {Text = UserRole.ProjectOwner.ToString(), Value = ((int) UserRole.ProjectOwner).ToString(CultureInfo.InvariantCulture)}, 
   
            };

            return View("CreateProjectJoinRequest", new ProjectJoinRequest { ProjectGuid = guid});
        }

        /// <summary>
        /// Creates a new InvitationRequest
        /// </summary>
        /// <param name="guid"> ProjectGuid</param>
        /// <returns>The Request to the CreateProjectInvitationRequest View. </returns>
        public ActionResult CreateInvitationRequest(Guid guid)
        {
            {
                ViewData["UserRoles"] = new List<SelectListItem>
                    {
                        new SelectListItem{Text = UserRole.Contributor.ToString(), Value = ((int) UserRole.Contributor).ToString(CultureInfo.InvariantCulture)},
                        new SelectListItem{Text = UserRole.ProjectOwner.ToString(),Value = ((int) UserRole.ProjectOwner).ToString(CultureInfo.InvariantCulture)},

                    };
                ProjectInvitationRequest request = new ProjectInvitationRequest {ProjectGuid = guid};
                //request.Project = TeamCollectionsClient.GetTeamProject(guid);
                return View("CreateProjectInvitationRequest", request);
            }
        }

        /// <summary>
        /// Called after project joining
        /// </summary>
        /// <param name="project">project to create</param>
        /// <returns>redirects to overview page</returns>
        [HttpPostAttribute]
        [ValidateAntiForgeryTokenAttribute]
        public ActionResult PostProjectJoinRequest(ProjectJoinRequest project)
        {
            if (ModelState.IsValid)
            {
                project.User = new User { Username = User.Identity.Name };
                TeamCollectionsClient.CreateJoinProjectRequest(project);
            }

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Shows the page, where user can rename a project
        /// </summary>
        /// <param name="guid">The guid of the project</param>
        /// <returns>The rename page view</returns>
        public ActionResult RenameProjectPage(Guid guid)
        {
            Project project = TeamCollectionsClient.GetTeamProject(guid);
            return View(project);
        }

        /// <summary>
        /// Renames the project
        /// </summary>
        /// <param name="project"></param>
        /// <returns>redirection to projectdetailspage</returns>
        [HttpPostAttribute]
        [ValidateAntiForgeryTokenAttribute]
        public ActionResult RenameProject(Project project)
        {
            if (ModelState.IsValid)
            {
                TeamCollectionsClient.RenameProject(project.Guid, project.Name);
            }
            return RedirectToAction("ProjectDetailsPage", new { project.Guid });
        }

        /// <summary>
        /// Leave a project
        /// </summary>
        /// <param name="projectGuid">ProjectGuid which is wanted to be leaved</param>
        /// <param name="username">Username who wants to leave a project</param>
        /// <param name="role">Which role the user wants to get rid of</param>
        /// <returns>redirection to projectdetailspage</returns>
        public ActionResult LeaveProject(Guid projectGuid, string username, UserRole role)
        {
            TeamCollectionsClient.LeaveProject(projectGuid, username, role);
            return RedirectToAction("ProjectDetailsPage", new { guid = projectGuid });
        }

        /// <summary>
        /// Posts the InvitationRequest
        /// </summary>
        /// <param name="invitation"></param>
        /// <returns>Returns to home view</returns>
        public ActionResult PostProjectInvitationRequest(ProjectInvitationRequest invitation)
        {
            if (ModelState.IsValid)
            {
                String userName = invitation.User.Username;
                User user = TeamCollectionsClient.GetUserByName(userName);
                if (user == null)
                    throw new Exception("Can't find user " + userName);


                if (invitation.UserRole == UserRole.ProjectOwner)
                {
                    Project project = TeamCollectionsClient.GetTeamProject(invitation.ProjectGuid);
                    IEnumerable<ProjectMember> projectMembers =
                        project.Users.Where(u => u.UserRole == UserRole.ProjectOwner);

                    //if (projectMembers.Any(projectMember => projectMember.User.Username == userName))
                    //{
                    //    throw new Exception("The user " + userName + " is already project owner in project " + project.Name);
                    //}
                }
                else
                {
                    Project project = TeamCollectionsClient.GetTeamProject(invitation.ProjectGuid);
                    IEnumerable<ProjectMember> projectMembers =
                        project.Users.Where(u => (u.UserRole == UserRole.ProjectOwner) || (u.UserRole == UserRole.Contributor));

                    //if (projectMembers.Any(projectMember => projectMember.User.Username == userName))
                    //{
                    //    throw new Exception("The user " + userName + " is already contributor or project owner in project " + project.Name);
                    //}
                }


                TeamCollectionsClient.CreateProjectInvitationRequest(invitation);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}