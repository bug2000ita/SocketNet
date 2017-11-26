using System;
using System.Text;
using System.Windows;
using System.Net.Sockets;
using System.Net;

namespace SocketLib.Client
{
    public class ClientSocket
    {


        byte[] bytes = new byte[1024];
        IPAddress ipAddr;
        Socket senderSock;
        public ClientSocket()
        {
            try
            {
                SocketPermission permission = new SocketPermission(
                    NetworkAccess.Connect,
                    TransportType.Tcp,
                    "",
                    SocketPermission.AllPorts
                );

                permission.Demand();

                IPHostEntry ipHost = Dns.GetHostEntry("");
                 ipAddr = ipHost.AddressList[0];

                senderSock = new Socket(
                    ipAddr.AddressFamily,
                    SocketType.Stream,
                    ProtocolType.Tcp);

                senderSock.NoDelay = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        public void Connect()
        {
            try
            {


                IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 4510);
                senderSock.Connect(ipEndPoint);
                Console.WriteLine("Socket Connected to" + senderSock.RemoteEndPoint.ToString());

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void SendMessage(string message)
        {
            try
            {
                byte[] msg = Encoding.Unicode.GetBytes(message + "<Client Quit>");

                int byteSend = senderSock.Send(msg);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString()); 
            }
        }

        private void Receive()
        {
            try
            {
                int bytesRec = senderSock.Receive(bytes);
                String RxMessage = Encoding.Unicode.GetString(bytes, 0, bytesRec);
                while(senderSock.Available>0)
                {
                    bytesRec = senderSock.Receive(bytes);
                    RxMessage += Encoding.Unicode.GetString(bytes, 0, bytesRec);
                }
                  
                Console.WriteLine("Received: " + RxMessage);
                
            }
            catch
            {
                
            }
        }



    }
}
