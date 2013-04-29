using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using SoftwareForge.Mvc.WebApiClient;

namespace SoftwareForge.Mvc.Controllers
{
    public class TeamCollectionsController : Controller
    {
        //
        // GET: /TeamCollections/

        public ActionResult Create()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(FormCollection collection)
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
