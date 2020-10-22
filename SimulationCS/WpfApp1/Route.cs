﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class Route
    {
        public static List<Route> routes = new List<Route>();
        private List<Node> nodes = new List<Node>();

        public Route(List<Node> nodes)
        {
            this.nodes = nodes;
        }

        public List<Node> GetNodes()
        {
            return nodes;
        }
    }
}