using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareForge.Common.Models
{
    public class ProjectInvitationMessageModel
    {
        /// <summary>
        /// The message model.
        /// </summary>
        public Message Message { get; set; }


        /// <summary>
        /// The projectJoinRequest model.
        /// </summary>
        public ProjectInvitationRequest ProjectInvitationRequest { get; set; }
    }
}
