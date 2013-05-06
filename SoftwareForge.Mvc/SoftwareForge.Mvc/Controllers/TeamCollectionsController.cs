using System.Net.Http;
using System.Web.Mvc;
using SoftwareForge.Mvc.WebApiClient;

namespace SoftwareForge.Mvc.Controllers
{
    public class TeamCollectionsController : Controller
    {
        public ActionResult CreateTeamCollection()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateTeamCollection(FormCollection collection)
        {
            try
            {

                ValueProviderResult name = collection.GetValue("Name");
                if (name == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                try
                {
                    TeamCollectionsClient.CreateTeamCollection(name.AttemptedValue);
                }
                catch (HttpRequestException)
                {
                    //TODO error
                    throw;
                }
        
                return RedirectToAction("Index","Home");
            }
            catch
            {
                return View();
            }
        }
    }
}
