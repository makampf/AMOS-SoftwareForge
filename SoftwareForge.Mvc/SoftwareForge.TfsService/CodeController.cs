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
using System.IO.Compression;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Microsoft.TeamFoundation.VersionControl.Client;
using SoftwareForge.Common.Models;
using SoftwareForge.DbService;
using Project = SoftwareForge.Common.Models.Project;


namespace SoftwareForge.TfsService
{
    /// <summary>
    /// The CodeController. Has the logic to control the workspace code in tfs.
    /// </summary>
    public class CodeController : AbstractTfsController
    {
        /// <summary>
        /// Constructor of the tfsController.
        /// </summary>
        /// <param name="tfsUri">The uri of the tfs</param>
        /// <param name="connectionString">The connection String to the mssql-server holding the ProjectCollections</param>
        public CodeController(Uri tfsUri, String connectionString)
            : base(tfsUri, connectionString)
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
                TfsConfigurationServer.GetTeamProjectCollection(project.TeamCollectionGuid).
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
                TfsConfigurationServer.GetTeamProjectCollection(project.TeamCollectionGuid).
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
        /// <returns>the file content as string</returns>
        public string DownloadFile(Guid teamProjectGuid, string serverPath)
        {
            Project project = ProjectsDao.Get(teamProjectGuid);
            VersionControlServer versionControlServer =
                TfsConfigurationServer.GetTeamProjectCollection(project.TeamCollectionGuid).
                                        GetService<VersionControlServer>();

            Item item = versionControlServer.GetItem(serverPath);

            String content = null;
            if (item.ItemType == ItemType.File)
            {
                using (Stream stream = item.DownloadFile())
                {
                    TextReader reader = new StreamReader(stream);
                    content = reader.ReadToEnd();
                }
            }

            return content;
        }


        /// <summary>
        /// Download code of a project as a zip file
        /// </summary>
        /// <param name="projectGuid">the guid of the project</param>
        /// <param name="serverPath">the selected branch</param>
        /// <returns>content of zip file as byte array</returns>
        public byte[] DownloadCode(Guid projectGuid, string serverPath)
        {
            Project project = ProjectsDao.Get(projectGuid);

            VersionControlServer versionControlServer = TfsConfigurationServer.GetTeamProjectCollection(project.TeamCollectionGuid).GetService<VersionControlServer>();

            String guid = Guid.NewGuid().ToString();
            string localPath = Properties.Settings.Default.LocalCacheFolder + "\\" + guid + "\\";

            Workspace workspace = versionControlServer.CreateWorkspace(guid);
            WorkingFolder workfolder = new WorkingFolder(serverPath, localPath);
            workspace.CreateMapping(workfolder);
            workspace.Get();
           

            String archiveName = Properties.Settings.Default.LocalCacheFolder + "\\" + guid + ".zip";

            //TODO: Ugly :)
            Thread.Sleep(3000);
            int createZipTries = 0;
            while (createZipTries < 30)
            {
                try
                {
                    ZipFile.CreateFromDirectory(localPath, archiveName);
                    createZipTries = 30;
                }
                catch (Exception)
                {
                    Thread.Sleep(500);
                    createZipTries++;
                }
            }


            byte[] fileBytes = File.ReadAllBytes(archiveName);

            workspace.DeleteMapping(workfolder);
            workspace.Delete();

            DirectoryInfo directory = new DirectoryInfo(localPath);
            directory.Delete(true);
            File.Delete(archiveName);

            return fileBytes;
        }


        /// <summary>
        /// Fork code from old project to new project
        /// </summary>
        /// <param name="oldProjectGuid">the guid of the old project</param>
        /// <param name="newProjectGuid">the guid of the new project</param>
        public void ForkCode(Guid oldProjectGuid, Guid newProjectGuid)
        {
            Project oldProject = ProjectsDao.Get(oldProjectGuid);
            Project newProject = ProjectsDao.Get(newProjectGuid);

            String sourceProjectPath = "$/" + oldProject.TfsName;
            VersionControlServer oldVersionControlServer = TfsConfigurationServer.GetTeamProjectCollection(oldProject.TeamCollectionGuid).GetService<VersionControlServer>();
            
            String guid = Guid.NewGuid().ToString();
            string localPath = Properties.Settings.Default.LocalCacheFolder + "\\" + guid + "\\";


            Workspace oldWorkspace = oldVersionControlServer.CreateWorkspace(guid);
            WorkingFolder workfolder = new WorkingFolder(sourceProjectPath, localPath);
            oldWorkspace.CreateMapping(workfolder);
            oldWorkspace.Get();
            oldWorkspace.DeleteMapping(workfolder);
            oldWorkspace.Delete();

            String newGuid = Guid.NewGuid().ToString();
            string newLocalPath = Properties.Settings.Default.LocalCacheFolder + "\\" + newGuid + "\\";
            VersionControlServer newVersionControlServer = TfsConfigurationServer.GetTeamProjectCollection(newProject.TeamCollectionGuid).GetService<VersionControlServer>();
            String destinationProjectPath = "$/" + newProject.TfsName;
            Workspace newWorkSpace = newVersionControlServer.CreateWorkspace(newGuid);
            WorkingFolder newWorkfolder = new WorkingFolder(destinationProjectPath, newLocalPath);
            newWorkSpace.CreateMapping(newWorkfolder);


            Directory.Move(localPath, newLocalPath);

            PendingChange[] pendingChanges = newWorkSpace.GetPendingChanges();
            //ItemSpec[] itemSpecs = new[] { new ItemSpec(localPath, RecursionType.Full) };
            //PendingSet[] pendingSets = newWorkSpace.VersionControlServer.QueryPendingSets(itemSpecs, newWorkSpace.Name, System.Security.Principal.WindowsIdentity.GetCurrent().Name);
            //PendingChange[] pendingChanges = pendingSets.SelectMany(pendingSet => pendingSet.PendingChanges).ToArray();
            newWorkSpace.CheckIn(pendingChanges, null);
                
                
            newWorkSpace.DeleteMapping(newWorkfolder);
            newWorkSpace.Delete();

        }
    }
}