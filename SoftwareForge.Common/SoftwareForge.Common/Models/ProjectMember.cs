using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SoftwareForge.Common.Models
{
    [DataContract]
    public class ProjectMember
    {
        [DataMember]
        public User User { get; set; }

        [DataMember]
        public UserRole UserRole { get; set; }
    }
}
