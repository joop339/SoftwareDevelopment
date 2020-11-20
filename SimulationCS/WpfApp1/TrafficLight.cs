using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp1
{
    public static class Radius
    {
        public const int TrafficLight = 20;
    }
    public class TrafficLight : Node
    {
        private Ellipse trafficLight;
        Color color = Color.Red;
        public string id;
        private bool waiting = false;

        public TrafficLight(int left, int top, string id) : base(left, top)
        {
            this.left = left;
            this.top = top;
            this.id = id;

            trafficLight = new Ellipse()
            {
                Fill = Brushes.Red,
                Height = Radius.TrafficLight,
                Width = Radius.TrafficLight,
            };
            Canvas.SetTop(trafficLight, top);
            Canvas.SetLeft(trafficLight, left);
            Panel.SetZIndex(trafficLight, 666);

            trafficLight.MouseDown += TrafficLight_MouseDown;
        }

        public TrafficLight(Ellipse ellipse, string id)
        {
            left = Canvas.GetLeft(ellipse);
            top = Canvas.GetTop(ellipse);
            this.id = id;

            trafficLight = ellipse;
            Panel.SetZIndex(trafficLight, 666);

            trafficLight.MouseDown += TrafficLight_MouseDown;
        }

        public void TrafficLight_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (trafficLight.Fill == Brushes.Green)
            {
                trafficLight.Fill = Brushes.Red;
                color = Color.Red;
            }
            else if (trafficLight.Fill == Brushes.Red)
            {
                trafficLight.Fill = Brushes.Green;
                color = Color.Green;
                waiting = false;
            }
            else
            {
                trafficLight.Fill = Brushes.Green;
                color = Color.Green;
            }
        }

        //public static implicit operator UIElement(TrafficLight trafficLight)
        //{
        //    return trafficLight;
        //}

        public UIElement ToUIElement()
        {
            return trafficLight;
        }

        public void Update()
        {
            if (waiting)
                Console.WriteLine("TL: " + id +" I got cars: " + waiting);
        }

        public Color GetColor()
        {
            return color;
        }

        public void SetWaiting(bool state)
        {
            waiting = state;
        }

        public void SetColor(Color color)
        {
            this.color = color;
            if (color == Color.Red)
            {
                trafficLight.Fill = Brushes.Red;
            }
            else if (color == Color.Green)
            {
               trafficLight.Fill = Brushes.Green;
                waiting = false;
            }

        }
    }
}
