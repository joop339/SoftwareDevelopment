using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Documents;

namespace WpfApp1
{
    /// <summary>
    /// Definitive address values [IP, Port]
    /// </summary>
    public static class Address
    {
        public const string IP = "127.0.0.1";
        public const int Port = 54000;
    }
    /// <summary>
    /// A tcp socket for receiving and handling data
    /// </summary>
    public static class SocketClient
    {
        /// <summary>
        /// A socket global
        /// </summary>
        private static Socket client;

        /// <summary>
        /// Queue of bytes for incoming data
        /// </summary>
        public static Queue<byte> queue = new Queue<byte>();

        /// <summary>
        /// List of jObjects each containing data for one phase
        /// </summary>
        public static List<JObject> jObjects = new List<JObject>();

        /// <summary>
        /// Initializes a socket which tries to connect to Address.IP with Address.Port
        /// </summary>
        /// <returns>
        /// Boolean true or false based on successful connection
        /// </returns>
        public static bool StartClient()
        {
            try
            {
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
        
        /// <summary>
        /// Receives data and parse to jObject then add to list
        /// </summary>
        public static void Receive()
        {
            // Init new buffer 
            byte[] buffer = new byte[1_000_000];

            // Receive and put in buffer
            int bytesRec = client.Receive(buffer);

            // Put buffer in static queue 
            for (int i = 0; i < bytesRec; i++)
            {
                queue.Enqueue(buffer[i]);
            }

            // Find header
            string header = GetHeader(buffer, bytesRec);

            //debug
            Debug(buffer, bytesRec, header);
            //           

            List<byte> bytesList = new List<byte>();// all bytes for one phase/json

            int headerOffset = header.Length + 1;
            int length = Convert.ToInt32(header);

            //get rid of header
            for (int i = 0; i < headerOffset; i++)
            {
                queue.Dequeue();
            }
               
            //put data into bytesList
            for (int i = 0; i < length; i++)
            {
                bytesList.Add(queue.Dequeue());
            }

            //convert bytesList to byteArray
            byte[] jsonBytes = bytesList.ToArray();

            //Console.WriteLine(Encoding.UTF8.GetString(jsonBytes, 0, jsonBytes.Length));

            // Parse received bytes to Json Object
            JObject jObject = JObject.Parse(Encoding.UTF8.GetString(jsonBytes, 0, jsonBytes.Length));

            jObjects.Add(jObject);
        }

        private static void Debug(byte[] buffer, int bytesRec, string header)
        {
            Console.WriteLine("Received Data: {0}",
                            Encoding.ASCII.GetString(buffer, 0, bytesRec));
            Console.WriteLine("bytesRec: " + bytesRec);
            Console.WriteLine("Received Header: " + header);
        }

        private static string GetHeader(byte[] buffer, int bytesRec)
        {
            string received = Encoding.UTF8.GetString(buffer, 0, bytesRec);
            string header = "";

            foreach (char c in received)
            {
                if (c != ':')
                {
                    header += c;
                }
                else
                {
                    break;
                }
            }

            return header;
        }


        //public static JObject SendReceiveReturn()
        //{
        //    string toSend = @"Resources\OutgoingJson\jason_simulation.json";

        //    // Encode the data string into a byte array.
        //    byte[] msg = File.ReadAllBytes(toSend);

        //    // Send the data through the socket
        //    int bytesSend = client.Send(msg);

        //    Console.WriteLine("SEND: {0}",
        //        Encoding.ASCII.GetString(msg, 0, bytesSend));

        //    byte[] bytes = new byte[4096];

        //    // Receive the response from the remote device.
        //    int bytesRec = client.Receive(bytes);
        //    Console.WriteLine("SERVER> {0}",
        //        Encoding.ASCII.GetString(bytes, 0, bytesRec));

        //    JObject jObject = JObject.Parse(Encoding.UTF8.GetString(bytes)); // Parse received bytes to Json Object

        //    return jObject;
        //}
    }
}
