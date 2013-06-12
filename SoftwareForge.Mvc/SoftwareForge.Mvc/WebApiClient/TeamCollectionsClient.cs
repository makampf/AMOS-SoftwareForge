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
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using Newtonsoft.Json;
using SoftwareForge.Common.Models;
using SoftwareForge.Common.Models.Requests;

namespace SoftwareForge.Mvc.WebApiClient
{
    /// <summary>
    /// This class provides methodes to create and list TeamCollections via Browser. 
    /// </summary>
    public static class TeamCollectionsClient
    {
        
        private static WebRequest CreateRequest(String url)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.PreAuthenticate = true;
            httpWebRequest.UseDefaultCredentials = false;
            httpWebRequest.Credentials = CredentialCache.DefaultCredentials;
            httpWebRequest.Timeout = 600000;
            httpWebRequest.ContentType = "application/json";

            return httpWebRequest;
        }

     


        /// <summary>
        /// Shows already created Team Collections.
        /// </summary>
        /// <returns>all Team Collections</returns>
        public static IEnumerable<TeamCollection> GetTeamCollections()
        {
            return CreateGet<IEnumerable<TeamCollection>>("api/TeamCollections");
        }

        /// <summary>
        /// Creates a new TeamCollection.
        /// </summary>
        /// <param name="name">Name of the TeamCollection.</param>
        /// <returns>The created Team Collection</returns>
        public static TeamCollection CreateTeamCollection(string name)
        {
            return CreatePost<TeamCollection, String>("api/TeamCollections", name);
        }


        /// <summary>
        /// Creates a watch request, so the user will get reader or will be removed as a reader in the project
        /// </summary>
        /// <param name="projectGuid">The guid of the project, the user watches/unwatches</param>
        /// <param name="username">The username of the user</param>
        /// <returns></returns>
        private static bool PostWatchRequest(Guid projectGuid, string username)
        {
            ProjectMembershipRequestModel joinProjectRequestModel = new ProjectMembershipRequestModel
                {
                    ProjectGuid = projectGuid,
                    Username = username,
                    UserRole = UserRole.Reader
                };
            return CreatePost<bool, ProjectMembershipRequestModel>("api/ProjectMembership", joinProjectRequestModel);
        }

        public static Project GetTeamProject(Guid teamProjectGuid)
        {
            return CreateGet<Project>("api/TeamProjects?guid=" + teamProjectGuid);
        }

        public static Project CreateProject(Project project)
        {
            return CreatePost<Project, Project>("api/TeamProjects", project);
        }

        /// <summary>
        /// Rename a project
        /// </summary>
        /// <param name="guid">The projectGuid to rename</param>
        /// <param name="newName">The new name of the project</param>
        /// <returns>True if successful, false in error case</returns>
        public static bool RenameProject(Guid guid, string newName)
        {
            RenameProjectModel rpm = new RenameProjectModel {Guid = guid, NewName = newName};
            return CreatePut<bool, RenameProjectModel>("api/TeamProjects", rpm);
        }


        /// <summary>
        /// Leave a Project.
        /// </summary>
        /// <param name="projectGuid">The projectGuid of the project to leave</param>
        /// <param name="username">The user that wants to leave</param>
        /// <returns>True if successful, false in error case</returns>
        public static bool UnwatchProject(Guid projectGuid, string username)
        {
            return PostWatchRequest(projectGuid, username);
        }

        /// <summary>
        /// Join a Project.
        /// </summary>
        /// <param name="projectGuid">The projectGuid of the project to join</param>
        /// <param name="username">The user that wants to join</param>
        /// <returns>True if successful, false in error case</returns>
        public static bool WatchProject(Guid projectGuid, string username)
        {
            return PostWatchRequest(projectGuid, username);
        }

        /// <summary>
        /// Creates the request for project joining
        /// </summary>
        /// <param name="projectJoinRequest"></param>
        /// <returns></returns>
        public static bool CreateJoinProjectRequest(ProjectJoinRequest projectJoinRequest)
        {
            return CreatePost<bool, ProjectJoinRequest>("api/ProjectMembershipRequest", projectJoinRequest);
        }
        /// <summary>
        /// Creates request for prject invitation
        /// </summary>
        /// <param name="projectInvitationRequest">the Request</param>
        /// <returns></returns>
        public static bool CreateProjectInvitationRequest(ProjectInvitationRequest projectInvitationRequest)
        {
            return CreatePost<bool, ProjectInvitationRequest>("api/ProjectInvitationRequest", projectInvitationRequest);
        }

        /// <summary>
        /// Lists all Project Join Request from a User
        /// </summary>
        /// <param name="username">User who will be shown all his request</param>
        /// <returns>A list with all Requests</returns>
        public static List<ProjectJoinRequest> GetProjectJoinRequests(String username)
        {
            return CreateGet<List<ProjectJoinRequest>>("api/ProjectMembershipRequest/?username=" + username);
        }

        
        /// <summary>
        /// Gets the ProjectJoiunRequesty by Id
        /// </summary>
        /// <param name="requestId">RequestId</param>
        /// <returns>the Requests</returns>
        public static ProjectJoinRequest GetProjectJoinRequestById(int requestId)
        {
            return CreateGet<ProjectJoinRequest>("api/ProjectMembershipRequest/?requestId=" + requestId);
        }

        /// <summary>
        /// Gets the InvitationRequests by Id
        /// </summary>
        /// <param name="invitationId"></param>
        /// <returns>the Invitationrequests</returns>
        public static ProjectInvitationRequest GetInvitationRequestById(int invitationId)
        {
            return CreateGet<ProjectInvitationRequest>("api/ProjectInvitationRequest/?invitationId=" + invitationId);
        }

        /// <summary>
        /// Leave a project
        /// </summary>
        /// <param name="projectGuid">Guid of the project</param>
        /// <param name="username">Username of the project leaver</param>
        /// <param name="role">Role of the project leaver</param>
        /// <returns></returns>
        public static bool LeaveProject(Guid projectGuid, string username, UserRole role)
        {
            ProjectMembershipRequestModel leaveProjectRequestModel = new ProjectMembershipRequestModel
            {
                ProjectGuid = projectGuid,
                Username = username,
                UserRole = role
            };

            
            return CreateDelete<bool, ProjectMembershipRequestModel>("api/ProjectMembership", leaveProjectRequestModel);
        }

        /// <summary>
        /// Creates the projectjoinmessage
        /// </summary>
        /// <param name="model">Message model</param>
        /// <returns>the model with the message</returns>
        public static bool CreateMessage(ProjectJoinMessageModel model)
        {
            return CreatePost<bool, ProjectJoinMessageModel>("api/Message", model);
        }

        /// <summary>
        /// Deletes the message 
        /// </summary>
        /// <param name="model">ProjectJoinModel</param>
        /// <returns>the model without the message</returns>
        public static bool DeleteMessage(ProjectJoinMessageModel model)
        {
            return CreateDelete<bool, ProjectJoinMessageModel>("api/Message", model);
        }

        /// <summary>
        /// Check if exists a user, if not create the user
        /// </summary>
        /// <param name="userName">Username</param>
        /// <returns>return the user</returns>
        public static User GetOrCreateUserByName(string userName)
        {
            return CreateGet<User>("api/User/?userName=" + userName);
        }

        /// <summary>
        /// Check if there exists a user
        /// </summary>
        /// <param name="user">Username</param>
        /// <returns>the User or null if no user exists with this username</returns>
        public static User GetUserByName(String user)
        {
            return CreateGet<User>("api/User/?user=" + user);
        }

        /// <summary>
        /// Lists all messages of user
        /// </summary>
        /// <param name="userName">Username</param>
        /// <returns>a list of messages</returns>
        public static List<Message> GetMessages(string userName)
        {
            return CreateGet<List<Message>>("api/Message/?userName=" + userName);
        }


        /// <summary>
        /// Lists the InvitationRequests of a user
        /// </summary>
        /// <param name="userName">Username</param>
        /// <returns>a list with all invitation requests</returns>
        public static List<ProjectInvitationRequest> GetInvitations(string userName)
        {
            return CreateGet<List<ProjectInvitationRequest>>("api/ProjectInvitationRequest/?userName=" + userName);
        }

        /// <summary>
        /// Deletes the invitation message
        /// </summary>
        /// <param name="model">the message model</param>
        /// <returns></returns>
        public static bool DeleteInvitationMessage(ProjectInvitationMessageModel model)
        {
            return CreateDelete<bool, ProjectInvitationMessageModel>("api/InvitationMessage", model);
        }


        /// <summary>
        /// Creates the Invitation Message 
        /// </summary>
        /// <param name="model"> The message model</param>
        /// <returns>a CreatePos</returns>
        public static bool CreateInvitationMessage(ProjectInvitationMessageModel model)
        {
            return CreatePost<bool, ProjectInvitationMessageModel>("api/InvitationMessage", model);
        }

        /// <summary>
        /// Create a Get webRequest
        /// </summary>
        /// <typeparam name="T">the expected type of the answer</typeparam>
        /// <param name="getUrl">the url for the get call</param>
        /// <returns>a object with type T</returns>
        private static T CreateGet<T>(string getUrl)
        {

            String url = Properties.Settings.Default.WebApiUri + getUrl;
            var httpWebRequest = CreateRequest(url);
            httpWebRequest.Method = "GET";

            using (HttpWebResponse httpResponse = httpWebRequest.GetResponse() as HttpWebResponse)
            {
                if (httpResponse != null)
                {
                    Stream stream = httpResponse.GetResponseStream();
                    if (stream != null)
                    {
                        T teamCollections =
                            JsonConvert.DeserializeObject<T>(new StreamReader(stream).ReadToEnd());
                        return teamCollections;
                    }
                }
 
            }
            return default(T);
        }

        /// <summary>
        /// Create a post WebRequest
        /// </summary>
        /// <typeparam name="T">the expected type of the answer</typeparam>
        /// <typeparam name="TM">the type of the posted model</typeparam>
        /// <param name="postUrl">the url for the post call</param>
        /// <param name="model">the model to post</param>
        /// <returns>a object with type T</returns>
        private static T CreatePost<T, TM>(string postUrl, TM model)
        {
            String url = Properties.Settings.Default.WebApiUri + postUrl;
            var httpWebRequest = CreateRequest(url);
            httpWebRequest.Method = "POST";

            using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(TM));
                MemoryStream ms = new MemoryStream();
                ser.WriteObject(ms, model);
                String json = Encoding.UTF8.GetString(ms.ToArray());

                streamWriter.Write(json);
                streamWriter.Flush();
            }

            using (HttpWebResponse httpResponse = httpWebRequest.GetResponse() as HttpWebResponse)
            {
                if (httpResponse != null)
                {
                    var responseStream = httpResponse.GetResponseStream();
                    if (responseStream != null)
                    {
                        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                        return (T)ser.ReadObject(responseStream);
                    }
                }
            }

            return default(T);
        }


        /// <summary>
        /// Create a post WebRequest
        /// </summary>
        /// <typeparam name="T">the expected type of the answer</typeparam>
        /// <typeparam name="TM">the type of the posted model</typeparam>
        /// <param name="postUrl">the url for the post call</param>
        /// <param name="model">the model to post</param>
        /// <returns>a object with type T</returns>
        private static T CreateDelete<T, TM>(string postUrl, TM model)
        {
            String url = Properties.Settings.Default.WebApiUri + postUrl;
            var httpWebRequest = CreateRequest(url);
            httpWebRequest.Method = "DELETE";

            using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(TM));
                MemoryStream ms = new MemoryStream();
                ser.WriteObject(ms, model);
                String json = Encoding.UTF8.GetString(ms.ToArray());

                streamWriter.Write(json);
                streamWriter.Flush();
            }

            using (HttpWebResponse httpResponse = httpWebRequest.GetResponse() as HttpWebResponse)
            {
                if (httpResponse != null)
                {
                    var responseStream = httpResponse.GetResponseStream();
                    if (responseStream != null)
                    {
                        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                        return (T)ser.ReadObject(responseStream);
                    }
                }
            }

            return default(T);
        }


        /// <summary>
        /// Create a put WebRequest
        /// </summary>
        /// <typeparam name="T">the expected type of the answer</typeparam>
        /// <typeparam name="TM">the type of the posted model</typeparam>
        /// <param name="postUrl">the url for the post call</param>
        /// <param name="model">the model to post</param>
        /// <returns>a object with type T</returns>
        private static T CreatePut<T, TM>(string postUrl, TM model)
        {
            String url = Properties.Settings.Default.WebApiUri + postUrl;
            var httpWebRequest = CreateRequest(url);
            httpWebRequest.Method = "PUT";

            using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(TM));
                MemoryStream ms = new MemoryStream();
                ser.WriteObject(ms, model);
                String json = Encoding.UTF8.GetString(ms.ToArray());

                streamWriter.Write(json);
                streamWriter.Flush();
            }

            using (HttpWebResponse httpResponse = httpWebRequest.GetResponse() as HttpWebResponse)
            {
                if (httpResponse != null)
                {
                    var responseStream = httpResponse.GetResponseStream();
                    if (responseStream != null)
                    {
                        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                        return (T)ser.ReadObject(responseStream);
                    }
                }
            }

            return default(T);
        }

        public static List<string> GetBranches(Guid guid)
        {
            return CreateGet<List<string>>("api/Branches/?projectguid=" + guid);
        }
    }
}