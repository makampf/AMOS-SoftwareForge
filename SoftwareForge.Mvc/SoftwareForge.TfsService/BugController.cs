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
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using SoftwareForge.Common.Models;
using SoftwareForge.DbService;
using Project = SoftwareForge.Common.Models.Project;
using ForgeWorkItem = SoftwareForge.Common.Models.WorkItem;
using WorkItem = Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItem;


namespace SoftwareForge.TfsService
{
    public class BugController : AbstractTfsController
    {
        /// <summary>
        /// Constructor of the BugController.
        /// </summary>
        /// <param name="tfsUri">The uri of the tfs</param>
        /// <param name="connectionString">The connection string for the database</param>
        public BugController(Uri tfsUri, String connectionString)
            : base(tfsUri, connectionString)
        {
        }

        public List<ForgeWorkItem> GetBugWorkItems(Guid teamProjectGuid)
        {
            if (HasAuthenticated == false)
                TfsConfigurationServer.Authenticate();

            

            List<ForgeWorkItem> workItems = new List<ForgeWorkItem>();
            Project project = ProjectsDao.Get(teamProjectGuid);
            

            TfsTeamProjectCollection tpc = TfsConfigurationServer.GetTeamProjectCollection(project.TeamCollectionGuid);
            WorkItemStore store = tpc.GetService<WorkItemStore>();
            
            WorkItemCollection workItemCollection = store.Query(
             " SELECT [System.Id], [System.WorkItemType]," +
             " [System.State], [System.AssignedTo], [System.Title], [System.Description] " +
             " FROM WorkItems " +
             " WHERE [System.TeamProject] = '" + project.Name +
            "' ORDER BY [System.WorkItemType], [System.Id]");
            foreach (WorkItem workItem in workItemCollection)
            {
                workItems.Add(new ForgeWorkItem
                    {
                        Id = workItem.Id,
                        Title = workItem.Title,
                        Description = workItem.Description,
                        State = workItem.State
                    });
            }
            return workItems;
        }


        public ForgeWorkItem GetWorkItemById(Guid teamProjectGuid, int id)
        {
            if (HasAuthenticated == false)
                TfsConfigurationServer.Authenticate();

            Project project = ProjectsDao.Get(teamProjectGuid);
            TfsTeamProjectCollection tpc = TfsConfigurationServer.GetTeamProjectCollection(project.TeamCollectionGuid);
            WorkItemStore store = tpc.GetService<WorkItemStore>();
            WorkItem item = store.GetWorkItem(id);
            return new ForgeWorkItem
                {
                    Id = item.Id,
                    Title = item.Title,
                    Description = item.Description,
                    State = item.State
                };
        }


        //type = "Fehler"
        public List<WorkItemField> GetAllFieldsOfType(Guid teamProjectGuid, String type)
        {
            List<WorkItemField> list = new List<WorkItemField>();
            
            Project project = ProjectsDao.Get(teamProjectGuid);

            TfsTeamProjectCollection tpc = TfsConfigurationServer.GetTeamProjectCollection(project.TeamCollectionGuid);
            WorkItemStore store = tpc.GetService<WorkItemStore>();

            Microsoft.TeamFoundation.WorkItemTracking.Client.Project p = store.Projects[project.TfsName];
            if (p == null)
                    throw new Exception("GetAllBugFields: Could not find tfs-project " + project.TfsName);

            WorkItemType wiType = p.WorkItemTypes[type];
            if (wiType == null)
                throw new Exception("GetAllBugFields: Could not find workitemtype " + type);

            foreach (FieldDefinition field in wiType.FieldDefinitions)
            {
                WorkItemField workItemField = new WorkItemField();
                foreach (string value in field.AllowedValues)
                    workItemField.AllowedValues.Add(value);

                workItemField.FieldType = field.FieldType.ToString();
                workItemField.Id = field.Id;
                workItemField.IsCoreField = field.IsCoreField;
                workItemField.Name = field.Name;
                workItemField.IsUserNameField = field.IsUserNameField;
                workItemField.IsEditable = field.IsEditable;
                
                list.Add(workItemField);
            }
            return list;
        }






        public void CreateBug(Guid teamProjectGuid, Common.Models.WorkItem item, Dictionary<String, String> fieldDictionary, String userToImpersonate)
        {
            Project project = ProjectsDao.Get(teamProjectGuid);

            using (ImpersonatedTfsTeamProjectCollection tpc = new ImpersonatedTfsTeamProjectCollection(TfsConfigurationServer, userToImpersonate))
            {
                WorkItemStore store = tpc.Impersonate(project.TeamCollectionGuid).GetService<WorkItemStore>();

                WorkItemType wiType = store.Projects[project.TfsName].WorkItemTypes["Fehler"];

                if (wiType == null)
                    throw new Exception("WorkItemType Bug could not be found");

                WorkItem wi = new WorkItem(wiType) {Title = item.Title, Description = item.Description};


                foreach (KeyValuePair<String, String> kvp in fieldDictionary)
                {
                    wi.Fields[kvp.Key].Value = kvp.Value;
                }


                foreach (Field field in wi.Fields)
                {
                    if (!field.IsValid)
                    {
                        throw new Exception("Invalid Field " + field.Name + ": " + field.Status + Environment.NewLine +
                                            "Current Value: " + field.Value);
                    }
                }

                wi.Save();
            }
        }



    }
}
