using System.Net.Http;
using System.Web.Mvc;
using SoftwareForge.Mvc.WebApiClient;

namespace SoftwareForge.Mvc.Controllers
{
    /// <summary>
    /// Controller for Teamcollections
    /// </summary>
    public class TeamCollectionsController : Controller
    {
        /// <summary>
        /// GET: /CreateTeamCollection.
        /// </summary>
        /// <returns>Teamcollection view for the browser</returns>
        public ActionResult CreateTeamCollection()
        {
            return View();
        }
        /// <summary>
        /// POST: /CreateTeamCollection with a message body.
        /// </summary>
        /// <param name="collection">Message body <example> name1=value1&name2=value2 </example></param>
        /// <returns>Either view with new collection, returns to the start page or view where 
        /// the user have to type the name of the colection. </returns>
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
