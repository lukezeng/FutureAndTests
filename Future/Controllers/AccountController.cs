using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using Future.Filters;
using Future.Models;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;

namespace Future.Controllers
{
    [Authorize]
    [InitializeSimpleMembership] 
    public class AccountController : Controller
    {
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                var userName = "";
                using (
                    SqlConnection conn =
                        new SqlConnection(
                            System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"]
                                .ConnectionString))
                {
                    try
                    {
                        conn.Open();
                        var cmd =
                            new SqlCommand(
                                "select UserName from UserProfile where UserName = '" + model.UserName + "'", conn);
                        var rdr = cmd.ExecuteReader();

                        // print the Title of each Movie
                        while (rdr.Read())
                        {
                            userName += rdr[0];
                        }
                    }
                    catch(Exception)
                    {
                        // ignored
                    }
                }
                HttpCookie cookieUserName = new HttpCookie("UserName")
                {
                    Value = userName,
                    Expires = DateTime.Now.AddDays(1d)
                };
                Response.Cookies.Add(cookieUserName);
                return RedirectToLocal(returnUrl);
            }
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            if (Request.Cookies["UserName"] != null)
            {
                var cookieUserName = new HttpCookie("UserName") {Expires = DateTime.Now.AddDays(-1d)};
                Response.Cookies.Add(cookieUserName);
            }
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (!ModelState.IsValid) return View(model);
            try
            {
                WebSecurity.CreateUserAndAccount(
                    model.UserName,
                    model.Password,
                    new
                    {
                           
                    },
                    true);
                string queryConfirmationToken = "select ConfirmationToken from webpages_Membership where UserId = (SELECT UserId FROM  UserProfile where UserName='" + model.UserName + "'  )";
                SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                SqlDataReader rdr = null;
                string confirmationToken = "";
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(queryConfirmationToken, conn);
                    rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        confirmationToken += rdr[0];
                    }
                }
                finally
                {
                    if (rdr != null)
                    {
                        rdr.Close();
                    }
                    conn.Close();
                }
                var fromAddress = new MailAddress("noreply@vifuture.com", "Vifuture Account Team");
                var toAddress = new MailAddress("lukezeng@live.com", model.UserName);
                var toAddressRc = new MailAddress(model.UserName, model.UserName);
                const string fromPassword = "head/16/fan";
                const string subject = "Welcome to Luke's Future";
                string body = "http://www.Vifuture.com/account/confirmaccount?username=" + model.UserName + "&confirmToken=" + confirmationToken;


                //Sending Email to greet the new registered user 
                var smtp = new SmtpClient
                {
                    Host = "mail.foxglove.arvixe.com",
                    Port = 587,
                    EnableSsl = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                using (var messageRc = new MailMessage(fromAddress, toAddressRc)
                {
                    Subject = subject,
                    Body = body
                })

                {
                    smtp.Send(message);
                    smtp.Send(messageRc);
                }
                return RedirectToAction("Registered", "Account", new {model.UserName });
            }
            catch (MembershipCreateUserException e)
            {
                ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
            }
            return View(model);
        }

        //
        //Post: /Account/Registered Welcome Page
        [AllowAnonymous]
        public ActionResult Registered(string userName)
        {
            ViewBag.UserName = userName;
            return View();

        }

        //
        //Get: /Account/ComfirmAccount
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ConfirmAccount(string username, string confirmToken)
        {
            if (WebSecurity.ConfirmAccount(username, confirmToken))
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                return RedirectPermanent("http://www.google.com");
            }
        }

        //
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", String.Format("Unable to create local account. An account with the name \"{0}\" may already exist.", User.Identity.Name));
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            var result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if (result.ExtraData.Keys.Contains("accesstoken"))
            {
                Session["facebooktoken"] = result.ExtraData["accesstoken"];
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                var userName = "";
                using (
                    var conn =
                        new SqlConnection(
                            System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"]
                                .ConnectionString))
                {
                    SqlDataReader rdr = null;
                    try
                    {
                        // 2. Open the connection
                        conn.Open();

                        // 3. Pass the connection to a command object
                        var cmd =
                            new SqlCommand(
                                "select UserName from UserProfile where UserID = (select UserID from webpages_OAuthMembership WHERE ProviderUserID = '" +
                                result.ProviderUserId + "')", conn);
                        rdr = cmd.ExecuteReader();

                        // print the Title of each Movie
                        while (rdr.Read())
                        {
                            userName += rdr[0];
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
                var cookieUserName = new HttpCookie("UserName")
                {
                    Value = userName,
                    Expires = DateTime.Now.AddDays(1d)
                };
                Response.Cookies.Add(cookieUserName);
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // If the current user is logged in add the new account
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // User is new, ask for their desired membership name
                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                //return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
                HttpCookie cookieUserName = new HttpCookie("UserName")
                {
                    Value = "abcc",
                    Expires = DateTime.Now.AddDays(1d)
                };
                Response.Cookies.Add(cookieUserName);
                return View("ExternalLoginConfirmation", new RegisterExternalLoginModel
                {
                    UserName = result.UserName,
                    ExternalLoginData = loginData,
                    FullName = result.ExtraData["name"],
                    Link = result.ExtraData["link"]
                });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider;
            string providerUserId;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Insert a new user into the database
                using (var db = new UsersContext())
                {
                    var user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
                    // Check if user already exists
                    if (user == null)
                    {
                        // Insert name into the profile table
                        var newUser = db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
                        db.SaveChanges();

                        var client = new Facebook.FacebookClient(Session["facebooktoken"].ToString());
                        dynamic response = client.Get("me", new { fields = "verified" });
                        bool facebookVerified = response.ContainsKey("verified") && response["verified"];


                        db.ExternalUsers.Add(new ExternalUserInformation
                        {
                            UserId = newUser.UserId,
                            FullName = model.FullName,
                            Link = model.Link,
                            Verified = facebookVerified
                        });
                        db.SaveChanges();

                        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
                        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
                    }
                }
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/ExternalLoginFailure

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            List<ExternalLogin> externalLogins = (from account in accounts
                let clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider)
                select new ExternalLogin
                {
                    Provider = account.Provider, ProviderDisplayName = clientData.DisplayName, ProviderUserId = account.ProviderUserId,
                }).ToList();

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
