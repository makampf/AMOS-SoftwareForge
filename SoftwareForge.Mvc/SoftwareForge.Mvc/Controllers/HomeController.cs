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
using System.Linq;
using System.Web.UI.WebControls;
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
        /// Fill the dashboard Model and return the dashboard view
        /// </summary>
        /// <returns>the dashboardview with partial views like random projects, own project, ...</returns>
        public ActionResult Dashboard()
        {
            DashboardModel dashboardModel = new DashboardModel();
            List<TeamCollection> teamCollections = TeamCollectionsClient.GetTeamCollections().ToList();
            
            dashboardModel.RandomProjects = GetRandomProjects(teamCollections);
            dashboardModel.MyProjects = GetMyProjects(teamCollections);

            dashboardModel.Requests = GetMyRequests();
            dashboardModel.Messages = GetMyMessages();

            return View(dashboardModel);
        }

        private List<string> GetMyMessages()
        {
            //TODO
            List<String> messages = new List<string>();

            return messages;
        }

        private List<ProjectJoinRequest> GetMyRequests()
        {
            List<ProjectJoinRequest> requests = TeamCollectionsClient.GetProjectJoinRequests(User.Identity.Name);
            return requests;
        }

        /// <summary>
        /// Search all projects the user is not joined yet. Then chooses 5 of them randomly.
        /// </summary>
        /// <returns>5 random projects for a user in which he is not joined yet.</returns>
        private List<Project> GetRandomProjects(IEnumerable<TeamCollection> teamCollections)
        {
            List<Project> randomProjects = new List<Project>();
            foreach (TeamCollection teamCollection in teamCollections)
            {
                foreach (Project project in teamCollection.Projects)
                {
                    if (!project.Users.Any(user => user.User.Username.Equals(User.Identity.Name)))
                    {
                        randomProjects.Add(project);
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
            return fiveProjects;
        }
        /// <summary>
        /// Search all projects the user has joined yet.
        /// </summary>
        /// <returns>all projects for a user in which he is member.</returns>
        private List<Project> GetMyProjects(IEnumerable<TeamCollection> teamCollections)
        {
            List<Project> myProjects = new List<Project>();
            foreach (TeamCollection teamCollection in teamCollections)
            {
                foreach (Project project in teamCollection.Projects)
                {
                    if (project.Users.Any(user => user.User.Username.Equals(User.Identity.Name)))
                    {
                        myProjects.Add(project);
                    }
                }
            }
            return myProjects;
        }

        public ActionResult AcceptRequest(int requestId)
        {

            return View("AcceptRequest", CreateMessageModel(requestId));
            
        }

        public ActionResult DeclineRequest(int requestId)
        {
            return View("DeclineRequest", CreateMessageModel(requestId));

        }

        public void PostDeclineMessage()
        {
            
        }

        public void PostAcceptMessage()
        {
            
        }

        private ProjectJoinMessageModel CreateMessageModel(int requestId)
        {
            ProjectJoinRequest request = TeamCollectionsClient.GetProjectJoinRequestById(requestId);
            ProjectJoinMessageModel model = new ProjectJoinMessageModel();
            model.ProjectJoinRequest = request;
            model.Message = new Message();
            model.Message.FromUser = new User();
            model.Message.FromUser.Username = User.Identity.Name;
            model.Message.ToUser = request.User;
            model.Message.ToUserId = request.UserId;
            return model;
        }
        
    }
}
