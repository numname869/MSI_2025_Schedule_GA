using System.Security.Cryptography.X509Certificates;
using msi_GA.GA;
using msi_GA.Task_handling;

namespace msi_GA.GA
{
    internal class Program
    {
        //targeted mutation splaszce wynik z jakiegos powodu

        

        public static void Main()
        {


          

            int x = 0;
            int y = 0;
            bool ConstantAware = false;
            Console.WriteLine("1. Constant Aware Gen , 2. Non-Aware gen");
            x = int.Parse(Console.ReadLine());
            if (x == 1)
            {
                ConstantAware = true;
            }
          

            bool exit = false;
            bool exit1 = false;
            Generation generation = null;
            generation = new Generation(ConstantAware);

           
            double v = 0.0;

            while (!exit)
            {
                Console.WriteLine("\n=== MENU ===");
                Console.WriteLine("1. Posortuj pierwszą generację");
                Console.WriteLine("2. Generuj następną generację");
                Console.WriteLine("3. Wyświetl generację");
                Console.WriteLine("4. Wyswietl opcje");
                Console.WriteLine("5. Dostosuj opcje");
                Console.WriteLine("6.Wygeneruj ilosc generacji");

                Console.WriteLine("7. Wyjdź");

                Console.Write("Wybierz opcję: ");

                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":

                        generation.SortGenerations();
                        generation.PrintGenerations();
                        Console.WriteLine("Utworzono pierwszą generację");
                        break;

                    case "2":
                        if (generation == null)
                        {
                            Console.WriteLine("Najpierw utwórz generację!");
                            break;
                        }
                        generation.AdaptiveNextGenerations(x);
                        generation.SortGenerations();
                        generation.PrintGenerations();
                        Console.WriteLine("Wygenerowano nową generację");
                        break;

                    case "3":
                        if (generation == null)
                        {
                            Console.WriteLine("Brak generacji do wyświetlenia!");
                            break;
                        }
                        generation.PrintGenerations();
                        break;

                    case "4":
                        generation.WyswietlOpcje();

                        break;

                    case "5":

                        while (!exit1)
                        {
                            

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

                            Console.WriteLine("10.Wyjdź z opcji");


                            string input1 = Console.ReadLine();

                            switch (input1)
                            {

                                case "1":
                                    generation.EtylistRateEnabled = !generation.EtylistRateEnabled;

                                    break;

                                case "2":
                                    x = int.Parse(Console.ReadLine());
                                    generation.ElitistRate = x;
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

                                    exit1 = true;
                                    break;

                                default:
                                    Console.WriteLine("Nieprawidłowy wybór!");
                                    break;
                            }


                        }

                        exit1 = false;

                        break;
                




                    case "6":
                                Console.WriteLine("Podaj ilość pokoleń do wygenerowania:");
                                x = int.Parse(Console.ReadLine());
                                generation.AdaptiveNextGenerations(x);

                        generation.PrintGenerations();

                        break;

                            case "7":
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
    


