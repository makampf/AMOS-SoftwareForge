using System;
using System.Collections.Generic;
using System.Web.Mvc;
using SoftwareForge.Common.Models;
using SoftwareForge.Mvc.WebApiClient;

namespace SoftwareForge.Mvc.Controllers
{
    public class ProjectsController : Controller
    {
        public ActionResult CreateProject(Guid teamCollectionGuid)
        {
            return View(new Project("", 0, new Guid(), teamCollectionGuid));
        }


        [HttpPostAttribute]
        [ValidateAntiForgeryTokenAttribute]
        public ActionResult Create(Project project)
        {
            if (ModelState.IsValid)
            {
                TeamCollectionsClient.CreateProject(project);
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index","Home");
        }

    }
}