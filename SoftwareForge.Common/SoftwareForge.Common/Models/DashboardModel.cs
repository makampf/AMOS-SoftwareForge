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

using System.Collections.Generic;

namespace SoftwareForge.Common.Models
{
    /// <summary>
    /// The model for the dashboard view
    /// </summary>
    public class DashboardModel
    {
        /// <summary>
        /// A list of random projects, the user is not yet member of.
        /// </summary>
        public List<Project> RandomProjects { get; set; }

        /// <summary>
        /// A list of all projects, the user is member of.
        /// </summary>
        public List<Project> MyProjects { get; set; }

        /// <summary>
        /// A list of Messages, the user will be informed
        /// </summary>
        public List<Message> Messages { get; set; }

        /// <summary>
        /// A list of Project Join Requests, the user gets
        /// </summary>
        public List<ProjectJoinRequest> Requests { get; set; }

        /// <summary>
        /// A list of Project Invitation Requests of the user
        /// </summary>
        public List<ProjectInvitationRequest> InvitationRequests { get; set; } 

    }
}
