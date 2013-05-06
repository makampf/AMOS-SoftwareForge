using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using SoftwareForge.Common.Models;

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
           
            // List all products.
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

    }
}