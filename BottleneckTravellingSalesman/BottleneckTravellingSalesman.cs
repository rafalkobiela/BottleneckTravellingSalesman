using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASD.Graphs
{

    /// <summary>
    /// Rozszerzenie klasy <see cref="Graph"/> o rozwiazywanie problemu komiwojazera-krótkodystansowca
    /// </summary>
    public static partial class BottleneckTravellingSalesman
    {
        public class MergeRet
        {
            public int v_i;
            public int? x_i;
            public List<int> hamiltionianPath;

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();

                sb.Append($"v_i: {v_i}, ");
                sb.Append($"x_i: {x_i}, ");
                sb.Append("hamiltonian path: ");
                sb.Append(hamiltionianPath == null ? "null" : " ");

                if (hamiltionianPath != null)
                {
                    foreach (var el in hamiltionianPath)
                    {
                        sb.Append($"{el} ");
                    }
                }

                return sb.ToString();
            }
        }

        static Graph spanningTree;

        public static List<int> ConstructHamiltonianPath(Graph spanning)
        {
            prohibitedVertices = new List<int>();
            spanningTree = spanning;
            int n_vertices = spanningTree.VerticesCount;

            // znajdz pierwszy mozliwy wierzcholek ze stopniem wiekszym niz 2
            // Jesli nie ma takiego wierzcholka (drzewo jest sciezka bez rozgalezien) - wez pierwszy wierzcholek
            int notLeafNum = 0;
            for (int i = 0; i < n_vertices; i++)
            {
                if (spanningTree.OutDegree(i) > 2)
                {
                    notLeafNum = i;
                    break;
                }
            }

            // Ekslorujemy wokól tego wierzcholka. Ten wierzcholek to nasze v.
            var res = Merge(notLeafNum, null, null).hamiltionianPath;

            return res;
        }

        static List<int> prohibitedVertices;

        /// <summary>
        /// Przyłączanie ścieżek Hamiltona do obecnego wierzchołka
        /// </summary>
        private static List<int> Bind(List<MergeRet> toBind, int vertex)
        {
            var hampath = new List<int>();

            var toBindWithHamilton = toBind.Where(x => x.hamiltionianPath != null);
            var toBindWith2Vertices = toBind.Where(x => (x.hamiltionianPath == null && x.x_i != null));
            var toBindWith1Vertex = toBind.Where(x => x.x_i == null);


            foreach (var el in toBindWith1Vertex)
            {
                hampath.Add(el.v_i);
            }

            foreach (var el in toBindWith2Vertices)
            {
                hampath.Add(el.x_i.Value);
                hampath.Add(el.v_i);
            }

            foreach (var el in toBindWithHamilton)
            {
                hampath.AddRange(el.hamiltionianPath);
            }

            if (!prohibitedVertices.Contains(vertex))
                hampath.Add(vertex);


            return hampath;
        }

        /// <summary>
        /// i - obecny wierzcholek, v
        /// j - obecny kierunek, w którym idziemy
        /// prohibited - wierzcholek z którego wlasnie przyszlismy, wiec nie mozemy juz tam pójsc
        /// </summary>
        static MergeRet Merge(int i, int? j, int? prohibited)
        {
            // ten przypadek dzieje sie, gdy jestesmy na jakichs rozgalezieniach
            if (j == null)
            {
                // Kierunki, w które mozemy pójsc z aktualnego wierzcholka
                var dirs = spanningTree.OutEdges(i).Select(x => x.To);

                // Trzeba zabronic chodzenia w kierunki, gdzie juz bylismy (od których zaczynalismy).
                //Do tego sluzy argument "prohibited"
                List<MergeRet> toBind = new List<MergeRet>();

                foreach (var dir in dirs)
                {
                    if (!(prohibited.HasValue && dir == prohibited.Value))
                    {
                        toBind.Add(Merge(i, dir, null));
                    }
                }

                List<int> hampath = Bind(toBind, i);
                return new MergeRet { hamiltionianPath = hampath, v_i = i, x_i = hampath[1] };
            }
            else
            {
                // j nie jest nullem - jestesmy w czesci grafu, gdzie idziemy w jakims kierunku
                // sprawdzamy, czy ta część grafu jest ścieżką (e.g. i-j-k-l).
                // Jeśli tak, zwracany ścieżkę Hamiltona od i do j
                // Jeśli nie, idziemy rekurencujnie od i-tego wierzchołka (ten i-ty wierzchołek będzie naszym v).
                // in this case we will need to prevent returning to previously seen vertices

                var verticesList = new List<int>();

                int curr = j.Value;
                int prev = i;


                verticesList.Add(curr);

                int branchLength = 1;

                while (true)
                {
                    if (spanningTree.OutDegree(curr) == 1)
                        break;
                    else if (spanningTree.OutDegree(curr) > 2)
                    {
                        var forward = new List<int>();
                        var backward = new List<int>();

                        if (verticesList.Count() == 1)
                        {
                            if (!prohibitedVertices.Contains(i))
                            {
                                forward.Add(i);
                                prohibitedVertices.Add(i);
                            }
                        }
                        else
                        {
                            if (verticesList.Count % 2 == 0)
                            {
                                for (int k = 1; k < verticesList.Count - 2; k += 2)
                                {
                                    forward.Add(verticesList[k]);
                                }

                                forward.Add(verticesList[verticesList.Count - 2]);

                                for (int k = verticesList.Count - 4; k > -1; k -= 2)
                                {
                                    backward.Add(verticesList[k]);
                                }
                            }
                            else
                            {
                                for (int k = 1; k < verticesList.Count; k += 2)
                                {
                                    forward.Add(verticesList[k]);
                                }

                                for (int k = verticesList.Count - 3; k > -1; k -= 2)
                                {
                                    backward.Add(verticesList[k]);
                                }
                            }
                        }

                        // return forward U merge U back
                        var merge = Merge(curr, null, prev);
                        merge.hamiltionianPath = forward.Concat(merge.hamiltionianPath).Concat(backward).ToList();

                        return merge;
                    }

                    int tmp = spanningTree.OutEdges(curr).Where(x => x.To != prev).ToArray()[0].To;
                    prev = curr;
                    curr = tmp;

                    branchLength++;
                    verticesList.Add(curr);
                }

                // spanningTree.OutDegree(curr) == 1
                // laczymy v_i z x_i
                // Mozliwe przypadki:
                // 1. Jest jedynie v_i - w tym etapie nic nie laczymy, bo za wczesnie
                // 2. Jest v_i i x_i - w tym etapie nic nie laczymy, bo za wczesnie
                // 3. Oprócz v_i i x_i sa jeszcze inne wierzcholki - laczymy

                if (branchLength == 1)
                {
                    return new MergeRet { v_i = curr, x_i = null, hamiltionianPath = null };
                }
                else if (branchLength == 2)
                {
                    return new MergeRet { v_i = prev, x_i = curr, hamiltionianPath = null };
                }
                else
                {
                    MergeRet ret = new MergeRet();
                    List<int> hamCycle = new List<int>();

                    ret.v_i = verticesList[0];
                    ret.x_i = verticesList[1];

                    int numVert = verticesList.Count();

                    //forward
                    for (int k = 1; k < numVert; k += 2)
                    {
                        hamCycle.Add(verticesList[k]);
                    }

                    //backward
                    for (int k = ((numVert - 1) / 2) * 2; k > -1; k -= 2)
                    {
                        hamCycle.Add(verticesList[k]);
                    }

                    ret.hamiltionianPath = hamCycle;
                    return ret;
                }
            }
        }
    }
}