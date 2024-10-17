using System.Web.Mvc;

namespace CJG.Web.External.Helpers
{
    /// <summary>
    /// <paramtyperef name="ActionExecutingContextExtensions"/> static class, provides extension methods for <paramtyperef name="ActionExecutingContext"/> objects.
    /// </summary>
    public static class ActionExecutingContextExtensions
    {

        /// <summary>
        /// Get the action name for the current request.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetAction(this ActionExecutingContext context)
        {
            return (string)context.RouteData.Values["action"];
        }
    }
}