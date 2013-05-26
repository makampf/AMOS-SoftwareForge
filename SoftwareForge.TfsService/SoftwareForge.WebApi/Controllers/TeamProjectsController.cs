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
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using SoftwareForge.Common.Models;
using SoftwareForge.Common.Models.Requests;
using SoftwareForge.DbService;
using SoftwareForge.TfsService;

namespace SoftwareForge.WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class TeamProjectsController : ApiController
    {
        //Lazy initialization
        private readonly Lazy<TfsController> _tfsController = new Lazy<TfsController>(CreateTfsController);

        TfsController TfsController
        {
            get { return _tfsController.Value; }
        }
        /// <summary>
        /// Function which creates a new TfsController
        /// </summary>
        /// <returns> new TfsController</returns>
        private static TfsController CreateTfsController()
        {
            return new TfsController(new Uri(Properties.Settings.Default.TfsServerUri), Properties.Settings.Default.DbConnectionString);
        }



        #region GET
        /// <summary>
        /// Get a project by guid
        /// </summary>
        /// <param name="guid">the guid of the project</param>
        /// <returns>A project</returns>
        [HttpGet]
        public Project GetTeamProject(Guid guid)
        {
            return ProjectsDao.Get(guid);
        }
        #endregion



        
        #region POST
        /// <summary>
        /// Create a new project
        /// </summary>
        /// <param name="project">project to create</param>
        /// <returns>the newly create project</returns>
        [HttpPost]
        public Project CreateProject([FromBody] Project project)
        {
            List<String> templates = TfsController.GetTemplatesInCollection(project.TeamCollectionGuid);
            if (templates.Count < 1)
                throw new ArgumentException("The project given is in a collection that has no templates! ");

            WindowsIdentity identity = (WindowsIdentity)HttpContext.Current.User.Identity;
            
            Project createdProject = TfsController.CreateTeamProjectInTeamCollection(project.TeamCollectionGuid, project.Name, project.TfsName, project.Description, project.ProjectType, templates[0]);
            ProjectsDao.ProcessMembershipRequest(new ProjectMembershipRequestModel { ProjectGuid = createdProject.Guid, Username = identity.Name, UserRole = UserRole.ProjectOwner });
            return createdProject;
        }
        #endregion



        #region PUT
        /// <summary>
        /// Renames a project
        /// </summary>
        /// <param name="rpm">rename project model</param>
        /// <returns>true if succesful, false in error case</returns>
        [HttpPut]
        public bool RenameProject([FromBody] RenameProjectModel rpm)
        {
            WindowsIdentity identity = (WindowsIdentity)HttpContext.Current.User.Identity;
            if (ProjectsDao.GetMembershipRoleOfUserInProject(rpm.Guid, identity.Name) != UserRole.ProjectOwner)
                throw new Exception("Only a project owner can rename a project!");

            return ProjectsDao.RenameProject(rpm.Guid, rpm.NewName);
        }
        #endregion
        


        //#region DELETE

        //#endregion
    }
}