using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareForge.Common.Models
{
    /// <summary>
    /// A Tfs Project model.
    /// </summary>
    public class Project
    {
        /// <summary>
        /// The Name of the Project
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The id of the Project
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Guid of the Project
        /// </summary>
        public Guid Guid { get; set; }
    }
}
