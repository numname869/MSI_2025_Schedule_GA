using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using msi_GA.Task_handling;
using System.IO;
using System.Text.Json;
using msi_GA.GA;
using System.Collections;
using System.Security;
using System.Numerics;

namespace msi_GA.GA
{

    public class SerializedGen
    {
        public double Fitness { get; set; }
        public int[][] Schedule { get; set; }
    }

    public class GenerationLog
    {
        public int ID { get; set; }
        public int MaxFitness { get; set; }
        public List<SerializedGen> Individuals { get; set; }
    }


    internal class Generation : Gen
    {

        private List<Gen> _generations = new List<Gen>();
        private object randLock = new object();

        private int GenerationLength = 24;
        public int ParentsCount = 2;
        public int ElitistRate = 0;
        public int BestParentsKept = 0;
        public double MutationRate = 0.1;
        public int CrossoverOption = 1;


        public int MaxFitness = 0;
        public int MaxHoursScore = 0;
        public int MaxShiftBreakScore = 0;
        public double MaxFitnessPerWorker = 0;
        public int MaxWorkerDispersionScore = 0;
        public int MaxShiftsScore = 0;
        public int MaxEachWorkerTypePerShift = 0;


        public double LowCellingFintessPerWorker = 0;
        public double MiddleCellingFintessPerWorker = 0;
        public int FasterMutationMultiplyPoint = 2;

        int ID = 0
            ;

       
        public bool EtylistRateEnabled = false;
        public bool ChangingMutationEbaled = false;
        public bool TargetedMutationEnabled = false;



        public int EtylistChangeInterval = 50;
        public int MutationChangeInterval = 10;

        public void WyswietlOpcje()
        {
            Console.WriteLine($"EtylistRate : {ElitistRate}");
            Console.WriteLine($"MutationRate : {MutationRate}");
            Console.WriteLine($"CrossoverOption : {MutationRate}");
            Console.WriteLine($"BestParentsKept : {BestParentsKept}");
            Console.WriteLine($"CrossoverOption : {CrossoverOption}");
            Console.WriteLine($"EtylistChangeInterval : {EtylistChangeInterval}");
            Console.WriteLine($"MutationChangeInterval : {MutationChangeInterval}");
            Console.WriteLine($"FasterMutationMultiplyPoint : {FasterMutationMultiplyPoint}");

            Console.WriteLine($"EtylistRateEnabled : {EtylistRateEnabled}");
            Console.WriteLine($"MutationRateEnabled : {ChangingMutationEbaled}");
            Console.WriteLine($"TargetetMutationEnabled: {TargetedMutationEnabled}");


        }

        private ThreadLocal<Random> threadLocalRandom = new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));

     



        Random random = new Random();

        public Generation(bool IsConstantAware)
        {

            GenerateFirstGeneration(IsConstantAware);
        }



        public void GenerateFirstGeneration(bool IsConstantAware = false)
        {
            for (int i = 0; i < GenerationLength; i++)
            {
                _generations.Add(new Gen(IsConstantAware));
            }
            ElitistRate = GenerationLength;

            MaxFitness = CalculateMaxFitness(_generations[0]._schedule);
            MaxFitnessPerWorker = WorkerStats(_generations[0]._schedule, 0);
            MaxShiftBreakScore = CalulateMaxShiftBreakScore(_generations[0]._schedule);
            MaxHoursScore = CalculateMaxHoursScore(_generations[0]._schedule);
            MaxWorkerDispersionScore = CalculateMaxWorkerDispersionScore(_generations[0]._schedule);
            MaxShiftsScore = CalculateMaxShiftsScore(_generations[0]._schedule);
            MaxEachWorkerTypePerShift = CalculateMaxEachWorkerTypePerShifts(_generations[0]._schedule);



            LowCellingFintessPerWorker = MaxFitnessPerWorker/3;
            MiddleCellingFintessPerWorker = MaxFitness / 2;
           
            SortGenerations();
            SaveGenerationsToJson("generacje.json");
            ID++;

        }

    


       public void AdaptiveNextGenerations(int amount)
        {

            for(int i = 0 ; i < amount; i++)
            {

                GenerateNextGeneration(CrossoverOption);


                
                if (ID % MutationChangeInterval == 0 && ChangingMutationEbaled)
                {
                  
                    MutationRate = MutationRate + 0.01;
                  

                }
                if(ID % EtylistChangeInterval == 0 && ElitistRate > 4 && EtylistRateEnabled)
                {
                    ElitistRate = ElitistRate - 1;

                }
              




            }




        }

    

        public List<Gen> ChooseCrossover(int option)
        {
            if (option == 1)
            {
                ParentsCount = 2;
                return MaskCrossoverTwoParents();

            }
            else if (option == 2)
            {
                ParentsCount = 3;
                return MaskCrossoverThreeParents();
            }
            else if (option == 3)
            {
                ParentsCount = random.Next(2, ElitistRate);
                return MaskCrossoverRandomParentsAmount();
            }
            else if (option == 4)
            {
               int parent1 = random.Next(0, ElitistRate);
                int parent2 = random.Next(0, ElitistRate);
                return PMXCrossover(_generations[parent1], _generations[parent2]);
            }
            else return null;


          
        }




        public void GenerateNextGeneration(int option)
        {
            List<Gen> newGenerations = new List<Gen>();
           
           
            CrossoverOption = option;
            int ChildAmount = 0;

           

            for (int i = BestParentsKept; i < GenerationLength; i = i + ParentsCount) 
            {
              

               var children = ChooseCrossover(CrossoverOption);
     

                foreach (var child in children)
                {
                    if (ChildAmount <= GenerationLength)
                    {
                        newGenerations.Add(child);
                        ChildAmount++;
                       
                    }
                    else break;
                }

            }


            if (TargetedMutationEnabled)
            //TargetedMutation(newGenerations, MutationRate);
            {
                foreach (var gen in newGenerations)
                {
                    SmartMutation(gen);
                }

            }
            else MutationForEach(newGenerations, MutationRate);




            CopyGeneration(newGenerations);
            
            SortGenerations();
            SaveGenerationsToJson("generacje.json");
            Console.WriteLine(ID);
            ID++;
            

        }

        public void CopyGeneration(List<Gen> NewGeneration)
        {
           
            int j = 0;
           
            for (int i = BestParentsKept; i < GenerationLength; i++)
            {
                
                _generations[i] = NewGeneration[j];
                _generations[i].Fitness = CalculateFitness(_generations[i]._schedule);
                _generations[i].HoursScore = HoursMeet(_generations[i]._schedule);
                _generations[i].ShiftBreakScore = ShiftBreak(_generations[i]._schedule);
                _generations[i].WorkerDispersionScore = WorkerDispersionGeneral(_generations[i]._schedule);
                _generations[i].MaxShiftScore = MaxShifts(_generations[i]._schedule);
                _generations[i].TypeOfWorkerPerShift = EachWorkerTypePerShifts(_generations[i]._schedule);

                j++;
            }
        }



        public  void SortGenerations()
        {
            _generations = _generations.OrderByDescending(x => x.Fitness).ToList();
        }

        public void PrintGenerations()
        {
            Console.WriteLine($"max fitness: {MaxFitness} , max HoursScore : {MaxHoursScore}, max ShiftBreakScore : {MaxShiftBreakScore}, max Worker Dispersion Score: {MaxWorkerDispersionScore}, Max shifs Score : {MaxShiftsScore} , Max eachworkerTypeperShift Score {MaxEachWorkerTypePerShift}");



            foreach (var gen in _generations)
            {
                for (int i = 0; i < gen.Schedule.Length; i++)
                {
                    if (i < 10) { Console.Write($"Worker  {i}: "); }
                    else
                        Console.Write($"Worker {i}: ");

                    for (int j = 0; j < gen.Schedule[i].Length; j++)
                    {
                        Console.Write($"{gen.Schedule[i][j]} ");
                        if ((j + 1) % 3 == 0) Console.Write($"|| ");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine($"Fitness: {gen.Fitness} , HoursScore : {gen.HoursScore} , ShiftsBreakScore : {gen.ShiftBreakScore}, WorkerDispersion : {gen.WorkerDispersionScore}, Shift Score : {gen._maxShiftsScore}, eachworkerTypeperShift Score : {gen.TypeOfWorkerPerShift} ");
            }
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (var gen in _generations)
            {
                Console.WriteLine($"Fitness: {gen.Fitness}");


            }
            Console.ResetColor(); 

            Console.WriteLine($"Etylist Rate: {ElitistRate}");
            Console.WriteLine($"Mutation Rate: {MutationRate}");
            Console.WriteLine($"Parents : {ParentsCount}");

        }

        public List<Gen> MaskCrossoverTwoParents()
        {
            int parent1 = random.Next(0, ElitistRate);
            int parent2 = random.Next(0, ElitistRate);
            Gen child1 = new Gen();
            Gen child2 = new Gen();
            List<Gen> children = new List<Gen>();

            Parallel.For(0, Workers, i =>
            {
                for (int j = 0; j < ShiftsAmount; j++)
                {
                    bool mask = GenerateMaskOnTheFly1(); 

                    if (mask)
                    {
                        child1.Schedule[i][j] = _generations[parent1].Schedule[i][j];
                        child2.Schedule[i][j] = _generations[parent2].Schedule[i][j];
                    }
                    else
                    {
                        child1.Schedule[i][j] = _generations[parent2].Schedule[i][j];
                        child2.Schedule[i][j] = _generations[parent1].Schedule[i][j];
                    }
                }
            });



            child1.Fitness = CalculateFitness(child1.Schedule);
            child2.Fitness = CalculateFitness(child2.Schedule);

            children.Add(child1);
            children.Add(child2);
            return children;
        }

        public List<Gen> MaskCrossoverThreeParents()
        {
            int parent1 = random.Next(0, ElitistRate);
            int parent2 = random.Next(0, ElitistRate);
            int parent3 = random.Next(0, ElitistRate);
            Gen child1 = new Gen();
            Gen child2 = new Gen();
            Gen child3 = new Gen();
            List<Gen> children = new List<Gen>();


            Parallel.For(0, Workers, i =>
            {
                for (int j = 0; j < ShiftsAmount; j++)
                {
                    int mask = GenerateMaskOnTheFly2();

                    if (mask == 0)
                    {
                        child1.Schedule[i][j] = _generations[parent1].Schedule[i][j];
                        child2.Schedule[i][j] = _generations[parent2].Schedule[i][j];
                        child1.Schedule[i][j] = _generations[parent3].Schedule[i][j];
                    }
                    else if ( mask == 1)
                    {
                        child1.Schedule[i][j] = _generations[parent2].Schedule[i][j];
                        child2.Schedule[i][j] = _generations[parent3].Schedule[i][j];
                        child3.Schedule[i][j] = _generations[parent1].Schedule[i][j];
                    }
                    else if (mask == 2)
                    {
                        child1.Schedule[i][j] = _generations[parent3].Schedule[i][j];
                        child2.Schedule[i][j] = _generations[parent1].Schedule[i][j];
                        child3.Schedule[i][j] = _generations[parent2].Schedule[i][j];
                    }
                }
            });

            children.Add(child1);
            children.Add(child2);
            children.Add(child3);
            return children;
        }


        public List<Gen> MaskCrossoverRandomParentsAmount()
        {
            List<int> parents = new List<int>();
            

            for(int i = 0; i < ParentsCount; i++)
            {
                int parent = random.Next(0, ElitistRate);
                parents.Add(parent);


            }



            List<Gen> children = new List<Gen>();
            for (int i = 0; i < ParentsCount; i++)
            {
                children.Add(new Gen());
            }


            
            Parallel.For(0, Workers, workerIndex =>
            {
                for (int shiftIndex = 0; shiftIndex < ShiftsAmount; shiftIndex++)
                {
                    int mask = GenerateMaskOnTheFly(ParentsCount);

                   
                    for (int childIndex = 0; childIndex < ParentsCount; childIndex++)
                    {
                        
                        int parentIndex = (childIndex + mask) % ParentsCount;
                        children[childIndex].Schedule[workerIndex][shiftIndex] =
                            _generations[parents[parentIndex]].Schedule[workerIndex][shiftIndex];
                    }
                }
            });

            return children;




        }



        public int GenerateMaskOnTheFly(int parentsAmount)
        {
            return random.Next(0, parentsAmount);

        }

        public int GenerateMaskOnTheFly2()
        {
            lock (randLock)
            {
                return random.Next(0, 2);
            }
        }

        public void Mutation(List<Gen> generation, double MutationRate)
        {

            foreach (Gen gen in generation)
            {

                for (int i = 0; i < gen.Schedule.Length; i++)
                {
                    for (int j = 0; j < gen.Schedule[i].Length; j++)
                    {
                        if (random.NextDouble() < MutationRate) 
                        {
                            gen.Schedule[i][j] = threadLocalRandom.Value.Next(0, 2);


                        }
                    }
                }
            }

            

        }


        public void MutationForEach(List<Gen> generation, double MutationRate)
        {

            foreach (Gen gen in generation)
            {

                foreach( var worker in gen.Schedule)
                {
                   
                    
                        for (int i = 0; i < worker.Length; i++)
                        {
                            
                            if (random.NextDouble() < MutationRate)
                            {
                                worker[i] = threadLocalRandom.Value.Next(0, 2);
                        }
                        }
                    
                }
            }



        }



        public int FindTheBestWorker(int[][] schedule)
        {
            int bestWorker = 0;
            for (int i = 1; i < schedule.Length; i++)
            {
                if (WorkerStats(schedule, i) < WorkerStats(schedule, bestWorker))
                {
                    bestWorker = i;
                }
            }
            return bestWorker;
        }


        public int FindTheWorstWorker(int[][] schedule)
        {
            int worstWorker = 0;
           
            for(int i = 1; i < schedule.Length; i++)
            {
                if (WorkerStats(schedule, i) > WorkerStats(schedule, worstWorker))
                {
                    worstWorker = i;
                }
            }


            return worstWorker;
        }


  


    
        public void MutateWorker(int[][] schedule, int WorstWorker , double NewMutationRate)
        {
            for(int j = 0; j < schedule[WorstWorker].Length; j++)
            {
                if (random.NextDouble() < NewMutationRate)
                {
                    schedule[WorstWorker][j] = random.Next(0, 2);
                }
            }
        }



        // we need to find not just emptu shift but also an illegal one
        // shift mutation must be more complex



        public int CheckDay(int[][] schedule, int Worker, int day)
        {

            int sum = 0;
            for (int i = day; i < day + 3; i++)
            {
                if (schedule[Worker][i] == 1)
                    sum++;
            }
           
            return sum;
            

        }


        public void MutateAllWorkersShitfs()
        {


            for (int i = 0; i < Workers; i++)
            {
                MutateIllegalShifts(_generations[i].Schedule, i);
            }

        }
     public void MutateIllegalShifts(int[][] schedule, int Worker)
        {

            for (int j = 0; j < ShiftsAmount; j = j +3)
            {
                int sum = CheckDay(schedule, Worker, j);
                if ( sum> 1)
                {
                    for(int k = j; k < j + 3; k++)
                    {
                        if (random.NextDouble() < MutationRate + sum/2)
                        {
                            schedule[Worker][k] = random.Next(0,2);
                            
                        }
                    }
                }

            }


        }

        public void GiveTheWorstWorkerStatsFromBestWorker(int[][] schedule, int WorstWorker, int BestWorker, double MutationRate)
        {

            for(int i = 0; i < ShiftsAmount ; i++)
            {
                if (random.NextDouble() < MutationRate)
                {
                    schedule[WorstWorker][i] = schedule[BestWorker][i];
                }
            }

        }


        public void MutatePoorStatsGlobally(int[][] schedule)
        {
            for (int i = 0; i < Workers; i++)
            {
                if (WorkerStats(schedule, i) < LowCellingFintessPerWorker)
                {
                    MutateWorker(schedule, i, MutationRate * FasterMutationMultiplyPoint);
                }
                else if (WorkerStats(schedule, i) > MiddleCellingFintessPerWorker)
                {
                    MutateWorker(schedule, i, MutationRate);
                }
            } 
        }
        public void TargetedMutation(List<Gen> generation, double MutationRate)
        {
            
            foreach (Gen gen in generation)
            {
               
                int WorstWorker = FindTheWorstWorker(gen.Schedule);
                int BestWorker = FindTheBestWorker(gen.Schedule);

                //albo go zmutuj randomowo albo daj mu troche z najlepszego pracownika
                //MutateWorker(gen.Schedule, WorstWorker , MutationRate * FasterMutationMultiplyPoint);
                GiveTheWorstWorkerStatsFromBestWorker(gen.Schedule, WorstWorker, BestWorker, MutationRate * FasterMutationMultiplyPoint);

                // lepiej mutować tylko tych którzy mają najgorsze statystyki
                //MutateAllWorkersShitfs();
               // MutateIllegalShifts(gen.Schedule, WorstWorker);


            }


        }



        public bool GenerateMaskOnTheFly1()
        {
            lock (randLock)
            {
                return random.Next(0, 2) == 1;
            }
        }

        private List<Gen> SelectParents(int tournamentSize)
        {
            return _generations
                .OrderBy(x => random.Next())
                .Take(tournamentSize)
                .OrderByDescending(x => x.Fitness)
                .Take(2)
                .ToList();
        }
        public List<Gen> PMXCrossover(Gen parent1, Gen parent2)
        {
            var child1 = new Gen();
            var child2 = new Gen();

            
            int point1 = random.Next(0, ShiftsAmount - 1);
            int point2 = random.Next(point1 + 1, ShiftsAmount);

           
            for (int i = point1; i <= point2; i++)
            {
                for (int w = 0; w < Workers; w++)
                {
                    child1.Schedule[w][i] = parent2.Schedule[w][i];
                    child2.Schedule[w][i] = parent1.Schedule[w][i];
                }
            }


            return new List<Gen> { child1, child2 };
        }

        public void SmartMutation(Gen individual)
        {
            // Mutacja tylko niepoprawnych fragmentów
            for (int w = 0; w < Workers; w++)
            {
                for (int d = 0; d < Days; d++)
                {
                    int shiftStart = d * 3;
                    var dayShifts = individual.Schedule[w].Skip(shiftStart).Take(3);

                    if (dayShifts.Count(s => s == 1) > 1) // Naruszenie zasad
                    {
                        if (random.NextDouble() < MutationRate)
                        {
                            // Intelligent repair
                            individual.Schedule[w][shiftStart + random.Next(3)] = 0;
                        }
                    }
                }
            }
        }

        public void SaveGenerationsToJson(string filePath)
        {
            Dictionary<string, List<object>> generationsDict;

            // Jeśli plik istnieje – wczytaj dane
            if (File.Exists(filePath))
            {
                string existingJson = File.ReadAllText(filePath);

                generationsDict = JsonSerializer.Deserialize<Dictionary<string, List<object>>>(existingJson)
                                  ?? new Dictionary<string, List<object>>();
            }
            else
            {
                generationsDict = new Dictionary<string, List<object>>();
            }

            // Tworzenie danych nowej generacji
            var generationData = new List<object>();
            for (int i = 0; i < GenerationLength; i++)
            {
                var genData = new
                {
                    Fitness = _generations[i].Fitness,
                    HoursScore = _generations[i].HoursScore,
                    ShiftBreakScore = _generations[i].ShiftBreakScore,
                    WorkerDispersionScore = _generations[i].WorkerDispersionScore,
                    ShiftsScore = _generations[i].MaxShiftScore,
                    TypeOfWorkerPerShift = _generations[i].TypeOfWorkerPerShift,
                };
                generationData.Add(genData);
            }

            // Dodajemy nową generację (np. "Generation5")
            generationsDict[$"Generation{ID}"] = generationData;

            // Serializacja i zapis do pliku
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            string jsonString = JsonSerializer.Serialize(generationsDict, options);
            File.WriteAllText(filePath, jsonString);
        }



        public void SaveGenerationsToJson_0(string filePath)
        {
            var generationsDict = new Dictionary<string, object>();

            // Sprawdzenie, czy plik już istnieje
            if (File.Exists(filePath))
            {
                // Odczytanie istniejących danych
                string existingJson = File.ReadAllText(filePath);
                generationsDict = JsonSerializer.Deserialize<Dictionary<string, object>>(existingJson);
            }

            // Dodanie MaxFitness do słownika (będzie to widoczne w każdej generacji)
            generationsDict["MaxFitness"] = MaxFitness;

            // Tworzymy dane dla tej generacji, z identyfikatorem ID
            var generationData = new List<object>();

            // Dla każdej generacji tworzysz nowy wpis w słowniku
            for (int i = 0; i < GenerationLength; i++)
            {
                var genData = new
                {
                    Fitness = _generations[i].Fitness,
                    Schedule = _generations[i].Schedule // Zakładając, że to jest dwuwymiarowa tablica
                };
                generationData.Add(genData);
            }

            // Dodanie danych generacji z ID jako klucz
            generationsDict[$"Generation{ID}"] = generationData;

            // Serializowanie do JSON
            var options = new JsonSerializerOptions
            {
                WriteIndented = true // Formatowanie JSON
            };

            string jsonString = JsonSerializer.Serialize(generationsDict, options);

            // Zapisanie danych zaktualizowanych do pliku JSON
            File.WriteAllText(filePath, jsonString);

            // Console.WriteLine($"Zapisano generacje do pliku: {filePath}");
        }


      











    }


}

