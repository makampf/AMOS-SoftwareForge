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
using System.IO;
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
    public class CodeViewController : AbstractTfsController
    {
        /// <summary>
        /// Constructor of the tfsController.
        /// </summary>
        /// <param name="tfsUri">The uri of the tfs</param>
        /// <param name="connectionString">The connection String to the mssql-server holding the ProjectCollections</param>
        public CodeViewController(Uri tfsUri, String connectionString) : base( tfsUri, connectionString)
        {
        }

        /// <summary>
        /// GetAllBranches
        /// </summary>
        /// <param name="teamProjectGuid">the guid of the project</param>
        /// <returns>a list of branches</returns>
        public List<string> GetBranches(Guid teamProjectGuid)
        {
            Project project = ProjectsDao.Get(teamProjectGuid);

            String serverPath = "$/" + project.TfsName;
            VersionControlServer versionControlServer =
                _tfsConfigurationServer.GetTeamProjectCollection(project.TeamCollectionGuid).
                                        GetService<VersionControlServer>();

            IEnumerable<BranchObject> branchObjects = versionControlServer.QueryRootBranchObjects(RecursionType.Full).
                                                                           Where(
                                                                               t =>
                                                                               t.Properties.RootItem.Item.StartsWith(
                                                                                   serverPath));

            List<string> listBranches = branchObjects.Select(branch => branch.Properties.RootItem.Item).ToList();
            //Add projectroot
            listBranches.Add("$/" + project.TfsName);
            return listBranches;
        }

        /// <summary>
        /// Get Files of a path
        /// </summary>
        /// <param name="teamProjectGuid">the guid of the project</param>
        /// <param name="path">the path to query</param>
        /// <returns>a list of branches</returns>
        public List<CompositeItem> GetFiles(Guid teamProjectGuid, string path)
        {
            Project project = ProjectsDao.Get(teamProjectGuid);
            VersionControlServer versionControlServer =
                _tfsConfigurationServer.GetTeamProjectCollection(project.TeamCollectionGuid).
                                        GetService<VersionControlServer>();

            return GetFilesRecursive(versionControlServer, path);
        }

        /// <summary>
        /// Get Files and Folders recursively from tfs
        /// </summary>
        /// <param name="versionControlServer">the version control server</param>
        /// <param name="path">the path to traverse</param>
        /// <returns></returns>
        private List<CompositeItem> GetFilesRecursive(VersionControlServer versionControlServer, string path)
        {
            List<CompositeItem> result = new List<CompositeItem>();

            ItemSet itemSet = versionControlServer.GetItems(path, RecursionType.OneLevel);
            foreach (Item item in itemSet.Items)
            {
                if (item.ServerItem == path)
                    continue;

                if (item.ItemType == ItemType.File)
                {
                    FileItem fileItem = new FileItem(item.ServerItem);
                    result.Add(fileItem);
                }
                else if (item.ItemType == ItemType.Folder)
                {
                    FolderItem folderItem = new FolderItem(item.ServerItem);
                    var subItems = GetFilesRecursive(versionControlServer, item.ServerItem);
                    foreach (var compositeItem in subItems)
                    {
                        folderItem.Add(compositeItem);
                    }
                    result.Add(folderItem);
                }
            }
            return result;
        }


        /// <summary>
        /// Download a codeviewfile
        /// </summary>
        /// <param name="teamProjectGuid">the guid of the project</param>
        /// <param name="serverPath">the serverpath</param>
        /// <returns>a string with the local filename</returns>
        public string DownloadFile(Guid teamProjectGuid, string serverPath)
        {
            string fileName = Path.GetFileName(serverPath);
            string localFileName = "C:\\MVCCache\\" + fileName;
            Project project = ProjectsDao.Get(teamProjectGuid);
            VersionControlServer versionControlServer =
                _tfsConfigurationServer.GetTeamProjectCollection(project.TeamCollectionGuid).
                                        GetService<VersionControlServer>();
            versionControlServer.DownloadFile(serverPath, localFileName);
            return localFileName;
        }
    }


}