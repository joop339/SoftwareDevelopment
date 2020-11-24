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
        RotateTransform rotateTransform = new RotateTransform();
        RotateTransform rotateTransform2 = new RotateTransform();



        public static List<Car> cars = new List<Car>();
        public static List<Car> destroyedCars = new List<Car>();
        Rectangle carRect; //car icon
        Rectangle rect;

        Rect hitboxCarRect; // carrect hitbox
        Rect hitboxRect; // rect hitbox

        //Node tl = MainWindow.GetNode();
        Node target;

        double left;
        double top;

        Route route;
        static int idCount = 0;
        int id;

        Path carLines;

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

            var x1 = this.left;
            var y1 = this.top;          

            rect = new Rectangle() // rectangle infront of carRect, This is for collision detection
            {
                Width = 7,
                Height = 13,
                Fill = Brushes.Red,
                Stroke = Brushes.Red,
                Opacity = 100
            };

            rotateTransform.CenterX = carRect.Width / 2;
            rotateTransform.CenterY = carRect.Height / 2;

            rotateTransform2.CenterX = rect.Width + carRect.Width / 2;
            rotateTransform2.CenterY = rect.Height / 2;

            hitboxCarRect = new Rect(left, top, carRect.Width, carRect.Height);
            hitboxRect = new Rect(left - rect.Width, top + 0, rect.Width, rect.Height);
        }

        public Car(Route route, bool isTest) //aanmaken van een nieuwe auto met de juiste coordinaten.
        {
            cars.Add(this);
            this.left = route.GetNodes()[0].GetLeft();
            this.top = route.GetNodes()[0].GetTop();
            this.id = idCount;
            idCount++;
            this.route = route;
            this.target = route.GetNodes()[0];

            carLines = new Path()
            {
                //Source = new BitmapImage(new Uri("/resources/car.png")),
                //Width = 19,
                //Height = 13,
                Fill = Brushes.Blue,
                Stroke = Brushes.Black
            };

            hitboxCarRect = new Rect(left, top, 19, 13);

            RectangleGeometry myRectangleGeometry1 = new RectangleGeometry(); // maak geometry met vorm rect
             myRectangleGeometry1.Rect = hitboxCarRect;

            GeometryGroup myGeometryGroup1 = new GeometryGroup(); // conversie
            myGeometryGroup1.Children.Add(myRectangleGeometry1);

            carLines.Data = myGeometryGroup1; // lijntjes met deze geometry

           
        }

        public UIElement ToUIElement()
        {
            return carRect;
        }
        public UIElement ToUIElement2()
        {
            return rect;
        }

        public UIElement ToUIElement3()
        {
            return carLines;
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
        public void Draw()
        {
            Canvas.SetLeft(carRect, left);
            Canvas.SetTop(carRect, top);

            Canvas.SetLeft(rect, left - rect.Width);
            Canvas.SetTop(rect, top + 0);

        }

        //public static void UpdateRects()
        //{
        //    foreach(var car in cars)
        //    {
        //        car.rect.X = car.left;
        //        car.rect.Y = car.top;
        //    }
        //}

        public void Drive() //Positie auto updaten wanneer stoplicht groen is. of de auto nog niet bij het stoplicht is aangekomen.
        {
            //directionCheck();
            //hitboxCarRect = new Rect(left, top, carRect.Width, carRect.Height);
            //hitboxRect = new Rect(left - rect.Width, top + 0, rect.Width, rect.Height);
            if (this.DetectCollision() == false)
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



        bool DetectCollision()
        {
           // Rect bodyRect = new Rect(Canvas.GetLeft(rect), Canvas.getTop(rect), rect.ActualWidth, rect.ActualHeight);

            foreach (Car car in cars) // check alle autos
            {
                if (car != this)
                { // als de auto niet this is           

                    //if (directionCheck(car))
                    //{

                    //var x1 = Canvas.GetLeft(this.carRect);
                    //var y1 = Canvas.GetTop(this.carRect);
                    //Rect r1 = new Rect(x1, y1, this.carRect.ActualWidth + 7, this.carRect.ActualHeight + 7);

                    //var x2 = Canvas.GetLeft(car.carRect);
                    //var y2 = Canvas.GetTop(car.carRect);
                    //Rect r2 = new Rect(x2, y2, car.carRect.ActualWidth + 7, car.carRect.ActualHeight + 7);

                    if (hitboxCarRect.IntersectsWith(car.hitboxRect))
                    {
                        return true;
                    }
                    //}
                }
            }

            return false;

        }



        void directionCheck() // check of de auto in de zelfde richting is als het huidige doelwit
        {
            

            double targetX = Math.Round(target.GetLeft());
            double targetY = Math.Round(target.GetTop());
            double thisX = Math.Round(left);
            double thisY = Math.Round(top);



            if (thisX > targetX && targetY == thisY) // West
            {
                rotateTransform.Angle = 0;             
            }

            else if (thisX < targetX && targetY == thisY) // East
            {
                rotateTransform.Angle = 180;
            }

            else if (thisY > targetY && targetX == thisX) // North
            {
                rotateTransform.Angle = 90;
            }

            else if (thisY < targetY && targetX == thisX) // South
            {
                rotateTransform.Angle = 270;
            }

            else if (thisX < targetX && thisY < targetY) // South East
            {                
                rotateTransform.Angle = 225;
            }

            else if (thisX > targetX && thisY > targetY) // North West
            {               
                rotateTransform.Angle = 45;
            }

            else if (thisX < targetX && thisY > targetY) // North East
            {                
                rotateTransform.Angle = 135;
            }

            else if (thisX > targetX && thisY < targetY) // South West
            {
                rotateTransform.Angle = 315;
            }

            rotateTransform2.Angle = rotateTransform.Angle;
            //carRect.RenderTransform = rotateTransform;
           // rect.RenderTransform = rotateTransform2;

            Matrix matrix = new Matrix(); // used for transformations

            matrix.Rotate(rotateTransform.Angle); // set rotation
            hitboxRect.Transform(matrix); // apply rotation to rect

            matrix.Rotate(rotateTransform.Angle); // set rotation
            hitboxCarRect.Transform(matrix); // apply rotation to rect
        }
    }

}

