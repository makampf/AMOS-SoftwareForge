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
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
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
        private readonly TfsConfigurationServer _tfsConfigurationServer;
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
            _tfsConfigurationServer = TfsConfigurationServerFactory.GetConfigurationServer(tfsUri);
            _tfsConfigurationServer.Authenticate();

            _tfsDbController = new TfsDbController(connectionString);
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
                    List<Project> projects = GetProjectsOfTeamCollectionGuid(guid);
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
        /// <param name="teamCollectionId">the TeamCollection guid</param>
        /// <returns>a list of of projects</returns>
        public List<Project> GetProjectsOfTeamCollectionGuid(Guid teamCollectionId)
        {
            List<Project> result = new List<Project>();

            if (HasAuthenticated == false)
                _tfsConfigurationServer.Authenticate();

            TfsTeamProjectCollection tpc = _tfsConfigurationServer.GetTeamProjectCollection(teamCollectionId);
            WorkItemStore store = tpc.GetService<WorkItemStore>();
            
            ProjectCollection projects = store.Projects;

            foreach (Microsoft.TeamFoundation.WorkItemTracking.Client.Project project in projects)
            {
                result.Add(new Project{Id = project.Id, Name = project.Name, Guid = new Guid(project.Guid)});
            }

            return result;
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



    }
}
