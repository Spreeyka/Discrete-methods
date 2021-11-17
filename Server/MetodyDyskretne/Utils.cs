using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MetodyDyskretne
{
    class Utils
    {
        static Element[,,] elements;
        static List<int> idList;
        static int[] idCounter;
        static int max;
        static int maxIndex;
        static int width;
        static int height;
        static int length;
        static string boundary;
        static string neighbourhoodType;
        static int numberOfNucleons;
        static int numberOfIteration;
        

        static List<Element> elementsToShuffle;
        static int randomIndex;
        static int[,,] indexes;
        static Element randomElement;
        static Random random;
        static int initialId;
        static int initialEnergy;
        static List<int> neighborsIds;
        static int randomIdIndex;
        static double randomProbability;
        static int testId;
        static int deltaEnergy;
        static double kt;
        static int numberOfMonteCarloIterations;

        public static string Filename { get; set; }
        public static string TimeOutputFilename { get; set; }
        public static Stopwatch Watch { get; set; }


        public static void InitializeDataFromFile()
        {

            var lines = File.ReadAllLines(@$"..\..\..\{Filename}").Skip(12).ToArray(); //tutaj dynamicznie
            width = Int32.Parse(lines[0]) + 2;
            height = Int32.Parse(lines[1]) + 2;
            length = Int32.Parse(lines[2]) + 2;
            boundary = lines[3];
            neighbourhoodType = lines[4];
            numberOfNucleons = Int32.Parse(lines[5]);
            numberOfIteration = Int32.Parse(lines[6]);          
            kt = Double.Parse(lines[7]);
            numberOfMonteCarloIterations = Int32.Parse(lines[8]);          
        }

        public static void SetInitials()
        {
            idList = new List<int>();
            elementsToShuffle = new List<Element>();
            
            for (int i = 0; i < 10; ++i) //cyfry od 0-9 jako początkowe ziarna
            {
                idList.Add(i);
            }
            idCounter = new int[idList.Count];
            for (int i = 0; i < idList.Count; ++i)
            {
                idCounter[i] = new int();
            }

            InitializeDataFromFile();
          
            random = new Random();
            neighborsIds = new List<int>();

            indexes = new int[width - 1, height - 1, length - 1];
            for (int j = 1; j < width - 1; j++)
            {
                for (int k = 1; k < height - 1; k++)
                {
                    for (int i = 1; i < length - 1; i++)
                    {
                        indexes[j, k, i] = new int();
                    }
                }
            }
        }

        public static void InitializeElements()
        {
            elements = new Element[width, height, length];

            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; j++)
                {
                    for (int k = 0; k < length; k++)
                    {
                        elements[i, j, k] = new Element(idList[0]);
                    }
                }               
            }
            for (int i = 1; i < width - 1; ++i)
            {
                for (int j = 1; j < height - 1; j++)
                {
                    for (int k = 1; k < length - 1; k++)
                    {
                        elements[i, j, k].neighbours = new List<Element>();
                    }
                }
            }
        }

        public static void SetInitialPattern()
        {
            int drawnColorIndex;
            Random random = new Random();
            for (int j = 1; j <= numberOfNucleons; j++)
            {
                drawnColorIndex = random.Next(1, idList.Count()); //random color
                elements[random.Next(1, width - 2), random.Next(1, height - 2), random.Next(1, length - 2)].id = idList.ElementAt(drawnColorIndex);
            }
            SetNeighbors();
        }
        
        public static void SetNeighbors()
        {
            if (neighbourhoodType == "Moore") NeighborhoodUtils.SetMooreNeighbours(width, height, length, elements);
            if (neighbourhoodType == "Neumann") NeighborhoodUtils.SetNeumannNeighbours(width, height, length, elements);
        }

        public static void SetIdForBorder()
        {
            //plaszczyzna dolna i gorna (bez krawędzi)
            for (int i = 1; i < width - 1; i++)
            {
                for (int j = 1; j < height - 1; j++)
                {
                    elements[i, j, 0].id = elements[i, j, length - 1].id;
                    elements[i, j, length - 1].id = elements[i, j, 0].id;
                }
            }

            //górna i dolna ściana
            for (int i = 1; i < height - 1; i++)
            {
                for (int j = 1; j < length - 1; j++)
                {
                    elements[0, i, j].id = elements[width - 2, i, j].id;
                    elements[width - 1, i, j].id = elements[1, i, j].id;
                }
            }
            //lewa i prawa ściana
            for (int i = 1; i < width - 1; i++)
            {
                for (int j = 1; j < length - 1; j++)
                {
                    elements[i, 0, j].id = elements[i, height - 2, j].id;
                    elements[i, height - 1, j].id = elements[i, 1, j].id;
                }
            }

            //wierzchołki         
            elements[0, 0, 0].id = elements[width - 2, height - 2, length - 2].id; //dolna pl lewa gora
            elements[0, height - 1 , 0].id = elements[width - 2, 1, length - 2].id; // dolna pl prawy gora ???? było height - 2 w pierwszej czesci rownania
            elements[width - 1, height - 1, 0].id = elements[1, 1, 1].id; //dolna pl prawy dol
            elements[width - 1, 0, 0].id = elements[1, height - 2, length - 2].id; //dolna pl lewy dol

            elements[0, 0, length - 1].id = elements[width - 2, height - 2, length - 2].id; //gorna pl lewa gora
            elements[0, height - 1 , length - 1].id = elements[width - 2, 1, length - 2].id; // gorna pl prawy gora ???? było height - 2 w pierwszej czesci rownania
            elements[width - 1, height - 1, length - 1].id = elements[1, 1, 1].id; //gorna pl prawy dol
            elements[width - 1, 0, length - 1].id = elements[1, height - 2, length - 2].id; //gorna pl lewy dol
            
        }

        public static void Grow()
        {
            for (int i = 0; i < numberOfIteration; i++)
            {
                {
                    if (boundary == "Periodic") SetIdForBorder();
                    IterateAndMarkForIdChange();
                }
                ChangeIdOfMarked();
            }
        }

        private static void IterateAndMarkForIdChange()
        {
            int winnerColor;
            for (int j = 1; j < width - 1; j++)
            {
                for (int k = 1; k < height - 1; k++)
                {
                    for (int i = 1; i < length - 1; i++)
                    {
                        if (elements[j, k, i].id == idList[0]) //lecimy po wszystkich białych (id = 0)
                        {
                            if (elements[j, k, i].neighbours.Any(m => m.id != idList[0])) //jesli element o id = 0 (bialy) ma jakiegokolwiek sąsiada o innym id (inny kolor)
                            {
                                elements[j, k, i].IsColored = true;
                                foreach (var neigh in elements[j, k, i].neighbours.Where(b => b.id != idList[0]))
                                {
                                    ++idCounter[idList.FindIndex(z => z == neigh.id)];                          //podliczanie sąsiadów
                                }
                                max = idCounter.Max();                                                          //który sąsiad wygrywa
                                maxIndex = Array.IndexOf(idCounter, max);
                                winnerColor = idList.ElementAt(maxIndex);
                                elements[j, k, i].idChanged = winnerColor;                                      //oznaczenie zmiany id
                                Array.Clear(idCounter, 0, idCounter.Count());
                            }
                        }
                    }
                }
            }
        }

        private static void ChangeIdOfMarked() 
        {
            //second phase -> malowanie oznaczonego (zmiana id)
            for (int j = 1; j < width - 1; j++)
            {
                for (int k = 1; k < height - 1; k++)
                {
                    for (int i = 1; i < length - 1; i++)
                    {
                        if (elements[j, k, i].IsColored == true)
                        {
                            elements[j, k, i].id = elements[j, k, i].idChanged;                            
                            elements[j, k, i].IsColored = false;                            
                        }
                    }
                }
            }
        }
     
        private static void IterateMonteCarlo()
        {
            for (int i = 0; i < numberOfMonteCarloIterations; i++)
            {
                MonteCarlo();
            }
        }

        private static void MonteCarlo()
        {
            AddElementsToShuffle(); //1. dodajemy elementy do listy
            for (int i = 0; i < elementsToShuffle.Count; i++)
            {
                DrawElement(); //2. losowanie

                if (randomElement.Energy != 0) //jak każdy sąsiad ma taki sam id (kolor), to nie ma sensu sprawdzać
                {
                    AddAllDifferentIdsOfRandomElementNeighbors();
                    DrawNewIdAndCheckEnergy();
                    CheckIfAcceptNewElement();
                    neighborsIds.Clear();
                }
                randomElement.Energy = 0;
                ListExt.RemoveBySwap(elementsToShuffle,randomIndex); //3. Usuwanie tego elementu z listy, żeby już go nie losowało
                //elementsToShuffle.Remove(randomElement); 
            }
        }
        

        private static void AddElementsToShuffle()
        {
            for (int j = 1; j < width - 1; j++)
            {
                for (int k = 1; k < height - 1; k++)
                {
                    for (int i = 1; i < length - 1; i++)
                    {
                        elementsToShuffle.Add(elements[j, k, i]); //dodajemy wszystkie elementy
                    }
                }
            }
        }

        private static void DrawElement()
        {
            randomIndex = random.Next(0, elementsToShuffle.Count);
            randomElement = elementsToShuffle.ElementAt(randomIndex);
            randomElement.SetEnergy();
        }

        private static void AddAllDifferentIdsOfRandomElementNeighbors()
        {
            initialId = randomElement.id;
            initialEnergy = randomElement.Energy;

            foreach (var diffColorElement in randomElement.neighbours.Where(c => c.id != 0)) //dodajemy wszystkie otaczające kolory (id)
            {
                if (neighborsIds.Exists(n => n == diffColorElement.id) == false)
                {
                    neighborsIds.Add(diffColorElement.id);
                }
            }
        }

        private static void DrawNewIdAndCheckEnergy()
        {
            randomIdIndex = random.Next(0, neighborsIds.Count); //losowanie koloru spośród sąsiadów i sprawdzenie energii
            testId = neighborsIds.ElementAt(randomIdIndex);
            randomElement.id = testId;
            randomElement.Energy = 0;
            randomElement.SetEnergy();
            deltaEnergy = randomElement.Energy - initialEnergy;
        }

        private static void CheckIfAcceptNewElement()
        {
            if (deltaEnergy > 0) //jeśli energia jest większa to korzystamy ze wzoru
            {
                randomProbability = random.NextDouble();
                if (randomProbability > Math.Exp(-(deltaEnergy / kt)))
                {
                    randomElement.id = initialId;
                    randomElement.Energy = initialEnergy;
                }
            }
        }

        public static void Show()
        {
            for (int k = 1; k < length - 1; k++)
            {
                for (int i = 1; i < width - 1; i++)
                {
                    for (int j = 1; j < height - 1; j++)
                    {                       
                        Console.Write(elements[i, j, k].id + " ");            
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
            
        }

        public static void Run()
         {
            SetInitials();
            InitializeElements();
            SetInitialPattern();
            //Console.WriteLine("Przed");
            //Show();

            Grow();

            //Console.WriteLine();
            //Console.WriteLine("Po");

            //Show();
            //Console.WriteLine("Monte carlo");

            IterateMonteCarlo();

            
        }

        public static void RunAndSaveOutput()
        {
            ClearTimeOutputFile();

            for (int i = 1; i <= CountParameterFiles(); i++)
            {
                InitializeWatch(i);


                SetInitials();
                InitializeElements();
                SetInitialPattern();

                Grow();

                IterateMonteCarlo();
                WriteOutputToFile(i);

                               
                SaveTimeElapsedInFile($"Execution Time: {Watch.ElapsedMilliseconds} ms \n");
            }
        }

        private static void InitializeWatch(int index)
        {
            Watch = new Stopwatch();
            Filename = $"parameters{index}.txt";
            Watch.Start();
        }

        private static void WriteOutputToFile(int fileIndex)
        {
            var outputFilename = $@"C:\Users\Shprei\Desktop\MetodyDyskretne-master\MetodyDyskretne\output{fileIndex}.txt"; //tutaj dynamicznie
            File.WriteAllText(outputFilename, String.Empty);

            using StreamWriter outputFile = new StreamWriter(outputFilename);
            for (int k = 1; k < length - 1; k++)
            {
                for (int i = 1; i < width - 1; i++)
                {
                    for (int j = 1; j < height - 1; j++)
                    {
                        outputFile.Write(elements[i, j, k].id + " ");
                    }
                    outputFile.WriteLine();
                }
                outputFile.WriteLine();
            }

            //var standardOutput = Console.Out;
            //var writer = new StreamWriter(file);
            //Console.SetOut(writer);
            //Show();
            //Console.SetOut(standardOutput);
            //return writer;
        }

        private static void ClearTimeOutputFile()
        {
            TimeOutputFilename = @"C:\Users\Shprei\Desktop\MetodyDyskretne-master\MetodyDyskretne\time.txt";
            File.WriteAllText(TimeOutputFilename, String.Empty);
        }

        public static int CountParameterFiles()
        {
            return Directory.GetFiles(@"C:\Users\Shprei\Desktop\MetodyDyskretne-master\MetodyDyskretne", "parameters*.txt", SearchOption.TopDirectoryOnly).Length;
        }
        
        public static void SaveTimeElapsedInFile(string timeString)
        {
            Watch.Stop();
            var fileName = @"C:\Users\Shprei\Desktop\MetodyDyskretne-master\MetodyDyskretne\time.txt";
            File.AppendAllText(fileName, timeString);
            Console.WriteLine($"Execution Time: {Watch.ElapsedMilliseconds} ms");
            //var file = new FileStream(fileName, FileMode.OpenOrCreate);
        }
    }
}
