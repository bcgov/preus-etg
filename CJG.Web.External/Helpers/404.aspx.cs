using System;
using System.Web.UI;

namespace CJG.Web.External.Helpers
{
    public partial class _404 : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect("/Error/NotFound");
        }
    }
}