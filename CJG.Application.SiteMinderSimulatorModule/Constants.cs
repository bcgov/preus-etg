namespace CJG.Application.SiteMinderSimulatorModule
{
    /// <summary>
    /// This class contains all the constants used in the module, to help provide context as to their purpose.
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// This inner class is used to group the method names for the ServerVariables instance that is being altered.
        /// </summary>
        internal static class MethodNames
        {
            internal static readonly string MakeReadWrite = "MakeReadWrite";
            internal static readonly string MakeReadOnly = "MakeReadOnly";
            internal static readonly string AddStatic = "AddStatic";
        }

        /// <summary>
        /// This is the key in appSettings used to contain the external login page.
        /// </summary>
        internal static readonly string ExternalLogInPageAppSettingsKey = "SiteMinderSimulatorModule_ExternalLogInPage";

        /// <summary>
        /// This is the key in appSettings used to contain the internal login page.
        /// </summary>
        internal static readonly string InternalLogInPageAppSettingsKey = "SiteMinderSimulatorModule_InternalLogInPage";

        /// <summary>
        /// This is the key in appSettings used to contain the list of areas protected by SiteMinder.
        /// </summary>
        internal static readonly string SiteMinderSimulatorModule_SecureAreas = "SiteMinderSimulatorModule_SecureAreas";
    }
}