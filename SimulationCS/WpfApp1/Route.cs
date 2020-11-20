using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class Route
    {
        public static List<Route> routes = new List<Route>();
        public static List<Route> altRoutes = new List<Route>();
        private List<Node> nodes = new List<Node>();

        public Route(List<Node> nodes, bool isAlt = false)
        {
            if (isAlt == false)
            {
                this.nodes = nodes;
                routes.Add(this);
            }
            else
            {
                this.nodes = nodes;
                altRoutes.Add(this);
            }

        }

        public List<Node> GetNodes()
        {
            return nodes;
        }     
    }
}
