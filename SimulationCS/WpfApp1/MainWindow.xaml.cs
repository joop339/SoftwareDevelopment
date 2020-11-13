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
        private DispatcherTimer socketReceiveTimer; // Timer for ticks
        private DispatcherTimer trafficLightTimer; // Timer for ticks
        private DispatcherTimer spawnCarTimer; // Timer for ticks

        private bool connected = false;

        private Node OW3;

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

        private Route EW;
        private Route NW_A;
        private Route NW_B;

        //TraffiqueLight traffiqueLight;
        //TraffiqueLight tl;

        public MainWindow()
        {
            InitializeComponent();

            InitializeNodes();

            InitializeRoutes();

            InitializeObjects();

            InitializeSocketClient();

            InitializeDispatcherTimer(simulationTickTimer, 1/60, SimulationTick);

            //InitializeDispatcherTimer(socketReceiveTimer, 1, SocketReceiveTick);

            InitializeDispatcherTimer(trafficLightTimer, 2, TrafficLightTick);

            //InitializeDispatcherTimer(spawnCarTimer, 5, SpawnCarTick);

            //Keydown events
            KeyDown += new KeyEventHandler(MainWindow_KeyDown);           
        }

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
        }

        private void InitializeRoutes()
        {
            EW = new Route(new List<Node> { A23, A61, OW3 }); // Route from East to West
            NW_A = new Route(new List<Node> { A11, A11B, A61});
            //NW_B = new Route(new List<Node> { A11, NW_2, NW_3B, A11, NW_5B });
        }
       
        private void InitializeObjects() //Initialize alle auto's en trafficlights uit lijst.
        {
            foreach (Car car in Car.cars)
            {
                canvas.Children.Add(car.ToUIElement());
            }

            //Draw traffic lights V2
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

        private void SocketReceiveTick(object sender, EventArgs e)
        {
            if (!connected)
            {
                InitializeSocketClient();
            }
            if (connected)
            {
                connected = SocketClient.Receive();
                SocketClient.HandleData();               
            }     
        }

        private void SimulationTick(object sender, EventArgs e)
        {
            lblTime.Content = DateTime.Now.ToLongTimeString();
            UpdateCars();           
        }

        private void TrafficLightTick(object sender, EventArgs e)
        {
            if (SocketClient.jObjects.Count > 0)
            {
                SetTrafficLightsFromJson(SocketClient.jObjects.Dequeue());
            }

            

        }

        private void SpawnCarTick(object sender, EventArgs e)
        {
            Car.cars.Add(new Car(NW_A.GetNodes()[0].GetLeft(), NW_A.GetNodes()[0].GetTop() - 100, NW_A));
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
                Car car = new Car(NW_A.GetNodes()[0].GetLeft(), NW_A.GetNodes()[0].GetTop(), NW_A);
                Car.cars.Add(car);
                canvas.Children.Add(car.ToUIElement());

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
               
               
               
               
               
               