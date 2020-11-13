using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace WpfApp1
{
    public class Node
    {
        public static List<Node> nodeList = new List<Node>();
        protected double left;
        protected double top;
        public Node()
        {
            nodeList.Add(this);
        }
        public Node(double left, double top)
        {
            nodeList.Add(this);
            this.left = left;
            this.top = top;
        }

        public Node(Ellipse ellipse)
        {
            nodeList.Add(this);
            left = Canvas.GetLeft(ellipse);
            top = Canvas.GetTop(ellipse);
        }

        public double GetLeft()
        {
            return left;
        }

        public double GetTop()
        {
            return top;
        }
    }
}
