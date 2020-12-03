using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Runtime;
using System.Windows.Shapes;

namespace WpfApp1
{
    public class Car
    {
        private RotateTransform rotateTransform = new RotateTransform();

        public static List<Car> cars = new List<Car>();
        public static List<Car> destroyedCars = new List<Car>();

        private Rectangle carRect; //car icon

        private bool waitingSend = false;
        public Node target; // next target

        private double left;
        private double top;

        private double tlLeft;
        private double tlTop;
        public static double speed = 0.008;

        private Route route; // to be followed route

        public Car(Route route) // initialise bus with route and coordinates.
        {
            cars.Add(this);
            this.left = route.GetNodes()[0].GetLeft();
            this.top = route.GetNodes()[0].GetTop();

            this.route = route;
            this.target = route.GetNodes()[0];

            carRect = new Rectangle()
            {
                Width = 19,
                Height = 13,
                Fill = Brushes.Maroon,
                Stroke = Brushes.Black
            };

            rotateTransform.CenterX = carRect.Width / 2;
            rotateTransform.CenterY = carRect.Height / 2;
        }


        public UIElement ToUIElement()
        {
            return carRect;
        }

        private Color CheckTrafficLight()
        {
            return ((TrafficLight)target).GetColor();
        }

        private void Destroy() // Remove car.
        {
            cars.Remove(this);
            destroyedCars.Add(this);
        }
        public void Draw()
        {
            Canvas.SetLeft(carRect, left);
            Canvas.SetTop(carRect, top);
        }

        private bool trafficlightDistanceCheck()  // check distance between car and target trafficlight. depending on amount of cars and busses already waiting
        {
            if (tlTop - 0.05 - (25 * ((TrafficLight)target).waitingCars) - (44 * ((TrafficLight)target).waitingBusses) < top
                    && tlTop + 0.05 + (25 * ((TrafficLight)target).waitingCars) + (44 * ((TrafficLight)target).waitingBusses) > top
                    && tlLeft - 0.05 - (25 * ((TrafficLight)target).waitingCars) - (44 * ((TrafficLight)target).waitingBusses) < left
                    && tlLeft + 0.05 + (25 * ((TrafficLight)target).waitingCars) + (44 * ((TrafficLight)target).waitingBusses) > left)
            {
                return true;
            }
            return false;
        }


        public void Drive() //Update position bus accoring to trafficlight being green or target not being a trafficlight
        {
            tlLeft = target.GetLeft();
            tlTop = target.GetTop();
            Rotate();

            if (target is TrafficLight) // if this cars target is Trafficlight
            { // If car is x distance away from trafficlight. This gets higher with more waiting cars or busses.
                if (trafficlightDistanceCheck())
                {
                    if (CheckTrafficLight() == Color.Green)
                    {// get next target
                        int index = route.GetNodes().IndexOf(target);
                        ((TrafficLight)target).SetWaiting(false);
                        ((TrafficLight)target).waitingCars = 0;
                        this.waitingSend = false;
                        if (index < route.GetNodes().Count - 1)
                        {
                            this.target = route.GetNodes()[index + 1];
                        }
                        else
                        {
                            this.Destroy();
                        }
                    }
                    else if (CheckTrafficLight() == Color.Red || CheckTrafficLight() == Color.Orange)
                    {
                        if (this.waitingSend == false)
                        {
                            ((TrafficLight)target).SetWaiting(true);
                            ((TrafficLight)target).waitingCars++;
                            this.waitingSend = true;
                        }
                    }
                }
                else
                {
                    this.waitingSend = false;
                    moveCar(target);
                }
            }
            else if (!(target is TrafficLight))
            { // if target is not Trafficlight use normal distance to reach node.
                if (target.GetTop() - 0.05 < top && target.GetTop() + 0.05 > top && target.GetLeft() - 0.05 < left && target.GetLeft() + 0.05 > left)
                {
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
                else
                {
                    moveCar(target);
                }
            }


        }

        private void moveCar(Node target)
        {
            if (target.GetLeft() > left)
            {
                left = left + speed;

            }
            else if (target.GetLeft() < left)
            {
                left = left - speed;

            }

            if (target.GetTop() > top)
            {
                top = top + speed;

            }
            else if (target.GetTop() < top)
            {
                top = top - speed;
            }
        }



        private void Rotate() // Rotates cars to face the way they're moving
        {
            double targetX = Math.Round(target.GetLeft());
            double targetY = Math.Round(target.GetTop());
            double thisX = Math.Round(left);
            double thisY = Math.Round(top);


            // rotate car according to direction of the target node.
            if (thisX > targetX && targetY == thisY) //W
            {
                rotateTransform.Angle = 0;
                tlLeft = target.GetLeft() + 20;
            }

            else if (thisX < targetX && targetY == thisY) //E
            {
                rotateTransform.Angle = 180;
                tlLeft = target.GetLeft() - 20;
            }

            else if (thisY > targetY && targetX == thisX) //S
            {
                rotateTransform.Angle = 90;
                tlTop = target.GetTop() + 20;
            }

            else if (thisY < targetY && targetX == thisX) //N
            {
                rotateTransform.Angle = 270;
                tlTop = target.GetTop() - 20;
            }

            else if (thisX < targetX && thisY < targetY)
            {
                rotateTransform.Angle = 225;
            }

            else if (thisX > targetX && thisY > targetY)
            {
                rotateTransform.Angle = 45;
            }

            else if (thisX < targetX && thisY > targetY)
            {
                rotateTransform.Angle = 135;

            }

            else if (thisX > targetX && thisY < targetY)
            {
                rotateTransform.Angle = 315;

            }

            carRect.RenderTransform = rotateTransform;
        }
    }
}

