using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
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

        private static char seperator = ':';
        /// <summary>
        /// Queue of bytes for incoming data
        /// </summary>
        public static Queue<byte> queue = new Queue<byte>();

        /// <summary>
        /// List of jObjects each containing data for one phase
        /// </summary>
        public static Queue<JObject> jObjects = new Queue<JObject>();

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
        /// Receives data and HandleData()
        /// </summary>
        /// <returns>
        /// Boolean true or false based on successful connection
        /// </returns>
        public static bool Receive()
        {
            // Init new buffer 
            byte[] buffer = new byte[1_000_000];

            try
            {
                // Receive and put in buffer
                int bytesRec = client.Receive(buffer);

                // Put buffer in static queue 
                for (int i = 0; i < bytesRec; i++)
                {
                    queue.Enqueue(buffer[i]);
                }
            }
            catch (SocketException se) // Handle error
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
                return false;
            }
            HandleData();
            return true;
        }

        /// <summary>
        /// Parse queue to list of JObjects
        /// </summary>
        public static void HandleData()
        {
            // Dequeue header
            string header = DequeueHeader(queue, seperator);

            int length = 0;

            if (int.TryParse(header, out length))
            {
                Console.WriteLine("header " + header + " was found! Parsing to int " + length);
            }
            else
            {
                Console.WriteLine("Can't parse header into int");
            }

            List<byte> bytesList = new List<byte>();// all bytes for one phase/json

            //put data into bytesList
            for (int i = 0; i < length; i++)
            {
                bytesList.Add(queue.Dequeue());
            }

            //convert bytesList to byteArray
            if (bytesList.Count > 0)
            {
                byte[] jsonBytes = bytesList.ToArray();

                //Console.WriteLine(Encoding.UTF8.GetString(jsonBytes, 0, jsonBytes.Length));

                // Parse received bytes to Json Object
                JObject jObject = JObject.Parse(Encoding.UTF8.GetString(jsonBytes, 0, jsonBytes.Length));

                jObjects.Enqueue(jObject);
            }           
            
        }

        /// <summary>
        /// Dequeues elements from queue until seperator
        /// </summary>
        /// <param name="queue">Queue with received data</param>
        /// <param name="seperator">Read until seperator is reached</param>
        /// <returns>String containing header</returns>
        private static string DequeueHeader(Queue<byte> queue, char seperator)
        {
            string sprtr = seperator.ToString();
            byte[] colon = Encoding.UTF8.GetBytes(sprtr);

            List<byte> hdr = new List<byte>();
            if (queue.Count > 0)
            {
                while (queue.Peek() != colon[0])
                {
                    hdr.Add(queue.Dequeue());
                }

                queue.Dequeue(); // remove seperator
            }
                    
            byte[] headerBytes = hdr.ToArray();

            string header = Encoding.UTF8.GetString(headerBytes);

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
