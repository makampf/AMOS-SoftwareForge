using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using SoftwareForge.Common.Models;

namespace SoftwareForge.Mvc.WebApiClient
{
    public static class TeamCollectionsClient
    {
        private static readonly Lazy<HttpClient> _client = new Lazy<HttpClient>(CreateHttpClient);
        private static HttpClient Client { get { return _client.Value; } }
        public static HttpClient CreateHttpClient()
        {
            HttpClient client = new HttpClient { BaseAddress = new Uri(Properties.Settings.Default.WebApiUri) };

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

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