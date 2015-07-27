using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Owin;
using Microsoft.Owin;
using Microsoft.AspNet.SignalR;
//using Microsoft.AspNet.SignalR.SqlServer;

[assembly: OwinStartup(typeof(SignalRServerSide.Models.Startup))]
namespace SignalRServerSide.Models
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            //string sqlConnectionString = "Data Source=user-pc\\sqlexpress;Initial Catalog=SignalRScalingDB;Integrated Security=True;";

            string sqlConnectionString = "Data Source=reprddbv01;Initial Catalog=SignalRScalingDB;User Id=cmsap;Password=19ssmc;";

            GlobalHost.DependencyResolver.UseSqlServer(sqlConnectionString);


            //defaule signalr proxy url is /signalr
            app.MapSignalR();


            //var hubConfiguration = new HubConfiguration();
            //hubConfiguration.EnableDetailedErrors = true;

            ////set true, if you want to refer to a physical file
            //hubConfiguration.EnableJavaScriptProxies = false;            

            ////define the custom url.
            //app.MapSignalR("/customURL", hubConfiguration);

            //enforce an authentication requirement for all hubs.
            //GlobalHost.HubPipeline.RequireAuthentication(); 

        }
    }
}