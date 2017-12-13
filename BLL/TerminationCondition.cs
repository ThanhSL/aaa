using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL
{
    public class TerminationCondition
    {
        private long _maximum_number_of_evaluations,             /* The maximum number of evaluations. */
                     _number_of_evaluations = 1;                     /* The current number of times a function evaluation was performed. */

        public long Number_of_evaluations
        {
            get { return _number_of_evaluations; }
            set { _number_of_evaluations = value; }
        }

        public long Maximum_number_of_evaluations
        {
            get { return _maximum_number_of_evaluations; }
            set { _maximum_number_of_evaluations = value; }
        }


        #region Section Termination
        public TerminationCondition(long maximum_number_of_evaluations, long number_of_evaluations)
        {
            _maximum_number_of_evaluations = maximum_number_of_evaluations;
            _number_of_evaluations = number_of_evaluations;
        }

        public TerminationCondition()
        {

        }


        /**************************************************************************************************************************************
         * Determines which solution is the best of all solutions in the current population.
         *************************************************************************************************************************************/
        void determineBestSolutionInCurrentPopulation(ref int index_of_best)
        {
            //int i;
            //index_of_best = 0;
            //for (i = 0; i < population_size; i++)
            //{
            //    if (betterFitness(objective_values[i], constraint_values[i], objective_values[index_of_best], constraint_values[index_of_best]))
            //    {
            //        index_of_best = i;
            //    }
            //}
        }


        /***************************************************************************************************************************************
         * Returns TRUE if the maximum number of evaluations
         * has been reached, FALSE otherwise.
         **************************************************************************************************************************************/
        bool checkNumberOfEvaluationsTerminationCondition()
        {
            //if (number_of_evaluations >= maximum_number_of_evaluations)
            //{
            //    return true;
            //}
            //return false;

            if (_number_of_evaluations >= _maximum_number_of_evaluations)
            {
                return false;
            }
            return true;
        }


        /**************************************************************************************************************************************
         * Returns TRUE if the value-to-reach has been reached. ->>>>>>>> gia tri tot nhat cua ham muc tieu (lon nhat) lon hon 1 gia tri nao do thi tra ve TRUE
         *************************************************************************************************************************************/
        bool checkVTRTerminationCondition()
        {
            int individual_index_best = 0;

            determineBestSolutionInCurrentPopulation(ref individual_index_best);

            //if ((constraint_values[individual_index_best] == 0) && (objective_values[individual_index_best] >= vtr))
            //{
            //    return true;
            //}

            //return false;

            //if ((constraint_values[individual_index_best] == 0) && (objective_values[individual_index_best] >= vtr))
            //{
            //    return false;
            //}

            return true;
        }


        /**************************************************************************************************************************************
         * Checks whether the fitness variance
         * has become too small (user-defined tolerance).->>>>phuong sai cua objective cua cac cac the nho hon muc cho phep thi tra ve TRUE
         *************************************************************************************************************************************/
        bool checkFitnessVarianceTermination()
        {
            //double objective_avg, objective_var;

            //objective_avg = 0.0;
            //for (int i = 0; i < population_size; i++)
            //{
            //    objective_avg += objective_values[i];
            //}
            //objective_avg = objective_avg / ((double)population_size);

            //objective_var = 0.0;
            //for (int i = 0; i < population_size; i++)
            //{
            //    objective_var += (objective_values[i] - objective_avg) * (objective_values[i] - objective_avg);
            //}
            //objective_var = objective_var / ((double)population_size);

            //if (objective_var <= 0.0)
            //{
            //    objective_var = 0.0;
            //}

            ////if(objective_var <= fitness_variance_tolerance)
            ////{
            ////    return true;
            ////}

            ////return false;

            //if (objective_var <= fitness_variance_tolerance)
            //{
            //    return false;
            //}

            return true;
        }


        /*************************************************************************************************************************************
         * Returns TRUE if termination should be enforced, FALSE otherwise.
         *************************************************************************************************************************************/
        public bool checkTerminationCondition()
        {
            if (_maximum_number_of_evaluations >= 0)
            {
                if (!checkNumberOfEvaluationsTerminationCondition())
                {
                    //Console.WriteLine("Thoat do maximum evaluations");
                    return false;
                }
            }

            //if (use_vtr > 0)
            //{
            //    if (!checkVTRTerminationCondition())
            //    {
            //        Console.WriteLine("Thoat do vtr");
            //        return false;
            //    }

            //}

            if (!checkFitnessVarianceTermination())
            {
                //Console.WriteLine("Thoat do variance");
                //return false;

            }

            return true;
        }



        #endregion


    }
}
