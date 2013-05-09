using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace SoftwareForge.Common.Models
{
    /// <summary>
    /// A projectUser (mapping projects to users)
    /// </summary>
    public class ProjectUser
    {
        /// <summary>
        /// The project
        /// </summary>
        [Key, ForeignKey("Project"), Column(Order = 0)]
        public Guid Project_Guid { get; set; }
        public virtual Project Project { get; set; }
        /// <summary>
        /// A user in this project
        /// </summary>
        [Key, ForeignKey("User"), Column(Order = 1)]
        public int User_Id { get; set; }
        public virtual User User { get; set; }
    }
}
