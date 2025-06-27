const connection = new signalR.HubConnectionBuilder()
  .withUrl("/notificationHub")
  .withAutomaticReconnect()
  .build();

connection
  .start()
  .then(() => console.log("Connected to NotificationHub!"))
  .catch((err) => console.error("SignalR Connection Error: ", err));