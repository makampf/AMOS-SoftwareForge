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

            dashboardModel.InvitationRequests = GetMyInvitationRequests();

            return View(dashboardModel);
        }

        /// <summary>
        /// lists all invitationrequests
        /// </summary>
        /// <returns>a list of invitation requests</returns>
        private List<ProjectInvitationRequest> GetMyInvitationRequests()
        {
            List<ProjectInvitationRequest> messages = TeamCollectionsClient.GetInvitations(User.Identity.Name);
            return messages;
        }

        /// <summary>
        /// lists all messages
        /// </summary>
        /// <returns>a list off messages</returns>
        private List<Message> GetMyMessages()
        {
            List<Message> messages = TeamCollectionsClient.GetMessages(User.Identity.Name);
            return messages;
        }

        /// <summary>
        /// Lists als requests
        /// </summary>
        /// <returns>a list of projects</returns>
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
        /// <summary>
        /// Accepting a request.
        /// </summary>
        /// <param name="requestId">Id of a request</param>
        /// <returns>A view where you can type in an answer for the request</returns>
        public ActionResult AcceptRequest(int requestId)
        {
            ProjectJoinMessageModel model = CreateMessageModel(requestId);
            return View("AcceptRequest", model);
        }


        /// <summary>
        /// Declining a request.
        /// </summary>
        /// <param name="requestId">Id of a request</param>
        /// <returns>A view where you can type in an answer for the request</returns>
        public ActionResult DeclineRequest(int requestId)
        {
            ProjectJoinMessageModel model = CreateMessageModel(requestId);
            return View("DeclineRequest", model);

        }

        /// <summary>
        /// Accepts the invitation
        /// </summary>
        /// <param name="invitationId"></param>
        /// <returns>the accept invitation view</returns>
        public ActionResult AcceptInvitation(int invitationId)
        {

            ProjectInvitationMessageModel model = CreateInvitationModel(invitationId);
            return View("AcceptInvitation", model);
        }

        /// <summary>
        /// Declines the activation
        /// </summary>
        /// <param name="invitationId">Id of the invitation</param>
        /// <returns>the decline invitation view</returns>
        public ActionResult DeclineInvitation(int invitationId)
        {
            ProjectInvitationMessageModel model = CreateInvitationModel(invitationId);
            return View("DeclineInvitation", model);
        }


        /// <summary>
        /// posts the decline message 
        /// </summary>
        /// <param name="collection"></param>
        /// <returns>redirection to dashboard</returns>
        [HttpPostAttribute]
        [ValidateAntiForgeryTokenAttribute]
        public ActionResult PostDeclineInvitationMessage(FormCollection collection)
        {
            String message = collection.GetValue("Message.Text").AttemptedValue;
            String invitationId = collection.GetValue("ProjectInvitationRequest.Id").AttemptedValue;

            ProjectInvitationMessageModel model = CreateInvitationModel(Convert.ToInt32(invitationId));

            model.Message.Text = message;

            TeamCollectionsClient.DeleteInvitationMessage(model);

            return RedirectToAction("Dashboard", "Home");
        }


        /// <summary>
        /// posts the invitation accept message
        /// </summary>
        /// <param name="collection"></param>
        /// <returns>redirection to dashboard</returns>
        [HttpPostAttribute]
        [ValidateAntiForgeryTokenAttribute]
        public ActionResult PostAcceptInvitationMessage(FormCollection collection)
        {
            String message = collection.GetValue("Message.Text").AttemptedValue;
            String invitationId = collection.GetValue("ProjectInvitationRequest.Id").AttemptedValue;

            ProjectInvitationMessageModel model = CreateInvitationModel(Convert.ToInt32(invitationId));

            model.Message.Text = message;

            TeamCollectionsClient.CreateInvitationMessage(model);

            return RedirectToAction("Dashboard", "Home");
        }
        

        /// <summary>
        /// posts the decline message
        /// </summary>
        /// <param name="collection"></param>
        /// <returns>redirection to dashboard</returns>
        [HttpPostAttribute]
        [ValidateAntiForgeryTokenAttribute]
        public ActionResult PostDeclineMessage(FormCollection collection)
        {
            String message = collection.GetValue("Message.Text").AttemptedValue;
            String requestId = collection.GetValue("ProjectJoinRequest.Id").AttemptedValue;

            ProjectJoinMessageModel model = CreateMessageModel(Convert.ToInt32(requestId));

            model.Message.Text = message;

            TeamCollectionsClient.DeleteMessage(model);

            return RedirectToAction("Dashboard", "Home");
        }
       

        /// <summary>
        /// Posts the accepts message
        /// </summary>
        /// <param name="collection"></param>
        /// <returns>redirection to dashboard</returns>
        [HttpPostAttribute]
        [ValidateAntiForgeryTokenAttribute]
        public ActionResult PostAcceptMessage(FormCollection collection)
        {
            String message = collection.GetValue("Message.Text").AttemptedValue;
            String requestId = collection.GetValue("ProjectJoinRequest.Id").AttemptedValue;

            ProjectJoinMessageModel model = CreateMessageModel(Convert.ToInt32(requestId));

            model.Message.Text = message;
            
            TeamCollectionsClient.CreateMessage(model);
            
            return RedirectToAction("Dashboard", "Home");
        }
        

        /// <summary>
        /// Creates invitation model
        /// </summary>
        /// <param name="invitationId">Id of the invitation</param>
        /// <returns>the invitation model</returns>
        private ProjectInvitationMessageModel CreateInvitationModel(int invitationId)
        {
            ProjectInvitationRequest request = TeamCollectionsClient.GetInvitationRequestById(invitationId);
            ProjectInvitationMessageModel model = new ProjectInvitationMessageModel();
            model.ProjectInvitationRequest = request;
            model.Message = new Message();
            model.Message.FromUserId = TeamCollectionsClient.GetOrCreateUserByName(User.Identity.Name).Id;
            model.Message.ToUserId = request.UserId;
            return model;
        }


        /// <summary>
        /// Creates the message model for a request
        /// </summary>
        /// <param name="requestId">Id off a request</param>
        /// <returns>the message model</returns>
        private ProjectJoinMessageModel CreateMessageModel(int requestId)
        {
            ProjectJoinRequest request = TeamCollectionsClient.GetProjectJoinRequestById(requestId);
            ProjectJoinMessageModel model = new ProjectJoinMessageModel();
            model.ProjectJoinRequest = request;
            model.Message = new Message();
            model.Message.FromUserId = TeamCollectionsClient.GetOrCreateUserByName(User.Identity.Name).Id;
            model.Message.ToUserId = request.UserId;
            return model;
        }

        
    }
}
