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
        protected int left;
        protected int top;
        
        public Node(int left, int top)
        {

            nodeList.Add(this);
            this.left = left;
            this.top = top;
        }

        public int GetLeft()
        {
            return left;
        }

        public int GetTop()
        {
            return top;
        }
    }
}
