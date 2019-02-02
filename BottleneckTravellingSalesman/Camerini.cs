using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASD.Graphs;

namespace ASD.Graphs
{
    public class PartGraph
    {
        private Graph graph;
        private Dictionary<int, int> numbersInOriginal;
        public Graph Graph { get; set; }
        public Dictionary<int, int> NumbersInOriginal { get; set; }
    }

    public static partial class SpanningTreeCamerini
    {
        // Graf ma być spójny i nieskierowany
        public static PartGraph Camerini(PartGraph partGraph)
        {
            var graph = partGraph.Graph;
            var edges = getGraphEdges(partGraph);

            if (edges.Count == 1)
                return partGraph;

            // A - waga wieksza lub równa niż w B
            (var A, var B) = setAandB(edges);

            PartGraph GB = createPartGraphFromEdges(B);
            var F = Forest(GB);

            if (isSpanningTree(graph, F.Graph))
            {
                return Camerini(GB);
            }
            else
            {
                var GA = createPartGraphFromEdges(A);

                var CameriniforGA = Camerini(GA);
                PartGraph ret = MergeGraphs(CameriniforGA, F);

                return ret;
            }
        }

        /// <summary>
        /// Zwraca maksymalny las (maximal forest) w grafie
        /// </summary>
        static PartGraph Forest(PartGraph partGraph)
        {
            var graph = partGraph.Graph;

            List<int> notVisited = new List<int>();
            for (int i=0; i<graph.VerticesCount; i++)
            {
                notVisited.Add(i);
            }

            HashSet<Edge> edges = new HashSet<Edge>();

            while(notVisited.Count > 0)
            {
                int curNum = notVisited[0];

                notVisited.Remove(curNum);

                foreach(var edge in graph.OutEdges(curNum))
                {
                    edges.Add(new Edge(partGraph.NumbersInOriginal[edge.From], partGraph.NumbersInOriginal[edge.To], edge.Weight));
                    notVisited.Remove(edge.To);
                }
            }

            return createPartGraphFromEdges(edges);
        }

        /// <summary>
        /// Łączy dwa PartGrafy, zwracając PartGraf, będący sumą. Etykiety wierzchołków są zgodne z etykietami w oryginalnym grafie.
        /// </summary>
        static PartGraph MergeGraphs(PartGraph A, PartGraph B)
        {
            HashSet<Edge> edges = new HashSet<Edge>();

            foreach (var e in getGraphEdges(A).ToArray())
                edges.Add(e);

            foreach (var e in getGraphEdges(B).ToArray())
                edges.Add(e);

            return createPartGraphFromEdges(edges);
        }

        /// <summary>
        /// Sprawdza, czy graf w drugim argumencie jest drzewem rozpinającym grafu z pierwszego argumentu
        /// </summary>
        static bool isSpanningTree(Graph original, Graph tree)
        {
            if (tree.EdgesCount != tree.VerticesCount - 1)
                return false;

            if (tree.VerticesCount != original.VerticesCount)
                return false;

            return true;
        }

        /// <summary>
        /// Zwraca kolejkę krawędzi, oznaczonych numeracją grafu oryginalnego
        /// </summary>
        static EdgesMaxPriorityQueue getGraphEdges(PartGraph partGraph)
        {
            var graph = partGraph.Graph;
            EdgesMaxPriorityQueue edges = new EdgesMaxPriorityQueue();

            int verticesCount = graph.VerticesCount;

            for (int i = 0; i < verticesCount; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (!double.IsNaN(graph.GetEdgeWeight(i, j)))
                    {
                        edges.Put(new Edge(partGraph.NumbersInOriginal[i] , partGraph.NumbersInOriginal[j], graph.GetEdgeWeight(i, j)));
                    }
                }
            }
            return edges;
        }

        /// <summary>
        /// Dzieli zbiór krawędzi na dwa podzbiory
        /// A - waga wieksza lub równa niż w B
        /// </summary>
        static (HashSet<Edge>, HashSet<Edge>) setAandB(EdgesMaxPriorityQueue edges)
        {
            // A - waga wieksza lub równa niż w B
            HashSet<Edge> A = new HashSet<Edge>();
            HashSet<Edge> B = new HashSet<Edge>();

            int numEdges = edges.Count;

            for (int i = 0; i < numEdges; i++)
            {
                if (i < numEdges / 2)
                {
                    A.Add(edges.Get());
                }
                else
                {
                    B.Add(edges.Get());
                }
            }

            return (A, B);
        }

        /// <summary>
        /// Na podstawie danych krawędzi, tworzy graf i zwraca go wraz ze słownikiem
        /// </summary>
        /// <param name="edges">Krawędzie mają być oznaczone zgodnie z oznaczeniem grafu oryginalnego</param>
        static PartGraph createPartGraphFromEdges(HashSet<Edge> edges)
        {
            HashSet<int> vertices = new HashSet<int>();
            foreach (Edge e in edges)
            {
                vertices.Add(e.From);
                vertices.Add(e.To);
            }

            var verticesList = vertices.ToList();
            verticesList.Sort();

            var dict = new Dictionary<int, int>();
            for (int i = 0; i < verticesList.Count; i++)
            {
                dict[i] = verticesList[i];
            }

            var revDict = dict.ToDictionary(x => x.Value, x => x.Key);

            var graph = new AdjacencyMatrixGraph(false, vertices.Count);

            foreach (Edge e in edges)
            {
                graph.AddEdge(revDict[e.From], revDict[e.To], e.Weight);
            }

            return new PartGraph { Graph = graph, NumbersInOriginal = dict };
        }

        /// <summary>
        /// Metoda pokazuje graf w przeglądarce i wypisuje informacje na temat grafu.
        /// </summary>
        public static int graphNum = 0;
        static void printGraph(PartGraph GB, string comment)
        {
            Console.WriteLine($"Graph number: {++graphNum}");
            Console.WriteLine(comment);
            foreach (var x in GB.NumbersInOriginal)
                Console.WriteLine($"{x.Key} : {x.Value}");
            Console.WriteLine();
        }
    }
}
