using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp1
{
    public class Bike
    {
        RotateTransform rotateTransform = new RotateTransform();

        public static List<Bike> bikes = new List<Bike>(); // List of Bikes
        public static List<Bike> destroyedBikes = new List<Bike>(); // List of Bikes


        private double left;
        private double top;

        public static double speed = Car.speed / 1.5;
        private bool waitingSend = true;

        private Route route; // To be followed route
        private Node target;

        private Shape body; // shape representation

        public Bike(Route route)
        {
            bikes.Add(this); // add to list
            this.left = route.GetNodes()[0].GetLeft();
            this.top = route.GetNodes()[0].GetTop();

            this.target = route.GetNodes()[0];
            this.route = route; // set route

            body = new Ellipse() // set body
            {
                Width = 20,
                Height = 8,
                Fill = Brushes.Green,
                Stroke = Brushes.Black
            };

            rotateTransform.CenterX = body.Width / 2;
            rotateTransform.CenterY = body.Height / 2;

        }

        public UIElement ToUIElement()
        {
            return body;
        }

        private Color CheckTrafficLight()
        {
            return ((TrafficLight)target).GetColor();
        }
        public void Draw()
        {
            Canvas.SetLeft(body, left);
            Canvas.SetTop(body, top);
        }


        public void Update()
        {
            SwitchTarget();
        }

    private void Walk()
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

        private void SwitchTarget()
        {
            Rotate();
            if (target is TrafficLight)
            { // if target is reached

                if (target.GetTop() - 0.05 - 15 * ((TrafficLight)target).waitingPedestrians < top && target.GetTop() + 0.05 + 15 * ((TrafficLight)target).waitingPedestrians > top && target.GetLeft() - 0.05 - 15 * ((TrafficLight)target).waitingPedestrians < left && target.GetLeft() + 0.05 + 15 * ((TrafficLight)target).waitingPedestrians > left)
                { // and target is TL AND TL is green
                    if (CheckTrafficLight() == Color.Green)
                    {// get next target
                        ((TrafficLight)target).SetWaiting(false);
                        ((TrafficLight)target).waitingPedestrians = 0;

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
                    else if (CheckTrafficLight() == Color.Red || CheckTrafficLight() == Color.Orange)
                    {
                        if (waitingSend == true)
                        {
                            ((TrafficLight)target).SetWaiting(true);
                            ((TrafficLight)target).waitingPedestrians++;
                            waitingSend = false;
                        }
                    }
                }
                else
                {
                    this.waitingSend = true;
                    this.Walk();

                }

            }
            else if (!(target is TrafficLight))
            {
                if (target.GetTop() - 0.05 < top && target.GetTop() + 0.05 > top && target.GetLeft() - 0.05 < left && target.GetLeft() + 0.05 > left)
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
                else
                {
                    this.Walk();
                }
            }
        }

        private void Destroy()
        {
            bikes.Remove(this);
            destroyedBikes.Add(this);
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
            }

            else if (thisX < targetX && targetY == thisY) //E
            {
                rotateTransform.Angle = 180;
            }

            else if (thisY > targetY && targetX == thisX) //S
            {
                rotateTransform.Angle = 90;
            }

            else if (thisY < targetY && targetX == thisX) //N
            {
                rotateTransform.Angle = 270;
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

            body.RenderTransform = rotateTransform;
        }
    }
}