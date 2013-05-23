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
using System.Threading;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using SoftwareForge.Common.Models;
using SoftwareForge.DbService;
using Project = SoftwareForge.Common.Models.Project;


namespace SoftwareForge.TfsService
{
    /// <summary>
    /// The TfsController. Has the logic to control and query the tfs.
    /// </summary>
    public class TfsController
    {
        private TfsConfigurationServer _tfsConfigurationServer;
        private readonly TfsDbController _tfsDbController;


        /// <summary>
        /// Bool if the tfs has authenticated.
        /// </summary>
        public bool HasAuthenticated { get { return _tfsConfigurationServer.HasAuthenticated; } }


        /// <summary>
        /// Constructor of the tfsController.
        /// </summary>
        /// <param name="tfsUri">The uri of the tfs</param>
        /// <param name="connectionString">The connection String to the mssql-server holding the ProjectCollections</param>
        public TfsController(Uri tfsUri, String connectionString)
        {
            
           _tfsConfigurationServer = new TfsConfigurationServer(tfsUri);
           _tfsConfigurationServer.Authenticate();
            _tfsDbController = new TfsDbController(connectionString);
        }

        /// <summary>
        /// Method to reconnect to the tfs, so the cache will be updated. Sometimes must be called multiple times.
        /// </summary>
        private void ForceTfsCacheClean()
        {
            _tfsConfigurationServer = new TfsConfigurationServer(_tfsConfigurationServer.Uri);
            _tfsConfigurationServer.Authenticate();
        }

        /// <summary>
        /// Get all Templates in the TeamProjectCollection.
        /// </summary>
        /// <returns>a list of Templates</returns>
        public List<String> GetTemplatesInCollection(Guid teamCollectionGuid)
        {
            if (HasAuthenticated == false)
                _tfsConfigurationServer.Authenticate();

            TeamCollection collection = GetTeamCollection(teamCollectionGuid);
            if (collection == null)
                throw new Exception("GetTemplatesInCollection: Could not find TeamCollection with Guid: " + teamCollectionGuid.ToString());

            IProcessTemplates processTemplates = _tfsConfigurationServer.GetTeamProjectCollection(collection.Guid).GetService<IProcessTemplates>();
            TemplateHeader[] templateHeaders = processTemplates.TemplateHeaders();

            List<String> templatesList = new List<String>();
            foreach (TemplateHeader header in templateHeaders)
            {
                templatesList.Add(header.Name);
            }

            return templatesList;
        }

        /// <summary>
        /// Get all TeamCollections.
        /// </summary>
        /// <returns>a list of TeamCollections</returns>
        public List<TeamCollection> GetTeamCollections()
        {

            if (HasAuthenticated == false)
                _tfsConfigurationServer.Authenticate();

            List<TeamCollection> teamCollectionsList = new List<TeamCollection>();


            ITeamProjectCollectionService teamProjectCollectionService = _tfsConfigurationServer.GetService<ITeamProjectCollectionService>();
            IList<TeamProjectCollection> collections = teamProjectCollectionService.GetCollections();

            foreach (TeamProjectCollection collection in collections)
            {
                if (collection.State == TeamFoundationServiceHostStatus.Started)
                {
                    Guid guid = collection.Id;
                    String name = collection.Name;
                    List<Project> projects = GetTeamProjectsOfTeamCollection(guid);
                    TeamCollection teamCol = new TeamCollection { Guid = guid, Name = name, Projects = projects };
                    teamCollectionsList.Add(teamCol);
                }
            }
            
            return teamCollectionsList;
        }


        /// <summary>
        /// Get specific TeamCollection.
        /// </summary>
        /// <returns>a TeamCollection or null if no suited collection was found</returns>
        public TeamCollection GetTeamCollection(Guid teamCollectionGuid)
        {
            List<TeamCollection> list = GetTeamCollections();
            return list.Find(c => (c.Guid == teamCollectionGuid));
        }


        /// <summary>
        /// Get all Projects of a TeamProjectCollection.
        /// </summary>
        /// <param name="teamCollectionGuid">the TeamCollection guid</param>
        /// <returns>a list of of projects</returns>
        public List<Project> GetTeamProjectsOfTeamCollection(Guid teamCollectionGuid)
        {

            ProjectsDao projectsDao = new ProjectsDao();
            List<Project> result = new List<Project>();

            if (HasAuthenticated == false)
                _tfsConfigurationServer.Authenticate();

            TfsTeamProjectCollection tpc = _tfsConfigurationServer.GetTeamProjectCollection(teamCollectionGuid);
            if (tpc == null)
                throw new Exception("GetTeamProjectsOfTeamCollection: Could not find TeamCollection with Guid: " + teamCollectionGuid.ToString());

            WorkItemStore store = tpc.GetService<WorkItemStore>();
            
            ProjectCollection projects = store.Projects;

            foreach (Microsoft.TeamFoundation.WorkItemTracking.Client.Project project in projects)
            {
                Guid guid = new Guid(project.Guid);
                Project projectModel = projectsDao.Get(new Guid(project.Guid));
                if (projectModel == null)
                {
                    //TODO: Language files - localization - no language specifictext in code!
                    const string noDescriptionAvailable = "No description available.";
                    projectModel = projectsDao.Add(new Project(project.Name, noDescriptionAvailable, project.Id, guid, teamCollectionGuid, ProjectType.Application));
                }
                result.Add(projectModel);
            }

            return result;
        }

        /// <summary>
        /// Gets the Project with the projectName in the collection with the teamCollectionGuid.
        /// </summary>
        /// <param name="projectName">the name of the Project</param>
        /// <param name="teamCollectionGuid">the guid of the TeamCollection</param>
        /// <returns>The Project if found, otherwise null</returns>
        private Microsoft.TeamFoundation.WorkItemTracking.Client.Project GetTfsProjectByName(string projectName, Guid teamCollectionGuid)
        {

            if (HasAuthenticated == false)
                _tfsConfigurationServer.Authenticate();

            TfsTeamProjectCollection tpc = _tfsConfigurationServer.GetTeamProjectCollection(teamCollectionGuid);
            WorkItemStore store = tpc.GetService<WorkItemStore>();

            ProjectCollection projects = store.Projects;

            foreach (Microsoft.TeamFoundation.WorkItemTracking.Client.Project project in projects)
            {
                if (project.Name.Equals(projectName)) return project;
            }

            return null;
        }


        /// <summary>
        /// Creates a TeamProjectCollection.
        /// </summary>
        /// <param name="collectionName">the TeamCollection name</param>
        /// <returns>the created TeamCollection</returns>
        public TeamCollection CreateTeamCollection(String collectionName)
        {
            if (HasAuthenticated == false)
                _tfsConfigurationServer.Authenticate();

            if (collectionName == null)
                throw new ArgumentNullException(collectionName);

            List<TeamCollection> teamCollections = GetTeamCollections();
            if (teamCollections.Any(a => a.Name == collectionName))
                throw new Exception("A collaction with name " + collectionName + " already exists!");

            ITeamProjectCollectionService tpcService = _tfsConfigurationServer.GetService<ITeamProjectCollectionService>();

            Dictionary<string, string> servicingTokens = new Dictionary<string, string>
                {
                    {"SharePointAction", "None"},
                    {"ReportingAction", "None"}
                };

           
            ServicingJobDetail tpcJob = tpcService.QueueCreateCollection(
                collectionName,
                "", // Description
                false, // IsDefaultCollection
                string.Format("~/{0}/", collectionName), // Virtual directory
                TeamFoundationServiceHostStatus.Started, // Initial state
                servicingTokens
                );

            TeamProjectCollection tpc = tpcService.WaitForCollectionServicingToComplete(tpcJob);

            TeamCollection result = new TeamCollection
                {
                    Name = tpc.Name,
                    Guid = tpc.Id,
                    Projects = new List<Project>()
                };

            return result;
        }


        
        /// <summary>
        /// Removes the TeamProjectCollection.
        /// </summary>
        /// <param name="collectionId">the Guid of the TeamProjectCollection to remove</param>
        public void RemoveTeamCollection(Guid collectionId)
        {
            if (HasAuthenticated == false)
                _tfsConfigurationServer.Authenticate();

            ITeamProjectCollectionService tpcService = _tfsConfigurationServer.GetService<ITeamProjectCollectionService>();
            TeamProjectCollection collection = tpcService.GetCollection(collectionId);
            
            RemoveTeamCollection(collection);
        }


        /// <summary>
        /// Removes the TeamProjectCollection.
        /// </summary>
        /// <param name="collection">the TeamProjectCollection to remove</param>
        private void RemoveTeamCollection(TeamProjectCollection collection)
        {
            if (HasAuthenticated == false)
                _tfsConfigurationServer.Authenticate();

            ITeamProjectCollectionService tpcService = _tfsConfigurationServer.GetService<ITeamProjectCollectionService>();

            Dictionary<string, string> servicingTokens = new Dictionary<string, string>
                {
                    {"SharePointAction", "None"},
                    {"ReportingAction", "None"}
                };

            String sqlConnectionString;

            ServicingJobDetail tpcJob = tpcService.QueueDetachCollection(
                collection,
                servicingTokens,
                "stop",
                out sqlConnectionString
                );

            tpcService.WaitForCollectionServicingToComplete(tpcJob);

            _tfsDbController.RemoveDatabase(collection.Name);
            //http://msdn.microsoft.com/en-us/library/vstudio/dd312130.aspx
            //http://msdn.microsoft.com/de-de/library/ms177419.aspx

           
        }

        /// <summary>
        /// Creates a TeamProjectCollection.
        /// </summary>
        /// <param name="teamCollectionGuid">the TeamProjectCollection Guid in which the project will be created</param>
        /// <param name="projectName">the TeamProject name</param>
        /// <param name="projectType">the type of the project</param>
        /// <param name="templateName">the Template, which should be used</param>
        /// <param name="projectDescription">the description of the project</param>
        public Project CreateTeamProjectInTeamCollection(Guid teamCollectionGuid, String projectName, string projectDescription, ProjectType projectType, String templateName)
        {
            if (HasAuthenticated == false)
                _tfsConfigurationServer.Authenticate();

            TeamCollection tc = GetTeamCollection(teamCollectionGuid);
            if (tc == null)
                throw new Exception("Could not found TeamCollection with Guid: " + teamCollectionGuid);

            if (tc.Projects.Count(a => a.Name == projectName) != 0)
                throw  new Exception("The Project " + projectName + " in the TeamCollection " + tc.Name + " already exists");

            List<String> templates = GetTemplatesInCollection(teamCollectionGuid);
            if (templates.Contains(templateName) == false)
                throw new Exception("Could not found templateName in collection " + tc.Name);

            PowerToolsUtil.CreateTfsProject(_tfsConfigurationServer.Uri.ToString(), tc.Name, projectName, templateName);

           

            Microsoft.TeamFoundation.WorkItemTracking.Client.Project tfsProject = null;

            //TODO: Ugly :)
            int getProjectTrys = 0;
            while (tfsProject == null && getProjectTrys < 30)
            {
                ForceTfsCacheClean();
                tfsProject = GetTfsProjectByName(projectName, teamCollectionGuid);
                Thread.Sleep(250);
                getProjectTrys++;
            }



            if (tfsProject == null)
            {
                throw new Exception("Error while creating new project. No new project found after creation command.");
            }
            ProjectsDao projectsDao = new ProjectsDao();
            return projectsDao.Add(new Project
                {
                    Description = projectDescription,
                    ProjectType = projectType,
                    Guid = new Guid(tfsProject.Guid),
                    Id = tfsProject.Id,
                    Name = projectName,
                    TeamCollectionGuid = teamCollectionGuid
                });
        }
    }
}
