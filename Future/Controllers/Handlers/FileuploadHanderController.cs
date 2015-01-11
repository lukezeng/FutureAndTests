using System;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace Future.Controllers.Handlers
{
    public class AjaxHandlerController : Controller
    {
        [HttpPost]
        public String UpdateProfilePic(string profilePicName)
        {
            var profilePicLocation = "/Files/Users/" + Request.Cookies["UserName"].Value + "/ProfilePic/" + profilePicName;
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (var conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    var cmd =
                        new SqlCommand(
                            "UPDATE UserProfile SET UserProfilePic = '" + profilePicLocation + "' where UserName = '" +
                            Request.Cookies["UserName"].Value + "'", conn);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            return profilePicLocation;
        }
    }
}
