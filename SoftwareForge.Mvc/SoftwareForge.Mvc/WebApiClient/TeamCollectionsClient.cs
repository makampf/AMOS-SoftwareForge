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
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using SoftwareForge.Common.Models;
using SoftwareForge.Common.Models.Requests;

namespace SoftwareForge.Mvc.WebApiClient
{
    /// <summary>
    /// This class provides methodes to create and list TeamCollections via Browser. 
    /// </summary>
    public static class TeamCollectionsClient
    {
        private static readonly Lazy<HttpClient> _client = new Lazy<HttpClient>(CreateHttpClient);
        private static HttpClient Client { get { return _client.Value; } }
        /// <summary>
        /// Initializes a new HttpClient.
        /// </summary>
        /// <returns>The new HttpClient.</returns>
        public static HttpClient CreateHttpClient()
        {
            HttpClient client = new HttpClient { BaseAddress = new Uri(Properties.Settings.Default.WebApiUri) };

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
        /// <summary>
        /// Shows already created Team Collections.
        /// </summary>
        /// <returns>all Team Collections</returns>
        public static IEnumerable<TeamCollection> GetTeamCollections()
        {
            // List all teamcollections.
            HttpResponseMessage response = Client.GetAsync("api/TeamCollections").Result;  // Blocking call!
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body. Blocking!
                return response.Content.ReadAsAsync<IEnumerable<TeamCollection>>().Result;
            }
            throw new HttpRequestException(response.StatusCode + ": " + response.ReasonPhrase);

        }
        /// <summary>
        /// Creates a new TeamCollection.
        /// </summary>
        /// <param name="name">Name of the TeamCollection.</param>
        /// <returns>The created Team Collection</returns>
        public static TeamCollection CreateTeamCollection(string name)
        {
          

            // List all products.
            HttpResponseMessage response = Client.PostAsJsonAsync("api/TeamCollections", name).Result;  // Blocking call!
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body. Blocking!
                return response.Content.ReadAsAsync<TeamCollection>().Result;
            }

            throw new HttpRequestException(response.StatusCode + ": " + response.ReasonPhrase);

        }

        public static bool JoinProject(string project, string username)
        {
            JoinProjectRequestModel joinProjectRequestModel = new JoinProjectRequestModel();
            joinProjectRequestModel.ProjectGuid = new Guid(project);
            joinProjectRequestModel.Username = username;

            // List all products.
            HttpResponseMessage response = Client.PostAsJsonAsync("api/ProjectMembership", joinProjectRequestModel).Result;  // Blocking call!
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body. Blocking!
                return response.Content.ReadAsAsync<bool>().Result;
            }

            throw new HttpRequestException(response.StatusCode + ": " + response.ReasonPhrase);



            throw new NotImplementedException();
        }
    }
}