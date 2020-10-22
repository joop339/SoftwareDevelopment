using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
        private List<Car> cars; // List of cars
        private const int fps = 60; // Simulation speed
        private DispatcherTimer simulationTickTimer; // Timer for ticks

        private Node A23;
        private Node A61;
        private Node OW3;

        private Node A11;
        private Node NW_2;
        private Node NW_3A;
        private Node NW_5A;
        private Node NW_3B;
        private Node A62;
        private Node NW_5B;

        private Route EW;
        private Route NW_A;
        private Route NW_B;

        TraffiqueLight traffiqueLight;
        TraffiqueLight tl;

        public MainWindow()
        {
            InitializeComponent();

            InitializeNodes();

            InitializeRoutes();

            // Init objects
            InitializeObjects();

            // Init timer
            InitializeSimulationTickTimer();

            //Keydown events
            KeyDown += new KeyEventHandler(MainWindow_KeyDown);

            InitializeSocketClient();
        }

        private void InitializeNodes()
        {
            A23 = new TrafficLight(1062, 223, "A2-3");
            A61 = new TrafficLight(392, 223, "A6-1");
            OW3 = new Node(10, 233);

            A11 = new TrafficLight(792, 80, "A1-1"); //beginpunt A1-1 naar A6-1 of A6-2
            NW_2 = new Node(792, 197);
            
            NW_3A = new Node(765, 223); //route A richting A6-1
            NW_5A = new Node(10, 223); //eindpunt A

            NW_3B = new Node(765, 256); //route B richting A6-2
            //A62 = new TrafficLight(394, 256, "A6-2");
            NW_5B = new Node(10, 256); //eindpunt Route B          

            //A2-3 to A6-1
        }

        private void InitializeRoutes()
        {
            EW = new Route(new List<Node> { A23, A61, OW3 }); // Route from East to West
            NW_A = new Route(new List<Node> { A11, NW_2, NW_3A, A61, NW_5A });
            NW_B = new Route(new List<Node> { A11, NW_2, NW_3B, A11, NW_5B });
        }
       
        private void InitializeObjects() //Initialize alle auto's en trafficlights uit lijst.
        {            
            cars = new List<Car> { new Car(NW_A.GetNodes()[0].GetLeft(), NW_A.GetNodes()[0].GetTop(), NW_A) };
            foreach (Car car in cars)
            {
                canvas.Children.Add(car.ToUIElement());
            }
            
            //Draw traffic lights V2
            foreach (Node node in Node.nodeList)
            {
                if (node is TrafficLight)
                {
                    canvas.Children.Add(((TrafficLight)node).ToUIElement());
                }
            }

        }

        private void InitializeSimulationTickTimer()
        {
            simulationTickTimer = new DispatcherTimer();

            int interval = (1 / fps); // fps to seconds per frame
            simulationTickTimer.Interval = TimeSpan.FromSeconds(interval);
            simulationTickTimer.Tick += SimulationTickTimer_Tick;
            simulationTickTimer.Start();
        }               

        private void SimulationTickTimer_Tick(object sender, EventArgs e)
        {
            lblTime.Content = DateTime.Now.ToLongTimeString();
            UpdateCars();
        }

        private void UpdateCars()
        {
            if (cars.Count > 0)
            {
                foreach (Car car in cars)
                {
                    car.Update();
                }

            }
        }

        private void InitializeSocketClient()
        {
            bool socket = SocketClient.StartClient();

            if (socket)
            {
                SetTrafficLightsFromJson(SocketClient.SendReceiveReturn());
            }
            else
            {
                SetTrafficLightsFromJson();
            }
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
                Car car = new Car(NW_A.GetNodes()[0].GetLeft(), NW_A.GetNodes()[0].GetTop() - 100, NW_A);
                cars.Add(car);
                canvas.Children.Add(car.ToUIElement());

            }
            e.Handled = true;
        }

        public void SetTrafficLightsFromJson()
        {
            // read JSON directly from a file https://www.newtonsoft.com/json/help/html/ReadJson.htm
            using (StreamReader file = File.OpenText(@"C:\Users\Micky Cheng\Desktop\SoftwareDevelopment\SimulationCS\WpfApp1\resources\incomingJson\jason_controller.json"))// link aanpassen
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


        public class TraffiqueLight
        {
            public int A1_1 {get; set;}
                 int A1_2;
                 int A1_3;
                 int B1_1;
                 int B1_2;
                 int F1_1;
                 int F1_2;
                 int V1_1;
                 int V1_2;
                 int V1_3;
                 int V1_4;
                 int A2_1;
                 int A2_2;
                 int A2_3;
                 int A2_4;
                 int F2_1;
                 int F2_2;
                 int V2_1;
                 int V2_2;
                 int V2_3;
                 int V2_4;
                 int A3_1;
                 int A3_2;
                 int A3_3;
                 int A3_4;
                 int A4_1;
                 int A4_2;
                 int A4_3;
                 int A4_4;
                 int B4_1;
                 int F4_1;
                 int F4_2;
                 int V4_1;
                 int V4_2;
                 int V4_3;
                 int V4_4;
                 int A5_1;
                 int A5_2;
                 int A5_3;
                 int A5_4;
                 int F5_1;
                 int F5_2;
                 int V5_1;
                 int V5_2;
                 int V5_3;
                 int V5_4;
                 int A6_1;
                 int A6_2;
                 int A6_3;
                 int A6_4;


        }      
    }          
}              
               
               
               
               
               
               