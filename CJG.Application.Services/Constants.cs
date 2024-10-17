namespace CJG.Application.Services
{
	/// <summary>
	/// This class contains constants that are needed in more than one project.
	/// </summary>
	public static class Constants
	{
		/// <summary>
		/// This is the name of the Server Variable for the user's GUID that is usually added by SiteMinder.
		/// </summary>
		public const string SiteMinder_UserGuid_ServerVariableName = "HTTP_SMGOV_USERGUID";
		/// <summary>
		/// This is the name of the Server Variable for the user's account type that is usually added by SiteMinder.
		/// </summary>
		public const string SiteMinder_UserType_ServerVariableName = "HTTP_SMGOV_USERTYPE";

		public const string SiteMinder_UserName_ServerVariableName = "HTTP_SM_USER";

		/// <summary>
		/// This is the parameter name used whenever we store the UserGuid in the Session.
		/// </summary>
		public const string SiteMinderSimulator_UserGuid_SessionVariableName = "SiteMinderSimulatorUserGuid";
		
		/// <summary>
		/// This is the parameter name used whenever we store the UserType in the Session.
		/// </summary>
		public const string SiteMinderSimulator_UserType_SessionVariableName = "SiteMinderSimulatorUserType";

		public const string SiteMinderSimulator_UserName_SessionVariableName = "SiteMinderSimulatorUserName";

		public const string BCeID_WebService_Logoff_URL = "BCeIDWebServiceLogoffURL";

		public const string BCeID_WebService_Logoff_Return_URL = "BCeIDWebServiceLogoffReturnURL";


		//public const string RADIOBUTTON_YES = "Yes";
		//public const string RADIOBUTTON_NO = "No";

		//move to constant file in core entities
		//public const string PostalCode_Validation_RegEx =
		//    "^[abceghjklmnprstvxyABCEGHJKLMNPRSTVXY][0-9][abceghjklmnprstvwxyzABCEGHJKLMNPRSTVWXYZ]\\s?[0-9][abceghjklmnprstvwxyzABCEGHJKLMNPRSTVWXYZ][0-9]$";
	}
}