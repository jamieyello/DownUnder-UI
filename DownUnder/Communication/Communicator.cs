//using Lidgren.Network;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace DownUnder.Content.Communication
//{
//    // http://genericgamedev.com/tutorials/lidgren-network-an-introduction-to-networking-in-csharp-games/
//    enum CommunicatorType
//    {
//        server,
//        client
//    }

//    class Communicator
//    {
//        NetServer server;
//        NetClient client;
//        NetPeerConfiguration config = new NetPeerConfiguration("DownUnder")
//        {
//            Port = 17287
//        };

        

//        public Communicator()
//        {
//            server = new NetServer(config);
//            client = new NetClient(config);
//        }

//        public void Start(CommunicatorType com_type)
//        {
//            Close();
//            if (com_type == CommunicatorType.client) StartClient();
//            if (com_type == CommunicatorType.server) StartServer();
//        }

//        public void Close()
//        {
//            CloseServer();
//            CloseClient();
//        }

//        void CloseServer()
//        {
//            if (server.Status == NetPeerStatus.Running)
//            {
//                server.Shutdown("Server shutting down.");
//            }
//            while (server.Status == NetPeerStatus.ShutdownRequested) { }
//        }

//        void StartServer()
//        {
//            server.Start();
//            while (server.Status != NetPeerStatus.Running) { }
//        }

//        void CloseClient()
//        {
//            if (client.Status == NetPeerStatus.Running)
//            {
//                client.Shutdown("Disconnecting.");
//            }
//            while (client.Status == NetPeerStatus.ShutdownRequested) { }
//        }

//        void StartClient()
//        {
//            client.Start();
//            client.Connect("24.33.81.197", config.Port);
//            while (client.Status != NetPeerStatus.Running) { }
//        }

//        public void ReadIncomingData()
//        {
//            NetIncomingMessage message;
//            if (server.Status == NetPeerStatus.Running)
//            {
//                while ((message = server.ReadMessage()) != null)
//                {
//                    ProcessMessage(ref message);
//                }
//                return;
//            }

//            if (client.Status == NetPeerStatus.Running)
//            {
//                while ((message = client.ReadMessage()) != null)
//                {
//                    ProcessMessage(ref message);
//                }
//                return;
//            }

//            Debug.WriteLine("ReadIncomingData: Online communications aren't enabled.");
//        }

//        void ProcessMessage(ref NetIncomingMessage message)
//        {
//            switch (message.MessageType)
//            {
//                case NetIncomingMessageType.Data:
//                    Debug.WriteLine("Data read");
//                    // handle custom messages
//                    //var data = message.Read * ();
//                    break;

//                case NetIncomingMessageType.StatusChanged:
//                    // handle connection status messages
//                    Debug.WriteLine("Status Changed");
//                    //switch (message.SenderConnection.Status)
//                    //{
//                    /* .. */
//                    //}
//                    break;

//                case NetIncomingMessageType.DebugMessage:
//                    // handle debug messages
//                    // (only received when compiled in DEBUG mode)
//                    Debug.WriteLine(message.ReadString());
//                    break;

//                /* .. */
//                default:
//                    Debug.WriteLine("unhandled message with type: "
//                        + message.MessageType);
//                    Debug.WriteLine(message.ReadString());
//                    break;
//            }
//        }

//        public void SendMessage(String text)
//        {
//            var message = client.CreateMessage();
//            message.Write(text);

//            if (client.Status == NetPeerStatus.Running)
//            {
//                client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
//                return;
//            }

//            if (server.Status == NetPeerStatus.Running)
//            {
//                server.SendMessage(message, server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
//                return;
//            }

//            Debug.WriteLine("No online services running, can't send message.");
//        }
//    }
//}
