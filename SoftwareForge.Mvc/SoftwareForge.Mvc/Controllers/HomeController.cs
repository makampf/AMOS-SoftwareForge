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
using SoftwareForge.Common.Models;
using System.Collections.Generic;
using System.Web.Mvc;
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
        /// <summary>
        /// Search all projects the user is not joined yet. Then chooses 5 of them randomly.
        /// </summary>
        /// <returns>5 random projects for a user in which he is not joined yet.</returns>
        public ActionResult Dashboard()
        {
            IEnumerable<TeamCollection> teamCollections = TeamCollectionsClient.GetTeamCollections();
            List<Project> randomProjects = new List<Project>();
            int control;
            foreach (var teamCollection in teamCollections)
            {
                foreach (Project p in teamCollection.Projects)
                {
                    control = 0;
                    foreach (User u in p.Users)
                    {
                        if (u.Username == User.Identity.Name)
                        {
                            control = 1;
                            break;
                        }
                    }
                    if (control == 0)
                    {
                        randomProjects.Add(p);
                    }      
                }
            }
            List<Project> fiveProjects = new List<Project>();
            Random rnd = new Random();
            int counter = 0;
            int iterations;
            int listelements = randomProjects.Count;
            if (listelements >= 5)
            {
                iterations = 5;
            }
            else
            {
                iterations = randomProjects.Count;
            }   
                while (iterations > 0)
                {
                    int random = rnd.Next(0, listelements - counter);
                    fiveProjects.Add(randomProjects[random]);
                    randomProjects.RemoveAt(random);
                    counter++;
                    iterations--;


                }
                /*
                 * old Version
                // choose first element
                int first = rnd.Next(0, listelements);
                fiveProjects.Add(randomProjects[first]);
                randomProjects.RemoveAt(first);

                // choose second element
                int second = rnd.Next(0, listelements - 1);
                fiveProjects.Add(randomProjects[second]);
                randomProjects.RemoveAt(second);

                // choose third element
                int third = rnd.Next(0, listelements - 2);
                fiveProjects.Add(randomProjects[third]);
                randomProjects.RemoveAt(third);

                // choose fourth element
                int fourth = rnd.Next(0, listelements - 3);
                fiveProjects.Add(randomProjects[fourth]);
                randomProjects.RemoveAt(fourth);

                // choose fifth element
                int fifth = rnd.Next(0, listelements - 4);
                fiveProjects.Add(randomProjects[fifth]);
                randomProjects.RemoveAt(fifth);
                 */

                return View(fiveProjects);
            
            
        }
    }
}
