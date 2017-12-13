using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DTO
{
    public class Mutation_Parameters
    {
        string _mutation_Name;
        public string Mutation_Name
        {
            get { return _mutation_Name; }
            set { _mutation_Name = value; }
        }

        double _mutation_Rate;
        public double Mutation_Rate
        {
            get { return _mutation_Rate; }
            set { _mutation_Rate = value; }
        }

        double _sigma;
        public double Sigma
        {
            get { return _sigma; }
            set { _sigma = value; }
        }
    }
}
