using System.Web;
using System.Web.Mvc;

namespace CJG.Web.External.Helpers
{
    public static class ActionResultExtensions
    {
        public static ActionResult HttpStatusCodeResultWithAlert(this HttpStatusCodeResult result, HttpResponseBase response, AlertType alertType)
        {
            response.AddHeader("Alert-Type", string.Format("alert--{0}", alertType.ToString().ToLower()));
            return result;
        }
    }
}