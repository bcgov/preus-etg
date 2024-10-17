using System.Configuration;

namespace CJG.Infrastructure.BCeID.WebService
{
	public static class Configuration
	{
		public static class AppSettings
		{
			public static bool BCeIDWebService_Simulator
			{
				get
				{
					bool.TryParse(ConfigurationManager.AppSettings["BCeIDWebService_Simulator"], out bool simulator);
					return simulator;
				}
			}

			public static string BCeIDWebService_UserName
			{
				get
				{
					return ConfigurationManager.AppSettings["BCeIDWebService_UserName"];
				}
			}

			public static string BCeIDWebService_Password
			{
				get
				{
					return ConfigurationManager.AppSettings["BCeIDWebService_Password"];
				}
			}

			public static string BCeIDWebService_OnlineServiceId
			{
				get
				{
					return ConfigurationManager.AppSettings["BCeIDWebService_OnlineServiceId"];
				}
			}

			/// <summary>
			/// get - The number of seconds before a request for BCeID account information will timeout.
			/// </summary>
			public static int BCeIDWebService_Timeout
			{
				get
				{
					var timeout = 5000;

					int.TryParse(ConfigurationManager.AppSettings["BCeIDWebService_Timeout"], out timeout);

					return timeout;
				}
			}
		}
	}
}