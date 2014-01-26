using System.Web.Mvc;

namespace Anyname_Projects.Areas.Home
{
    public class HomeAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Home";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Home_default",
                "Home/{controller}/{action}/{id}",
                new { action = "Home", id = UrlParameter.Optional },
                namespaces: new[] { "Anyname_Projects.Areas.Home.Controllers" }
            );
        }
    }
}
