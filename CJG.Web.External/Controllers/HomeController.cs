using System.Web.Mvc;

namespace CJG.Web.External.Controllers
{
	public class HomeController : Controller
	{
		// GET: Home
		public ActionResult Index()
		{
			return RedirectPermanent("/Ext/Home");
		}
	}
}