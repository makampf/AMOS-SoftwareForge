﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareForge.Common.Models.Requests
{
    public class JoinProjectRequestModel
    {
        public Guid ProjectGuid { get; set; }
        public String Username { get; set; }
    }
}