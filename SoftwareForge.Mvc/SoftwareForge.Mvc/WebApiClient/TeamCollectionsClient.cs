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


        public static bool CreateJoinProjectRequest(ProjectJoinRequest projectJoinRequest)
        {
            return CreatePost<bool, ProjectJoinRequest>("api/ProjectMembershipRequest", projectJoinRequest);
        }



        public static List<ProjectJoinRequest> GetProjectJoinRequests(String username)
        {
            return CreateGet<List<ProjectJoinRequest>>("api/ProjectMembershipRequest/?username=" + username);
        }


        public static ProjectJoinRequest GetProjectJoinRequestById(int requestId)
        {
            //TODO
            return CreateGet<ProjectJoinRequest>("api/" + requestId);
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


        

    }
}