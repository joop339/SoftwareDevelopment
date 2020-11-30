﻿using System.Collections.Generic;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    public class Bike
    {
        public static List<Bike> Bikes = new List<Bike>(); // List of Bikes
        public static List<Bike> destroyedBikes = new List<Bike>(); // List of Bikes

        double left;
        double top;

        double speed = Car.speed / 2;
        bool waitingSend = true;

        Route route; // To be followed route
        Node target;

        Shape body; // shape representation

        public Bike(Route route)
        {
            Bikes.Add(this); // add to list
            this.left = route.GetNodes()[0].GetLeft();
            this.top = route.GetNodes()[0].GetTop();

            this.target = route.GetNodes()[0];
            this.route = route; // set route

            body = new Ellipse() // set body
            {
                Width = 8,
                Height = 20,
                Fill = Brushes.Green,
                Stroke = Brushes.Black
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
        public void Draw()
        {
            Canvas.SetLeft(body, left);
            Canvas.SetTop(body, top);
        }


        public void Update()
        {
            SwitchTarget();
        }

        public void Walk()
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

        public void SwitchTarget()
        {
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
                    else if (CheckTrafficLight() == Color.Red)
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

        void Destroy()
        {
            Bikes.Remove(this);
            destroyedBikes.Add(this);
        }
    }

}