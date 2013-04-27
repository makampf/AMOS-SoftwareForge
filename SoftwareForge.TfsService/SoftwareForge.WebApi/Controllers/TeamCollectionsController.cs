using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using SoftwareForge.Common.Models;
using SoftwareForge.TfsService;

namespace SoftwareForge.WebApi.Controllers
{
    public class TeamCollectionsController : ApiController
    {
        //Lazy initialization
        private readonly Lazy<TfsController> _tfsController = new Lazy<TfsController>(CreateTfsController);

        public TfsController TfsController
        {
            get { return _tfsController.Value; }
        }

        private static TfsController CreateTfsController()
        {
            return new TfsController(new Uri(Properties.Settings.Default.TfsServerUri));
        }


        #region GET
        // GET api/TeamCollections
        public List<TeamCollection> GetTeamCollections()
        {
            return TfsController.GetTeamCollections(); 
        }

        // GET api/TeamCollections?guid=fe4ad9d6-6936-4f09-842c-4d56f4276cee
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



        //#region POST
        //// POST api/teamcollections
        //public void Post([FromBody]string value)
        //{
        //}
        //#endregion



        //#region PUT
        //// PUT api/teamcollections/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}
        //#endregion



        //#region Delete
        //// DELETE api/teamcollections/5
        //public void Delete(int id)
        //{
        //}
        //#endregion
    }
}