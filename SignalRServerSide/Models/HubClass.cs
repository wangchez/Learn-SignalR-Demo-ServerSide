using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Diagnostics;

namespace SignalRServerSide.Models
{
    //[Authorize]
    //[Authorize(Users = "123")]
    //[Authorize(RequireOutgoing = false)]
    //[CustomAuthorize]
    [HubName("HubCenter")] // If this attribute is not setting, the class name will be changed to camelcase convention.
    public class HubClass : Hub
    {
        private static int connectionCount = 0;

        // If this attribute is not setting, the method name will be changed to camelcase convention.
        [HubMethodName("SendMessage")]
        public void SendMessage(string message, string dispatchType) // The method that clients can call.
        {

            //if(Context.User.Identity.IsAuthenticated) use the authentication information.               

            switch (dispatchType)
            {
                //To call client method from the server. There is no return and case insensitive.
                case "1":
                    Clients.All.NewMessage(Clients.Caller.name, message);
                    break;
                case "2":
                    Clients.Caller.NewMessage(Clients.Caller.name, message);
                    break;
                case "3":
                    Clients.Client(Context.ConnectionId).NewMessage(Clients.Caller.name, message);
                    //Clients.Clients(new List<string> {  Context.ConnectionId }).NewMessage(Clients.Caller.name, message);
                    break;
                case "4":
                    Clients.User(Context.User.Identity.Name).NewMessage(Clients.Caller.name, message);
                    //Clients.Users(new string[] { Context.User.Identity.Name }).NewMessage(Clients.Caller.name, message);
                    break;
                case "5":
                    string methodToCall = "NewMessage";
                    IClientProxy proxy = Clients.Others;
                    proxy.Invoke(methodToCall, Clients.Caller.name, message);
                    //Clients.AllExcept(Context.ConnectionId).NewMessage(Clients.Caller.name, message);
                    break;
                case "6":
                    Clients.Groups(new List<string> { "TestGroup" }).NewMessage(Clients.Caller.name, message);

                    //All connected clients in a specified group except the specified clients, identified by connection ID.
                    Clients.Group("TestGroup", Context.ConnectionId).NewMessage(Clients.Caller.name, message);
                    //Clients.OthersInGroup("TestGroup").NewMessage(Clients.Caller.name, message);
                    break;
                default:
                    throw new HubException("No such dispatch type.", new { user = Context.User.Identity.Name, message = dispatchType });
            }
        }

        public void TriggerClosedFlag(string name)
        {
            Clients.All.CloseChat(name);
        }

        public void CallFromConsole(dynamic model)
        {
            try
            {
                Clients.All.NewMessage(model.Name, model.Message);
            }
            catch(Exception ex)
            {
                throw new HubException("This message will flow to the client", new { user = Context.User.Identity.Name, message = ex.GetBaseException().Message });
            }
        }

        public Task JoinGroup()
        {
            return Groups.Add(Context.ConnectionId, "TestGroup");
        }

        public Task LeaveGroup(string groupName)
        {
            return Groups.Remove(Context.ConnectionId, groupName);
        }

        public void SendLogInfo(LogInfo info)
        {
            Clients.Caller.ShowLogInfo(info);
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
            Clients.All.UpdateConnectCount(connectionCount);
            Clients.Others.ShowLogInfo(new LogInfo { ClientId = Context.ConnectionId, Name = Context.User.Identity.Name, LogMessage = "a new guest come in"});
            Debug.WriteLine("OnConnected: " + Context.ConnectionId);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            // Add your own code here.
            // For example: in a chat application, mark the user as offline, 
            // delete the association between the current connection id and user name.
            connectionCount--;
            Clients.All.UpdateConnectCount(connectionCount);
            Clients.Others.ShowLogInfo(new LogInfo { ClientId = Context.ConnectionId, Name = Context.User.Identity.Name, LogMessage = "a guest leave" });
            Debug.WriteLine("OnDisconnected: " + Context.ConnectionId);

            // stopCalled
            // true: if stop was called on the client closing the connection gracefully
            // false: if the connection has been lost for longer than the disconnect timeout of configuration 
            return base.OnDisconnected(stopCalled); 
        }

        public override Task OnReconnected()
        {
            // Add your own code here.
            // For example: in a chat application, you might have marked the
            // user as offline after a period of inactivity; in that case 
            // mark the user as online again.

            Debug.WriteLine("OnReconnected: " + Context.ConnectionId);
            return base.OnReconnected();
        }
    }

    public class LogInfo
    {
        public string ClientId { get; set; }

        public string Name { get; set; }

        public string LogMessage { get; set; }
    }
}