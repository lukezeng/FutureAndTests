using System.Web.Mvc;
using Future.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Future.Test.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void HomeController_GivenNothing_Returns()
        {
            var homeController = new HomeController();
            var result = homeController.Index() as ViewResult;
             Assert.AreEqual("This is the ViewBag.Message of the Index Page.", result.ViewBag.Message );
        }
    }
}
