using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SoftwareForge.Common.Models;
using SoftwareForge.DbService;

namespace SoftwareForge.WebApi.Controllers
{
    public class InvitationMessageController : ApiController
    {
        [HttpDelete]
        public bool Delete([FromBody] ProjectInvitationMessageModel model)
        {
            //send message to all project owners
            MessageDao.AddMessageForAllProjectOwner(model.Message, model.ProjectInvitationRequest.ProjectGuid);

            //remove invitation request
            ProjectMembershipDao.RemoveProjectInvitationRequest(model.ProjectInvitationRequest);

            return true;
        }
    }
}
