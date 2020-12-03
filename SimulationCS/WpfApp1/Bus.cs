using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp1
{
    public class Bus
    {
        private RotateTransform rotateTransform = new RotateTransform();

        public static List<Bus> busses = new List<Bus>();
        public static List<Bus> destroyedBusses = new List<Bus>();

        private Rectangle busRect; //Bus icon

        private bool waitingSend = false;
        private bool hasCollided = false;
        private Node target; // next target

        private double left;
        private double top;
        private double tlLeft;
        private double tlTop;

        public static double speed = Car.speed;

        private Route route; // to be followed route

        public Bus(Route route) //innitialising bus with coordinates and route.
        {
            busses.Add(this);
            this.left = route.GetNodes()[0].GetLeft();
            this.top = route.GetNodes()[0].GetTop();

            this.route = route;
            this.target = route.GetNodes()[0];

            busRect = new Rectangle()
            {
                //Source = new BitmapImage(new Uri("/resources/Bus.png")),
                Width = 38,
                Height = 13,
                Fill = Brushes.LightBlue,
                Stroke = Brushes.Black
            };

            rotateTransform.CenterX = busRect.Width / 3;
            rotateTransform.CenterY = busRect.Height / 2;
        }


        public UIElement ToUIElement()
        {
            return busRect;
        }

        private Color CheckTrafficLight()
        {
            return ((TrafficLight)target).GetColor();
        }

        private void Destroy()
        {
            busses.Remove(this);
            destroyedBusses.Add(this);
        }
        public void Draw()
        {
            Canvas.SetLeft(busRect, left);
            Canvas.SetTop(busRect, top);
        }

        private bool trafficlightDistanceCheck() // check distance between bus and target trafficlight. depending on amount of cars and busses already waiting
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

            if (target is TrafficLight)
            { // and target is TL AND TL is green
                if (trafficlightDistanceCheck())
                { // If bus is x distance away from trafficlight. This gets higher with more waiting cars or busses.
                    if (CheckTrafficLight() == Color.Green)
                    {// get next target
                        int index = route.GetNodes().IndexOf(target);
                        ((TrafficLight)target).SetWaiting(false);
                        ((TrafficLight)target).waitingBusses = 0;
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
                            ((TrafficLight)target).waitingBusses++;
                            this.waitingSend = true;
                        }
                    }
                }
                else
                {
                    this.waitingSend = false;
                    MoveBus(target);
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
                    MoveBus(target);
                }
            }


        }

        private void MoveBus(Node target)
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



        void Rotate() // check of de auto in de zelfde richting is als het huidige doelwit
        {
            double targetX = Math.Round(target.GetLeft());
            double targetY = Math.Round(target.GetTop());
            double thisX = Math.Round(left);
            double thisY = Math.Round(top);


            // rotate and change hitbox bus according to direction of the target node.
            if (thisX > targetX && targetY == thisY) //W
            {
                rotateTransform.Angle = 0;
                tlLeft = target.GetLeft() + 20;
            }

            else if (thisX < targetX && targetY == thisY) //E
            {
                rotateTransform.Angle = 180;
                tlLeft = target.GetLeft() - 30;
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

            busRect.RenderTransform = rotateTransform;
        }
    }
}

