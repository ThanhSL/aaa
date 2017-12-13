using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DTO
{
    public class Crossover_Parameters
    {
        double _cf;//Use in crossver index 1 
        public double Cf
        {
            get { return _cf; }
            set { _cf = value; }
        }

        string _crossver_Name;
        public string Crossver_Name
        {
            get { return _crossver_Name; }
            set { _crossver_Name = value; }
        }
       
        double _rmp;
        public double Rmp
        {
            get { return _rmp; }
            set { _rmp = value; }
        }

    }
}
