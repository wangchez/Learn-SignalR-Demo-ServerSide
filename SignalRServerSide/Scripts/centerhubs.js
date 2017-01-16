function messageModel(name, msg) {
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

var chatProxy;
$(function () {
    // Declare a proxy to reference the hub. 
    //$.connection.hub.url = "/customURL";
    //$.connection.hub.qs = { 'version': '1.0' };
    //$.connection.hub.logging = true;
    chatProxy = $.connection.HubCenter;   
    chatProxy.logging = true;

    // Create a function that the hub can call to broadcast messages.
    chatProxy.client.newMessage = function (name, message) {
        messageModels.history_stack.push(new messageModel(name, message));
    };

    chatProxy.client.updateConnectCount = function (count) {
        messageModels.connect_count(count);
    }

    chatProxy.client.closeChat = function (groupName) {
        if (groupName == messageModels.current_client())
            messageModels.close_flag(false);
    }

    chatProxy.client.showLogInfo = function (info) {
        alert("client id:" + info.ClientId + ", " + info.LogMessage);
    }

    //Alternate way to define method on client (with the generated proxy)
    //$.extend(chatProxy.client, {
    //    newMessage: function(name, message) {
    //        messageModels.history_stack.push(new clientModel(name, message));
    //    }
    //});

    //without generate proxy
    //var connection = $.hubConnection("/customURL", { useDefaultPath: false });
    //var connection = $.hubConnection() equal to $.connection.hub;
    //connection.qs = { 'version' : '1.0' };
    //chatProxy = connection.createHubProxy('HubCenter');

    //chatProxy.on('newMessage', function (name, message) {
    //    messageModels.history_stack.push(new clientModel(name, message));
    //});

    messageModels.current_client(prompt('Enter your name:', ''));
    chatProxy.state.name = messageModels.current_client();
    //without generate proxy
    //connection.start().done(function(){}).fail(function(){});

    //{ transport: 'longPolling' }
    $.connection.hub.start().done(function () {

        console.log("Connected, transport = " + $.connection.hub.transport.name);

        chatProxy.server.joinGroup();
        chatProxy.server.sendLogInfo({ ClientId: chatProxy.connection.id, LogMessage: "I am coming." });

        //without generate proxy
        //chatProxy.invoke('sendLogInfo', { ClientId: chatProxy.connection.id, LogMessage: "I am coming." });

        //$('#send_all').click(function () {
        //    callToServer(1);
        //});
        //$('#send_caller').click(function () {
        //    callToServer(2);
        //});
        //$('#send_others').click(function () {
        //    callToServer(3);
        //});
        //$('#send_group').click(function () {
        //    callToServer(4);
        //});
    });
});

$('#close_chat').click(function () {
    messageModels.close_flag(false);
    chatProxy.server.triggerClosedFlag(messageModels.current_client());
});

function callToServer(type) {
    $('#message').focus();

    // Call the Send method on the hub. 
    chatProxy.server.SendMessage($('#message').val(), type).fail(function (e) {
        if (e.source === 'HubException') {
            console.log(e.message + ' : ' + e.data.user);
        }
    });

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