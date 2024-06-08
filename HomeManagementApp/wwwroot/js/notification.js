var connection = new signalR.HubConnectionBuilder().withUrl("/notificationHub").build();

connection.on("ReceiveMessage", function (message) {
    alert("New message: " + message);
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});
