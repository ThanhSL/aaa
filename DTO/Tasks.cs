using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DTO
{
    public class Tasks
    {
        int _maxDims;           //Số chiều lớn nhất của tất các tasks
        public int MaxDims
        {
            get { return _maxDims; }
            set { _maxDims = value; }
        }
        int _dims;              //dimension
        public int Dims
        {
            get { return _dims; }
            set { _dims = value; }
        }

        string _data_file_path;

        public string Data_file_path
        {
            get { return _data_file_path; }
            set { _data_file_path = value; }
        }

        double[] _lb;

        public double[] Lb
        {
            get { return _lb; }
            set { _lb = value; }
        }

        double[] _ub;

        public double[] Ub
        {
            get { return _ub; }
            set { _ub = value; }
        }

        string _function_Cost_Name;
        public string Function_Cost_Name
        {
            get { return _function_Cost_Name; }
            set { _function_Cost_Name = value; }
        }


        //Ma trận trọng số đầu vào - Nếu bài toán sử dụng
        //Với funtions 1 - 7 đóng vai trò là Rotation matrix;
        //Với ONE_MAX_TREE là ma trận tối ưu????

        double[,] _weight_Matrix;
        public double[,] Weight_Matrix
        {
            get { return _weight_Matrix; }
            set { _weight_Matrix = value; }
        }

        //Lời giải tối ưu: dùng cho bài toán benchmark sử dụng functions: 1 - 7
        //Tương đương tham số: opt
        double[] _global_Optima;

        public double[] Global_Optima
        {
            get { return _global_Optima; }
            set { _global_Optima = value; }
        }

        //Lưu các đỉnh trong từng cluster của bài toán CLUSTERED TREE
        //D'Emidio, M., Forlizzi, L., Frigioni, D., Leucci, S., & Proietti, G. (2016). On the Clustered Shortest-Path Tree Problem. In ICTCS (pp. 263-268).
        int[][] _vertex_In_Cluster;

        public int[][] Vertex_In_Cluster
        {
            get { return _vertex_In_Cluster; }
            set { _vertex_In_Cluster = value; }
        }

    

        int _num_Cluster;

        public int Num_Cluster
        {
            get { return _num_Cluster; }
            set { _num_Cluster = value; }
        }

        int _source_Vertex;

        public int Source_Vertex
        {
            get { return _source_Vertex; }
            set { _source_Vertex = value; }
        }

      
        public Tasks()
        {
        }

        public Tasks(int dimension)
        {
            _dims = dimension;
            _weight_Matrix = new double[dimension, dimension];
            _ub = new double[dimension];
            _lb = new double[dimension];
            _global_Optima = new double[dimension];
        }


    }
}
