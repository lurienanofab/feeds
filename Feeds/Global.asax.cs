using LNF;
using LNF.Repository;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Feeds
{
    public class MvcApplication : HttpApplication
    {
        private IUnitOfWork uow;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_BeginRequest()
        {
            uow = Providers.DataAccess.StartUnitOfWork();
        }

        protected void Application_EndRequest()
        {
            uow.Dispose();
        }
    }
}
