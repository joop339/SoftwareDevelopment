using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
