using System;
using System.Text;
using System.Windows;
using System.Net;
using System.Net.Sockets;

using System.Threading;

namespace SocketServer_Client
{
    public class Server
    {
        SocketPermission permission;
        Socket sListener;
        IPEndPoint ipEndPoint;
        Socket handler;

        public Server()
        {
            SocketPermission permission;
            Socket sListener;
            IPEndPoint ipEndPoint;
            Socket handler;

        }

        public void Start()
        {
            try
            {
                permission = new SocketPermission(
                    NetworkAccess.Accept,
                    TransportType.Tcp,
                    "",
                    SocketPermission.AllPorts);

                sListener = null;

                permission.Demand();

                IPHostEntry ipHost = Dns.GetHostEntry("");
                IPAddress ipAddr = ipHost.AddressList[0];
                ipEndPoint = new IPEndPoint(ipAddr, 4510);

                sListener = new Socket(
                    ipAddr.AddressFamily,
                    SocketType.Stream,
                    ProtocolType.Tcp);

                sListener.Bind(ipEndPoint);

                Console.WriteLine("Server Started");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception" + ex.ToString());
            }

        }

        public void Listen()
        {
            try
            {
                sListener.Listen(10);
                AsyncCallback asyncCallback = new AsyncCallback(AcceptCallback);
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
                throw;
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            Socket listener = null;
            Socket handler = null;

            try
            {
                byte[] buffer = new byte[1024];
                listener = (Socket) ar.AsyncState;
                handler = listener.EndAccept(ar);
                handler.NoDelay = false;

                object[] obj = new object[2];
                obj[0] = buffer;
                obj[1] = handler;

                handler.BeginReceive(
                    buffer,
                    0,
                    buffer.Length,
                    SocketFlags.None,
                    new AsyncCallback(ReceiveCallback),
                    obj);
                AsyncCallback asyncCallback = new AsyncCallback(AcceptCallback);
                listener.BeginAccept(asyncCallback, listener);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

        }

        public void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                var obj = new object[2];

                obj = (object[]) ar.AsyncState;

                // Received byte array 
                byte[] buffer = (byte[]) obj[0];

                // A Socket to handle remote host communication. 
                handler = (Socket) obj[1];

                // Received message 
                string content = string.Empty;


                // The number of bytes received. 
                int bytesRead = handler.EndReceive(ar);

                if (bytesRead <= 0) return;

                content += Encoding.Unicode.GetString(buffer, 0,
                    bytesRead);

                // Continues to asynchronously receive data

                byte[] buffernew = new byte[1024];
                obj[0] = buffernew;
                obj[1] = handler;
                handler.BeginReceive(buffernew, 0, buffernew.Length,
                    SocketFlags.None,
                    new AsyncCallback(ReceiveCallback),
                    obj);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        public void Send(string message)
        {
            try
            {
                byte[] byteData = Encoding.Unicode.GetBytes(message);

                handler.BeginSend(byteData,
                    0, 
                    byteData.Length, 
                    0,
                    new AsyncCallback(SendCallback), 
                    handler);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket) ar.AsyncState;
                int bytesSend = handler.EndSend(ar);
                Console.WriteLine(
                    "Sent {0} bytes to Client", bytesSend);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public void Stop()
        {
            try
            {
                if (sListener.Connected)
                {
                    sListener.Shutdown(SocketShutdown.Receive);
                    sListener.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

    }
}
