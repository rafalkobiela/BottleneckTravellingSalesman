using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASD.Graphs;

namespace ASD.Graphs
{
    public static class HamiltonPathChecker
    {
        /// <summary>
        /// Funkcja sprawdza, czy drugi argument jest poprawnym cyklem Hamiltona dla grafu w pierwszym argumencie.
        /// Oczywiście chodzi o cykl Hamiltona utworzony z możliwością przeskoków.
        /// </summary>
        public static bool isCorrectHamiltonianCycle(Graph g, List<int> cycle)
        {
            // Cykl Hamiltona musi być tak długi, jak ilość wierzchołków w grafie
            if (cycle.Count() != g.VerticesCount)
                return false;

            // W cyklu Hamiltona wierzchołki nie mogą się powtarzać
            bool isUnique = cycle.Distinct().Count() == cycle.Count();

            if (!isUnique)
                return false;

            var onesGraph = new AdjacencyMatrixGraph(false, g.VerticesCount);
            for (int i = 0; i < g.VerticesCount; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (!double.IsNaN(g.GetEdgeWeight(i, j)))
                    {
                        onesGraph.AddEdge(i, j);
                    }
                }
            }

            List<PathsInfo[]> pathsFromVertices = new List<PathsInfo[]>();

            for (int i = 0; i < onesGraph.VerticesCount; i++)
            {
                PathsInfo[] path;
                onesGraph.DijkstraShortestPaths(i, out path);
                pathsFromVertices.Add(path);
            }

            bool good = true;

            for (int i = 0; i < cycle.Count - 1; i++)
            {
                if (pathsFromVertices[cycle[i]][cycle[i + 1]].Dist > 3)
                {
                    good = false;
                    Console.WriteLine($"{cycle[i]}, {cycle[i + 1]}, cost: {pathsFromVertices[cycle[i]][cycle[i + 1]].Dist}");
                }
            }

            return good;
        }

        /// <summary>
        /// Wyznacza koszt ścieżki Hamiltona. 
        /// Gdy następuje przeskakiwanie miedzy wierzchołkami, to do sumy dodajemy sumę wag najkrószej ścieżki łączącej te wierzchołki.
        /// </summary>
        public static int costOfHamiltonianPath(Graph g, List<int> cycle)
        {
            int sum = 0;

            List<PathsInfo[]> pathsFromVertices = new List<PathsInfo[]>();
            for (int i = 0; i < g.VerticesCount; i++)
            {
                PathsInfo[] path;
                g.DijkstraShortestPaths(i, out path);
                pathsFromVertices.Add(path);
            }

            for (int i = 0; i < cycle.Count - 1; i++)
            {
                sum += (int)pathsFromVertices[cycle[i]][cycle[i + 1]].Dist;
            }

            return sum;
        }
    }
}
