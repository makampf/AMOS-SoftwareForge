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
using System.Linq;
using System.IO;
using System.Net.Mime;
using SoftwareForge.Common.Models;
using SoftwareForge.Common.Models.Requests;
using SoftwareForge.DbService;
using SoftwareForge.TfsService;
using Project = SoftwareForge.Common.Models.Project;

namespace SoftwareForge.Mvc.Facade
{
    /// <summary>
    /// This class provides methodes to create and list TeamCollections via Browser. 
    /// </summary>
    public class SoftwareForgeFacade
    {
        
        public static SoftwareForgeFacade Client
        {
            get { return _facade ?? (_facade = new SoftwareForgeFacade()); }
        }
        private static SoftwareForgeFacade _facade;

        private readonly ProjectsController _projectsController;
        ProjectsController ProjectsController
        {
            get { return _projectsController; }
        }


        private readonly CodeViewController _codeViewController;
        CodeViewController CodeViewController
        {
            get { return _codeViewController; }
        }

        private readonly BugController _bugController;
        BugController BugController
        {
            get { return _bugController; }
        }


        public SoftwareForgeFacade()
        {
            _projectsController = new ProjectsController(new Uri(Properties.Settings.Default.TfsServerUri), Properties.Settings.Default.DbConnectionString);
            _codeViewController = new CodeViewController(new Uri(Properties.Settings.Default.TfsServerUri), Properties.Settings.Default.DbConnectionString);
            _bugController = new BugController(new Uri(Properties.Settings.Default.TfsServerUri), Properties.Settings.Default.DbConnectionString);
        }
   
        /// <summary>
        /// Shows already created Team Collections.
        /// </summary>
        /// <returns>all Team Collections</returns>
        public IEnumerable<TeamCollection> GetTeamCollections()
        {
            List<TeamCollection> teamCollections =  ProjectsController.GetTeamCollections();
            foreach (TeamCollection teamCollection in teamCollections)
            {
                teamCollection.Projects = ProjectsController.GetTeamProjectsOfTeamCollection(teamCollection.Guid);
            }
            return teamCollections;
        }

        /// <summary>
        /// Shows already created Team Collections.
        /// </summary>
        /// <returns>all Team Collections</returns>
        public IEnumerable<TeamCollection> GetTeamCollections(string searchFilter)
        {
            if (searchFilter == null)
            {
                searchFilter = "";
            }
            List<TeamCollection> teamCollections = ProjectsController.GetTeamCollections();
            List<TeamCollection> filteredTeamCollections = new List<TeamCollection>();
            foreach (TeamCollection teamCollection in teamCollections)
            {
                teamCollection.Projects = ProjectsController.GetTeamProjectsOfTeamCollection(teamCollection.Guid);
                if (!String.IsNullOrWhiteSpace(searchFilter))
                {
                    teamCollection.Projects = teamCollection.Projects.Where(t => t.Name.ToLower().Contains(searchFilter.ToLower()) ||
                        t.Guid.ToString().Equals(searchFilter) || t.TfsName.ToLower().Contains(searchFilter.ToLower())).ToList();
                }
                if (teamCollection.Projects.Any() || teamCollection.Name.ToLower().Contains(searchFilter.ToLower()))
                {
                    filteredTeamCollections.Add(teamCollection);
                }
            }
            return filteredTeamCollections;
        }

        /// <summary>
        /// Creates a new TeamCollection.
        /// </summary>
        /// <param name="name">Name of the TeamCollection.</param>
        /// <returns>The created Team Collection</returns>
        public TeamCollection CreateTeamCollection(string name)
        {
            return ProjectsController.CreateTeamCollection(name);
        }


        /// <summary>
        /// Creates a watch request, so the user will get reader or will be removed as a reader in the project
        /// </summary>
        /// <param name="projectGuid">The guid of the project, the user watches/unwatches</param>
        /// <param name="username">The username of the user</param>
        /// <returns></returns>
        private void PostWatchRequest(Guid projectGuid, string username)
        {
            ProjectMembershipRequestModel joinProjectRequestModel = new ProjectMembershipRequestModel
                {
                    ProjectGuid = projectGuid,
                    Username = username,
                    UserRole = UserRole.Reader
                };
            ProjectsDao.ProcessMembershipRequest(joinProjectRequestModel);
        }

        public Project GetTeamProject(Guid teamProjectGuid)
        {
            return ProjectsDao.Get(teamProjectGuid);
        }

        public Project CreateProject(Project project, String username)
        {
            List<String> templates = ProjectsController.GetTemplatesInCollection(project.TeamCollectionGuid);
            if (templates.Count < 1)
                throw new ArgumentException("The project given is in a collection that has no templates! ");

            if (string.IsNullOrEmpty(project.TfsName) || (string.IsNullOrEmpty(project.Name)))
            {
                throw new ArgumentException("Tfs Name and Project Name must not be empty");
            }
      
            Project createdProject = ProjectsController.CreateTeamProjectInTeamCollection(project.TeamCollectionGuid, project.Name, project.TfsName, project.Description, project.ProjectType, templates[0]);
            ProjectsDao.ProcessMembershipRequest(new ProjectMembershipRequestModel { ProjectGuid = createdProject.Guid, Username = username, UserRole = UserRole.ProjectOwner });
            return createdProject;
        }

        /// <summary>
        /// Rename a project
        /// </summary>
        /// <param name="guid">The projectGuid to rename</param>
        /// <param name="newName">The new name of the project</param>
        /// <param name="username">The username of the project owner in the project</param>
        /// <returns>True if successful, false in error case</returns>
        public void RenameProject(Guid guid, string newName, string username)
        {
            RenameProjectModel rpm = new RenameProjectModel {Guid = guid, NewName = newName};
            if (ProjectsDao.GetMembershipRoleOfUserInProject(rpm.Guid, username) != UserRole.ProjectOwner)
                throw new Exception("Only a project owner can rename a project!");

            ProjectsDao.RenameProject(rpm.Guid, rpm.NewName);
        }


        /// <summary>
        /// Leave a Project.
        /// </summary>
        /// <param name="projectGuid">The projectGuid of the project to leave</param>
        /// <param name="username">The user that wants to leave</param>
        /// <returns>True if successful, false in error case</returns>
        public void UnwatchProject(Guid projectGuid, string username)
        {
            PostWatchRequest(projectGuid, username);
        }

        /// <summary>
        /// Join a Project.
        /// </summary>
        /// <param name="projectGuid">The projectGuid of the project to join</param>
        /// <param name="username">The user that wants to join</param>
        /// <returns>True if successful, false in error case</returns>
        public void WatchProject(Guid projectGuid, string username)
        {
            PostWatchRequest(projectGuid, username);
        }

        /// <summary>
        /// Creates the request for project joining
        /// </summary>
        /// <param name="projectJoinRequest"></param>
        /// <returns></returns>
        public void CreateJoinProjectRequest(ProjectJoinRequest projectJoinRequest)
        {
            ProjectMembershipDao.ProcessProjectJoinRequest(projectJoinRequest);
        }

        /// <summary>
        /// Creates request for prject invitation
        /// </summary>
        /// <param name="projectInvitationRequest">the Request</param>
        /// <returns></returns>
        public void CreateProjectInvitationRequest(ProjectInvitationRequest projectInvitationRequest)
        {
            ProjectMembershipDao.AddProjectInvitationRequest(projectInvitationRequest);
        }

        /// <summary>
        /// Lists all Project Join Request from a User
        /// </summary>
        /// <param name="username">User who will be shown all his request</param>
        /// <returns>A list with all Requests</returns>
        public List<ProjectJoinRequest> GetProjectJoinRequests(String username)
        {
            return ProjectMembershipDao.GetProjectRequestsOfUser(username);
        }

        
        /// <summary>
        /// Gets the ProjectJoiunRequesty by Id
        /// </summary>
        /// <param name="requestId">RequestId</param>
        /// <returns>the Requests</returns>
        public ProjectJoinRequest GetProjectJoinRequestById(int requestId)
        {
            return ProjectMembershipDao.GetProjectRequestById(requestId);
        }

        /// <summary>
        /// Gets the InvitationRequests by Id
        /// </summary>
        /// <param name="invitationId"></param>
        /// <returns>the Invitationrequests</returns>
        public ProjectInvitationRequest GetInvitationRequestById(int invitationId)
        {
            return ProjectMembershipDao.GetProjectInvitationRequestById(invitationId);
        }

        /// <summary>
        /// Leave a project
        /// </summary>
        /// <param name="projectGuid">Guid of the project</param>
        /// <param name="username">Username of the project leaver</param>
        /// <param name="role">Role of the project leaver</param>
        /// <returns></returns>
        public void LeaveProject(Guid projectGuid, string username, UserRole role)
        {
            ProjectMembershipRequestModel leaveProjectRequestModel = new ProjectMembershipRequestModel
            {
                ProjectGuid = projectGuid,
                Username = username,
                UserRole = role
            };

            ProjectsDao.LeaveProject(leaveProjectRequestModel);
        }

        /// <summary>
        /// Creates the projectjoinmessage
        /// </summary>
        /// <param name="model">Message model</param>
        /// <returns>the model with the message</returns>
        public void CreateMessage(ProjectJoinMessageModel model)
        {
            MessageDao.AddMessage(model.Message);
            ProjectMembershipRequestModel requestModel = new ProjectMembershipRequestModel
                {
                    Username = model.ProjectJoinRequest.User.Username,
                    UserRole = model.ProjectJoinRequest.UserRole,
                    ProjectGuid = model.ProjectJoinRequest.ProjectGuid
                };

            ProjectsDao.JoinProject(requestModel);

            ProjectMembershipDao.RemoveProjectJoinRequest(model.ProjectJoinRequest);

        }

        /// <summary>
        /// Deletes the message 
        /// </summary>
        /// <param name="model">ProjectJoinModel</param>
        /// <returns>the model without the message</returns>
        public void DeleteMessage(ProjectJoinMessageModel model)
        {
            MessageDao.AddMessage(model.Message);
            ProjectMembershipDao.RemoveProjectJoinRequest(model.ProjectJoinRequest);
        }

        /// <summary>
        /// Check if exists a user, if not create the user
        /// </summary>
        /// <param name="userName">Username</param>
        /// <returns>return the user</returns>
        public User GetOrCreateUserByName(string userName)
        {
            return ProjectMembershipDao.GetOrCreateUser(userName);
        }

        /// <summary>
        /// Check if there exists a user
        /// </summary>
        /// <param name="user">Username</param>
        /// <returns>the User or null if no user exists with this username</returns>
        public User GetUserByName(String user)
        {
            return ProjectMembershipDao.GetUser(user);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="username">Username</param>
        /// /// <param name="guid">Projectguid</param>
        /// <returns></returns>
        public UserRole GetUserRoleinProject(Guid guid, string username)
        {
            return ProjectsDao.GetMembershipRoleOfUserInProject(guid, username);
        }

        /// <summary>
        /// Lists all messages of user
        /// </summary>
        /// <param name="userName">Username</param>
        /// <returns>a list of messages</returns>
        public List<Message> GetMessages(string userName)
        {
            return MessageDao.GetMessagesOfUser(ProjectMembershipDao.GetUser(userName));
        }

        /// <summary>
        /// Lists the InvitationRequests of a user
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>a list with all invitation requests</returns>
        public List<ProjectInvitationRequest> GetInvitations(string username)
        {
            return ProjectMembershipDao.GetProjectInvitationsOfUser(username);
        }

        /// <summary>
        /// Deletes the invitation message
        /// </summary>
        /// <param name="model">the message model</param>
        /// <returns></returns>
        public void DeleteInvitationMessage(ProjectInvitationMessageModel model)
        {
            //send message to all project owners
            MessageDao.AddMessageForAllProjectOwner(model.Message, model.ProjectInvitationRequest.ProjectGuid);

            //remove invitation request
            ProjectMembershipDao.RemoveProjectInvitationRequest(model.ProjectInvitationRequest);
        }


        /// <summary>
        /// Creates the Invitation Message 
        /// </summary>
        /// <param name="model"> The message model</param>
        /// <returns>a CreatePos</returns>
        public void CreateInvitationMessage(ProjectInvitationMessageModel model)
        {
            ProjectMembershipRequestModel requestModel = new ProjectMembershipRequestModel
            {
                Username = model.ProjectInvitationRequest.User.Username,
                UserRole = model.ProjectInvitationRequest.UserRole,
                ProjectGuid = model.ProjectInvitationRequest.ProjectGuid
            };

            ProjectsDao.JoinProject(requestModel);

            ProjectMembershipDao.RemoveProjectInvitationRequest(model.ProjectInvitationRequest);

            //send message to all project owners
            MessageDao.AddMessageForAllProjectOwner(model.Message, model.ProjectInvitationRequest.ProjectGuid);
        }

        /// <summary>
        /// Get all branches of a project
        /// </summary>
        /// <param name="teamProjectGuid">the guid of the project</param>
        /// <returns>a list of branches</returns>
        public List<string> GetBranches(Guid teamProjectGuid)
        {
            return CodeViewController.GetBranches(teamProjectGuid);
        }

        /// <summary>
        /// Get all files of a path in a project
        /// </summary>
        /// <param name="teamProjectGuid">the guid of the project</param>
        /// <param name="path">the path in the project</param>
        /// <returns>a list of files</returns>
        public List<CompositeItem> GetFiles(Guid teamProjectGuid, string path)
        {
            return CodeViewController.GetFiles(teamProjectGuid, path);
        }



        /// <summary>
        /// Get a file
        /// </summary>
        /// <param name="serverPath">the serverPath</param>
        /// <param name="teamProjectGuid">the guid of the project</param>
        /// <returns>the content as a array of lines</returns>
        public String[] GetFileContent(string serverPath, Guid teamProjectGuid)
        {
            string content = CodeViewController.DownloadFile(teamProjectGuid, serverPath);
            try
            {
                string fileName = Path.GetFileName(serverPath);
                return new FileTypeReader().GetFilesFromPath(fileName, content);

            }
            catch
            {
                return new[] {"Can not show " + serverPath, "It seems to be a binary file!"};
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public List<WorkItem> GetWorkItems(Guid guid)
        {
            return BugController.GetBugWorkItems(guid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workItem"></param>
        public void CreateBug(WorkItem workItem)
        {
            BugController.CreateBug(workItem.TeamProjectGuid, workItem, new Dictionary<string, string>(), System.Web.HttpContext.Current.User.Identity.Name);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectGuid"></param>
        /// <returns></returns>
        public List<WikiModel> GetEntriesOfProject(Guid projectGuid)
        {
            return WikiDao.GetEntriesForProject(projectGuid);
        }

        public WikiModel GetEntry(int id)
        {
            return WikiDao.GetEntry(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        public void CreateEntry(WikiModel model)
        {
            WikiDao.AddEntry(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="selectedbranch"></param>
        /// <returns></returns>
        public byte[] DownloadCode(Guid guid, string selectedbranch)
        {
            return CodeViewController.DownloadCode(guid, selectedbranch);
        }
    }
}
