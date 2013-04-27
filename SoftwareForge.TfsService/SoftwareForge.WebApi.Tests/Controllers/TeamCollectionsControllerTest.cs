using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftwareForge.Common.Models;
using SoftwareForge.WebApi;
using SoftwareForge.WebApi.Controllers;

namespace SoftwareForge.WebApi.Tests.Controllers
{
    [TestClass]
    public class TeamCollectionsControllerTest
    {
        [TestMethod]
        public void Get()
        {
            // Anordnen
            TeamCollectionsController controller = new TeamCollectionsController();

            // Aktion ausführen
            List<TeamCollection> result = controller.Get();

            // Bestätigen
            Assert.IsNotNull(result);
        }
    }
}
