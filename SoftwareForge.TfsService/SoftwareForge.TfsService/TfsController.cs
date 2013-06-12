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
using Microsoft.TeamFoundation.VersionControl.Client;
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
                Project projectModel = ProjectsDao.Get(new Guid(project.Guid));
                if (projectModel == null)
                {
                    //TODO: Language files - localization - no language specifictext in code!
                    const string noDescriptionAvailable = "No description available.";
                    projectModel = ProjectsDao.Add(new Project(project.Name, noDescriptionAvailable, project.Id, guid, teamCollectionGuid, ProjectType.Application));
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
                throw new ArgumentNullException("collectionName");

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
        /// Creates a TeamProjectCollection
        /// </summary>
        /// <param name="collectionGuid">The TeamProjectCollection Guid in which the project will be created</param>
        /// <param name="projectName">The project name</param>
        /// <param name="projectType">The type of the project</param>
        /// <param name="templateName">The template, which should be used</param>
        /// <param name="tfsProjectName">The tfs project name</param>
        /// <param name="projectDescription">The description of the project</param>
        public Project CreateTeamProjectInTeamCollection(Guid collectionGuid, String projectName, String tfsProjectName, string projectDescription, ProjectType projectType, String templateName)
        {
            if (HasAuthenticated == false)
                _tfsConfigurationServer.Authenticate();

            TeamCollection tc = GetTeamCollection(collectionGuid);
            if (tc == null)
                throw new Exception("Could not found TeamCollection with Guid: " + collectionGuid);

            if (tc.Projects.Count(a => a.TfsName == projectName) != 0)
                throw  new Exception("The Project " + projectName + " in the TeamCollection " + tc.Name + " already exists");

            List<String> templates = GetTemplatesInCollection(collectionGuid);
            if (templates.Contains(templateName) == false)
                throw new Exception("Could not found templateName in collection " + tc.Name);

            PowerToolsUtil.CreateTfsProject(_tfsConfigurationServer.Uri.ToString(), tc.Name, projectName, templateName);

           

            Microsoft.TeamFoundation.WorkItemTracking.Client.Project tfsProject = null;

            //TODO: Ugly :)
            int getProjectTrys = 0;
            while (tfsProject == null && getProjectTrys < 30)
            {
                ForceTfsCacheClean();
                tfsProject = GetTfsProjectByName(projectName, collectionGuid);
                Thread.Sleep(250);
                getProjectTrys++;
            }



            if (tfsProject == null)
            {
                throw new Exception("Error while creating new project. No new project found after creation command.");
            }
            
            return ProjectsDao.Add(new Project
                {
                    Description = projectDescription,
                    ProjectType = projectType,
                    Guid = new Guid(tfsProject.Guid),
                    Id = tfsProject.Id,
                    TfsName = tfsProjectName,
                    Name = projectName,
                    TeamCollectionGuid = collectionGuid
                });
        }


        /// <summary>
        /// Get all reader, contributer and administrator of all projects in all team collections
        /// </summary>
        /// <returns>List with all user, reader and administrators</returns>
        public List<ProjectUser> GetTfsProjectUserList()
        {
            if (HasAuthenticated == false)
                _tfsConfigurationServer.Authenticate();

            List<ProjectUser> result = new List<ProjectUser>();

            List<TeamCollection> list = GetTeamCollections();
            foreach (var tc in list)
            {
                TfsTeamProjectCollection tpc = _tfsConfigurationServer.GetTeamProjectCollection(tc.Guid);
                if (tpc == null)
                    throw new Exception("GetTeamProjectsOfTeamCollection: Could not find TeamCollection with Guid: " + tc.Guid.ToString());

                WorkItemStore store = tpc.GetService<WorkItemStore>();


                foreach (Microsoft.TeamFoundation.WorkItemTracking.Client.Project project in store.Projects)
                {
                    IGroupSecurityService sec = (IGroupSecurityService) tpc.GetService(typeof(IGroupSecurityService));
                    Identity[] appGroups = sec.ListApplicationGroups(project.Uri.AbsoluteUri);

                    foreach (Identity group in appGroups)
                    {
                        //ToDo: Only in German TFS Projektadministratoren!
                        if (group.DisplayName.Equals("Projektadministratoren") || (group.DisplayName.Equals("Readers")) || (group.DisplayName.Equals("Contributors")))
                        {
                            Identity[] groupMembers = sec.ReadIdentities(SearchFactor.Sid, new[] {group.Sid},
                                                                         QueryMembership.Expanded);
                            foreach (Identity member in groupMembers)
                            {
                                if (member.Members != null)
                                {
                                    foreach (string memberSid in member.Members)
                                    {
                                        Identity memberInfo = sec.ReadIdentity(SearchFactor.Sid, memberSid, QueryMembership.Direct);
                                        ProjectUser projectUser = new ProjectUser
                                            {
                                                User = new User { Username = memberInfo.Domain + "\\" + memberInfo.AccountName },
                                                ProjectGuid = new Guid(project.Guid),
                                                Project = new Project {Guid = tc.Guid, TfsName = tc.Name}
                                            };
                                        if (group.DisplayName.Equals("Projektadministratoren"))
                                            projectUser.UserRole = UserRole.ProjectOwner;
                                        else if (group.DisplayName.Equals("Contributors"))
                                            projectUser.UserRole = UserRole.Contributor;
                                        else if (group.DisplayName.Equals("Readers"))
                                            projectUser.UserRole = UserRole.Reader;

                                        if (memberInfo.Domain.StartsWith("vstfs://"))
                                            continue;

                                        result.Add(projectUser);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        public List<string> GetBranches(Guid teamProjectGuid)
        {
            Project project = ProjectsDao.Get(teamProjectGuid);

            String serverPath ="$/" + project.TfsName;
             VersionControlServer versionControlServer = _tfsConfigurationServer.GetTeamProjectCollection(project.TeamCollectionGuid).
                GetService<VersionControlServer>();

             IEnumerable<BranchObject> branchObjects = versionControlServer.QueryRootBranchObjects(RecursionType.Full).
                 Where(t => t.Properties.RootItem.Item.StartsWith(serverPath));

            return branchObjects.Select(branch => branch.Properties.RootItem.Item).ToList();
        }

    }
}
