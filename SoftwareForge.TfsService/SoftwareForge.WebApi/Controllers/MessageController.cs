/*
 * Copyright (c) 2013 by Denis Bach, Marvin Kampf, Konstantin Tsysin, Taner Tunc, Florian Wittmann
 *
 * This file is part of the Software Forge Overlay rating application.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as
 * published by the Free Software Foundation, either version 3 of the
 * License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public
 * License along with this program. If not, see
 * <http://www.gnu.org/licenses/>.
 */

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
