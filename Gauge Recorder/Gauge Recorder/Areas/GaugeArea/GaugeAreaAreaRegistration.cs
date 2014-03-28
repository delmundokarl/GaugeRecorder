using System.Web.Mvc;

namespace Gauge_Recorder.Areas.GaugeArea
{
    public class GaugeAreaAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "GaugeArea";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "GaugeArea_default",
                "GaugeArea/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
