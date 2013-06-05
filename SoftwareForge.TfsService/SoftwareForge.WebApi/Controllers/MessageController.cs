using System.Collections.Generic;
using System.Web.Http;
using SoftwareForge.Common.Models;
using SoftwareForge.Common.Models.Requests;
using SoftwareForge.DbService;

namespace SoftwareForge.WebApi.Controllers
{
    public class MessageController : ApiController
    {

         #region GET
        [HttpGet]
        public List<Message> Get(string userName)
        {
            return MessageDao.GetMessagesOfUser(ProjectMembershipDao.GetUser(userName));
        }
        #endregion

        #region POST
        [HttpPost]
        public bool Post([FromBody] ProjectJoinMessageModel model)
        {
            MessageDao.AddMessage(model.Message);
            ProjectMembershipRequestModel requestModel = new ProjectMembershipRequestModel();
            requestModel.Username = model.ProjectJoinRequest.User.Username;
            requestModel.UserRole = model.ProjectJoinRequest.UserRole;
            requestModel.ProjectGuid = model.ProjectJoinRequest.ProjectGuid;

            ProjectsDao.JoinProject(requestModel);

            ProjectMembershipDao.RemoveProjectJoinRequest(model.ProjectJoinRequest);

            return true;
        }
        #endregion


        #region POST
        [HttpDelete]
        public bool Delete([FromBody] ProjectJoinMessageModel model)
        {
            MessageDao.AddMessage(model.Message);
            ProjectMembershipDao.RemoveProjectJoinRequest(model.ProjectJoinRequest);

            return true;
        }
        #endregion
    }
}
