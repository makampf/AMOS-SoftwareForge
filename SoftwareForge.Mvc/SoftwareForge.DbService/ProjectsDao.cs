/*
 * Copyright (c) 2013 by Denis Bach, Konstantin Tsysin, Taner Tunc, Marvin Kampf, Florian Wittmann
 *
 * This file is part of the Software Forge Overlay application.
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
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using SoftwareForge.Common.Models;
using SoftwareForge.Common.Models.Requests;

namespace SoftwareForge.DbService
{
    /// <summary>
    /// The project Dao class.
    /// </summary>
    public class ProjectsDao
    {
        /// <summary>
        /// lock project adding/getting
        /// </summary>
        private static object _projectLock = new object();


        /// <summary>
        /// The DatabaseContext
        /// </summary>
        private static readonly SoftwareForgeDbContext SoftwareForgeDbContext = new SoftwareForgeDbContext();
       


        /// <summary>
        /// Add a project
        /// </summary>
        /// <param name="project">Project to add</param>
        public static Project Add(Project project)
        {
            lock (_projectLock)
            {
                return AddImpl(project);
            }
        }


        private static Project AddImpl(Project project)
        {
            if (SoftwareForgeDbContext.Projects.Any(t => t.Guid == project.Guid))
            {
                throw new DataException("A project with the same guid is already in database.");
            }

            SoftwareForgeDbContext.Projects.Add(project);
            SoftwareForgeDbContext.SaveChanges();

            return project;
        }

        /// <summary>
        /// Rename a project in the database
        /// </summary>
        /// <param name="projectGuid">The GUID of the project</param>
        /// <param name="newName">The new name of the project</param>
        /// <returns>True if succesful, otherwise false</returns>
        public static void RenameProject(Guid projectGuid, String newName)
        {
            Project project = SoftwareForgeDbContext.Projects.SingleOrDefault(t => t.Guid == projectGuid);
            if (project == null)
                throw new Exception("RenameProject: Could not find the project with GUID: " + projectGuid);

            project.Name = newName;
            SoftwareForgeDbContext.SaveChanges();
        }

        /// <summary>
        /// Process a membership request. Joining or leaving of projects
        /// </summary>
        /// <param name="requestModel">The request model</param>
        public static void ProcessMembershipRequest(ProjectMembershipRequestModel requestModel)
        {
            Project project = SoftwareForgeDbContext.Projects.Single(t => t.Guid == requestModel.ProjectGuid);
            if (project == null)
                throw new Exception("ProcessMembershipRequest: Could not find the project with GUID: " + requestModel.ProjectGuid);

            User user = SoftwareForgeDbContext.Users.SingleOrDefault(t => t.Username == requestModel.Username);
            UserRole role = requestModel.UserRole;
            if (user == null)
            {
                user = new User { Username = requestModel.Username };
                SoftwareForgeDbContext.Users.Add(user);
            }

            try
            {
                ProjectUser projectUser = SoftwareForgeDbContext.ProjectUsers.Single(t => t.ProjectGuid == project.Guid && t.UserId == user.Id);
                SoftwareForgeDbContext.ProjectUsers.Remove(projectUser);
            }
            catch
            {
                SoftwareForgeDbContext.ProjectUsers.Add(new ProjectUser
                {
                    Project = project,
                    ProjectGuid = project.Guid,
                    User = user,
                    UserId = user.Id,
                    UserRole = role
                });
            }

            SoftwareForgeDbContext.SaveChanges();
        }


        /// <summary>
        /// Get a project by guid or create given one
        /// </summary>
        /// <param name="guid">guid of project</param>
        /// <param name="projectToAdd">project to add if new</param>
        /// <returns>A project or null if no project is found</returns>
        public static Project GetOrAdd(Guid guid, Project projectToAdd)
        {
            lock (_projectLock)
            {
                Project project = Get(guid);
                if (project != null)
                {
                    return project;
                }
                return AddImpl(projectToAdd); 
            }


        }



        /// <summary>
        /// Get a project by guid
        /// </summary>
        /// <param name="guid">guid of project</param>
        /// <returns>A project or null if no project is found</returns>
        public static Project Get(Guid guid) 
        {
            try
            {
                Project project = SoftwareForgeDbContext.Projects.Single(t => t.Guid == guid);
                project.Users = GetUsers(project.Guid);
                return project;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the role of a user in a project
        /// </summary>
        /// <param name="projectGuid">The guid of the project</param>
        /// <param name="username">The username</param>
        /// <returns>The UserRole of a user in a project</returns>
        public static UserRole GetMembershipRoleOfUserInProject(Guid projectGuid, String username)
        {
            ProjectUser singleOrDefault = SoftwareForgeDbContext.ProjectUsers.SingleOrDefault(t => (t.ProjectGuid == projectGuid && t.User.Username == username));
            if (singleOrDefault == null)
                return UserRole.Reader;

            return singleOrDefault.UserRole;
        }


        /// <summary>
        /// Get the users of a project
        /// </summary>
        /// <param name="guid">guid of the project</param>
        /// <returns>A collection of users in the specific project</returns>
        public static Collection<ProjectMember> GetUsers(Guid guid)
        {
            Collection<ProjectMember> users = new Collection<ProjectMember>();
            IQueryable<ProjectUser> x = SoftwareForgeDbContext.ProjectUsers.Where(t => t.Project.Guid == guid);
            foreach (ProjectUser projectUser in x)
            {
                users.Add(new ProjectMember{User =projectUser.User, UserRole =  projectUser.UserRole});
            }
            return users;
        }


        public static void JoinProject(ProjectMembershipRequestModel requestModel)
        {
            Project project = SoftwareForgeDbContext.Projects.Single(t => t.Guid == requestModel.ProjectGuid);
            if (project == null)
                throw new Exception("JoinProject: Could not find the project with GUID: " + requestModel.ProjectGuid);

            User user = SoftwareForgeDbContext.Users.SingleOrDefault(t => t.Username == requestModel.Username);
            if (user == null)
                throw new Exception("JoinProject: Could not find user:" + requestModel.Username);

            ProjectUser projectUser = SoftwareForgeDbContext.ProjectUsers.SingleOrDefault(t => t.ProjectGuid == project.Guid && t.UserId == user.Id);
            if (projectUser != null)
            {
                SoftwareForgeDbContext.ProjectUsers.Remove(projectUser);
            }

            //After leaving you still watch the project as a reader
            SoftwareForgeDbContext.ProjectUsers.Add(new ProjectUser
            {
                Project = project,
                ProjectGuid = project.Guid,
                User = user,
                UserId = user.Id,
                UserRole = requestModel.UserRole
            });

            SoftwareForgeDbContext.SaveChanges();
        }

        public static void LeaveProject(ProjectMembershipRequestModel requestModel)
        {
            Project project = SoftwareForgeDbContext.Projects.Single(t => t.Guid == requestModel.ProjectGuid);
            if (project == null)
                throw new Exception("LeaveProject: Could not find the project with GUID: " + requestModel.ProjectGuid);

            User user = SoftwareForgeDbContext.Users.SingleOrDefault(t => t.Username == requestModel.Username);
            if (user == null)
                throw new Exception("LeaveProject: Could not find user:" + requestModel.Username);

            ProjectUser projectUser = SoftwareForgeDbContext.ProjectUsers.SingleOrDefault(t => t.ProjectGuid == project.Guid && t.UserId == user.Id);
            if (projectUser == null)
                throw new Exception("LeaveProject: Could not find ProjectUser with projectGuid " + project.Guid + " and UserId " + user.Id);

            Collection<ProjectMember> users = GetUsers(project.Guid);

            if (requestModel.UserRole == UserRole.ProjectOwner)
            {
                if (users.Count(u => u.UserRole == UserRole.ProjectOwner) <= 1)
                    throw new Exception("LeaveProject: You can't leave the project, because you are the only project owner!");
            }

            SoftwareForgeDbContext.ProjectUsers.Remove(projectUser);

            //After leaving you still watch the project as a reader
            SoftwareForgeDbContext.ProjectUsers.Add(new ProjectUser
            {
                Project = project,
                ProjectGuid = project.Guid,
                User = user,
                UserId = user.Id,
                UserRole = UserRole.Reader
            });
            
            
            SoftwareForgeDbContext.SaveChanges();

        }
    }
}
