using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Reflection.Metadata;
using System.Xml;

namespace msi_GA.GA
{
    public abstract class GeneticOperations
    {


        Random random = new Random();

        protected const int HoursScore = 1;
        protected const int MaxShiftScore = 1;
        protected const int ShiftBreakScore = 3;
        protected const int WorkerDispersionScore = 1;
     




        protected const int LowHoursScore = -10;
        protected const int MidHoursScore = 5;
        protected const int PerfectHoursScore = 10;
        protected const int OverHoursScore = -5;



        protected const int OccupationMatchedScore = 3;


        List<string> WorkTitles = new List<string>
        {
            "Operator",
            "Dispatcher",
            "Engineer"
        };

        public int CalculateFitness(int[][] schedule)
        {

            int sum = 0;


            sum += HoursMeet(schedule);
            sum +=  MaxShifts(schedule);
            sum += ShiftBreak(schedule);
            sum += WorkerDispersionGeneral(schedule);
            sum += EachWorkerTypePerShifts(schedule);


            return sum;
        }







        // 0 - intern , 1 - junior, 2 - mid, 3 - senior

        // zadanie bazowe -> kazdego dnia musi byc chociaz jedna osoba z danego stanowiska na zmianie - jezeli jest niedoswiadczona 
        //to musi jej pomagac osoba z doswiadczeniem


        protected virtual int EachWorkerTypePerShifts(int[][] schedule)
        {
            int points = 0;

            foreach (string worktitle in WorkTitles)
            {
                

                for (int i = 0; i < schedule[0].Length; i++)
                {
                    for (int j = 0; j < schedule.Length; j++)
                    {
                        int sum = 0;
                        if (schedule[j][i] == 1)
                        {
                           
                            if (Gen.WorkersList[j].Occupation == worktitle)
                            {
                               
                                if (Gen.WorkersList[j].Experience != 0)
                                {
                                    sum++;
                                   
                                }
                                else
                                {
                                    if (FindExperiencedPeer(schedule, j, i))
                                    {
                                        sum++;
                                        
                                    }
     
                                }

                            
                            }


                           
                        }
                        if (sum != 0)
                        {
                            points++;
                            break; ;
                        }
                    }


                }
            }


            return points * OccupationMatchedScore;
        }


        protected virtual bool FindExperiencedPeer(int[][] schedule, int i, int j)
        {
            for(int k = 0; k < schedule.Length; k++)
            {
                if (schedule[k][j] == 1 && Gen.WorkersList[k].Experience > Gen.WorkersList[i].Experience)
                {
                    return true;
                }
            }

            return false;
        }


        protected virtual int HoursMeet(int[][] schedule)
        {
            int sum = 0;
            if (schedule == null || schedule.Length == 0)
                return 0;



            for (int i = 0; i < schedule.Length; i++)
            {

                int hours = 0;
                for (int j = 0; j < schedule[i].Length; j++)
                {
                    if (schedule[i][j] != 0)
                    {
                        hours += 8;
                    }
                }


                if (hours < 24) sum = sum + LowHoursScore;
                if (hours >= 30 && hours < 48 && hours != 40) sum = sum + MidHoursScore;
                if (hours == 40) sum = sum + PerfectHoursScore;
                if (hours > 48) sum = sum + OverHoursScore;

            }

            return sum;
        }



        protected virtual int MaxShifts(int[][] schedule)
        {

            int sum = 0;
            int daysMatched = 0;
            for (int i = 0; i < schedule.Length; i++)
            {
                for (int j = 0; j < schedule[i].Length; j = j + 3)
                {


                    for (int k = j; k < j + 3; k++)
                    {
                        if (schedule[i][k] == 1) sum++;
                    }


                    if (sum == 1) daysMatched++;
                    sum = 0;
                }
            }


            return daysMatched * MaxShiftScore;

        }


        protected virtual int ShiftBreak(int[][] schedule)
        {
            int sum = 0;

            for (int i = 0; i < schedule.Length; i++)
            {
                for (int j = 2; j < schedule[i].Length - 3; j = j + 3)
                {
                    
                    if (schedule[i][j] != 1 || schedule[i][j + 1] != 1)
                    {
                        
                        sum++;
                       
                    }
                }
                
            }

            return  sum * ShiftBreakScore;

        }

        // chcemy zeby byl ktos 24/7 wiec tak naprawde musimy sprawdzic kazda zmiane czy ktos jest 
        protected virtual int WorkerDispersionGeneral(int[][] shedule)
        {
           
            int sum = 0;

            for (int i = 0; i < shedule[0].Length; i++) // teraz sprawdzamy kazdy dzien
            {
                for (int j = 0; j < shedule.Length; j++) // sprawdzamy czy jest jakis pracownik na tej zmianie
                {
                    if (shedule[j][i] == 1)
                    {
                        sum++;
                        break;
                       
                    }

                 }

                }

            return sum * WorkerDispersionScore;
        }










        public int CalculateMaxFitness(int[][] schedule)
        {
            return CalculateMaxHoursScore(schedule) + CalulateMaxShiftBreakScore(schedule) + CalculateMaxWorkerDispersionScore(schedule) + CalculateMaxShiftsScore(schedule) + CalculateMaxEachWorkerTypePerShifts(schedule);
        }

        public int CalculateMaxHoursScore(int[][] schedule)
        {
            return PerfectHoursScore * schedule.Length;
        }

        public int CalulateMaxShiftBreakScore(int[][] schedule)
        {
            return ShiftBreakScore * ((schedule[0].Length - 1) / 3) * schedule.Length;
        }

        public int CalculateMaxWorkerDispersionScore(int[][] schedule)
        {
            return WorkerDispersionScore * schedule[0].Length;
        }
        public double GetNormalizedFitness(int[][] schedule)
        {
            int actualFitness = CalculateFitness(schedule);
            int maxFitness = CalculateMaxFitness(schedule);
            return (double)actualFitness / maxFitness;
        }

        public int CalculateMaxShiftsScore(int[][] schedule)
        {
           
            return MaxShiftScore * schedule[0].Length/3  * schedule.Length;
        }

        public int HoursPerWorker(int[][] schedule, int i)
        {

            int sum = 0;
            if (schedule == null || schedule.Length == 0)
                return 0;



            

                int hours = 0;
                for (int j = 0; j < schedule[i].Length; j++)
                {
                    if (schedule[i][j] != 0)
                    {
                        hours += 8;
                    }
                }


                if (hours < 24) sum = sum + LowHoursScore;
                if (hours >= 30 && hours > 48 && hours != 40) sum = sum + MidHoursScore;
                if (hours == 40) sum = sum + PerfectHoursScore;
            if (hours > 48) sum = sum + OverHoursScore;

            

            return sum;


        }

        public int WorkerStats(int[][]schedule, int i)
        {

            return HoursPerWorker(schedule, i) + SchiftBreaksPerWorker(schedule, i);

        }

        public double WorkerMaxStats(int[][] schedule)
        {
            return PerfectHoursScore + (ShiftBreakScore * ((schedule[0].Length -1) / 3 ));

        }

        public int CalculateMaxEachWorkerTypePerShifts(int[][] schedule)
        {

           return  schedule[0].Length * OccupationMatchedScore * WorkTitles.Count;
        }
        public int SchiftBreaksPerWorker(int[][]schedule, int i)
        {
            int sum = 0;

           
                for (int j = 2; j < schedule[i].Length - 3; j = j + 3)
                {
                    if (schedule[i][j] == 1 && schedule[i][j + 1] == 1)
                    {
                        sum++;
                    }
                }
            

            return ShiftBreakScore * sum;
        }


   
    }
}