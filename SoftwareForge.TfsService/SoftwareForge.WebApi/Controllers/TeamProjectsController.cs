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
using System.Web.Http;
using SoftwareForge.Common.Models;
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
        /// Get all projects of a TeamCollection
        /// </summary>
        /// <param name="teamCollectionGuid">guid of teamCollection</param>
        /// <returns>A list of projects</returns>
        [HttpGet]
        public List<Project> GetTeamProjectsOfTeamCollection(Guid teamCollectionGuid)
        {
            return TfsController.GetTeamProjectsOfTeamCollection(teamCollectionGuid);
        }
        #endregion


        /// <summary>
        /// Create a new project
        /// </summary>
        /// <param name="project">project to create</param>
        /// <returns>the newly create project</returns>
        #region POST
        [HttpPost]
        public Project CreateProject([FromBody] Project project)
        {
            List<String> templates = TfsController.GetTemplatesInCollection(project.TeamCollectionGuid);
            if (templates.Count< 1) 
                throw new ArgumentException("The project given is in a collection that has no templates! ");
            TfsController.CreateTeamProjectInTeamCollection(project.TeamCollectionGuid, project.Name, project.Description, templates[0]);
            return project;
        }
        #endregion



        //#region PUT

        //#endregion



        //#region DELETE

        //#endregion
    }
}