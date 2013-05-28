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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace SoftwareForge.Common.Models
{
    public class ProjectJoinRequest
    {
        /// <summary>
        /// The project
        /// </summary>
        [Key, ForeignKey("Project"), Column(Order = 0)]
        public Guid ProjectGuid { get; set; }

        /// <summary>
        /// A user in this project
        /// </summary>
        [Key, ForeignKey("User"), Column(Order = 1)]
        public int UserId { get; set; }

        /// <summary>
        /// The requested role of the user for this project
        /// </summary>
        [DataMember]
        public int UserRoleValue { get; set; }
        public UserRole UserRole
        {
            get { return (UserRole)UserRoleValue; }
            set { UserRoleValue = (int)value; }
        }

        [DataMember]
        public string Message { get; set; }
    }
}
