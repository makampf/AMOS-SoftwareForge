using System;
using System.Web.Http;
using SoftwareForge.Common.Models;
using SoftwareForge.DbService;

namespace SoftwareForge.WebApi.Controllers
{
    public class UserController : ApiController
    {

        #region GET
        [HttpGet]
        public User Get(String userName)
        {
            return ProjectMembershipDao.GetUser(userName);
        }
        #endregion

    }
}
