using System.Collections.Generic;
using System.Web.Mvc;
using SoftwareForge.Common.Models;
using SoftwareForge.Mvc.WebApiClient;

namespace SoftwareForge.Mvc.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            IEnumerable<TeamCollection> teamCollections = TeamCollectionsClient.GetTeamCollections();
            return View(teamCollections);
        }
    }
}
