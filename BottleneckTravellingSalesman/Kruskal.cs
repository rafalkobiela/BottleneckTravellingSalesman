using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASD.Graphs;

namespace ASD.Graphs
{
    /// <summary>
    /// Znajdowanie minimalnego drzewa rozpinającego metodą Kruskala (MST jest MBST)
    /// </summary>
    public static partial class SpanningTreeKruskal
    {
        public static Graph Kruskal(Graph graph)
        {
            var edges = getGraphEdgesMin(graph);

            UnionFind uf = new UnionFind(graph.VerticesCount);
            var retGraph = new AdjacencyMatrixGraph(false, graph.VerticesCount);

            foreach (Edge e in edges.ToArray())
            {
                if (uf.Find(e.From) != uf.Find(e.To))
                {
                    retGraph.AddEdge(e);
                    uf.Union(e.From, e.To);
                }
            }

            return retGraph;
        }

        public static EdgesMinPriorityQueue getGraphEdgesMin(Graph graph)
        {
            EdgesMinPriorityQueue edges = new EdgesMinPriorityQueue();

            int verticesCount = graph.VerticesCount;

            for (int i = 0; i < verticesCount; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (!double.IsNaN(graph.GetEdgeWeight(i, j)))
                    {
                        edges.Put(new Edge(i, j, graph.GetEdgeWeight(i, j)));
                    }
                }
            }
            return edges;
        }

    }
}
