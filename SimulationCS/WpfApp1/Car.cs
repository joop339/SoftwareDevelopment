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

        public static List<Car> cars = new List<Car>();
        public static List<Car> destroyedCars = new List<Car>();
        Rectangle carRect; //car icon
        //Node tl = MainWindow.GetNode();
        Node target;

        double left;
        double top;

        Route route;
        static int idCount = 0;
        int id;

        public Car(Route route) //aanmaken van een nieuwe auto met de juiste coordinaten.
        {
            cars.Add(this);
            this.left = route.GetNodes()[0].GetLeft();
            this.top = route.GetNodes()[0].GetTop();
            this.id = idCount;
            idCount++;
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
            Canvas.SetLeft(carRect, left);
            Canvas.SetTop(carRect, top);
        }

        //public Car(double left, double top, Route route) //aanmaken van een nieuwe auto met de juiste coordinaten.
        //{
        //    cars.Add(this);
        //    this.left = left;
        //    this.top = top;
        //    this.id = idCount;
        //    idCount++;
        //    this.route = route;
        //    this.target = route.GetNodes()[0];

        //    carRect = new Rectangle()
        //    {
        //        //Source = new BitmapImage(new Uri("/resources/car.png")),
        //        Width = 19,
        //        Height = 13,
        //        Fill = Brushes.Maroon,
        //        Stroke = Brushes.Black
        //    };
        //    Canvas.SetLeft(carRect, left);
        //    Canvas.SetTop(carRect, top);
        //}

        //~Car()
        //{
        //    cars.Remove(this);        
        //}

        //public static implicit operator UIElement(Car car) //Omzetten UI-element
        //{

        //}

        public UIElement ToUIElement()
        {
            return carRect;
        }

        public void Update(/*Color trafficLightColor*/) //Visueel updaten kleur stoplicht.
        {
            Drive();
        }

        //void Drive(double distance) //Positie auto updaten.
        //{

        //    Console.WriteLine("IK BEN AAAN HET RIJJDEN");
        //    //car.Margin = new Thickness(car.Margin.Left + 10, 314, car.Margin.Right - 10, 0);
        //    Canvas.SetTop(car, y);
        //    Canvas.SetLeft(car, x - 1);
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

        void Drive() //Positie auto updaten wanneer stoplicht groen is. of de auto nog niet bij het stoplicht is aangekomen.
        {
            if (this.DetectCollision() == false)
            {

                if (target.GetLeft() > left)
                {
                    left = left + 0.025;
                    Canvas.SetLeft(carRect, left);
                }
                else if (target.GetLeft() < left)
                {
                    left = left - 0.025;
                    Canvas.SetLeft(carRect, left);
                }

                if (target.GetTop() > top)
                {
                    top = top + 0.025;
                    Canvas.SetTop(carRect, top);
                }
                else if (target.GetTop() < top)
                {
                    top = top - 0.025;
                    Canvas.SetTop(carRect, top);
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



        bool DetectCollision()
        {
            foreach (Car car in cars) // check alle autos
            {
                if (car != this)
                { // als de auto niet this is           

                    if (directionCheck(car))
                    {

                        var x1 = Canvas.GetLeft(this.carRect);
                        var y1 = Canvas.GetTop(this.carRect);
                        Rect r1 = new Rect(x1, y1, this.carRect.ActualWidth + 7, this.carRect.ActualHeight + 7);

                        var x2 = Canvas.GetLeft(car.carRect);
                        var y2 = Canvas.GetTop(car.carRect);
                        Rect r2 = new Rect(x2, y2, car.carRect.ActualWidth + 7, car.carRect.ActualHeight + 7);

                        if (r1.IntersectsWith(r2))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;

        }

        bool directionCheck(Car car) // check of de auto in de zelfde richting is als het huidige doelwit
        {
            double roundedLeft = Math.Round(left);
            double roundedTop = Math.Round(top);

            double carLeft = Math.Round(car.left);
            double carTop = Math.Round(car.top);

            double targetLeft = Math.Round(target.GetLeft());
            double targetTop = Math.Round(target.GetTop());

            bool isTargetAndCarLeft = roundedLeft > carLeft && roundedLeft > targetLeft;
            bool isTargetAndCarRight = roundedLeft < carLeft && roundedLeft < targetLeft;
            bool isTargetAndCarTop = roundedTop > carTop && roundedTop > targetTop;
            bool isTargetAndCarDown = roundedTop < carTop && roundedTop < targetTop;

            
            if (
                        (isTargetAndCarLeft) // doelwit en andere auto zijn links van this
                        || (isTargetAndCarRight) // doelwit en andere auto zijn rechts van this
                        || (isTargetAndCarTop) // doelwit en andere auto zijn boven this
                        || (isTargetAndCarDown) // doelwit en andere auto zijn onder this
               )
            {
                return true;
            }

            return false;


        }
    }

}

