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
using System.Linq;
using SoftwareForge.Common.Models;

namespace SoftwareForge.DbService
{
    public class ProjectMembershipDao
    {
        private static readonly SoftwareForgeDbContext SoftwareForgeDbContext = new SoftwareForgeDbContext();

        public static List<Project> GetProjectOwnerProjects(User user)
        {
            List<Project> projects = new List<Project>();
            foreach (ProjectUser projectUser in SoftwareForgeDbContext.ProjectUsers)
            {
               if (projectUser.UserRole == UserRole.ProjectOwner && projectUser.User.Username == user.Username)
                   projects.Add(ProjectsDao.Get(projectUser.ProjectGuid));
            }

            return projects;
        }


        public static void AddProjectInvitationRequest(ProjectInvitationRequest model)
        {
            Project project = SoftwareForgeDbContext.Projects.Single(t => t.Guid == model.ProjectGuid);
            if (project == null)
                throw new Exception("AddProjectInvitationRequest: Could not found the project with GUID: " + model.ProjectGuid);

            User user = SoftwareForgeDbContext.Users.SingleOrDefault(t => t.Username == model.User.Username);
            if (user == null)
                throw new Exception("Can't find username: " + model.User.Username);

            model.User = user;


            SoftwareForgeDbContext.Invitations.Add(model);
            SoftwareForgeDbContext.SaveChanges();
        }


        public static void ProcessProjectJoinRequest(ProjectJoinRequest model)
        {
            Project project = SoftwareForgeDbContext.Projects.Single(t => t.Guid == model.ProjectGuid);
            if (project == null)
                throw new Exception("ProcessProjectJoinRequest: Could not found the project with GUID: " + model.ProjectGuid);

            User user = SoftwareForgeDbContext.Users.SingleOrDefault(t => t.Username == model.User.Username);
            UserRole role = model.UserRole;
            if (user == null)
            {
                user = new User { Username = model.User.Username };
                SoftwareForgeDbContext.Users.Add(user);
            }

            

            try
            {
                //First try to remove old requests 
                ProjectJoinRequest projectUser =
                    SoftwareForgeDbContext.ProjectJoinRequests.Single(
                        t => t.ProjectGuid == project.Guid && t.UserId == user.Id);
                SoftwareForgeDbContext.ProjectJoinRequests.Remove(projectUser);
            }
            catch (Exception)
            {
            }
            SoftwareForgeDbContext.ProjectJoinRequests.Add(new ProjectJoinRequest
                {
                    Project = project,
                    ProjectGuid = project.Guid,
                    User = user,
                    UserId = user.Id,
                    UserRole = role,
                    Message = model.Message
                });

            SoftwareForgeDbContext.SaveChanges();
        }


        public static User GetOrCreateUser(string userName)
        {
            User user = GetUser(userName);
            if (user == null)
            {
                user = new User { Username = userName };
                SoftwareForgeDbContext.Users.Add(user);
                SoftwareForgeDbContext.SaveChanges();

                user = SoftwareForgeDbContext.Users.Single(a => a.Username == userName);
            }
            return user;
        }

        public static User GetUser(String username)
        {
            return SoftwareForgeDbContext.Users.SingleOrDefault(a => a.Username == username);
        }

        public static User GetUserById(int userId)
        {
            return SoftwareForgeDbContext.Users.SingleOrDefault(a => a.Id == userId);
        }

        public static List<ProjectJoinRequest> GetProjectRequestsOfUser(String username)
        {
            List<ProjectJoinRequest> requests = new List<ProjectJoinRequest>();

            User user = GetUser(username);
            //if user does not exist in db, he has no requests
            if (user == null)
                return requests;

            IEnumerable<Project> projects = GetProjectOwnerProjects(user);
            foreach (var project in projects)
            {
                ProjectJoinRequest projectJoinRequest = SoftwareForgeDbContext.ProjectJoinRequests.SingleOrDefault(p => p.ProjectGuid == project.Guid);
                if (projectJoinRequest != null)
                    requests.Add(projectJoinRequest);
            }

            return requests;
        }

        public static ProjectJoinRequest GetProjectRequestById(int requestId)
        {
            ProjectJoinRequest request = SoftwareForgeDbContext.ProjectJoinRequests.SingleOrDefault(r => (r.Id == requestId));
            if (request == null)
                throw new Exception("GetProjectRequestById: Could not find ProjectJoinRequest with id: " + requestId);
            return request;
        }

        public static void RemoveProjectJoinRequest(ProjectJoinRequest projectJoinRequest)
        {
            ProjectJoinRequest req = SoftwareForgeDbContext.ProjectJoinRequests.SingleOrDefault(r => r.Id == projectJoinRequest.Id);
            if (req != null)
            {
                SoftwareForgeDbContext.ProjectJoinRequests.Remove(req);
                SoftwareForgeDbContext.SaveChanges();
            }
        }

        public static void RemoveProjectInvitationRequest(ProjectInvitationRequest projectJoinRequest)
        {
            ProjectInvitationRequest req = SoftwareForgeDbContext.Invitations.SingleOrDefault(r => r.Id == projectJoinRequest.Id);
            if (req != null)
            {
                SoftwareForgeDbContext.Invitations.Remove(req);
                SoftwareForgeDbContext.SaveChanges();
            }
        }

        public static ProjectInvitationRequest GetProjectInvitationRequestById(int requestId)
        {
            ProjectInvitationRequest request = SoftwareForgeDbContext.Invitations.SingleOrDefault(r => (r.Id == requestId));
            if (request == null)
                throw new Exception("GetProjectInvitationRequestById: Could not find ProjectInvitationRequest with id: " + requestId);
            return request;
        }

        public static List<ProjectInvitationRequest> GetProjectInvitationsOfUser(string username)
        {
            try
            {
                User user = GetUser(username);
                return SoftwareForgeDbContext.Invitations.Where(i => i.UserId == user.Id).ToList();
            }
            catch
            {
                return new List<ProjectInvitationRequest>();
            }
         
        }


        
    }
}
