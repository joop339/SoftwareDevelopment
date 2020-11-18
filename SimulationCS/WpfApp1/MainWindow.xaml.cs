using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApp1
{ 
    public static class Settings
    {
        public const bool SimulationOnly = true;
        public const int SpawnCarInterval = 1;

    }

    // Traffic light colors
    public enum Color
    {
        Red, // 0
        Green, // 1
        None, // 2
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// 
    /// Stoplichten zijn klikbaar!
    /// Druk S op keyboard: Spawn auto op bij stoplicht A2-3L
    /// </summary>
    public partial class MainWindow : Window
    {
        //private List<Car> cars; // List of cars
        private const int fps = 60; // Simulation speed
        private DispatcherTimer simulationTickTimer; // Timer for ticks
        private DispatcherTimer socketConnectTimer; // Timer for ticks
        private DispatcherTimer socketSendTimer; // Timer for ticks
        private DispatcherTimer socketReceiveTimer; // Timer for ticks
        private DispatcherTimer trafficLightTimer; // Timer for ticks
        private DispatcherTimer spawnCarTimer; // Timer for ticks

        private bool connected = false; // 

        //Startpunten
        private List<Node> spawnNodes = new List<Node>();

        private Node A11S;
        private Node A12S;
        private Node A13S;
        private Node A14S;

        private Node A21S;
        private Node A22S;
        private Node A23S;
        private Node A24S;

        private Node A41S;
        private Node A42S;
        private Node A43S;
        private Node A44S;
        private Node B41S;

        private Node A51S;
        private Node A52S;
        private Node A53S;
        private Node A54S;

        // de rest van de punten
        // TODO: netjes maken
        private Node A11;
        private Node A12;
        private Node A13;
        private Node B12;
        private Node A21;
        private Node A22;
        private Node A23;
        private Node A24;
        private Node A31;
        private Node A32;
        private Node A33;
        private Node A34;
        private Node A41;
        private Node A42;
        private Node A43;
        private Node A44;
        private Node B41;
        private Node A51;
        private Node A52;
        private Node A53;
        private Node A54;
        private Node A61;
        private Node A62;
        private Node A63;
        private Node A64;

        private Node A11B;
        private Node A11BA;
        private Node A12B;
        private Node NW_3B;
        private Node NW_5B;

        private Node A11Bocht;
        private Node A12Bocht;
        private Node A13Bocht;

        private Node A21Bocht;
        private Node A22Bocht;
        private Node A23Bocht;
        private Node A24Bocht;

        private Node A41Bocht;
        private Node A42Bocht;
        private Node A43Bocht;
        private Node A44Bocht;

        private Node A31Bocht;
        private Node A32Bocht;
                     
                     
        private Node A51Bocht;
        private Node A52Bocht;
        private Node A53Bocht;
        private Node A54Bocht;
                     
                     
        private Node A63Bocht;
        private Node A64Bocht;

        private Node A21E;
        private Node A22E;


        private Node A33E;
        private Node A34E;


        private Node A53E;
        private Node A54E;


        private Node A61E;
        private Node A62E;

        /// <summary>
        /// Do on MainWindow creation
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            InitializeNodes();

            InitializeRoutes();

            InitializeObjects();

            InitializeSocketClient();

            InitializeDispatcherTimer(simulationTickTimer, 1/fps, SimulationTick);

            InitializeDispatcherTimer(socketConnectTimer, 1, SocketConnectTick);

            InitializeDispatcherTimer(socketSendTimer, 1, SocketSendTick);

            InitializeDispatcherTimer(socketReceiveTimer, 1/60, SocketReceiveTick);

            InitializeDispatcherTimer(trafficLightTimer, 1/60, TrafficLightTick);

            InitializeDispatcherTimer(spawnCarTimer, Settings.SpawnCarInterval, SpawnCarTick);

            SendJson();

            //Keydown events
            KeyDown += new KeyEventHandler(MainWindow_KeyDown);         
            
        }

        /// <summary>
        ///Initialize nodes
        /// </summary>
        private void InitializeNodes()
        {
            //A23 = new TrafficLight(1062, 223, "A2-3");
            //A61 = new TrafficLight(392, 223, "A6-1");
            //OW3 = new Node(10, 233);

            /*A11 = new TrafficLight(792, 80, "A1-1");*/ //beginpunt A1-1 naar A6-1 of A6-2
            A11 = new TrafficLight(A1_1, "A1-1");
            A12 = new TrafficLight(A1_2, "A1-2");
            A13 = new TrafficLight(A1_3, "A1-3");
            B12 = new TrafficLight(B1_2, "B1-2");

            A21 = new TrafficLight(A2_1, "A2-1");
            A22 = new TrafficLight(A2_2, "A2-2");
            A23 = new TrafficLight(A2_3, "A2-3");
            A24 = new TrafficLight(A2_4, "A2-4");

            A31 = new TrafficLight(A3_1, "A3-1");
            A32 = new TrafficLight(A3_2, "A3-2");
            A33 = new TrafficLight(A3_3, "A3-3");
            A34 = new TrafficLight(A3_4, "A3-4");

            A41 = new TrafficLight(A4_1, "A4-1");
            A42 = new TrafficLight(A4_2, "A4-2");
            A43 = new TrafficLight(A4_3, "A4-3");
            A44 = new TrafficLight(A4_4, "A4-4");
            B41 = new TrafficLight(B4_1, "B4-1");

            A51 = new TrafficLight(A5_1, "A5-1");
            A52 = new TrafficLight(A5_2, "A5-2");
            A53 = new TrafficLight(A5_3, "A5-3");
            A54 = new TrafficLight(A5_4, "A5-4");

            A61 = new TrafficLight(A6_1, "A6-1");
            A62 = new TrafficLight(A6_2, "A6-2");
            A63 = new TrafficLight(A6_3, "A6-3");
            A64 = new TrafficLight(A6_4, "A6-4");

            A11B = new Node(A1_1Bocht);

            //A11BA = new Node(A1_1_BA); //route A richting A6-1
            //A12B = new Node(A1_2_B); //eindpunt A

            NW_3B = new Node(765, 256); //route B richting A6-2
            //A62 = new TrafficLight(394, 256, "A6-2");
            NW_5B = new Node(10, 256); //eindpunt Route B          

            //A2-3 to A6-1

            A11S = new Node(A1_1S);
            A12S = new Node(A1_2S);
            A13S = new Node(A1_3S);
            A14S = new Node(A1_4S);
                              
            A21S = new Node(A2_1S);
            A22S = new Node(A2_2S);
            A23S = new Node(A2_3S);
            A24S = new Node(A2_4S);
                              
            A41S = new Node(A4_1S);
            A42S = new Node(A4_2S);
            A43S = new Node(A4_3S);
            A44S = new Node(A4_4S);
            B41S = new Node(B4_1S);
                              
            A51S = new Node(A5_1S);
            A52S = new Node(A5_2S);
            A53S = new Node(A5_3S);
            A54S = new Node(A5_4S);

            // Bochten
            A11Bocht = new Node(A1_1Bocht);
            A12Bocht = new Node(A1_2Bocht);
            A13Bocht = new Node(A1_3Bocht);

            A21Bocht = new Node(A2_1Bocht);
            A22Bocht = new Node(A2_2Bocht);
            A23Bocht = new Node(A2_3Bocht);
            A24Bocht = new Node(A2_4Bocht);

            A41Bocht = new Node(A4_1Bocht);
            A42Bocht = new Node(A4_2Bocht);
            A43Bocht = new Node(A4_3Bocht);
            A44Bocht = new Node(A4_4Bocht);

            A31Bocht = new Node(A3_1Bocht);
            A32Bocht = new Node(A3_2Bocht);

            A51Bocht = new Node(A5_1Bocht);
            A52Bocht = new Node(A5_2Bocht);
            A53Bocht = new Node(A5_3Bocht);
            A54Bocht = new Node(A5_4Bocht);

            A63Bocht = new Node(A6_3Bocht);
            A64Bocht = new Node(A6_4Bocht);

            A21E = new Node(A2_1E);
            A22E = new Node(A2_2E);

            A33E = new Node(A3_3E);
            A34E = new Node(A3_4E);

            A53E = new Node(A5_3E);
            A54E = new Node(A5_4E);

            A61E = new Node(A6_1E);
            A62E = new Node(A6_2E);




            //foreach (var element in canvas.Children)
            //{
            //    if (element is Ellipse)
            //    {
            //        Ellipse ellipse = (Ellipse)element;
            //        if (ellipse.Name.Contains("S"))
            //        {
            //            spawnNodes.Add(new Node(ellipse));
            //        }
            //    }
            //}
        }

        /// <summary>
        ///Initialize routes
        /// </summary>
        private void InitializeRoutes()
        {
            Route Route1 = new Route(new List<Node> { A11S, A11, A11Bocht, A61, A61E });
            Route Route1_1 = new Route(new List<Node> { A11S, A11, A11Bocht, A62, A62E });

            Route Route2 = new Route(new List<Node> { A12S, A12, A12Bocht, A63, A63Bocht, A53E });
            Route Route2_1 = new Route(new List<Node> { A12S, A12, A12Bocht, A64, A64Bocht, A54E });

            Route Route3 = new Route(new List<Node> { A13S, A13, A13Bocht, A33E });

            Route Route4 = new Route(new List<Node> { A21S, A21, A21Bocht, A21E });

            Route Route5 = new Route(new List<Node> { A22S, A22, A22Bocht, A22E });

            Route Route6 = new Route(new List<Node> { A23S, A23, A23Bocht, A61, A61E });
            Route Route6_1 = new Route(new List<Node> { A23S, A23, A23Bocht, A62, A62E });

            Route Route7 = new Route(new List<Node> { A24S, A24, A24Bocht, A63, A63Bocht, A53E });
            Route Route7_1 = new Route(new List<Node> { A24S, A24, A24Bocht, A64, A64Bocht, A54E });

            Route Route8 = new Route(new List<Node> { A41S, A41, A41Bocht, A62E });

            Route Route9 = new Route(new List<Node> { A42S, A42, A42Bocht, A61E });

            Route Route10 = new Route(new List<Node> { A43S, A43, A43Bocht, A31, A31Bocht, A22E });
            Route Route10_1 = new Route(new List<Node> { A43S, A43, A43Bocht, A32, A32Bocht, A21E });

            Route Route11 = new Route(new List<Node> { A44S, A44, A44Bocht, A33, A33E });
            Route Route11_1 = new Route(new List<Node> { A44S, A44, A44Bocht, A34, A34E });

            Route Route12 = new Route(new List<Node> { A51S, A51Bocht, A31, A31Bocht, A22E });
            Route Route12_1 = new Route(new List<Node> { A51S, A51Bocht, A32, A32Bocht, A21E });

            Route Route13 = new Route(new List<Node> { A52S, A52Bocht, A33, A33E });
            Route Route13_1 = new Route(new List<Node> { A52S, A52Bocht, A34, A34E });

            Route Route14 = new Route(new List<Node> { A53S, A53Bocht, A54E });

            Route Route15 = new Route(new List<Node> { A54S, A54Bocht, A53E });
        }

        /// <summary>
        ///Initialize alle bestaande auto's en trafficlights uit lijst.
        /// </summary>
        private void InitializeObjects() 
        {
            //Draw predefined cars
            foreach (Car car in Car.cars)
            {
                canvas.Children.Add(car.ToUIElement());
            }

            //Draw predefined traffic lights V2
            foreach (Node node in Node.nodeList)
            {
                if (node is TrafficLight)
                {
                    if (!canvas.Children.Contains(((TrafficLight)node).ToUIElement()))
                    {
                        canvas.Children.Add(((TrafficLight)node).ToUIElement());
                    }

                }
            }

        }              
       
        /// <summary>
        /// Initializes a DispatcherTimer
        /// </summary>
        /// <param name="timer">timer</param>
        /// <param name="interval">interval in seconds</param>
        private void InitializeDispatcherTimer(DispatcherTimer timer, int interval, EventHandler e)
        {
            timer = new DispatcherTimer();

            timer.Interval = TimeSpan.FromSeconds(interval);
            timer.Tick += e;

            timer.Start();
        }

        private void SimulationTick(object sender, EventArgs e)
        {
            lblTime.Content = DateTime.Now.ToLongTimeString();
            UpdateCars();
            UpdateTrafficLights();
        }
        /// <summary>
        /// Try connecting to socket
        /// </summary>
        private void SocketConnectTick(object sender, EventArgs e)
        {
            if (!Settings.SimulationOnly) 
            {
                if (!connected)
                {
                    InitializeSocketClient();
                }
            }
            
        }

        JObject jObject1 = new JObject();

        /// <summary>
        /// Send to over socket
        /// </summary>
        private void SocketSendTick(object sender, EventArgs e)
        {
            foreach(JProperty p in jObject1.Properties())
            {

            }
            //Console.Clear();
            Console.WriteLine("-------------------------------");
            UpdateTrafficLights();
            Console.WriteLine("-------------------------------");

            if (connected)
            {
                //send json over socket
            }

        }

        /// <summary>
        /// Receive and Handle data when connected
        /// </summary>
        private void SocketReceiveTick(object sender, EventArgs e)
        {
            if (connected)
            {
                connected = SocketClient.Receive();
                SocketClient.HandleData();               
            }     
        }

        /// <summary>
        /// Set traffic Lights from jObject buffer
        /// </summary>
        private void TrafficLightTick(object sender, EventArgs e)
        {
            if (SocketClient.jObjects.Count > 0)
            {
                SetTrafficLightsFromJson(SocketClient.jObjects.Dequeue());
            }

        }

        int counter = 0;
        private void SpawnCarTick(object sender, EventArgs e)
        {
            int amountOfCars = 0;

            foreach (var element in canvas.Children)
            {
                if (element is Rectangle)
                {
                    amountOfCars++;
                }
            }

            if (amountOfCars < 20)
            {
                
                if(counter == 23)
                {
                    counter = 0;
                }

                Car.cars.Add(new Car(Route.routes[counter]));
                //Console.WriteLine("Spawning: '" + 1 + "' car, at: '" + Route.routes[counter].GetNodes()[0].name + "'");
                counter++;

            }
            else
            {
                //Console.WriteLine("Spawning stopped cars at: " + amountOfCars);
            }
        }

        private void UpdateCars()
        {
            if (Car.cars.Count > 0)
            {
                for (int i = 0; i < Car.cars.Count - 1;i++)
                //foreach (Car car in Car.cars)
                {
                    Car.cars[i].Update();

                    if (!canvas.Children.Contains(Car.cars[i].ToUIElement()))
                    {
                        canvas.Children.Add(Car.cars[i].ToUIElement());
                    }


                }
            }

            /* destroy rectangles */
            if (Car.destroyedCars.Count > 0)
            {
                for (int i = 0; i < Car.destroyedCars.Count - 1; i++)
                {
                    if (canvas.Children.Contains(Car.destroyedCars[i].ToUIElement()))
                    {
                        canvas.Children.Remove(Car.destroyedCars[i].ToUIElement());
                    }
                }
            }
        }
        public class data
        {
            public int Id { get; set; }
            public int SSN { get; set; }
            public string Message { get; set; }
        }

        private void SendJson()
        {
           
            int Id = 341;
            int SSN = 2;
            string Message = "A Message";

            JObject jObject1 = new JObject();
            jObject1.Add("A1-1", 0);
            jObject1.Add("A1-2", 1);

            string json = JsonConvert.SerializeObject(jObject1, Formatting.Indented);

            //write string to file
            System.IO.File.WriteAllText(@"path.txt", json);
        }

        private void UpdateTrafficLights()
        {
            if (Node.nodeList.Count > 0)
            {
                for (int i = 0; i < Node.nodeList.Count - 1; i++)
                {
                    if (Node.nodeList[i] is TrafficLight)
                    {
                        ((TrafficLight)Node.nodeList[i]).Update();
                    }

                }
            }
        }

        private void RandomSpawnCars()
        {
            Random random = new Random();

            int randomAmountOfCars = random.Next(4);

            Route randomRoute = Route.GetRandomRoute();

            for (int i = 0; i < randomAmountOfCars; i++)
            {
                Car.cars.Add(new Car(randomRoute));
            }

            Console.WriteLine("Spawning: '" + randomAmountOfCars + "' cars, at: '" + randomRoute.GetNodes()[0].name + "'");
        }


        private void InitializeSocketClient()
        {
            connected = SocketClient.StartClient();          
        }

        private void CycleTrafficLight(TrafficLight trafficLight)//traffic light color cycle
        {
            if (trafficLight.GetColor() == Color.Red)
            {
                trafficLight.SetColor(Color.Green);
            }
            else
            {
                trafficLight.SetColor(Color.Red);
            }
        }


        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D1)
            {
                CycleTrafficLight((TrafficLight)A23);
            }
            else if (e.Key == Key.D2)
            {
                CycleTrafficLight((TrafficLight)A61);
            }
            else if (e.Key == Key.S)
            {
                //Car car = new Car(NW_A.GetNodes()[0].GetLeft(), NW_A.GetNodes()[0].GetTop(), NW_A);
                //Car.cars.Add(car);
                //canvas.Children.Add(car.ToUIElement());

            }
            e.Handled = true;
        }

        public void SetTrafficLightsFromJson(JObject jObject)
        {
            foreach (JProperty property in jObject.Properties())
            {
                foreach (Node node in Node.nodeList)
                {
                    if (node is TrafficLight)
                    {
                        if (property.Name == ((TrafficLight)node).id)
                        {
                            ((TrafficLight)node).SetColor((Color)property.Value.ToObject<int>());
                        }
                    }

                }
            }
        }

        public void SetTrafficLightsFromJson()
        {
            // read JSON directly from a file https://www.newtonsoft.com/json/help/html/ReadJson.htm
            var path = (@"Resources\IncomingJson\jason_controller.json");
            using (StreamReader file = File.OpenText(path))// link aanpassen
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JObject o = (JObject)JToken.ReadFrom(reader);
                foreach (JProperty property in o.Properties())
                {
                    foreach (Node node in Node.nodeList)
                    {
                        if (node is TrafficLight)
                        {
                            if (property.Name == ((TrafficLight)node).id)
                            {
                                ((TrafficLight)node).SetColor((Color)property.Value.ToObject<int>());
                            }
                        }
                        
                    }                 
                }
            }
        }
    }          
}              
               
               
               
               
               
               