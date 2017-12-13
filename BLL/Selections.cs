using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DTO;

namespace BLL
{
    class Selections
    {

        public int roulette_Wheel_Selection(double[] arrayInput, int array_Length, Random rnd)
        {
            int index = -1;
            double temp = 0;
            double[] temp_Prob = new double[array_Length];
            if (array_Length == 1)
            {
                return 1;
            }

            for (int i = 0; i < array_Length; i++)
            {
                temp_Prob[i] = temp + arrayInput[i];
                temp = temp_Prob[i];
            }

            temp = Math.Round(rnd.NextDouble() * Math.Floor(temp_Prob[array_Length - 1]));
            for (int i = 0; i < array_Length; i++)
            {
                if (temp_Prob[i] > temp)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        public int touranmentSelection(double[] population_Fitness, int popSize, int touramentSize, Random rnd)
        {
            int[] touranmetGroup = new int[touramentSize];
            int index = 0;
            double tempTour = 0;
            for (int i = 0; i < touramentSize; i++)
            {
                touranmetGroup[i] = rnd.Next(popSize);
            }

            tempTour = population_Fitness[0];
            index = 0;
            for (int i = 1; i < touramentSize; i++)
            {
                if (population_Fitness[i] > tempTour)
                {
                    tempTour = touranmetGroup[i];
                    index = i;
                }
            }
            return index;
        }

        
    }
}
