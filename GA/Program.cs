using System.ComponentModel.Design;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using msi_GA.GA;
using msi_GA.Task_handling;

namespace msi_GA.GA
{

    
    internal class Program
    {




        public static void DostosujOpcje(Generation generation)
        {
            int x = 0;
            double v = 0.0;
            bool exit = false;

            Console.WriteLine("1.Włącz/Wyłącz Etylist Rate");
            Console.WriteLine("2.Ustaw Etylist Rate");
            Console.WriteLine("3.Włącz/Wyłącz Zwiększającą się mutację");
            Console.WriteLine("4.Ustaw wartość mutacji");
            Console.WriteLine("5.Ustaw przyrost mutacji co iteracje");
            Console.WriteLine("4.Ustaw wartość mutacji");
            Console.WriteLine("6.Ustaw zmiejszane etylistrate co iteracje");
            Console.WriteLine("7.Ustaw Rodzaj rodzaj Crossoveru");
            Console.WriteLine("8.Ilość rodziców zostawiona");
            Console.WriteLine("9.Włącz/Wyłącz Targeted mutation");
            Console.WriteLine("10.Włącz/Wyłącz Targeted mutation for better hours");
            Console.WriteLine("11. ignore mutation rate");
            Console.WriteLine("12.TryPreferences toggle");
            Console.WriteLine("NN");

            Console.WriteLine("13.Wyjdź z opcji");


            string input1 = Console.ReadLine();

            switch (input1)
            {

                case "1":
                    generation.EtylistRateEnabled = !generation.EtylistRateEnabled;

                    break;

                case "2":
                    x = int.Parse(Console.ReadLine());
                    generation.ReprodcutionRate = x;
                    break;
                case "3":
                    generation.ChangingMutationEbaled = !generation.ChangingMutationEbaled;
                    break;
                case "4":
                    v = double.Parse(Console.ReadLine());
                    generation.MutationRate = v;
                    break;
                case "5":
                    x = int.Parse(Console.ReadLine());
                    generation.MutationChangeInterval = x;
                    break;
                case "6":
                    x = int.Parse(Console.ReadLine());
                    generation.EtylistChangeInterval = x;

                    break;

                case "7":
                    Console.WriteLine("1.mask crossover 2 parents , 2. mask crossover 3 parents , 3. mask crossover random amount parents , 4.PMXC(Partially Mapped Crossover)");
                    x = int.Parse(Console.ReadLine());
                    generation.CrossoverOption = x;
                    break;

                case "8":

                    x = int.Parse(Console.ReadLine());
                    generation.BestParentsKept = x;
                    break;

                case "9":
                    generation.TargetedMutationEnabled = !generation.TargetedMutationEnabled;
                 

                    break;

                case "10":

                   generation.BetterHoursMutationEnabled = !generation.BetterHoursMutationEnabled;
                    break;

                case "11":

                  generation.IgnoreMutationRate = !generation.IgnoreMutationRate;
                    break;

                case "12":

                    generation.Preferences = !generation.Preferences;
                    break;

                case "13":

                   generation.useNeuralMutation = !generation.useNeuralMutation;
                    break;
                case "14":

                    exit = true;
                    break;

                default:
                    Console.WriteLine("Nieprawidłowy wybór!");
                    break;
            }


        }
        

        public static void Menu(Generation generation, SaveManagment saveManagment)
        {

            bool exit = false;

            int x = 0;

           



            while (!exit)
            {

                Console.WriteLine("1. Zapisz Generacje do pliku");
                Console.WriteLine("2. Wyświetl Opcje");
                Console.WriteLine("3. Dostosuj opcje");
                Console.WriteLine("4. Wygeneruj kolejne generacje");
                Console.WriteLine("5 Clib Hill");
                switch (x = int.Parse(Console.ReadLine()))
                {
                    case 1:
                        saveManagment.SaveGeneration(generation);
                        break;
                    case 2:
                        generation.WyswietlOpcje();
                        break;
                    case 3:

                        DostosujOpcje(generation);
                        break;
                    case 4:

                        Console.WriteLine("Ilosc generacji: ");
                        int ilosc = int.Parse(Console.ReadLine());
                        for(int i = 0; i < ilosc; i++)
                        {
                            generation.GenerateNextGeneration();
                            
                        }
                        generation.PrintGenerations();
                        break;
                    case 5:

                       generation.ClibGeneration();
                        generation.PrintGenerations();
                        break;
                    default:
                        Console.WriteLine("Nieprawidłowy wybór!");
                        break;
                }
            }

        }
        public static void Creating(SaveManagment saveManagment)
        {
            int x = 0;

            bool exit = false;
            bool choice = false;
            
            string input = "";

            Console.WriteLine("Wybierz pracowników");

string directoryPath = "D:\\Programowanie IV\\msi_GA\\msi_GA\\Saved\\Workers";


            var files = Directory.GetFiles(directoryPath)
                       .Select(Path.GetFileName)
                       .ToList();


            if (files.Count == 0)
            {
                Console.WriteLine("Brak plików z pracownikami w folderze.");
                return;
            }

            Console.WriteLine("Dostępne pliki z pracownikami:");
            files.ForEach(Console.WriteLine);

            while (!exit)
            {
                Console.WriteLine("Wpisz nazwe pliku: ");
                input = Console.ReadLine();
                    foreach (string file in Directory.GetFiles("D:\\Programowanie IV\\msi_GA\\msi_GA\\Saved\\Workers"))
                {
                    if (file.Contains(input))
                    {
                        exit = true;
                    }
                }
            }

            string filepath = "D:\\Programowanie IV\\msi_GA\\msi_GA\\Saved\\Workers\\" + input;

            Console.WriteLine("1.Constant Aware, 2. Not Constant Aware");

            x = int.Parse(Console.ReadLine());

            if(x == 1)
            {
                choice = true;
            }
         

            Generation generation = new Generation(choice, filepath, saveManagment);
            generation.PrintGenerations();


            Menu(generation, saveManagment);


        }


        public static void Main()
        {


          bool exit = false;
            int x = 0;

             SaveManagment saveManagment = new SaveManagment("..\\msi_GA\\Saved");

            while (!exit)
            {
                Console.WriteLine("1.Wygeneruj nową generację");
                Console.WriteLine("2.Wczytaj Generacje");
               
                Console.WriteLine("3. Wyjdz");
            


                switch (x = int.Parse(Console.ReadLine()))
                {
                    case 1:

                        Creating(saveManagment);
                      

                        break;
                    case 2:

                        Directory.GetDirectories("..msi_GA\\Saved\\Saves")
                          .ToList()
                          .ForEach(Console.WriteLine);

                        Console.WriteLine("Wybierz folder z zapisaną generacją:");
                        string folder = Console.ReadLine();
                        Directory.GetFiles($"..\\msi_GA\\Saved\\Saves\\{folder}")
                         .ToList()
                         .ForEach(Console.WriteLine);
                        
                        Console.WriteLine("Wybierz nr save:");
                        int nr = int.Parse(Console.ReadLine());
                        Console.WriteLine("Series:");

                        int series = int.Parse(Console.ReadLine());

                        Generation generation = saveManagment.LoadGeneration(nr, series);
                        generation.PrintGenerations();
                        Menu(generation, saveManagment);

                        break;
                    case 3:
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Nieprawidłowy wybór!");
                        break;
                }

            }





      
                        }
                }
            }
        
    


