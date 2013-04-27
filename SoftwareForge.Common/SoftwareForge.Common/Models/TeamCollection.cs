using System;
using System.Collections.Generic;

namespace SoftwareForge.Common.Models
{
    /// <summary>
    /// A Tfs TeamCollection Model.
    /// </summary>
    public class TeamCollection
    {
        /// <summary>
        /// The Name of the TeamCollection
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Guid of the TeamCollection
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        /// The Projects of the TeamCollection
        /// </summary>
        public List<Project> Projects { get; set; }

    }
}
