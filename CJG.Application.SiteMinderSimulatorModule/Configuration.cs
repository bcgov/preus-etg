using System;
using System.Configuration;

namespace CJG.Application.SiteMinderSimulatorModule
{
    internal static class Configuration
    {
        /// <summary>
        /// This is the path to the external login page within the website.
        /// </summary>
        internal static string ExternalLogInPage
        {
            get
            {
                var loginPage = ConfigurationManager.AppSettings[Constants.ExternalLogInPageAppSettingsKey];
                if (String.IsNullOrWhiteSpace(loginPage))
                {
                    loginPage = "/";
                }
                return loginPage;
            }
        }

        /// <summary>
        /// This is the path to the internal login page within the website.
        /// </summary>
        internal static string InternalLogInPage
        {
            get
            {
                var loginPage = ConfigurationManager.AppSettings[Constants.InternalLogInPageAppSettingsKey];
                if (String.IsNullOrWhiteSpace(loginPage))
                {
                    loginPage = "/";
                }
                return loginPage;
            }
        }

        internal static string[] SecureAreas
        {
            get
            {
                var secureAreas = ConfigurationManager.AppSettings[Constants.SiteMinderSimulatorModule_SecureAreas];
                if (secureAreas == null)
                {
                    return new string[] { };
                }
                else
                {
                    return secureAreas.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                }
            }
        }
    }
}
