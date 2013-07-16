/*
 * Copyright (c) 2013 by Denis Bach, Marvin Kampf, Konstantin Tsysin, Taner Tunc, Florian Wittmann
 *
 * This file is part of the Software Forge Overlay rating application.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as
 * published by the Free Software Foundation, either version 3 of the
 * License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public
 * License along with this program. If not, see
 * <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Web.Mvc;
using SoftwareForge.Common.Models;
using SoftwareForge.Mvc.Facade;

namespace SoftwareForge.Mvc.Controllers
{
    public class WikiController : Controller
    {
        public ActionResult WikiView(Guid guid)
       {
           ViewBag.TeamProjectGuid = guid;
           List<WikiModel> wikiEntriesList = SoftwareForgeFacade.Client.GetEntriesOfProject(guid);
           return View(wikiEntriesList);
       }


        public ActionResult WikiEntry(Guid guid, int id )
        {
            ViewBag.TeamProjectGuid = guid;
            WikiModel wikiEntry = SoftwareForgeFacade.Client.GetEntry(id);
            return View(wikiEntry);
        }


       public ActionResult CreateWikiEntry(Guid projectGuid)
       {
           WikiModel model = new WikiModel();
           model.ProjectGuid = projectGuid;

           return View(model);
       }

        public ActionResult PostCreateWikiEntry(WikiModel model)
        {
            if (string.IsNullOrEmpty(model.Title))
                throw new Exception("Titel must not be empty");

            SoftwareForgeFacade.Client.CreateEntry(model);

            return RedirectToAction("WikiView", new {guid = model.ProjectGuid});
        }
       
    }
}
