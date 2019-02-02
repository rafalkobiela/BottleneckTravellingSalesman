using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASD.Graphs;

namespace ASD.Graphs
{
    /// <summary>
    /// Generator losowych drzew.
    /// </summary>
    public static class TreeGenerator
    {
        static Graph graph;
        static Random rand;

        public static List<Graph> generateTrees(int lowNumberOfGraphs, int highNumberOfGraphs)
        {
            var ret = new List<Graph>();
            rand = new Random(0);

            for (int i=lowNumberOfGraphs; i< highNumberOfGraphs + 1; i++)
            {
                ret.Add(generateTree(i));
            }

            return ret;
        }

        static Graph generateTree(int numberOfVertices)
        {
            graph = new AdjacencyMatrixGraph(false, numberOfVertices);
            
            var forward = new List<int>();
            forward.Add(0);

            int curr = 0;

            while (true)
            {
                var tmp = new List<int>();

                    foreach (var ver in forward)
                    {
                        if (curr + 1 < numberOfVertices)
                            graph.AddEdge(ver, ++curr);

                        var lim = rand.Next(0, 2) + 1;

                        for (int i = 0; i < lim; i++)
                        {
                            if (curr + 1 < numberOfVertices)
                                graph.AddEdge(curr, ++curr);
                        }

                        tmp.Add(curr);
                    }

                forward = new List<int>();

                foreach(var ver in tmp)
                {
                    var lim = rand.Next(2, 5);
                    for (int i = 0; i < lim; i++)
                    {
                        if (curr + 1 < numberOfVertices)
                        {
                            graph.AddEdge(ver, ++curr);
                            forward.Add(curr);
                        }
                    }
                }

                if (curr == numberOfVertices - 1)
                    break;
            }

            return graph;
        }
    }
}
