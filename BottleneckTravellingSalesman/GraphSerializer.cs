using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASD.Graphs;
using System.IO;

namespace ASD.Graphs
{
    class GraphSerializer
    {
        string path;

        public GraphSerializer(string path)
        {
            this.path = Directory.GetCurrentDirectory() + path;
        }

        public Graph graphFromFile()
        {
            using (StreamReader sr = File.OpenText(path))
            {
                int numberOfVertices = int.Parse(sr.ReadLine());

                var ret = new AdjacencyMatrixGraph(false, numberOfVertices);

                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    var numbers = line.Split(' ');
                    ret.AddEdge(int.Parse(numbers[0]), int.Parse(numbers[1]), int.Parse(numbers[2]));
                }

                return ret;
            }
        }

        public void graphToFile(Graph g)
        {
            var edges = new List<Edge>();

            for (int i = 0; i < g.VerticesCount; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (!double.IsNaN(g.GetEdgeWeight(i, j)))
                    {
                        edges.Add(new Edge(i,j,g.GetEdgeWeight(i,j)));
                    }
                }
            }

            using (StreamWriter sr = new StreamWriter(path))
            {
                sr.WriteLine(g.VerticesCount);

                foreach(var edge in edges)
                {
                    sr.WriteLine($"{edge.From} {edge.To} {edge.Weight}");
                }
            }
        }
    }
}
