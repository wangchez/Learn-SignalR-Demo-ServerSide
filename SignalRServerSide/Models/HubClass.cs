using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace SignalRServerSide.Models
{
    //[Authorize]
    //[Authorize(Users = "123")]
    //[Authorize(RequireOutgoing=false)]
    //[CustomAuthorize]
    [HubName("HubCenter")] // If this attribute is not setting, the class name will be changed to camelcase convention.
    public class HubClass : Hub
    {
        private static int connectionCount = 0;

        // If this attribute is not setting, the method name will be changed to camelcase convention.
        [HubMethodName("SendMessage")]
        public void SendMessage(string name, string message, string call_type) // The method that clients can call.
        {

            //if(Context.User.Identity.IsAuthenticated) use the authentication information.               

            switch (call_type)
            {
                //To call client method from the server. There is no return and case insensitive.
                case "1": Clients.All.broadcastMessage(name, message);
                    break;
                case "2": Clients.Caller.broadcastMessage(name, message);
                    break;
                case "3": Clients.Others.broadcastMessage(name, message);
                    break;
                case "4": Clients.Group(name).broadcastMessage(name, message);
                    break;
            }
        }

        public void CloseFlagTrigger(string name)
        {
            Clients.All.closeChat(name);
        }

        public void CallFromConsole(dynamic model)
        {
            try
            {
                Clients.All.broadcastMessage(model.Name, model.Message);
            }
            catch(Exception ex)
            {
                // handle error
            }
        }

        public class ClientModel
        {
            public string Name { get; set; }

            public string Message { get; set; }
        }

        public Task JoinGroup(string groupName)
        {
            return Groups.Add(Context.ConnectionId, groupName);
        }

        public Task LeaveGroup(string groupName)
        {
            return Groups.Remove(Context.ConnectionId, groupName);
        }

        public void GetLogInfo(LogInfo info)
        {
            Clients.Caller.showLogInfo(info);
        }

        public override Task OnConnected()
        {
            // Add your own code here.
            // For example: in a chat application, record the association between
            // the current connection ID and user name, and mark the user as online.
            // After the code in this method completes, the client is informed that
            // the connection is established; for example, in a JavaScript client,
            // the start().done callback is executed.
            connectionCount++;
            Clients.All.updateConnectCount(connectionCount);
            Clients.Others.showLogInfo(new LogInfo { ClientId = Context.ConnectionId, LogMessage = "a connection is Coming"});
            Console.WriteLine("OnConnected");
            return base.OnConnected();
        }

        public override Task OnDisconnected()
        {
            // Add your own code here.
            // For example: in a chat application, mark the user as offline, 
            // delete the association between the current connection id and user name.
            connectionCount--;
            Clients.All.updateConnectCount(connectionCount);
            Clients.Others.showLogInfo(new LogInfo { ClientId = Context.ConnectionId, LogMessage = "a connection is leaving" });
            Console.WriteLine("OnDisconnected");
            return base.OnDisconnected();
        }

        public override Task OnReconnected()
        {
            // Add your own code here.
            // For example: in a chat application, you might have marked the
            // user as offline after a period of inactivity; in that case 
            // mark the user as online again.

            Console.WriteLine("OnReconnected.");

            return base.OnReconnected();
        }
    }

    public class LogInfo
    {
        public string ClientId { get; set; }

        public string LogMessage { get; set; }
    }
}