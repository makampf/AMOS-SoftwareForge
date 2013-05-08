using System;
using System.Web.Http;
using SoftwareForge.Common.Models.Requests;
using SoftwareForge.DbService;
using SoftwareForge.TfsService;

namespace SoftwareForge.WebApi.Controllers
{
    public class ProjectMembershipController : ApiController
    {

        //Lazy initialization
        private readonly Lazy<TfsController> _tfsController = new Lazy<TfsController>(CreateTfsController);

        TfsController TfsController
        {
            get { return _tfsController.Value; }
        }
        /// <summary>
        /// Function which creates a new TfsController
        /// </summary>
        /// <returns> new TfsController</returns>
        private static TfsController CreateTfsController()
        {
            return new TfsController(new Uri(Properties.Settings.Default.TfsServerUri), Properties.Settings.Default.DbConnectionString);
        }

        #region POST
        // POST api/teamcollections
        [HttpPost]
        public bool Post([FromBody] ProjectMembershipRequestModel project)
        {
            new ProjectsDao().ProcessMembershipRequest(project);
            return true;
        }
        #endregion



    }
}
