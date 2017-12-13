using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DTO
{
    public class Data_MFEA
    {
        double _running_Time;
        public double Running_Time
        {
            get { return _running_Time; }
            set { _running_Time = value; }
        }

        List<double[]> _ev_Best_Fitness;

        public List<double[]> Ev_Best_Fitness
        {
            get { return _ev_Best_Fitness; }
            set { _ev_Best_Fitness = value; }
        }

        List<double> _diversity_of_Pop;

        public List<double> Diversity_of_Pop
        {
            get { return _diversity_of_Pop; }
            set { _diversity_of_Pop = value; }
        }


        //Luu xem moi the he cao bao nhieu ind thuoc moi task o moi the he
        List<double[]> _num_Ind_in_Task;

        public List<double[]> Num_Ind_in_Task
        {
            get { return _num_Ind_in_Task; }
            set { _num_Ind_in_Task = value; }
        }
      

        Chromosome[] _best_Ind_Data;

        public Chromosome[] Best_Ind_Data
        {
            get { return _best_Ind_Data; }
            set { _best_Ind_Data = value; }
        }

     
        long _total_Evaluations;

        public long Total_Evaluations
        {
            get { return _total_Evaluations; }
            set { _total_Evaluations = value; }
        }

        

        public Data_MFEA(int num_Genes, int num_Tasks)
        {
            _best_Ind_Data = new Chromosome[num_Tasks];
            _ev_Best_Fitness = new List<double[]>();
            _num_Ind_in_Task = new List<double[]>();
            _diversity_of_Pop = new List<double>();
            for(int i = 0; i < num_Tasks; i++)
            {
                _best_Ind_Data[i] = new Chromosome(num_Genes, num_Tasks);
            }
        }
    }
}
