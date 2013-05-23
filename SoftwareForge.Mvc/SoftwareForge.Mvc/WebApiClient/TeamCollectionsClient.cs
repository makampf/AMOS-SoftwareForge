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
using SoftwareForge.Common.Models;
using SoftwareForge.Common.Models.Requests;

namespace SoftwareForge.Mvc.WebApiClient
{
    /// <summary>
    /// This class provides methodes to create and list TeamCollections via Browser. 
    /// </summary>
    public static class TeamCollectionsClient
    {
        
        private static WebRequest CreateGetRequest(String url)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.PreAuthenticate = true;
            httpWebRequest.UseDefaultCredentials = false;
            httpWebRequest.Credentials = CredentialCache.DefaultCredentials;
            httpWebRequest.Timeout = 600000;

            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";

            return httpWebRequest;
        }

        private static WebRequest CreatePostRequest(string url)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.PreAuthenticate = true;
            httpWebRequest.UseDefaultCredentials = false;
            httpWebRequest.Credentials = CredentialCache.DefaultCredentials;
            httpWebRequest.Timeout = 600000;
            
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            return httpWebRequest;
        }


        /// <summary>
        /// Shows already created Team Collections.
        /// </summary>
        /// <returns>all Team Collections</returns>
        public static IEnumerable<TeamCollection> GetTeamCollections()
        {
            String url = Properties.Settings.Default.WebApiUri + "api/TeamCollections";
            var httpWebRequest = CreateGetRequest(url);
           
            using (HttpWebResponse httpResponse = httpWebRequest.GetResponse() as HttpWebResponse)
            {
                if (httpResponse != null)
                {
                    var stream = httpResponse.GetResponseStream();
                    if (stream != null)
                    {
                        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(IEnumerable<TeamCollection>));
                        return (IEnumerable<TeamCollection>) ser.ReadObject(stream);
                    }
                }
                return null;
            }
        }



        /// <summary>
        /// Creates a new TeamCollection.
        /// </summary>
        /// <param name="name">Name of the TeamCollection.</param>
        /// <returns>The created Team Collection</returns>
        public static TeamCollection CreateTeamCollection(string name)
        {
            String url = Properties.Settings.Default.WebApiUri + "api/TeamCollections";
            var httpWebRequest = CreatePostRequest(url);

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(String));
                MemoryStream ms = new MemoryStream();
                ser.WriteObject(ms, name);
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
                        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(TeamCollection));
                        return (TeamCollection)ser.ReadObject(responseStream);
                    }
                }
                return null;
            }
        }

        private static bool PostProjectMembershipRequest(Guid projectGuid, string username)
        {
            ProjectMembershipRequestModel joinProjectRequestModel = new ProjectMembershipRequestModel
                {
                    ProjectGuid = projectGuid,
                    Username = username,
                    UserRole = UserRole.Reader
                };

            string url = Properties.Settings.Default.WebApiUri + "api/ProjectMembership";
            var httpWebRequest = CreatePostRequest(url);

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ProjectMembershipRequestModel));
                MemoryStream ms = new MemoryStream();
                ser.WriteObject(ms, joinProjectRequestModel);
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
                        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(bool));
                        return (bool)ser.ReadObject(responseStream);
                    }
                }
                return false;
            }
        }

        public static Project GetTeamProject(Guid teamProjectGuid)
        {
            String url =  Properties.Settings.Default.WebApiUri + "api/TeamProjects?guid=" + teamProjectGuid;
            var httpWebRequest = CreateGetRequest(url);

            using (HttpWebResponse httpResponse = httpWebRequest.GetResponse() as HttpWebResponse)
            {
                if (httpResponse != null)
                {
                    var stream = httpResponse.GetResponseStream();
                    if (stream != null)
                    {
                        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Project));
                        return (Project)ser.ReadObject(stream);
                    }
                }
                return null;
            }
        }





        public static Project CreateProject(Project project)
        {
            String url = Properties.Settings.Default.WebApiUri + "api/TeamProjects";
            var httpWebRequest = CreatePostRequest(url);

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Project));
                MemoryStream ms = new MemoryStream();
                ser.WriteObject(ms, project);
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
                        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Project));
                        return (Project)ser.ReadObject(responseStream);
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Leave a Project
        /// </summary>
        /// <param name="projectGuid">The projectGuid of the project to leave</param>
        /// <param name="username">The user that wants to leave</param>
        /// <returns>true if successful, false in error case</returns>
        public static bool LeaveProject(Guid projectGuid, string username)
        {
            return PostProjectMembershipRequest(projectGuid, username);
        }

        public static bool JoinProject(Guid projectGuid, string username)
        {
            return PostProjectMembershipRequest(projectGuid, username);
        }

    }
}