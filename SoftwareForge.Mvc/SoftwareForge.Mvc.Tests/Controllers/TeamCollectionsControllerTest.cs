using System.Collections.Specialized;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftwareForge.Mvc.Controllers;

namespace SoftwareForge.Mvc.Tests.Controllers
{
    [TestClass]
    public class TeamCollectionControllerTest
    {
        private TeamCollectionsController _controller;

        /// <summary>
        /// Init the TeamCollectionController.
        /// </summary>
        [TestInitialize]
        public void TestInit()
        {
            _controller = new TeamCollectionsController();
        }

        /// <summary>
        /// Test the CreateTeamCollection() method.
        /// </summary>
        [TestMethod]
        public void CreateTeamCollectionTest()
        {
            //Without parameters
            ActionResult result = _controller.CreateTeamCollection();
            Assert.IsNotNull(result);
            
            //With Parameters
            const string key = "Name";
            const string value = "testCollection";
            NameValueCollection nameValuecollection = new NameValueCollection();
            nameValuecollection.Add(key, value);
            FormCollection formCollection = new FormCollection(nameValuecollection);
            result = _controller.CreateTeamCollection(formCollection);
            Assert.IsNotNull(result);

            //TODO: remove testCollection
        }
    }
}
