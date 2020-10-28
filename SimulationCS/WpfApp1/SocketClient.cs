using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace WpfApp1
{
    public static class Address
    {
        public const string IP = "127.0.0.1";
        public const int Port = 54000;
    }

    public class SocketClient
    {
        private static Socket client;
        public static bool StartClient()
        {
            byte[] bytes = new byte[4096];

            try
            {
                // Connect to a Remote server

                // Get Host IP Address 
                //IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                //IPAddress ipAddress = host.AddressList[0];
                //IPEndPoint remoteEP = new IPEndPoint(ipAddress, 54000);

                // Create a TCP/IP socket.
                 client = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.
                try
                {
                    // Connect to Remote EndPoint
                    client.Connect(Address.IP, Address.Port);

                    Console.WriteLine("Socket connected to {0}",
                        client.RemoteEndPoint.ToString());                      

                    // Release the socket.
                    //sender.Shutdown(SocketShutdown.Both);
                    //sender.Close();
                }
                catch (ArgumentNullException ane) // Handle error
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                    return false;
                }
                catch (SocketException se) // Handle error
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                    return false;
                }
                catch (Exception e) // Handle error
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
            return true;
        }

        public static JObject SendReceiveReturn()
        {
            string toSend = @"Resources\OutgoingJson\jason_simulation.json";

            // Encode the data string into a byte array.
            byte[] msg = File.ReadAllBytes(toSend);

            // Send the data through the socket
            int bytesSend = client.Send(msg);

            Console.WriteLine("SEND: {0}",
                Encoding.ASCII.GetString(msg, 0, bytesSend));

            byte[] bytes = new byte[4096];

            // Receive the response from the remote device.
            int bytesRec = client.Receive(bytes);
            Console.WriteLine("SERVER> {0}",
                Encoding.ASCII.GetString(bytes, 0, bytesRec));

            JObject jObject = JObject.Parse(Encoding.UTF8.GetString(bytes)); // Parse received bytes to Json Object

            return jObject;
        }
        public static JObject Receive()
        {
            //buffer 
            byte[] bytes = new byte[4096];

            // Receive the response from the remote device.
            int bytesRec = client.Receive(bytes);

            //debug
            Console.WriteLine("Received: {0}",
                Encoding.ASCII.GetString(bytes, 0, bytesRec));

            // Parse received bytes to Json Object
            JObject jObject = JObject.Parse(Encoding.UTF8.GetString(bytes)); 

            return jObject;
        }
    }

}
