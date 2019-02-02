using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ASD.Graphs
{
    class Program
    {
        enum WHAT_TO_DO
        {
            test_camerini,                      // Odpala testy dla algorytmu Cameriniego
            construct_hamiltonian_path,         // Konstrukcja ścieżki Hamiltona wedle założeń dla grafu, będącego drzewem
            construct_MBST_and_hamiltonian_path // Konstrukcja ścieżki Hamiltona wedle założeń dla grafu
        }

        public static void Main()
        {
            WHAT_TO_DO todo = WHAT_TO_DO.construct_MBST_and_hamiltonian_path;

            if (todo == WHAT_TO_DO.test_camerini)
            {
                SpanningTreeCamerini.testCamerini();
            }
            else if (todo == WHAT_TO_DO.construct_MBST_and_hamiltonian_path)
            {
                    DirectoryInfo dinfo = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\GRAPHS");
                    FileInfo[] Files = dinfo.GetFiles("*.txt");

                    //wczytanie grafów z plików
                    foreach (FileInfo file in Files)
                    {
                        var gs = new GraphSerializer("\\GRAPHS\\" + file.Name);
                        var graph = gs.graphFromFile();
                        
                        // Zanlezienie i zapisanie drzewa rozpinającego
                        gs = new GraphSerializer("\\GRAPHS\\MBST\\mbst_for_" + file.Name);
                        var spanning = SpanningTreeKruskal.Kruskal(graph);
                        gs.graphToFile(spanning);

                        Console.WriteLine($"\nGRAPH: {file.Name}");
                        (var weigth, var dummy) = BranchAndBoundTSPGraphExtender.BranchAndBoundTSP(graph);
                        Console.WriteLine($"Dokładne rozwiązanie problemu komiwojażera dla tego grafu: {weigth}");

                        var hampath = BottleneckTravellingSalesman.ConstructHamiltonianPath(spanning);

                        // Wypisanie ścieżki Hamiltona
                        Console.WriteLine("Utworzona ścieżka Hamiltona: ");
                        Console.WriteLine(string.Join(" ", hampath));

                        // Zapis ścieżki Hamiltona do pliku
                        using (StreamWriter sw = new StreamWriter(Directory.GetCurrentDirectory() + "\\GRAPHS\\PATHS\\path_for_" + Path.GetFileNameWithoutExtension(file.Name) + ".txt"))
                        {
                            sw.WriteLine(string.Join(" ", hampath));
                        }

                        Console.WriteLine($"Przybliżone rozwiązanie za pomocą algorytmu komiwojażera - krótkodystansowca: {HamiltonPathChecker.costOfHamiltonianPath(graph, hampath)}");
                    }
                    // czyszczenie z niepotrzebnych plików, wygenerowanych przez eksporter
                    Directory.EnumerateFiles(Directory.GetCurrentDirectory() + "\\GRAPHS\\MBST", "*.dot").ToList().ForEach(x => File.Delete(x));
            }
            else
            {
                DirectoryInfo dinfo = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\TREES");
                FileInfo[] Files = dinfo.GetFiles("*.txt");

                //wczytanie grafów z plików
                foreach (FileInfo file in Files)
                {
                    // Utworzenie grafu z pliku
                    var gs = new GraphSerializer("\\TREES\\" + file.Name);
                    var graph = gs.graphFromFile();

                    // Konstrukcja ścieżki Hamiltona
                    var hampath = BottleneckTravellingSalesman.ConstructHamiltonianPath(graph);

                    // Wypisanie nazwy grafu
                    Console.WriteLine($"\nGRAPH: {file.Name}");

                    // Wypisanie ścieżki Hamiltona
                    Console.WriteLine("Utworzona ścieżka Hamiltona: ");
                    Console.WriteLine(string.Join(" ", hampath));

                    if (HamiltonPathChecker.isCorrectHamiltonianCycle(graph, hampath))
                        Console.WriteLine("Ścieżka jest poprawna");
                    else
                        Console.WriteLine("Ścieżka nie jest poprawna");

                    // Zapis ścieżki Hamiltona do pliku
                    using (StreamWriter sw = new StreamWriter(Directory.GetCurrentDirectory() + "\\TREES\\PATHS\\path_for_" + Path.GetFileNameWithoutExtension(file.Name) + ".txt"))
                    {
                        sw.WriteLine(string.Join(" ", hampath));
                    }
                }
                // czyszczenie z niepotrzebnych plików, wygenerowanych przez eksporter
                Directory.EnumerateFiles(Directory.GetCurrentDirectory() + "\\GRAPHS\\MBST", "*.dot").ToList().ForEach(x => File.Delete(x));
            }

            Console.ReadKey();
        }
    }
}
