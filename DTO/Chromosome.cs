using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


namespace DTO
{
    public class Chromosome
    {
        private double[] _obj_value; //k object value
        public double[] Obj_value
        {
            get { return _obj_value; }
            set { _obj_value = value; }
        }


        double[] _constraint_violation;

        public double[] Constraint_violation
        {
            get { return _constraint_violation; }
            set { _constraint_violation = value; }
        }
       

        double[] _factorial_Cost;//k factorial_Cost

        public double[] Factorial_Cost
        {
            get { return _factorial_Cost; }
            set { _factorial_Cost = value; }
        }


        int[] _factorial_Rank;

        public int[] Factorial_Rank
        {
            get { return _factorial_Rank; }
            set { _factorial_Rank = value; }
        }
      

        double _scalar_Fitness;
        public double Scalar_Fitness
        {
            get { return _scalar_Fitness; }
            set { _scalar_Fitness = value; }
        }

        int _skill_Factor;
        public int Skill_Factor
        {
            get { return _skill_Factor; }
            set { _skill_Factor = value; }
        }

      
        private double[] _rnvec; //real_number_vecto: (genotype)--> decode to find design variables --> (phenotype) ;

        public double[] Rnvec
        {
            get { return _rnvec; }
            set { _rnvec = value; }
        }

        private int[] _invec;   //Integer n vector --> dung cho bieu dien so nguyen

        public int[] Invec
        {
          get { return _invec; }
          set { _invec = value; }
        }

        private List<Edge> _envec; //Edge n vector  -> bieu dien danh sach cac canh

        public List<Edge> Envec
        {
          get { return _envec; }
          set { _envec = value; }
        }

        double[,] _edges_Matrix; //Ma tran canh

        public double[,] Edges_Matrix
        {
            get { return _edges_Matrix; }
            set { _edges_Matrix = value; }
        }

        public Chromosome(int num_Genens, int num_Tasks)//Có thể thêm Index bài toán vào để khởi tạo biến cho bài toán đó thôi
        {
            _rnvec = new double[num_Genens];
            _invec = new int[num_Genens];       //prufer thì chỉ n-2 phần tử đầu có giá trị
            _envec = new List<Edge>();
            _edges_Matrix = new double[num_Genens, num_Genens];

            _obj_value = new double[num_Tasks];
            _factorial_Cost = new double[num_Tasks];
            _factorial_Rank = new int[num_Tasks];
            _constraint_violation = new double[num_Tasks];
            _scalar_Fitness = 0.0f;
            _skill_Factor = 0;
        }

        

        public void copy_From_Individual(Chromosome from_Ind, int numGenes, int numTasks)
        {
            for (int i = 0; i < numTasks; i++)
            {
                this.Obj_value[i] = from_Ind.Obj_value[i];
                this.Constraint_violation[i] = from_Ind.Constraint_violation[i];
                this.Factorial_Cost[i] = from_Ind.Factorial_Cost[i];
                this.Factorial_Rank[i] = from_Ind.Factorial_Rank[i];
            }
            this.Scalar_Fitness = from_Ind.Scalar_Fitness;
            this.Skill_Factor = from_Ind.Skill_Factor;

            for (int i = 0; i < numGenes; i++)
            {
                this.Rnvec[i] = from_Ind.Rnvec[i];
                this.Invec[i] = from_Ind.Invec[i];
            }

            foreach(Edge tmp in from_Ind.Envec)
            {
                this.Envec.Add(tmp);
            }

            for (int i = 0; i < numGenes; i++)
            {
                for (int j = 0; j < numGenes; j++)
                {
                     this.Edges_Matrix[i, j] = from_Ind.Edges_Matrix[i, j];
                }
            }
        }

        #region Solve Cluster shortest-path tree by using Prufer code

        /*********************************************************************************************************************************************
         * Khởi tạo cá thể cho biểu diễn Prufer code giải bài toán CSTP.
         * element_in_a_Cluster: số phần tử trong cluster (cluster có thể của các task khác nhau -> cluster 1 của task 1, cluster 2 của task 2)
         * 
         ********************************************************************************************************************************************/
        int[] _task_Index;//Lưu thông tin cluster nào lấy từ task nào
        int[] _segment_of_a_Cluster; //Lưu thông tin cluster có gen nằm từ phần tử [i-1] tới [i]
        int _max_Cluster;

        public int Max_Cluster
        {
            get { return _max_Cluster; }
            set { _max_Cluster = value; }
        }

        public int[] Segment_of_a_Cluster
        {
            get { return _segment_of_a_Cluster; }
            set { _segment_of_a_Cluster = value; }
        }

        public int[] Task_Index
        {
            get { return _task_Index; }
            set { _task_Index = value; }
        }

        public Chromosome(int num_Genens, int num_Tasks, int num_Cluster, int[] element_in_a_Cluster)
        {
            //_rnvec = new double[num_Genens];
            _invec = new int[num_Genens];       //prufer thì chỉ n-2 phần tử đầu có giá trị
            //_envec = new List<Edge>();
            _edges_Matrix = new double[num_Genens, num_Genens];

            _obj_value = new double[num_Tasks];
            _factorial_Cost = new double[num_Tasks];
            _factorial_Rank = new int[num_Tasks];
            _constraint_violation = new double[num_Tasks];
            _scalar_Fitness = 0.0f;
            _skill_Factor = 0;
            //Prufer
            _task_Index = new int[num_Cluster];
          

        }

        public Chromosome(int num_Tasks,Tasks[] tsks)
        {
            //01. Xác định số max clusters
            _max_Cluster = tsks[0].Num_Cluster;
            for (int i = 1; i < num_Tasks; i++)
            {
                if(_max_Cluster < tsks[i].Num_Cluster)
                {
                    _max_Cluster = tsks[i].Num_Cluster;
                }
            }

            //02. Xác đinh ind lấy cluster nào và số gen của mỗi cluster, số gen của ind
            _task_Index = new int[_max_Cluster];
            _segment_of_a_Cluster = new int[_max_Cluster + 1];//vì +1 segment là uper đồ thị
            _segment_of_a_Cluster[0] = _max_Cluster;

            int num_Gen_Tmp = _max_Cluster;
            for (int i = 0; i < _max_Cluster; i++)
            {
                int tmp = 0;
                int idx_tmp = 0;
                for(int j = 0; j < num_Tasks; j++)
                {
                    if(i < tsks[j].Num_Cluster)
                    {
                        if(tmp < tsks[j].Vertex_In_Cluster[i].Length)
                        {
                            tmp = tsks[j].Vertex_In_Cluster[i].Length;
                            idx_tmp = i;
                        }
                    }
                }
                _task_Index[i] = idx_tmp;
                num_Gen_Tmp = num_Gen_Tmp + tmp;
                _segment_of_a_Cluster[i + 1] = num_Gen_Tmp;
            }

            //03. 

            _invec = new int[num_Gen_Tmp];       //prufer thì chỉ n-2 phần tử đầu có giá trị           
            _edges_Matrix = new double[num_Gen_Tmp, num_Gen_Tmp];

            _obj_value = new double[num_Tasks];
            _factorial_Cost = new double[num_Tasks];
            _factorial_Rank = new int[num_Tasks];
            _constraint_violation = new double[num_Tasks];
            _scalar_Fitness = 0.0f;
            _skill_Factor = 0;
        }

        #endregion

    }

    //Định nghĩa cạnh trong đồ thị

    public class Edge
    {
        int _start_Point;

        public int Start_Point
        {
          get { return _start_Point; }
          set { _start_Point = value; }
        }
        int _end_Point;

        public int End_Point
        {
          get { return _end_Point; }
          set { _end_Point = value; }
        }

        public Edge(int s_point, int e_point)
        {
            _start_Point = s_point;
            _end_Point = e_point;
        }
    }
}
