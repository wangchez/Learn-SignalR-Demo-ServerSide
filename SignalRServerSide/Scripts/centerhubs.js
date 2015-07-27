function clientModel(name, msg) {
    this.client_name = ko.observable(name);
    this.say_what = ko.observable(msg);
}
var messageModels = {
    history_stack: ko.observableArray([]),
    current_client: ko.observable(),
    connect_count: ko.observable(0),
    close_flag: ko.observable(true)
}

ko.applyBindings(messageModels);

var chat;
$(function () {
    // Declare a proxy to reference the hub. 
    //$.connection.hub.url = "/customURL";
    chat = $.connection.HubCenter;   
    chat.logging = true;

    // Create a function that the hub can call to broadcast messages.
    chat.client.broadcastMessage = function (name, message) {
        messageModels.history_stack.push(new clientModel(name, message));
    };

    chat.client.updateConnectCount = function (count) {
        messageModels.connect_count(count);
    }

    chat.client.closeChat = function (groupName) {
        if (groupName == messageModels.current_client())
        messageModels.close_flag(false);
    }

    chat.client.showLogInfo = function (info) {
        alert("client id:" + info.ClientId + ", " + info.LogMessage);
    }

    //without generate proxy
    //connection = $.hubConnection("/customURL", { useDefaultPath: false });
    //connection = $.hubConnection();
    //hubProxy = connection.createHubProxy('HubCenter');

    //hubProxy.on('broadcastMessage', function (name, message) {
    //    messageModels.history_stack.push(new clientModel(name, message));
    //});

    messageModels.current_client(prompt('Enter your name:', ''));

    //{ transport: 'longPolling' }
    $.connection.hub.start().done(function () {
        chat.server.joinGroup(messageModels.current_client());
        chat.server.getLogInfo({ ClientId: chat.connection.id, LogMessage: "I am coming." });

        $('#send_all').click(function () {
            callToServer(1);
        });
        $('#send_caller').click(function () {
            callToServer(2);
        });
        $('#send_others').click(function () {
            callToServer(3);
        });
        $('#send_group').click(function () {
            callToServer(4);
        });
    });
});

$('#close_chat').click(function () {
    messageModels.close_flag(false);
    chat.server.closeFlagTrigger(messageModels.current_client());
});

function callToServer(type) {
    $('#message').focus();

    // Call the Send method on the hub. 
    chat.server.SendMessage(messageModels.current_client(), $('#message').val(), type);

    //without generate proxy
    //hubProxy.invoke('SendMessage', messageModels.current_client(), $('#message').val(), type);


    // Clear text box and reset focus for next comment. 
    $('#message').val('').focus();
}

//when the connection has disconnected.
$.connection.hub.disconnected(function () {
    alert("disconnect.");
});

// before any data is sent over the connection.
$.connection.hub.starting(function () {
});

//when any data is received on the connection.
$.connection.hub.received(function (log) {

});

//when the client detects a slow or frequently dropping connection.
$.connection.hub.connectionSlow(function () {
    alert("connect slow.");
});

//when the underlying transport begins reconnecting.
$.connection.hub.reconnecting(function () {
    alert("reconnecting.");
});

//when the underlying transport has reconnected.
$.connection.hub.reconnected(function () {
    alert("reconnected.");
});

//when the connection state changes.
// 0: connecting, 1: connected, 2: Reconnecting, 3:Reconnected, 4: disconnected
$.connection.hub.stateChanged(function (log) {
});