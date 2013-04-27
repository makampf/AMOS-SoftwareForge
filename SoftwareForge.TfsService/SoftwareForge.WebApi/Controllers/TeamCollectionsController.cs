using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.TeamFoundation.Framework.Client;
using SoftwareForge.Common.Models;
using SoftwareForge.TfsService;

namespace SoftwareForge.WebApi.Controllers
{
    public class TeamCollectionsController : ApiController
    {

        // GET api/teamcollections
        public List<TeamCollection> Get()
        {
            return new TfsController(new Uri("http://localhost:8080/tfs")).GetTeamCollections();
        }

        // GET api/teamcollections/5
        public string Get(int id)
        {
            return "val" + id;
        }

        // POST api/teamcollections
        public void Post([FromBody]string value)
        {
        }

        // PUT api/teamcollections/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/teamcollections/5
        public void Delete(int id)
        {
        }
    }
}