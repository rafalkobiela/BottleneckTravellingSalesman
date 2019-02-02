using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASD.Graphs;

namespace ASD.Graphs
{
    public static partial class SpanningTreeCamerini
    {
        public static void testCamerini()
        {
            testForest();
            testCreatePartGraphFromEdges();
            testGetGraphEdges();
            testMergeGraphs();
        }

        static void testForest()
        {
            var x = new HashSet<Edge>
            {
                new Edge(2, 6),
                new Edge(2, 4),
                new Edge(4, 6),
                new Edge(8, 6),
                new Edge(2, 8)
            };

            var res = createPartGraphFromEdges(x);
            var spanning = Forest(res);
            printGraph(res, "");
            printGraph(spanning, "");
        }

        static void testCreatePartGraphFromEdges()
        {
            var x = new HashSet<Edge>
            {
                new Edge(2, 6),
                new Edge(2, 4),
                new Edge(4, 6),
                new Edge(8, 6),
                new Edge(2, 8)
            };

            var res = createPartGraphFromEdges(x);
            printGraph(res, "");
        }

        static void testGetGraphEdges()
        {
            var x = new HashSet<Edge>
            {
                new Edge(2, 6),
                new Edge(2, 4),
                new Edge(4, 6),
                new Edge(8, 6),
                new Edge(2, 8)
            };

            var res = createPartGraphFromEdges(x);
            var edges = getGraphEdges(res).ToArray();

            foreach (Edge e in edges)
            {
                Console.WriteLine(e);
            }
        }

        static void testMergeGraphs()
        {
            Graph one = new AdjacencyMatrixGraph(false, 4);
            one.AddEdge(0, 1);
            one.AddEdge(0, 2);
            one.AddEdge(2, 3);
            one.AddEdge(3, 1);

            Dictionary<int, int> dictOne = new Dictionary<int, int>
            {
                { 0, 8 },
                { 1, 7 },
                { 2, 5 },
                { 3, 6 }
            };

            Graph two = new AdjacencyMatrixGraph(false, 3);
            two.AddEdge(0, 1);
            two.AddEdge(1, 2);

            Dictionary<int, int> dictTwo = new Dictionary<int, int>
            {
                { 0, 12 },
                { 1, 10 },
                { 2, 8 }
            };

            var res = MergeGraphs(
                new PartGraph { Graph = one, NumbersInOriginal = dictOne },
                new PartGraph { Graph = two, NumbersInOriginal = dictTwo });

            printGraph(res, "");
        }
    }
}
