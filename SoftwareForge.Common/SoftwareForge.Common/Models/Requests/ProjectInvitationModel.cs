using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareForge.Common.Models.Requests
{
    class ProjectInvitationModel
    {
        /// <summary>
        /// the projectguid
        /// </summary>
        public Guid ProjectGuid { get; set; }


        /// <summary>
        /// The username.
        /// </summary>
        public String Username { get; set; }


        /// <summary>
        /// The userrole.
        /// </summary>
        public UserRole UserRole { get; set; }
    }
}
