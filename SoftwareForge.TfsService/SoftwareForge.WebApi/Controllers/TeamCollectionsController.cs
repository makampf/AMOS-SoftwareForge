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
using System.Net.Http;
using System.Web.Http;
using SoftwareForge.Common.Models;
using SoftwareForge.TfsService;

namespace SoftwareForge.WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class TeamCollectionsController : ApiController
    {
        //Lazy initialization
        private readonly Lazy<TfsController> _tfsController = new Lazy<TfsController>(CreateTfsController);

        TfsController TfsController
        {
            get { return _tfsController.Value; }
        }
        /// <summary>
        /// Function which creates a new TfsController
        /// </summary>
        /// <returns> new TfsController</returns>
        private static TfsController CreateTfsController()
        {
                 return new TfsController(new Uri(Properties.Settings.Default.TfsServerUri), Properties.Settings.Default.DbConnectionString);
        }

        #region GET
        // GET api/TeamCollections
        [HttpGet]
        public List<TeamCollection> GetTeamCollections()
        {
            return TfsController.GetTeamCollections(); 
        }

        // GET api/TeamCollections?guid=fe4ad9d6-6936-4f09-842c-4d56f4276cee
        [HttpGet]
        public TeamCollection GetTeamCollection(Guid guid)
        {
            TeamCollection result = TfsController.GetTeamCollection(guid);
            if (result == null)
            {
                HttpResponseMessage responseMessage = new HttpResponseMessage
                    {
                        Content = new StringContent(string.Format("No team collection found with GUID = {0}", guid)),
                        ReasonPhrase = "GUID Not Found"
                    };
                throw new HttpResponseException(responseMessage);
            }
                
            
            return result;
        }
        #endregion



        #region POST
        // POST api/teamcollections
        [HttpPost]
        public TeamCollection CreateTeamCollection([FromBody] string testCollectionName)
        {
            return TfsController.CreateTeamCollection(testCollectionName);
        }
        #endregion



        //#region PUT
        //// PUT api/TeamCollections
        //[HttpPut]
        //public void Put(int id, [FromBody]string value)
        //{
        //}
        //#endregion



        #region Delete
        // DELETE api/teamcollections/5

        //TODO: Test it from browser
        [HttpDelete]
        public void RemoveTeamCollection(Guid testCollectionGuid)
        {
            TfsController.RemoveTeamCollection(testCollectionGuid);
        }
        #endregion
    }
}