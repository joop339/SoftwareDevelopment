using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public enum RoadType
    {
        Road, // 0
        NonCarRoad, // 1
        BusRoad, // 0
    }

    public class Route
    {
        public static List<Route> routes = new List<Route>();
        public static List<Route> nonCarRoutes = new List<Route>();
        public static List<Route> busRoutes = new List<Route>();
        private List<Node> nodes = new List<Node>();

        public Route(List<Node> nodes, RoadType roadType = RoadType.Road)
        {
            this.nodes = nodes;
            if (roadType == RoadType.Road)
            {
                routes.Add(this);
            }
            else if (roadType == RoadType.NonCarRoad)
            {
                nonCarRoutes.Add(this);
            }
            else if (roadType == RoadType.BusRoad)
            {              
                busRoutes.Add(this);
            }

        }

        public List<Node> GetNodes()
        {
            return nodes;
        }
    }
}
