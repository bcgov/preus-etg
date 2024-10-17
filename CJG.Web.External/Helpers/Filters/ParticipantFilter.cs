using NLog;
using System;
using System.Web.Mvc;

namespace CJG.Web.External.Helpers.Filters
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
	public class ParticipantFilter : ActionFilterAttribute
	{
		private readonly ILogger _logger;

		public ParticipantFilter()
		{
			_logger = LogManager.GetCurrentClassLogger();
		}

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			try
			{
				filterContext.RouteData.Values["IsExternal"] = true;
			}
			catch (Exception e)
			{
				_logger.Error(e);
			}
		}
	}
}
