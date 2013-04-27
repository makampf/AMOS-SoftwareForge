using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using SoftwareForge.Common.Models;
using Project = SoftwareForge.Common.Models.Project;


namespace SoftwareForge.TfsService
{
    /// <summary>
    /// The TfsController. Has the logic to control and query the tfs.
    /// </summary>
    public class TfsController
    {
        private readonly TfsConfigurationServer _tfsServer;

        /// <summary>
        /// Bool if the tfs has authenticated.
        /// </summary>
        public bool HasAuthenticated { get { return _tfsServer.HasAuthenticated; } }

        /// <summary>
        /// Create a new tfsController
        /// </summary>
        /// <param name="tfsUri">The uri of the tfs</param>
        public TfsController(Uri tfsUri)
        {
            _tfsServer = TfsConfigurationServerFactory.GetConfigurationServer(tfsUri);
            _tfsServer.Authenticate();
        }

       

        /// <summary>
        /// Get all TeamCollections
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
        /// Get specific TeamCollection
        /// </summary>
        /// <returns>a TeamCollection or null if no suited collection was found</returns>
        public TeamCollection GetTeamCollection(Guid id)
        {
            List<TeamCollection> list = GetTeamCollections();
            return list.Find(c => (c.Guid == id));
        }


        /// <summary>
        /// Get all Projects of a TeamCollection
        /// </summary>
        /// <param name="teamCollectionId">the TeamCollection guid</param>
        /// <returns>a list of of projects</returns>
        public List<Project> GetProjectsOfTeamCollectionGuid(Guid teamCollectionId)
        {
            List<Project> result = new List<Project>();

            TfsTeamProjectCollection tpc = _tfsServer.GetTeamProjectCollection(teamCollectionId);
            WorkItemStore store = tpc.GetService<WorkItemStore>();
            
            ProjectCollection projects = store.Projects;

            foreach (Microsoft.TeamFoundation.WorkItemTracking.Client.Project project in projects)
            {
                result.Add(new Project{Id = project.Id, Name = project.Name, Guid = new Guid(project.Guid)});
            }

            return result;
        }

    }
}
