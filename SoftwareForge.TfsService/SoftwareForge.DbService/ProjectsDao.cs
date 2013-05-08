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

        private readonly SoftwareForgeDbContext _softwareForgeDbContext = new SoftwareForgeDbContext();

        /// <summary>
        /// Add a project
        /// </summary>
        /// <param name="project"></param>
        public void Add(Project project)
        {
            if (_softwareForgeDbContext.Projects.Any(t => t.Guid == project.Guid))
            {
                //TODO: Error, adding a project second time.
                return;
            }
            _softwareForgeDbContext.Projects.Add(project);
            _softwareForgeDbContext.SaveChanges();
        }

        public void ProcessMembershipRequest(ProjectMembershipRequestModel requestModel)
        {
            Project project = _softwareForgeDbContext.Projects.Single(t => t.Guid == requestModel.ProjectGuid);
            User user = _softwareForgeDbContext.Users.SingleOrDefault(t => t.Username == requestModel.Username);
            if (user == null)
            {
                user = new User {Username = requestModel.Username};
                _softwareForgeDbContext.Users.Add(user);
            }
            if (project.Users.Contains(user))
            {
                project.Users.Remove(user);
            }
            else
            {
                project.Users.Add(user);
            }
            _softwareForgeDbContext.SaveChanges();

        }
    }
}
