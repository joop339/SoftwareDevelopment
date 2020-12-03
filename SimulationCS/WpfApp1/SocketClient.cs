#undef DEBUG_SOCKET
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

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
#if DEBUG_SOCKET
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
#endif
                    return false;
                }
                catch (SocketException se) // Handle error
                {
#if DEBUG_SOCKET
                    Console.WriteLine("SocketException : {0}", se.ToString());
#endif
                    return false;
                }
                catch (Exception e) // Handle error
                {
#if DEBUG_SOCKET
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
#endif
                    return false;
                }
            }
            catch (Exception e)
            {
#if DEBUG_SOCKET
                Console.WriteLine(e.ToString());
#endif
                return false;
            }

            return true;
        }

        /// <summary>
        /// Receives data in a buffer and enqueues to queue
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
#if DEBUG_SOCKET
                Console.WriteLine("RECEIVED: \n" + Encoding.UTF8.GetString(buffer, 0, buffer.Length));
#endif
                // Put buffer in static queue 
                for (int i = 0; i < bytesRec; i++)
                {
                    queue.Enqueue(buffer[i]);
                }

            }
            catch (SocketException se) // Handle error
            {
#if DEBUG_SOCKET
                Console.WriteLine("SocketException : {0}", se.ToString());
#endif
                return false;
            }
            catch (Exception e)
            {
#if DEBUG_SOCKET
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
#endif
                return false;
            }

            return true;
        }

        /// <summary>
        /// Parse queue to list of JObjects
        /// </summary>
        public static void HandleData()
        {
            // Dequeue header
            string header = DequeueHeader(queue, seperator);

            // header length
            int length = 0;

            // try to parse header to int
            if (int.TryParse(header, out length))
            {
#if DEBUG
                Console.WriteLine("header " + header + " was found! Parsing to int " + length);
#endif
            }
            else
            {
#if DEBUG
                Console.WriteLine("Can't parse header into int");
#endif
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
            byte[] sprtrBytes = Encoding.UTF8.GetBytes(sprtr);

            List<byte> hdr = new List<byte>();
            if (queue.Count > 0)
            {
                // while first element of queue is not sprtr
                while (queue.Peek() != sprtrBytes[0])
                {
                    // add first element of queue to hdr 
                    // remove first element from queue
                    hdr.Add(queue.Dequeue());
                }

                queue.Dequeue(); // remove seperator
            }
               
            // Convert hdr to bytes for Encoding
            byte[] headerBytes = hdr.ToArray();

            // Decode headerBytes to string
            string header = Encoding.UTF8.GetString(headerBytes);

            return header;
        }

        /// <summary>
        /// Adds a header to a jObject as string and encodes the whole package to bytes array
        /// </summary>
        /// <param name="jObject">jObject</param>
        /// <returns>bytes array from header:json string</returns>
        private static byte[] ConvertToByteArrayWithHeader(JObject jObject)
        {
            // Convert jObject to string with no formatting for space savings
            string json = jObject.ToString(Formatting.None);

            // Add json.Length + ':' as header to jObject as string
            string package = json.Length + ":" + json;

            // Encode package to bytes
            byte[] bytes = Encoding.UTF8.GetBytes(package);

            return bytes;
        }

        /// <summary>
        /// Sends bytes over socket
        /// </summary>
        /// <param name="jObject">jObject</param>
        public static void Send(JObject jObject)
        {
            // Encode the data string into a byte array.
            byte[] msg = ConvertToByteArrayWithHeader(jObject);

            // Send the data through the socket
            int bytesSend = client.Send(msg);
#if DEBUG_SOCKET
            Console.WriteLine("SENT: \n" + Encoding.UTF8.GetString(msg, 0, msg.Length));
#endif
        }
    }
}
