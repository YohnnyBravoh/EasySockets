# EasySockets
A wrapper for sockets making networking easy!

## Installation
Simply reference the 'EasySockets.dll' file in your project.

## Usage
The following example can be applied to both models "Server" and "Client".
```
Server server = new Server();
server.ClientAccepted += Server_ClientAccepted;
server.DataReceived += Server_DataReceived;
server.ClientDisconnected += Server_ClientDisconnected;

server.Encryption = new RijndaelEncryption("encryptionKey");
server.Compression = new GZip();

ServerController.Listen(server, 100);
```

## Credits
* Tyrone (organisation)
* Yohnny Bravoh

## License
This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details
