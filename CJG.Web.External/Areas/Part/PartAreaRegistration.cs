using System.Web.Mvc;

namespace CJG.Web.External.Areas.Part
{
    public class PartAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Part";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ParticipantInfo",
                "Part/{controller}/{action}/{invitationKey}",
                new { action = "ParticipantInfo", isExternal = true }
            );

            context.MapRoute(
                "Part_default",
                "Part/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional, isExternal = true }
            );
        }
    }
}