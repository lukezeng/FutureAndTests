using System;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace Future.Controllers
{
    public class HomeController : Controller
    {
        private static readonly string ConnectionString =
            System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public ActionResult Index()
        {
            ViewBag.Message = "This is the ViewBag.Message of the Index Page.";

            return View();
        }

        public ActionResult About()
        {
            //This is testing the build-in SqlClient 
            //Connection tested sucessfully on 11/6/2013
            using (var conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                    var cmd = new SqlCommand("SELECT AboutInfo FROM ViFutureInfo", conn);
                    var rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        ViewBag.AboutInfo = rdr.GetString(0);
                    }
                }
                catch(Exception)
                {
                    // ignored
                }
            }
            ViewBag.Message = "This is the ViewBag.Message of the Index Page.";

            return View();
        }

        public ActionResult Contact()
        {
            //This is testing the build-in SqlClient 
            //Connection tested sucessfully on 11/6/2013
            using (var conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                    var cmd = new SqlCommand("SELECT * FROM ViFutureInfo", conn);
                    var rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        ViewBag.MainPhone = rdr.GetString(1);
                        ViewBag.AfterHourPhone = rdr.GetString(2);
                        ViewBag.SupportEmail = rdr.GetString(3);
                        ViewBag.MarketingEmail = rdr.GetString(4);
                        ViewBag.GeneralEmail = rdr.GetString(5);
                        ViewBag.AddressSt = rdr.GetString(6);
                        ViewBag.AddressCity = rdr.GetString(7);
                        ViewBag.AddressState = rdr.GetString(8);
                        ViewBag.AddressZipCode = rdr.GetString(9);
                    }
                }catch (Exception)
                {
                    // ignored
                }
            }
            ViewBag.Message = "This is the ViewBag.Message of the Index Page.";

            return View();
        }

        [Authorize]
        public ActionResult MyProfile()
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();

                    var cmd =
                        new SqlCommand(
                            "SELECT UserProfilePic FROM UserProfile where UserName = '" +
                            Request.Cookies["UserName"].Value + "'", conn);
                    var rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        ViewBag.UserProfilePic = !rdr.IsDBNull(0)
                            ? rdr.GetString(0)
                            : "/Files/Users/_DefaultUser/_DefaultProfilePic/DefaultProfilePic.jpg";
                    }
                }
                catch(Exception)
                {
                    // ignored
                }
            }


            ViewBag.Message = "This is the ViewBag.Message of the Index Page.";
            return View();
        }
    }
}
