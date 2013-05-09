using System;
using System.Web.Http;
using SoftwareForge.Common.Models.Requests;
using SoftwareForge.DbService;

namespace SoftwareForge.WebApi.Controllers
{
    public class ProjectMembershipController : ApiController
    {

        //Lazy initialization
        private readonly Lazy<ProjectsDao> _projectsDao = new Lazy<ProjectsDao>(CreateProjectsDao);

        ProjectsDao ProjectsDao
        {
            get { return _projectsDao.Value; }
        }
        /// <summary>
        /// Function which creates a new ProjectsDao
        /// </summary>
        /// <returns> new ProjectsDao</returns>
        private static ProjectsDao CreateProjectsDao()
        {
            return new ProjectsDao();
        }

        #region POST
        // POST api/teamcollections
        [HttpPost]
        public bool Post([FromBody] ProjectMembershipRequestModel project)
        {
            ProjectsDao.ProcessMembershipRequest(project);
            return true;
        }
        #endregion



    }
}
