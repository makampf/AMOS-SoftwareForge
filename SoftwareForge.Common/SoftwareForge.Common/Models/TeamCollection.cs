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

        public List<Project> Projects { get; set; }

    }
}
