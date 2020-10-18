# Simple client/server chat application

This project is split in two applications:

A "Server" application, that is a rest service that manages the route messages and handshake connection.

A "Client", that is a simple html/javascript file that connects to the server to send and receive messages.
 
Follow the instructions bellow to run the application.

---

### Tools requirements

.Net Core SDK 3.1

To run the Chat server, follow the steps bellow;

1 Find the take.sln file on the root repository.<br/>
2 Open on a visual studio 2019<br/>
3 Press the visual studio play button to run the project under the iisexpress. (Make sure the project will start on http://localhost:55242/ address.)

To run the client, follow the steps bellow:

1 Find the chat.html file on the root repository<br/>
2 Open the file on a browser that supports websocket connections.<br/>

Notes:
Every instance from the chat.html stands for one client application.


---

### Initializing the application by Visual Studio.
Server:
Build the project and run it under the issexpress. (Visual studio default option)

Client:
Open the chat.html file on a browser that support websocket connections.

Note:
This is the first version of the application and some features are still not implemented.
This application is not yet availlable on docker
The client is not implemented using the modern js framework 

