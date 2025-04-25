using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace msi_GA.GA
{

    
    public class Worker
    {

       
        // 0 - intern , 1 - junior, 2 - mid, 3 - senior
      
        private string _occupation;
       
        private int _experience;

        public Worker( string occupation, int experience )
        {
            
            _occupation = occupation;
            
            _experience = experience;

           
        }


        public int Experience
        {
            get { return _experience; }

        }


        public string Occupation
        {
            get { return _occupation; }
        }

     





    }
}
