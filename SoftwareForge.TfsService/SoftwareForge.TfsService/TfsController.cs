/*
 * Copyright (c) 2013 by Denis Bach, Konstantin Tsysin, Taner Tunc, Marvin Kampf, Florian Wittmann
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
using System.Collections.ObjectModel;
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
        private readonly TfsConfigurationServer _tfsServer;
        private readonly TfsDbController _tfsDbController;


        /// <summary>
        /// Bool if the tfs has authenticated.
        /// </summary>
        public bool HasAuthenticated { get { return _tfsServer.HasAuthenticated; } }


        /// <summary>
        /// Create a new tfsController.
        /// </summary>
        /// <param name="tfsUri">The uri of the tfs</param>
        /// <param name="connectionString">The connection String to the mssql-server holding the ProjectCollections</param>
        public TfsController(Uri tfsUri, String connectionString)
        {
            _tfsServer = TfsConfigurationServerFactory.GetConfigurationServer(tfsUri);
            _tfsServer.Authenticate();

            _tfsDbController = new TfsDbController(connectionString);
        }

       

        /// <summary>
        /// Get all TeamCollections.
        /// </summary>
        /// <returns>a list of TeamCollections</returns>
        public List<TeamCollection> GetTeamCollections()
        {
            if (HasAuthenticated == false)
                _tfsServer.Authenticate();

            List<TeamCollection> teamCollectionsList = new List<TeamCollection>();


            // Get the catalog of team project collections
            ReadOnlyCollection<CatalogNode> tpcNodes = _tfsServer.CatalogNode.QueryChildren(
                new[] { CatalogResourceTypes.ProjectCollection },
                false, CatalogQueryOptions.None);

            foreach (CatalogNode catalogNode in tpcNodes)
            {
                Guid guid = new Guid(catalogNode.Resource.Properties["InstanceId"]);
                String name = catalogNode.Resource.DisplayName;
                //List<Project> projects = GetProjectsOfCollectionNode(catalogNode);
                List<Project> projects = GetProjectsOfTeamCollectionGuid(guid);

                TeamCollection teamCol = new TeamCollection { Guid = guid, Name = name, Projects = projects };
                teamCollectionsList.Add(teamCol);
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
                _tfsServer.Authenticate();

            TfsTeamProjectCollection tpc = _tfsServer.GetTeamProjectCollection(teamCollectionId);
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
                _tfsServer.Authenticate();

            ITeamProjectCollectionService tpcService = _tfsServer.GetService<ITeamProjectCollectionService>();

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
                _tfsServer.Authenticate();

            ITeamProjectCollectionService tpcService = _tfsServer.GetService<ITeamProjectCollectionService>();
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
                _tfsServer.Authenticate();

            ITeamProjectCollectionService tpcService = _tfsServer.GetService<ITeamProjectCollectionService>();

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
