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

            //string sqlConnectionString = "Data Source=chezwang-pc;Initial Catalog=SignalRScalingDB;Integrated Security=True;";

            //GlobalHost.DependencyResolver.UseSqlServer(sqlConnectionString);


            //defaule signalr proxy url is /signalr
            app.MapSignalR();


            //var hubConfiguration = new HubConfiguration();
            //hubConfiguration.EnableDetailedErrors = true;

            ////set true, if you want to refer to a physical file
            //hubConfiguration.EnableJavaScriptProxies = false;            

            // Any connection or hub wire up and configuration should go here
            //GlobalHost.HubPipeline.AddModule(new ErrorHandlingPipelineModule());

            ////define the custom url.
            //app.MapSignalR("/customURL", hubConfiguration);

            //enforce an authentication requirement for all hubs.
            //GlobalHost.HubPipeline.RequireAuthentication(); 

            // Make long polling connections wait a maximum of 110 seconds for a
            // response. When that time expires, trigger a timeout command and
            // make the client reconnect.
            //GlobalHost.Configuration.ConnectionTimeout = TimeSpan.FromSeconds(110);

            // Wait a maximum of 30 seconds after a transport connection is lost
            // before raising the Disconnected event to terminate the SignalR connection.
            //GlobalHost.Configuration.DisconnectTimeout = TimeSpan.FromSeconds(30);

            // For transports other than long polling, send a keepalive packet every
            // 10 seconds. 
            // This value must be no more than 1/3 of the DisconnectTimeout value.
            //GlobalHost.Configuration.KeepAlive = TimeSpan.FromSeconds(10);

        }
    }
}