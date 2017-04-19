using Castle.Facilities.WcfIntegration;
using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace KishTestProject
{
    public class Global : System.Web.HttpApplication
    {
        static IWindsorContainer container;

        protected void Application_Start(object sender, EventArgs e)
        {
            container = new WindsorContainer();
            container.AddFacility<WcfFacility>();
            container.Install(new WindsorInstaller()); 
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}