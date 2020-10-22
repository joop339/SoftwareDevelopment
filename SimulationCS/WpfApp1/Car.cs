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

namespace WpfApp1
{
    public class Car
    {
        Image car; //car icon
        //Node tl = MainWindow.GetNode();
        Node target;

        double left;
        double top;

        Route route;

        public Car(double left, double top, Route route) //aanmaken van een nieuwe auto met de juiste coordinaten.
        {

            this.left = left;
            this.top = top;

            this.route = route;
            this.target = route.GetNodes()[0];

            car = new Image()
            {
                //Source = new BitmapImage(new Uri("/resources/car.png")),
                Source = new BitmapImage(new Uri("pack://application:,,,/Resources/car.png")),
            };
            Canvas.SetLeft(car, left);
            Canvas.SetTop(car, top);          
        }

        //public static implicit operator UIElement(Car car) //Omzetten UI-element
        //{
            
        //}

        public UIElement ToUIElement()
        {
            return car;
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

        void Drive() //Positie auto updaten wanneer stoplicht groen is. of de auto nog niet bij het stoplicht is aangekomen.
        {
            
                if (target.GetLeft() > left)
                {
                    left = left + 0.025;
                    Canvas.SetLeft(car, left);
                }
                else if (target.GetLeft() < left)
                {
                    left = left - 0.025;
                    Canvas.SetLeft(car, left);
                }
                
                if(target.GetTop() > top)
                {
                    top = top + 0.025;
                    Canvas.SetTop(car, top);
                }
                else if (target.GetTop() < top)
                {
                    top = top - 0.025;
                    Canvas.SetTop(car, top);
                }

            if (target.GetTop() - 0.05 < top && target.GetTop() + 0.05 > top && target.GetLeft() - 0.05 < left && target.GetLeft() + 0.05 > left) // if target is reached

                if (target is TrafficLight)
                { // and target is TL AND TL is green
                    if (CheckTrafficLight() == Color.Green)
                    {// get next target
                        int index = route.GetNodes().IndexOf(target);
                        this.target = route.GetNodes()[index + 1];

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
                    }
                }
                }

            

        }
    }

