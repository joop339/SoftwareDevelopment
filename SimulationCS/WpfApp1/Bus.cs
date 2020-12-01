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
    public class Bus
    {
        //MainWindow main = (MainWindow)Application.Current.MainWindow;
        RotateTransform rotateTransform = new RotateTransform();
        RotateTransform rotateTransform2 = new RotateTransform();

        public static List<Bus> busses = new List<Bus>();
        public static List<Bus> destroyedBusses = new List<Bus>();

        public Rectangle busRect; //Bus icon
        //public Rectangle carHitbox;

        public bool waitingSend = false;
        public bool hasCollided = false;
        public Node target; // next target

        double left;
        double top;
        double tlLeft;
        double tlTop;

        public static double speed = 0.008;

        Route route; // to be followed route
        //static int idCount = 0;
        //int id;

        public Bus(Route route) //aanmaken van een nieuwe auto met de juiste coordinaten.
        {
            busses.Add(this);
            this.left = route.GetNodes()[0].GetLeft();
            this.top = route.GetNodes()[0].GetTop();
            //this.id = idCount;
            //idCount++;
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

            //carHitbox = new Rectangle() // rectangle infront of carRect, This is for collision detection
            //{
            //    Width = 7,
            //    Height = 13,
            //    Fill = Brushes.Red,
            //    Stroke = Brushes.Red,
            //    //Opacity = 100,
            //};

            rotateTransform.CenterX = busRect.Width / 3;
            rotateTransform.CenterY = busRect.Height / 2;

            rotateTransform2.CenterX = busRect.Width / 3;
            rotateTransform2.CenterY = busRect.Height / 2;
        }


        public UIElement ToUIElement()
        {
            return busRect;
        }

        //public UIElement ToUIElement2()
        //{
        //    return carHitbox;
        //}

        public Color CheckTrafficLight()
        {
            return ((TrafficLight)target).GetColor();
        }

        void Destroy()
        {
            busses.Remove(this);
            destroyedBusses.Add(this);
        }
        public void Draw()
        {
            Canvas.SetLeft(busRect, left);
            Canvas.SetTop(busRect, top);

            //Canvas.SetLeft(carHitbox, left);
            //Canvas.SetTop(carHitbox, top);
        }



        public void Drive() //Positie auto updaten wanneer stoplicht groen is. of de auto nog niet bij het stoplicht is aangekomen.
        {
            if (this.hasCollided == false)
            {
                tlLeft = target.GetLeft();
                tlTop = target.GetTop();
                directionCheck();


                // if target is reached

                if (target is TrafficLight)
                { // and target is TL AND TL is green
                    if (tlTop - 0.05 - (25 * ((TrafficLight)target).waitingCars) - (44 * ((TrafficLight)target).waitingBusses) < top && tlTop + 0.05 + (25 * ((TrafficLight)target).waitingCars) + (44 * ((TrafficLight)target).waitingBusses) > top && tlLeft - 0.05 - (25 * ((TrafficLight)target).waitingCars) - (44 * ((TrafficLight)target).waitingBusses) < left && tlLeft + 0.05 + (25 * ((TrafficLight)target).waitingCars) + (44 * ((TrafficLight)target).waitingBusses) > left)
                    {
                        if (CheckTrafficLight() == Color.Green)
                        {// get next target
                            int index = route.GetNodes().IndexOf(target);
                            ((TrafficLight)target).SetWaiting(false);
                            ((TrafficLight)target).waitingBusses = 0;
                            this.waitingSend = false;
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
                        else if (CheckTrafficLight() == Color.Red)
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
                        moveBus(target);
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
                        moveBus(target);
                    }
                }

            }
        }

        void moveBus(Node target)
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



        void directionCheck() // check of de auto in de zelfde richting is als het huidige doelwit
        {
            double targetX = Math.Round(target.GetLeft());
            double targetY = Math.Round(target.GetTop());
            double thisX = Math.Round(left);
            double thisY = Math.Round(top);



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

            rotateTransform2.Angle = rotateTransform.Angle;
            busRect.RenderTransform = rotateTransform;
            //carHitbox.RenderTransform = rotateTransform2;
        }
    }
}

