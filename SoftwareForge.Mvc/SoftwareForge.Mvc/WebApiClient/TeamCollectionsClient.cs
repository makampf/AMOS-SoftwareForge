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
using System.Net;
using System.Runtime.Serialization.Json;
using SoftwareForge.Common.Models;

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
            //httpWebRequest.UnsafeAuthenticatedConnectionSharing = true;

            httpWebRequest.ContentType = "text/json";
            httpWebRequest.Method = "GET";

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
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(IEnumerable<TeamCollection>));
                if (httpResponse != null)
                {
                    var stream = httpResponse.GetResponseStream();
                    if (stream != null)
                        return (IEnumerable<TeamCollection>) ser.ReadObject(stream);
                }
                return null;
            }
        }



        ///// <summary>
        ///// Creates a new TeamCollection.
        ///// </summary>
        ///// <param name="name">Name of the TeamCollection.</param>
        ///// <returns>The created Team Collection</returns>
        //public static TeamCollection CreateTeamCollection(string name)
        //{

        //    // List all products.
        //    HttpResponseMessage response = Client.PostAsJsonAsync("api/TeamCollections", name).Result;  // Blocking call!
        //    if (response.IsSuccessStatusCode)
        //    {
        //        // Parse the response body. Blocking!
        //        return response.Content.ReadAsAsync<TeamCollection>().Result;
        //    }

        //    throw new HttpRequestException(response.StatusCode + ": " + response.ReasonPhrase);

        //}

        //public static bool JoinProject(Guid projectGuid, string username)
        //{
        //    return PostProjectMembershipRequest(projectGuid, username);
        //}

        //public static TeamCollection GetTeamCollection(Guid teamCollectionGuid)
        //{
        //    // Get teamcollection.
        //    HttpResponseMessage response = Client.GetAsync("api/TeamCollections?guid=" + teamCollectionGuid).Result;  // Blocking call!
        //    if (response.IsSuccessStatusCode)
        //    {
        //        // Parse the response body. Blocking!
        //        return response.Content.ReadAsAsync<TeamCollection>().Result;
        //    }
        //    throw new HttpRequestException(response.StatusCode + ": " + response.ReasonPhrase);
        //}

        //public static Project CreateProject(Project project)
        //{
        //    HttpResponseMessage response = Client.PostAsJsonAsync("api/TeamProjects", project).Result;  // Blocking call!
        //    if (response.IsSuccessStatusCode)
        //    {
        //        // Parse the response body. Blocking!
        //        return response.Content.ReadAsAsync<Project>().Result;
        //    }

        //    throw new HttpRequestException(response.StatusCode + ": " + response.ReasonPhrase);
        //}

        ///// <summary>
        ///// Leave a Project
        ///// </summary>
        ///// <param name="projectGuid">The projectGuid of the project to leave</param>
        ///// <param name="username">The user that wants to leave</param>
        ///// <returns>true if successful, false in error case</returns>
        //public static bool LeaveProject(Guid projectGuid, string username)
        //{
        //    return PostProjectMembershipRequest(projectGuid, username);
        //}

        //public static Project GetTeamProject(Guid teamProjectGuid)
        //{
        //    // Get teamcollection.
        //    HttpResponseMessage response = Client.GetAsync("api/TeamProjects?guid=" + teamProjectGuid).Result;  // Blocking call!
        //    if (response.IsSuccessStatusCode)
        //    {
        //        // Parse the response body. Blocking!
        //        return response.Content.ReadAsAsync<Project>().Result;
        //    }
        //    throw new HttpRequestException(response.StatusCode + ": " + response.ReasonPhrase);
        //}

        //private static bool PostProjectMembershipRequest(Guid projectGuid, string username)
        //{

        //    ProjectMembershipRequestModel joinProjectRequestModel = new ProjectMembershipRequestModel
        //        {
        //            ProjectGuid = projectGuid,
        //            Username = username,
        //            UserRole = UserRole.Reader
        //        };

        //    // Post a membership request
        //    HttpResponseMessage response = Client.PostAsJsonAsync("api/ProjectMembership", joinProjectRequestModel).Result;
        //        // Blocking call!
        //    if (response.IsSuccessStatusCode)
        //    {
        //        // Parse the response body. Blocking!
        //        return response.Content.ReadAsAsync<bool>().Result;
        //    }

        //    throw new HttpRequestException(response.StatusCode + ": " + response.ReasonPhrase);
        //}
        public static void CreateTeamCollection(string attemptedValue)
        {
            throw new NotImplementedException();
        }

        public static void CreateProject(Project project)
        {
            throw new NotImplementedException();
        }

        public static Project GetTeamProject(Guid guid)
        {
            throw new NotImplementedException();
        }

        public static void JoinProject(Guid guid, string username)
        {
            throw new NotImplementedException();
        }

        public static void LeaveProject(Guid guid, string username)
        {
            throw new NotImplementedException();
        }
    }
}