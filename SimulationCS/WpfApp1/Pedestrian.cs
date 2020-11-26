using System.Collections.Generic;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;

namespace WpfApp1
{
    public class Pedestrian
    {
        public static List<Pedestrian> pedestrians = new List<Pedestrian>(); // List of pedestrians

        double left;
        double top;


        Route route; // To be followed route
        Node target;

        Shape body; // shape representation

        public Pedestrian(Route route)
        {
            pedestrians.Add(this); // add to list

            this.route = route; // set route

            body = new Ellipse() // set body
            {
                Fill = Brushes.LightBlue,
                Stroke = Brushes.Black,
            };
        }

        public UIElement ToUIElement()
        {
            return body;
        }

        public Color CheckTrafficLight()
        {
            return ((TrafficLight)target).GetColor();
        }

        public void Update()
        {
            this.Walk();
            this.CheckTrafficLight();
        }

        public void Walk()
        {
            if (target.GetLeft() > left)
            {
                left = left + 0.025;

            }
            else if (target.GetLeft() < left)
            {
                left = left - 0.025;

            }

            if (target.GetTop() > top)
            {
                top = top + 0.025;

            }
            else if (target.GetTop() < top)
            {
                top = top - 0.025;
            }
        }

        public void SwitchTarget()
        {
            if (target.GetTop() - 0.05 < top && target.GetTop() + 0.05 > top && target.GetLeft() - 0.05 < left && target.GetLeft() + 0.05 > left)
            { // if target is reached

                if (target is TrafficLight)
                { // and target is TL AND TL is green
                    if (CheckTrafficLight() == Color.Green)
                    {// get next target
                        int index = route.GetNodes().IndexOf(target);
                        if (index < route.GetNodes().Count - 1)
                        {
                            this.target = route.GetNodes()[index + 1];
                        }
                        else
                        {
                            this.Destroy();
                        }
                    }
                    else if (CheckTrafficLight() == Color.Red)
                    {
                        ((TrafficLight)target).SetWaiting(true);
                    }
                }
                else if (!(target is TrafficLight))
                {
                    int index = route.GetNodes().IndexOf(target);
                    if (index < route.GetNodes().Count - 1)
                    {
                        this.target = route.GetNodes()[index + 1];
                        //Console.WriteLine(target.name);
                    }
                    else
                    {
                        this.Destroy();
                    }
                }
            }
        }

        void Destroy()
        {
            pedestrians.Remove(this);
        }
    }

}