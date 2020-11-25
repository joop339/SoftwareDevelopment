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
        //MainWindow main = (MainWindow)Application.Current.MainWindow;
        RotateTransform rotateTransform = new RotateTransform();
        RotateTransform rotateTransform2 = new RotateTransform();

        public static List<Car> cars = new List<Car>();
        public static List<Car> destroyedCars = new List<Car>();

        public Rectangle carRect; //car icon
        //public Rectangle carHitbox;

        public bool hasCollided = false;
        Node target; // next target

        double left;
        double top;

        Route route; // to be followed route
        //static int idCount = 0;
        //int id;

        public Car(Route route) //aanmaken van een nieuwe auto met de juiste coordinaten.
        {
            cars.Add(this);
            this.left = route.GetNodes()[0].GetLeft();
            this.top = route.GetNodes()[0].GetTop();
            //this.id = idCount;
            //idCount++;
            this.route = route;
            this.target = route.GetNodes()[0];

            carRect = new Rectangle()
            {
                //Source = new BitmapImage(new Uri("/resources/car.png")),
                Width = 19,
                Height = 13,
                Fill = Brushes.Maroon,
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

            rotateTransform.CenterX = carRect.Width / 2;
            rotateTransform.CenterY = carRect.Height / 2;

            rotateTransform2.CenterX = carRect.Width / 2;
            rotateTransform2.CenterY = carRect.Height / 2;
        }


        public UIElement ToUIElement()
        {
            return carRect;
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
            cars.Remove(this);
            destroyedCars.Add(this);
        }
        public void Draw()
        {
            Canvas.SetLeft(carRect, left);
            Canvas.SetTop(carRect, top);

            //Canvas.SetLeft(carHitbox, left);
            //Canvas.SetTop(carHitbox, top);
        }



        public void Drive() //Positie auto updaten wanneer stoplicht groen is. of de auto nog niet bij het stoplicht is aangekomen.
        {
            if (this.hasCollided == false)
            {
            directionCheck();
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
                                //Console.WriteLine(target.name);
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
        }       


    


        void directionCheck() // check of de auto in de zelfde richting is als het huidige doelwit
        {
            double targetX = Math.Round(target.GetLeft());
            double targetY = Math.Round(target.GetTop());
            double thisX = Math.Round(left);
            double thisY = Math.Round(top);



            if (thisX > targetX && targetY == thisY)
            {
                rotateTransform.Angle = 0;
            }

            else if (thisX < targetX && targetY == thisY)
            {
                rotateTransform.Angle = 180;
            }

            else if (thisY > targetY && targetX == thisX)
            {
                rotateTransform.Angle = 90;
            }

            else if (thisY < targetY && targetX == thisX)
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

            rotateTransform2.Angle = rotateTransform.Angle;
            carRect.RenderTransform = rotateTransform;
            //carHitbox.RenderTransform = rotateTransform2;
        }
        }
    //bool isTargetAndCarLeft = roundedLeft > carLeft && roundedLeft > targetLeft;
    //bool isTargetAndCarRight = roundedLeft < carLeft && roundedLeft < targetLeft;
    //bool isTargetAndCarTop = roundedTop > carTop && roundedTop > targetTop;
    //bool isTargetAndCarDown = roundedTop < carTop && roundedTop < targetTop;


    //if (
    //            (isTargetAndCarLeft) // doelwit en andere auto zijn links van this
    //            || (isTargetAndCarRight) // doelwit en andere auto zijn rechts van this
    //            || (isTargetAndCarTop) // doelwit en andere auto zijn boven this
    //            || (isTargetAndCarDown) // doelwit en andere auto zijn onder this
    //   )
    //{
    //    return true;
    //}
}

