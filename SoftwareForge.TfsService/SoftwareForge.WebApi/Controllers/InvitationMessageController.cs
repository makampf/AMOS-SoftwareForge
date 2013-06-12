using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SoftwareForge.Common.Models;
using SoftwareForge.Common.Models.Requests;
using SoftwareForge.DbService;

namespace SoftwareForge.WebApi.Controllers
{
    public class InvitationMessageController : ApiController
    {
        #region POST
        [HttpPost]
        public bool Post([FromBody] ProjectInvitationMessageModel model)
        {
            ProjectMembershipRequestModel requestModel = new ProjectMembershipRequestModel
            {
                Username = model.ProjectInvitationRequest.User.Username,
                UserRole = model.ProjectInvitationRequest.UserRole,
                ProjectGuid = model.ProjectInvitationRequest.ProjectGuid
            };

            ProjectsDao.JoinProject(requestModel);

            ProjectMembershipDao.RemoveProjectInvitationRequest(model.ProjectInvitationRequest);

            //send message to all project owners
            MessageDao.AddMessageForAllProjectOwner(model.Message, model.ProjectInvitationRequest.ProjectGuid);

            return true;
        }
        #endregion


        #region DELETE
        [HttpDelete]
        public bool Delete([FromBody] ProjectInvitationMessageModel model)
        {
            //send message to all project owners
            MessageDao.AddMessageForAllProjectOwner(model.Message, model.ProjectInvitationRequest.ProjectGuid);

            //remove invitation request
            ProjectMembershipDao.RemoveProjectInvitationRequest(model.ProjectInvitationRequest);

            return true;
        }
        #endregion
    }
}
