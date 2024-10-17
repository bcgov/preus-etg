using CJG.Application.Services;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.SessionState;

namespace CJG.Application.SiteMinderSimulatorModule
{
    /// <summary>
    /// The purpose of this class is to add the relevant ServerHeaders to the request to simulate what SiteMinder does.
    /// It's based on code found here: http://forums.asp.net/post/1788294.aspx
    /// </summary>
    public class SiteMinderSimulatorModule : IHttpModule
    {
        /// <summary>
        /// These are the MethodInfo items for the relevant methods needed to add the UserGuid to the Server Variables.
        /// Keeping a static reference here reduces the performance impact of using reflection.
        /// </summary>
        private static Dictionary<string, MethodInfo> _methods = new Dictionary<string, MethodInfo>();

        public void Init(HttpApplication application)
        {
            application.PostAcquireRequestState += Application_PostAcquireRequestState;
        }

        public void Dispose()
        {

        }

        private void Application_PostAcquireRequestState(object sender, EventArgs e)
        {
            ProcessRequest(((HttpApplication)sender).Request.ServerVariables, HttpContext.Current);
        }

        /// <summary>
        /// We check to see if the UserGuid exists in the session.
        /// If so, we add it to the Server Variables with the appropriate name to be
        /// consistent with SiteMinder.
        /// </summary>
        /// <param name="serverVariables"></param>
        /// <param name="httpContext"></param>
        private void ProcessRequest(NameValueCollection serverVariables, HttpContext httpContext)
        {
            // Microsoft Identity may already have the user logged in.  If it does then update the session variables.
            var userGuid = GetUserGuidFromSession(httpContext.Session) ?? httpContext.User.GetBCeIdGuid();
            var userType = GetUserTypeFromSession(httpContext.Session) ?? (userGuid.HasValue ? "Internal" : null);
            var userName = GetUserNameFromSession(httpContext.Session) ?? httpContext.User.GetUserName();

            string redirectPath;

            if (userGuid.HasValue && !String.IsNullOrWhiteSpace(userType))
            {
                AddValuesToServerVariables(userGuid.Value, userType, userName, serverVariables);
            }
            else if (!String.IsNullOrWhiteSpace(userName) && !String.IsNullOrWhiteSpace(userType))
            {
                AddValuesToServerVariables(null, userType, userName, serverVariables);
            }
            else if (ShouldRedirectToLogInPage(httpContext.Request.Path, out redirectPath))
            {
                RedirectToLogInPage(httpContext, redirectPath);
            }
        }

        private static bool ShouldRedirectToLogInPage(string requestPath, out string redirectPath)
        {
            redirectPath = null;
            bool shouldRedirect = false;

            if (String.Compare(requestPath, Configuration.ExternalLogInPage, true) != 0 && String.Compare(requestPath, Configuration.InternalLogInPage, true) != 0)
            {
                redirectPath = "/";

                if (requestPath.StartsWith("/Ext", StringComparison.InvariantCultureIgnoreCase))
                {
                    redirectPath = Configuration.ExternalLogInPage;
                    shouldRedirect = true;
                }
                else if (requestPath.StartsWith("/Int", StringComparison.InvariantCultureIgnoreCase))
                {
                    redirectPath = Configuration.InternalLogInPage;
                    shouldRedirect = true;
                }
            }

            return shouldRedirect;
        }

        private static void RedirectToLogInPage(HttpContext httpContext, string logInPage)
        {
            try
            {
                httpContext.Response.Redirect(logInPage, false);
                httpContext.ApplicationInstance.CompleteRequest();
            }
            catch (ThreadAbortException)
            {
                // do nothing
            }
        }

        private static Guid? GetUserGuidFromSession(HttpSessionState session)
        {
            return session == null ? null : (Guid?)session[Services.Constants.SiteMinderSimulator_UserGuid_SessionVariableName];
        }

        private string GetUserNameFromSession(HttpSessionState session)
        {
            return session == null ? null : (string)session[Services.Constants.SiteMinderSimulator_UserName_SessionVariableName];
        }

        private string GetUserTypeFromSession(HttpSessionState session)
        {
            return session == null ? null : (string)session[Services.Constants.SiteMinderSimulator_UserType_SessionVariableName];
        }

        private static void AddValuesToServerVariables(Guid? userGuid, string userType, string userName, NameValueCollection serverVariables)
        {
            if (_methods.Count == 0)
            {
                PopulateMethodInfos(serverVariables);
            }

            string[] values = new string[2];

            _methods[Constants.MethodNames.MakeReadWrite].Invoke(serverVariables, null);

            if (userGuid.HasValue && !String.IsNullOrWhiteSpace(userType))
            {
                values[0] = Services.Constants.SiteMinder_UserGuid_ServerVariableName;
                values[1] = userGuid.Value.ToString("N");
                _methods[Constants.MethodNames.AddStatic].Invoke(serverVariables, values);

                values[0] = Services.Constants.SiteMinder_UserType_ServerVariableName;
                values[1] = userType;
                _methods[Constants.MethodNames.AddStatic].Invoke(serverVariables, values);
            }

            if (!string.IsNullOrWhiteSpace(userName) && !String.IsNullOrWhiteSpace(userType))
            {
                values[0] = Services.Constants.SiteMinder_UserName_ServerVariableName;
                values[1] = userName;
                _methods[Constants.MethodNames.AddStatic].Invoke(serverVariables, values);

                values[0] = Services.Constants.SiteMinder_UserType_ServerVariableName;
                values[1] = userType;
                _methods[Constants.MethodNames.AddStatic].Invoke(serverVariables, values);
            }

            _methods[Constants.MethodNames.MakeReadOnly].Invoke(serverVariables, null);
        }

        private static void PopulateMethodInfos(NameValueCollection serverVariables)
        {
            Type type = serverVariables.GetType();
            var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
            _methods.Add(Constants.MethodNames.AddStatic, type.GetMethod(Constants.MethodNames.AddStatic, bindingFlags));
            _methods.Add(Constants.MethodNames.MakeReadWrite, type.GetMethod(Constants.MethodNames.MakeReadWrite, bindingFlags));
            _methods.Add(Constants.MethodNames.MakeReadOnly, type.GetMethod(Constants.MethodNames.MakeReadOnly, bindingFlags));
        }

    }
}
