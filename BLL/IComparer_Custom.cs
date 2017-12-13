using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using DTO;

namespace BLL
{
    public class IComparer_Custom : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            // Compare y and x in reverse order.
            return y.CompareTo(x);
        }
    }


    public class SortAscScalarFitness : IComparer<Chromosome>
    {
        public int Compare(Chromosome x, Chromosome y)
        {
            if (x.Scalar_Fitness > y.Scalar_Fitness)
            {
                return 1;
            }
            else
            {
                if (x.Scalar_Fitness < y.Scalar_Fitness)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }

    public class SortDecsScalarFitness : IComparer<Chromosome>
    {
        public int Compare(Chromosome x, Chromosome y)
        {
            if (x.Scalar_Fitness < y.Scalar_Fitness)
            {
                return 1;
            }
            else
            {
                if (x.Scalar_Fitness > y.Scalar_Fitness)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }

    public class SortAscFactorialCostIdx : IComparer<Chromosome>//Sử dụng cho GA khi đó có 1 task
    {
        public int Compare(Chromosome x, Chromosome y)
        {
            if (x.Factorial_Cost[0] > y.Factorial_Cost[0])
            {
                return 1;
            }
            else
            {
                if (x.Factorial_Cost[0] < y.Factorial_Cost[0])
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }

}
