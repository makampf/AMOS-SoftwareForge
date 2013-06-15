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
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace SoftwareForge.Common.Models
{
    /// <summary>
    /// A Tfs Project model.
    /// </summary>
    [DataContract]
    public class Project
    {
        /// <summary>
        /// The Name of the Project in SoftwareForge.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// The Name of the Project in TFS.
        /// </summary>
        [DataMember]
        public string TfsName { get; set; }

        /// <summary>
        /// The Description of the Project.
        /// </summary>
        [DataType(DataType.MultilineText)]
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// The id of the Project.
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// The Guid of the Project.
        /// </summary>
        [Key]
        [DataMember]
        public Guid Guid { get; set; }

        /// <summary>
        /// The list with all users that are member of this project.
        /// </summary>
        [NotMapped]
        [DataMember]
        public ICollection<ProjectMember> Users { get; set; }

        /// <summary>
        /// The teamCollection it is part of
        /// </summary>
        [DataMember]
        public Guid TeamCollectionGuid { get; set; }

        [DataMember]
        public int ProjectTypeValue { get; set; }

        [DataMember]
        public ProjectType ProjectType
        {
            get { return (ProjectType)ProjectTypeValue; }
            set { ProjectTypeValue = (int) value; }
        }



        public Project() {}

        public Project(string name, string description, int id, Guid guid, Guid teamCollectionGuid, ProjectType projectType)
        {
            Name = name;
            TfsName = name;
            Description = description;
            Id = id;
            Guid = guid;
            Users = new Collection<ProjectMember>();
            TeamCollectionGuid = teamCollectionGuid;
            ProjectType = projectType;
        }
    }
}
