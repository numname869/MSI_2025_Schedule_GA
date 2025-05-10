using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace msi_GA.GA
{

    public enum OccupationType
    {
        Operator,
        ControlTechnician,
        MaintenanceEngineer,
        RadiationTechnician,
        ElectricalEngineer,
        ChemistryTechnician
    }


    public class Gen : GeneticOperations
    {


        public int[][] _schedule;
        private int _fitness;
        private int _HoursScore;
        private int _ShiftBreakScore;
        private int _workerDispersionScore;
        public int _maxShiftsScore;
        private int _TypeOfWorkerPerShift;



        [JsonIgnore]
        public int days = 7;
       

        [JsonIgnore]
        private static  List<Worker> _workers = new List<Worker>
{
    // Operators (20 workers)
    new Worker( "Operator", 1),
    new Worker( "Operator", 0),
    new Worker("Operator", 2),
    new Worker( "Operator", 2),




    // Dispatchers (15 workers)
    new Worker( "Dispatcher", 2),
  //  new Worker(8, "Dispatcher", 1),
    new Worker("Dispatcher", 0),
   new Worker( "Dispatcher", 3),
   new Worker( "Dispatcher", 0),





    // Engineers (10 workers)
    new Worker( "Engineer", 0),
    new Worker("Engineer", 2),
    new Worker( "Engineer", 2),
     new Worker( "Engineer", 3),
        new Worker( "Engineer", 0),
    new Worker("Engineer", 0),
    new Worker( "Engineer", 2),
     new Worker( "Engineer", 3),

   // new Worker(15, "Engineer", 1),

};
        public static  List<Worker> WorkersList
        {


            get { return _workers; }
           

        }

            Random random = new Random();


        public Gen(bool IsconstantAware)
        {
            
            _schedule = new int[_workers.Count][];
  
            for (int i = 0; i < _workers.Count; i++)
            {
                _schedule[i] = new int[3*days];
            }

            PickGeneration(IsconstantAware);
            _fitness = CalculateFitness(_schedule);
            _HoursScore = HoursMeet(_schedule);
            _ShiftBreakScore = ShiftBreak(_schedule);
            _workerDispersionScore = WorkerDispersionGeneral(_schedule);
            _maxShiftsScore = MaxShifts(_schedule);
            _TypeOfWorkerPerShift = EachWorkerTypePerShifts(_schedule);

        }
        public Gen()
        {

            _schedule = new int[_workers.Count][];

            for (int i = 0; i < _workers.Count; i++)
            {
                _schedule[i] = new int[3 * days];
            }

            GenerateSchedule();
            _fitness = CalculateFitness(_schedule);
            _HoursScore = HoursMeet(_schedule);
            _ShiftBreakScore = ShiftBreak(_schedule);
            _workerDispersionScore = WorkerDispersionGeneral(_schedule);

        }



        public int ShiftBreakScore
        {
            get { return _ShiftBreakScore; }
            set { _ShiftBreakScore = value; }

        }

        public int WorkerDispersionScore
        {
            get { return _workerDispersionScore; }
            set { _workerDispersionScore = value; }
        }


        public int HoursScore
        {
            get { return _HoursScore; }
            set { _HoursScore = value; }

        }

     
        public int MaxShiftScore
        {
            get { return _maxShiftsScore; }
            set { _maxShiftsScore = value; }
        }

        public int TypeOfWorkerPerShift
        {
            get { return _TypeOfWorkerPerShift; }
            set { _TypeOfWorkerPerShift = value; }
        }

        public int Days
        {
            get { return days; }
            set { days = value; }
        }

        public int Workers
        {
            get { return _workers.Count; }
        }
        public int Fitness
        {
            get { return _fitness; }
            set { _fitness = value; }
        }

        public int[][] Schedule
        {
            get { return _schedule; }
        }


        public int WorketsAmount
        {

            get { return _schedule.Length; }
        }

        public int ShiftsAmount
        {
            get { return days*3; }
        }

        
        public void PickGeneration(bool generation)
        {
            if (generation)
            {
                ConstantAwareGeneration();
              
            }
            else
            {
                GenerateSchedule();
            }
        }

        public void GenerateSchedule()
        {
            

            for (int i = 0; i < _schedule.Length; i++)
            {

                for (int j = 0; j < _schedule[i].Length; j = j+ 3)
                {
                    
                    
                    for(int k = j; k < j+3; k++)
                    {
                        _schedule[i][k] = random.Next(0, 2);

                    }


                }
            }



        }

        public void ConstantAwareGeneration()
        {

            for (int i = 0; i < _schedule.Length; i++)
            {

                for (int j = 0; j < _schedule[i].Length; j = j + 3)
                {
                    

                    for (int k = j; k < j + 3; k++)
                    {
                       
                        
                        _schedule[i][k] = random.Next(0, 2);

                        if((k != 0 && _schedule[i][k - 1] == 1 ) || (k > 1 && _schedule[i][k-2] == 1) )
                        {
                            _schedule[i][k] = 0;
                        }
                       


                    }


                }
            }


        }

    







    }
}
