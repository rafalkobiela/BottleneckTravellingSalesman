using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASD.Graphs;

namespace ASD.Graphs
{
    class FullGraphGenerator
    {
        static Random rand;

        public static List<Graph> generateFullGraphs(int lowNumberOfGraphs, int highNumberOfGraphs)
        {
            var ret = new List<Graph>();
            rand = new Random(0);

            for (int i = lowNumberOfGraphs; i < highNumberOfGraphs + 1; i++)
            {
                ret.Add(generateTree(i));
            }

            return ret;
        }

        static Graph generateTree(int numberOfVertices)
        {
            var graph = new AdjacencyMatrixGraph(false, numberOfVertices);
    
            for (int i=0; i<numberOfVertices; i++)
            {
                for (int j=0; j<i; j++)
                {
                    graph.AddEdge(i, j, rand.Next(3, 8));
                }
            }

            return graph;
        }
    }
}
