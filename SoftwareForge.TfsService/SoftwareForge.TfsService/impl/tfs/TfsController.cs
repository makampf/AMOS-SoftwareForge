using System;
using System.Collections.ObjectModel;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;

namespace SoftwareForge.TfsService.impl.tfs
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
        /// <returns>a ReadOnlyCollection of CatalogNodes</returns>
        public ReadOnlyCollection<CatalogNode> GetTeamCollections()
        {
            // Get the catalog of team project collections
            return _tfsServer.CatalogNode.QueryChildren(
                new[] { CatalogResourceTypes.ProjectCollection },
                false, CatalogQueryOptions.None);

        }

        /// <summary>
        /// Get all Projects of a collection
        /// </summary>
        /// <param name="collectionNode">the Team collection</param>
        /// <returns>a ReadOnlyCollection of CatalogNodes</returns>
        public ReadOnlyCollection<CatalogNode> GetProjectsOfCollection(CatalogNode collectionNode)
        {
             // Get a catalog of team projects for the collection
            return collectionNode.QueryChildren(
                new[] { CatalogResourceTypes.TeamProject },
                false, CatalogQueryOptions.None);
        }
    }
}
