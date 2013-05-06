
using System.Collections.Generic;
using System.Web.Mvc;
using SoftwareForge.Common.Models;
using SoftwareForge.Mvc.WebApiClient;

namespace SoftwareForge.Mvc.Controllers
{
    /// <summary>
    /// Controller for home view.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// GET: /Home.
        /// </summary>
        /// <returns>Home view with teamcollections.</returns>
        public ActionResult Index()
        {
            IEnumerable<TeamCollection> teamCollections = TeamCollectionsClient.GetTeamCollections();
            return View(teamCollections);
        }
    }
}
