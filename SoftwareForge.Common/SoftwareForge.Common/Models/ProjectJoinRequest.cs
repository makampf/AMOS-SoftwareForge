using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SoftwareForge.Common.Models
{
    public class ProjectJoinRequest
    {
        /// <summary>
        /// The project
        /// </summary>
        [Key, ForeignKey("Project"), Column(Order = 0)]
        public Guid ProjectGuid { get; set; }
        public virtual Project Project { get; set; }


        /// <summary>
        /// A user in this project
        /// </summary>
        [Key, ForeignKey("User"), Column(Order = 1)]
        public int UserId { get; set; }
        public virtual User User { get; set; }


        /// <summary>
        /// The requested role of the user for this project
        /// </summary>
        public int UserRoleValue { get; set; }
        public UserRole UserRole
        {
            get { return (UserRole)UserRoleValue; }
            set { UserRoleValue = (int)value; }
        }

        public string Message { get; set; }
    }
}
