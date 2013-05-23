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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using SoftwareForge.Common.Models;
using SoftwareForge.Common.Models.Requests;

namespace SoftwareForge.DbService
{
    /// <summary>
    /// The project dao class.
    /// </summary>
    public class ProjectsDao
    {
        /// <summary>
        /// The DatabaseContext
        /// </summary>
        private readonly SoftwareForgeDbContext _softwareForgeDbContext;


        /// <summary>
        /// Create a new ProjectDao.
        /// </summary>
        public ProjectsDao()
        {
            _softwareForgeDbContext =  new SoftwareForgeDbContext();
        }


        /// <summary>
        /// Add a project
        /// </summary>
        /// <param name="project">project to add</param>
        public Project Add(Project project)
        {
            if (_softwareForgeDbContext.Projects.Any(t => t.Guid == project.Guid))
            {
                throw new DataException("A project with the same guid is already in database.");
            }
            _softwareForgeDbContext.Projects.Add(project);
            _softwareForgeDbContext.SaveChanges();
            return project;
        }

        /// <summary>
        /// Process a membership request. Joining or leaving of projects
        /// </summary>
        /// <param name="requestModel">the request model</param>
        public void ProcessMembershipRequest(ProjectMembershipRequestModel requestModel)
        {
            Project project = _softwareForgeDbContext.Projects.Single(t => t.Guid == requestModel.ProjectGuid);
            User user = _softwareForgeDbContext.Users.SingleOrDefault(t => t.Username == requestModel.Username);
            UserRole role = requestModel.UserRole;
            if (user == null)
            {
                user = new User {Username = requestModel.Username};
                _softwareForgeDbContext.Users.Add(user);
            }

            try
            {
                ProjectUser projectUser = _softwareForgeDbContext.ProjectUsers.Single(t => t.Project_Guid == project.Guid && t.User_Id == user.Id);
                _softwareForgeDbContext.ProjectUsers.Remove(projectUser);
            }
            catch
            {
                _softwareForgeDbContext.ProjectUsers.Add(new ProjectUser
                    {
                        Project = project,
                        Project_Guid = project.Guid,
                        User = user,
                        User_Id = user.Id,
                        UserRole = role
                    });

            }
            _softwareForgeDbContext.SaveChanges();

        }

        /// <summary>
        /// Get a project by guid
        /// </summary>
        /// <param name="guid">guid of project</param>
        /// <returns>A project or null if no project is found</returns>
        public Project Get(Guid guid) 
        {
            try
            {
                Project project = _softwareForgeDbContext.Projects.Single(t => t.Guid == guid);
                project.Users = GetUsers(project.Guid);
                return project;
            }
            catch (Exception)
            {
                return null;
            }
           
        }

        /// <summary>
        /// Get the users of a project
        /// </summary>
        /// <param name="guid">guid of the project</param>
        /// <returns>a collection of users in the specific project</returns>
        public ICollection<User> GetUsers(Guid guid)
        {
            Collection<User> users = new Collection<User>();
            IQueryable<ProjectUser> x = _softwareForgeDbContext.ProjectUsers.Where(t => t.Project.Guid == guid);
            foreach (ProjectUser projectUser in x)
            {
                users.Add(projectUser.User);
            }
            return users;
        }
    }
}
