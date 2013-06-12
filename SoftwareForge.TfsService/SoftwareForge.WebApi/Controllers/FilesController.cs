using System;
using System.Collections.Generic;
using System.Web.Http;
using SoftwareForge.TfsService;

namespace SoftwareForge.WebApi.Controllers
{
    public class FilesController : ApiController
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


        #region GET
        [HttpGet]
        public List<string> Get(string projectguid, string path)
        {
            return TfsController.GetFiles(new Guid(projectguid),path);
        }
        #endregion
    }
}
