﻿using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NUnit.Framework;
using SynthesisAPI.AssetManager;
using SynthesisAPI.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProtoBuf;
using Google.Protobuf;
using Mirabuf.Signal;
using Mirabuf;


namespace TestApi 
{
    [TestFixture]
    public static class TestServers
    {
        //Bad naming...
        static ConnectionMessage connectionRequest;
        static ConnectionMessage resourceOwnershipRequest;
        static ConnectionMessage terminateConnectionRequest;
        static ConnectionMessage heartbeat;

        static ConnectionMessage secondResourceOwnershipRequest;
        static ConnectionMessage secondTerminateConnectionRequest;

        static ConnectionMessage response;
        static ConnectionMessage secondResponse;
        static ByteString guid;
        static int generation;
        static ByteString secondGuid;
        static int secondGeneration;

        static TcpClient client;
        static int port = 13000;
        static int udpListenPort = 13001;
        static NetworkStream firstStream;
        static NetworkStream secondStream;

        static IPEndPoint remoteIpEndPoint;
        static UdpClient udpClient;

        static Thread heartbeatThread;

        [Test]
        public static void TestUpdating()
        {
            connectionRequest = new ConnectionMessage()
            {
                ConnectionRequest = new ConnectionMessage.Types.ConnectionRequest()
            };
            resourceOwnershipRequest = new ConnectionMessage()
            {
                ResourceOwnershipRequest = new ConnectionMessage.Types.ResourceOwnershipRequest()
                {
                    ResourceName = "Robot"
                }
            };
            heartbeat = new ConnectionMessage()
            {
                Heartbeat = new ConnectionMessage.Types.Heartbeat()
            };

            heartbeatThread = new Thread(() =>
            {
                Thread.Sleep(100);
                SendData(heartbeat, firstStream);
            });
            Thread udpThread = new Thread(() =>
            {
                UdpServerManager.Start();
                udpClient.JoinMulticastGroup(IPAddress.Parse("224.100.0.1"));
                System.Diagnostics.Debug.WriteLine("Start Udp stuff...");
                var data = UpdateSignals.Parser.ParseDelimitedFrom(new MemoryStream(udpClient.Receive(ref remoteIpEndPoint)));
                System.Diagnostics.Debug.WriteLine(data);
                System.Diagnostics.Debug.WriteLine("End Udp stuff");
                UdpServerManager.Stop();
            });

            RobotManager.Instance.AddSignalLayout(new Signals()
            {
                Info = new Info()
                {
                    Name = "Robot",
                    GUID = Guid.NewGuid().ToString()
                }
            });

            heartbeatThread.Start();
            TcpServerManager.Start();
            StartClient("127.0.0.1", ref firstStream);
            remoteIpEndPoint = new IPEndPoint(IPAddress.Any, udpListenPort);
            udpClient = new UdpClient(udpListenPort);
            

            System.Diagnostics.Debug.WriteLine("Sending Connection Request");
            SendData(connectionRequest, firstStream);

            response = ReadData(firstStream);
            if (response.MessageTypeCase == ConnectionMessage.MessageTypeOneofCase.ConnectionResonse && response.ConnectionResonse.Confirm)
            {
                System.Diagnostics.Debug.WriteLine("Sending Resource Ownership Request");
                SendData(resourceOwnershipRequest, firstStream);
            }

            response = ReadData(firstStream);
            System.Diagnostics.Debug.WriteLine(response);
            if (response.MessageTypeCase == ConnectionMessage.MessageTypeOneofCase.ResourceOwnershipResponse && response.ResourceOwnershipResponse.Confirm)
            {
                guid = response.ResourceOwnershipResponse.Guid;
                generation = response.ResourceOwnershipResponse.Generation;
            }
            System.Diagnostics.Debug.WriteLine("Guid is: {0}", guid);
            

            udpThread.Start();

            Thread.Sleep(2000);

            terminateConnectionRequest = new ConnectionMessage()
            {
                TerminateConnectionRequest = new ConnectionMessage.Types.TerminateConnectionRequest()
                {
                    ResourceName = "Robot",
                    Guid = guid,
                    Generation = generation
                }
            };
            System.Diagnostics.Debug.WriteLine("Sending Terminate Connection Request");
            SendData(terminateConnectionRequest, firstStream);
            if (response.MessageTypeCase == ConnectionMessage.MessageTypeOneofCase.TerminateConnectionResponse && response.TerminateConnectionResponse.Confirm)
            {
                System.Diagnostics.Debug.WriteLine("Termination Successful");
                StopClient(firstStream);
                TcpServerManager.Stop();
            }
            udpThread.Join();
        }

        [Test]
        public static void TestConnecting()
        {
            connectionRequest = new ConnectionMessage()
            {
                ConnectionRequest = new ConnectionMessage.Types.ConnectionRequest()
            };
            resourceOwnershipRequest = new ConnectionMessage()
            {
                ResourceOwnershipRequest = new ConnectionMessage.Types.ResourceOwnershipRequest()
                {
                    ResourceName = "Robot"
                }
            };
            heartbeat = new ConnectionMessage()
            {
                Heartbeat = new ConnectionMessage.Types.Heartbeat()
            };

            heartbeatThread = new Thread(() =>
            {
                Thread.Sleep(100);
                SendData(heartbeat, firstStream);
            });

            RobotManager.Instance.AddSignalLayout(new Signals()
            {
                Info = new Info()
                {
                    Name = "Robot",
                    GUID = Guid.NewGuid().ToString()
                }
            });

            heartbeatThread.Start();
            TcpServerManager.Start();
            StartClient("127.0.0.1", ref firstStream);

            System.Diagnostics.Debug.WriteLine("Sending Connection Request");
            SendData(connectionRequest, firstStream);

            response = ReadData(firstStream);
            if (response.MessageTypeCase == ConnectionMessage.MessageTypeOneofCase.ConnectionResonse && response.ConnectionResonse.Confirm)
            {
                System.Diagnostics.Debug.WriteLine("Sending Resource Ownership Request");
                SendData(resourceOwnershipRequest, firstStream);
            }

            response = ReadData(firstStream);
            System.Diagnostics.Debug.WriteLine(response);
            if (response.MessageTypeCase == ConnectionMessage.MessageTypeOneofCase.ResourceOwnershipResponse && response.ResourceOwnershipResponse.Confirm)
            {
                guid = response.ResourceOwnershipResponse.Guid;
                generation = response.ResourceOwnershipResponse.Generation;
            }
            System.Diagnostics.Debug.WriteLine("Guid is: {0}", guid);
            Thread.Sleep(1000);

            


            terminateConnectionRequest = new ConnectionMessage()
            {
                TerminateConnectionRequest = new ConnectionMessage.Types.TerminateConnectionRequest()
                {
                    ResourceName = "Robot",
                    Guid = guid,
                    Generation = generation
                }
            };
            System.Diagnostics.Debug.WriteLine("Sending Terminate Connection Request");
            SendData(terminateConnectionRequest, firstStream);
            if (response.MessageTypeCase == ConnectionMessage.MessageTypeOneofCase.TerminateConnectionResponse && response.TerminateConnectionResponse.Confirm)
            {
                StopClient(firstStream);
            }
        }

        [Test]
        public static void TestMultipleConnections()
        {
            RobotManager.Instance.AddSignalLayout(new Signals()
            {
                Info = new Info()
                {
                    Name = "Robot1",
                    GUID = Guid.NewGuid().ToString()
                }
            });
            RobotManager.Instance.AddSignalLayout(new Signals()
            {
                Info = new Info()
                {
                    Name = "Robot2",
                    GUID = Guid.NewGuid().ToString()
                }
            });

            connectionRequest = new ConnectionMessage()
            {
                ConnectionRequest = new ConnectionMessage.Types.ConnectionRequest()
            };
            resourceOwnershipRequest = new ConnectionMessage()
            {
                ResourceOwnershipRequest = new ConnectionMessage.Types.ResourceOwnershipRequest()
                {
                    ResourceName = "Robot1"
                }
            };
            secondResourceOwnershipRequest = new ConnectionMessage()
            {
                ResourceOwnershipRequest = new ConnectionMessage.Types.ResourceOwnershipRequest()
                {
                    ResourceName = "Robot2"
                }
            };
            heartbeat = new ConnectionMessage()
            {
                Heartbeat = new ConnectionMessage.Types.Heartbeat()
            };
            heartbeatThread = new Thread(() =>
            {
                Thread.Sleep(100);
                SendData(heartbeat, firstStream);
                SendData(heartbeat, secondStream);
            });

            heartbeatThread.Start();
            TcpServerManager.Start();
            StartClient("127.0.0.1", ref firstStream);
            StartClient("127.0.0.1", ref secondStream);

            System.Diagnostics.Debug.WriteLine("Sending Connection Requests");
            SendData(connectionRequest, firstStream);
            SendData(connectionRequest, secondStream);

            response = ReadData(firstStream);
            secondResponse = ReadData(secondStream);

            if (response.MessageTypeCase == ConnectionMessage.MessageTypeOneofCase.ConnectionResonse && response.ConnectionResonse.Confirm)
            {
                System.Diagnostics.Debug.WriteLine("Sending Resource Ownership Request1");
                SendData(resourceOwnershipRequest, firstStream);
            }
            if (secondResponse.MessageTypeCase == ConnectionMessage.MessageTypeOneofCase.ConnectionResonse && secondResponse.ConnectionResonse.Confirm)
            {
                System.Diagnostics.Debug.WriteLine("Sending Resource Ownership Request2");
                SendData(resourceOwnershipRequest, secondStream);
            }

            response = ReadData(firstStream);
            secondResponse = ReadData(secondStream);

            if (response.MessageTypeCase == ConnectionMessage.MessageTypeOneofCase.ResourceOwnershipResponse && response.ResourceOwnershipResponse.Confirm)
            {
                guid = response.ResourceOwnershipResponse.Guid;
                generation = response.ResourceOwnershipResponse.Generation;
            }
            if (secondResponse.MessageTypeCase == ConnectionMessage.MessageTypeOneofCase.ResourceOwnershipResponse && secondResponse.ResourceOwnershipResponse.Confirm)
            {
                secondGuid = secondResponse.ResourceOwnershipResponse.Guid;
                secondGeneration = secondResponse.ResourceOwnershipResponse.Generation;
            }
            //Isnt getting second resource...

            System.Diagnostics.Debug.WriteLine("Guid1", guid);
            System.Diagnostics.Debug.WriteLine("Guid1", secondGuid);
            Thread.Sleep(1000);

            terminateConnectionRequest = new ConnectionMessage()
            {
                TerminateConnectionRequest = new ConnectionMessage.Types.TerminateConnectionRequest()
                {
                    ResourceName = "Robot1",
                    Guid = guid,
                    Generation = generation
                }
            };
            secondTerminateConnectionRequest = new ConnectionMessage()
            {
                TerminateConnectionRequest = new ConnectionMessage.Types.TerminateConnectionRequest()
                {
                    ResourceName = "Robot2",
                    Guid = secondGuid,
                    Generation = secondGeneration
                }
            };
            System.Diagnostics.Debug.WriteLine("Sending Terminate Connection Requests");
            SendData(terminateConnectionRequest, firstStream);
            SendData(secondTerminateConnectionRequest, secondStream);
            if (response.MessageTypeCase == ConnectionMessage.MessageTypeOneofCase.TerminateConnectionResponse && response.TerminateConnectionResponse.Confirm)
            {
                StopClient(firstStream);
            }
            if (secondResponse.MessageTypeCase == ConnectionMessage.MessageTypeOneofCase.TerminateConnectionResponse && secondResponse.TerminateConnectionResponse.Confirm)
            {
                StopClient(secondStream);
            }
        }

        public static void StartClient(string server, ref NetworkStream clientStream)
        {
            client = new TcpClient(server, port);
            clientStream = client.GetStream();
        }

        public static void StopClient(NetworkStream clientStream)
        {
            //may need error handling
            clientStream.Close();
            client.Close();
        }

        public static void SendData(ConnectionMessage message, NetworkStream stream)
        {
            message.WriteDelimitedTo(stream);
        }

        public static ConnectionMessage ReadData(NetworkStream clientStream)
        {
            return ConnectionMessage.Parser.ParseDelimitedFrom(clientStream);
        }
    }
    
}
