using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DTO;
using DAL;
using System.Collections;
using System.Diagnostics;
using System.IO;

namespace BLL
{
    public class ReadWriteFile
    {
        DAL.ReadWriteFile DAL_rwFile = new DAL.ReadWriteFile();
        Initialize_Chromosome init_Chromo = new Initialize_Chromosome();
        //In ket qua cac seed ra file tong hop
        public void writeOptimalSolutionofSeed(string fileName, int seed, double best_Ind_Fitness, string running_Time)
        {
            DAL_rwFile.writeOptimalSolutionofSeed(fileName, seed, best_Ind_Fitness, running_Time);
        }

        #region MFO
        public void read_01KP(string fileName, ref double caption, out double[] weight_arr, out double[] values_arr, out int numGenes)
        {
            DAL_rwFile.read_01KP(fileName, ref caption, out weight_arr, out values_arr, out numGenes);
        }

        public void write_Solution_to_File(string fileName, int seed, double[] array, int num_Genes,  double fitness)
        {
            DAL_rwFile.write_Solution_to_File(fileName, seed, array, num_Genes, fitness);
        }

        public void write_TSP_Solution_to_File(string fileName, int seed, int[] array, int num_Genes, double fitness)
        {
            DAL_rwFile.write_TSP_Solution_to_File(fileName, seed, array, num_Genes, fitness);
        }

        public void writeArrayToFile(string file_Path, double[] arr, int array_Size, bool append)
        {
            DAL_rwFile.writeArrayToFile(file_Path, arr, array_Size, append);
        }

        public void writeMatrixToFile(string file_Path, double[,] matrixWeight, int matrix_Size, bool append)
        {
            DAL_rwFile.writeMatrixToFile(file_Path, matrixWeight, matrix_Size, append);
        }

        public void write_Opt_Solution_File(string file_Path, int seed, int[,] matrixWeight, int matrix_Size, double fitness, string running_Time, bool append)
        {
            DAL_rwFile.write_Opt_Solution_File(file_Path, seed, matrixWeight, matrix_Size, fitness, running_Time, append);
        }

        public void writeMatrixToFile(string file_Path, int[,] matrixWeight, int matrix_Size, bool append)
        {
            DAL_rwFile.writeMatrixToFile(file_Path, matrixWeight, matrix_Size, append);
        }

        public void writeBestFitnessInPop(string file_Path, int ord_Generations, double best_Ind_Fitness)
        {
            DAL_rwFile.writeBestFitnessInPop(file_Path, ord_Generations, best_Ind_Fitness);
        }

        public void create_Tree_Instance_Random(string file_Name, int num_Vertex, double low_Weight_Edge, double height_Weight_Edge, double low_Weight_Vertex, double height_Weight_Vertex, Random rnd)
        {
            DAL_rwFile.create_Tree_Instance_Random(file_Name, num_Vertex, low_Weight_Edge, height_Weight_Edge, low_Weight_Vertex, height_Weight_Vertex, rnd);
        }

        public void read_Tree_Instance(string filePath, out int num_Vertex, out double[,] edge_Weight, out double[] vertex_Weight)
        {
            DAL_rwFile.read_Tree_Instance(filePath, out num_Vertex, out edge_Weight, out vertex_Weight);
        }

        public double[,] read_TSP_Instance(string filePath, out int numOfCity)
        {
            return DAL_rwFile.read_TSP_Instance(filePath, out numOfCity);
        }

        /*********************************************************************************************************************************************
        * Đọc dữ liệu từ file cho 7 hàm benchmark chuẩn
        * Output:   + mang chưa gia tri tối ưu: global_Optima
        *           + mảng 2 chiều chứa rotation_Matrix: rotation_Matrix
        *           + số biến của solution
        *                       
        ********************************************************************************************************************************************/
        public void read_Data_For_Real_Functions(string filePath, out double[] global_Optima, out double[,] rotation_Matrix, ref int dimention)
        {
            DAL_rwFile.read_Data_For_Real_Functions(filePath, out global_Optima, out rotation_Matrix, ref dimention);
        }

        //01. Function 01: sphere
        //02. Function 02: rosenbrock
        //03. Function 03: ackley
        //04. Function 04: rastrigin
        //05. Function 05: griewank
        //06. Function 06: weierstrass
        //07. Function 07: schwefel
        //08. Function 08: TSP

        public void read_Data(ref Tasks tasks)
        {
            double[] global_Optima;
            double[,] rotation_Matrix;
            int dimension = 1;
            Tasks tmp_Task = new Tasks();
           
            switch (tasks.Function_Cost_Name.Trim())
            {
                case "Sphere":
                    #region sphere
                    read_Data_For_Real_Functions(tasks.Data_file_path, out global_Optima, out rotation_Matrix, ref dimension);
                    tasks.Lb = new double[dimension];
                    tasks.Ub = new double[dimension];
                    tasks.Global_Optima = new double[dimension];
                    tasks.Weight_Matrix = new double[dimension, dimension];
                    for (int i = 0; i < dimension; i++)
                    {
                        tasks.Lb[i] = -100;
                        tasks.Ub[i] = 100;
                        tasks.Global_Optima[i] = global_Optima[i];
                        for(int j = 0; j < dimension; j++)
                        {
                            tasks.Weight_Matrix[i, j] = rotation_Matrix[i, j];
                        }
                    }
                    tasks.Dims = dimension;
                    #endregion
                    break;
                case "Rosenbrock":
                    #region rosenbrock
                    read_Data_For_Real_Functions(tasks.Data_file_path, out global_Optima, out rotation_Matrix, ref dimension);
                    tasks.Lb = new double[dimension];
                    tasks.Ub = new double[dimension];
                    tasks.Global_Optima = new double[dimension];
                    tasks.Weight_Matrix = new double[dimension, dimension];
                    for (int i = 0; i < dimension; i++)
                    {
                        tasks.Lb[i] = -50;
                        tasks.Ub[i] = 50;
                        tasks.Global_Optima[i] = global_Optima[i];
                        for(int j = 0; j < dimension; j++)
                        {
                            tasks.Weight_Matrix[i, j] = rotation_Matrix[i, j];
                        }
                    }
                    tasks.Dims = dimension;
                    #endregion
                    break;
                case "Ackley":
                    #region ackley
                    read_Data_For_Real_Functions(tasks.Data_file_path, out global_Optima, out rotation_Matrix, ref dimension);
                    tasks.Lb = new double[dimension];
                    tasks.Ub = new double[dimension];
                    tasks.Global_Optima = new double[dimension];
                    tasks.Weight_Matrix = new double[dimension, dimension];
                    for (int i = 0; i < dimension; i++)
                    {
                        tasks.Lb[i] = -50;
                        tasks.Ub[i] = 50;
                        tasks.Global_Optima[i] = global_Optima[i];
                        for (int j = 0; j < dimension; j++)
                        {
                            tasks.Weight_Matrix[i, j] = rotation_Matrix[i, j];
                        }
                    }
                    tasks.Dims = dimension;
                    #endregion
                    break;
                case "Rastrigin":
                    #region rastrigin
                    read_Data_For_Real_Functions(tasks.Data_file_path, out global_Optima, out rotation_Matrix, ref dimension);
                    tasks.Lb = new double[dimension];
                    tasks.Ub = new double[dimension];
                    tasks.Global_Optima = new double[dimension];
                    tasks.Weight_Matrix = new double[dimension, dimension];
                    for (int i = 0; i < dimension; i++)
                    {
                        tasks.Lb[i] = -50;
                        tasks.Ub[i] = 50;
                        tasks.Global_Optima[i] = global_Optima[i];
                        for (int j = 0; j < dimension; j++)
                        {
                            tasks.Weight_Matrix[i, j] = rotation_Matrix[i, j];
                        }
                    }
                    tasks.Dims = dimension;
                    #endregion
                    break;
                case "Griewank":
                    #region griewank
                    read_Data_For_Real_Functions(tasks.Data_file_path, out global_Optima, out rotation_Matrix, ref dimension);
                    tasks.Lb = new double[dimension];
                    tasks.Ub = new double[dimension];
                    tasks.Global_Optima = new double[dimension];
                    tasks.Weight_Matrix = new double[dimension, dimension];
                    for (int i = 0; i < dimension; i++)
                    {
                        tasks.Lb[i] = -100;
                        tasks.Ub[i] = 100;
                        tasks.Global_Optima[i] = global_Optima[i];
                        for (int j = 0; j < dimension; j++)
                        {
                            tasks.Weight_Matrix[i, j] = rotation_Matrix[i, j];
                        }
                    }
                    tasks.Dims = dimension;
                    #endregion
                    break;
                case "Weierstrass":
                    #region weierstrass
                    read_Data_For_Real_Functions(tasks.Data_file_path, out global_Optima, out rotation_Matrix, ref dimension);
                    tasks.Lb = new double[dimension];
                    tasks.Ub = new double[dimension];
                    tasks.Global_Optima = new double[dimension];
                    tasks.Weight_Matrix = new double[dimension, dimension];
                    for (int i = 0; i < dimension; i++)
                    {
                        tasks.Lb[i] = -0.5f;
                        tasks.Ub[i] = 0.5f;
                        tasks.Global_Optima[i] = global_Optima[i];
                        for (int j = 0; j < dimension; j++)
                        {
                            tasks.Weight_Matrix[i, j] = rotation_Matrix[i, j];
                        }
                    }
                    tasks.Dims = dimension;
                    #endregion
                    break;
                case "Schwefel":
                    #region schwefel
                    read_Data_For_Real_Functions(tasks.Data_file_path, out global_Optima, out rotation_Matrix, ref dimension);
                    tasks.Lb = new double[dimension];
                    tasks.Ub = new double[dimension];
                    tasks.Global_Optima = new double[dimension];
                    tasks.Weight_Matrix = new double[dimension, dimension];
                    for (int i = 0; i < dimension; i++)
                    {
                        tasks.Lb[i] = -500;
                        tasks.Ub[i] = 500;
                        tasks.Global_Optima[i] = global_Optima[i];
                        for (int j = 0; j < dimension; j++)
                        {
                            tasks.Weight_Matrix[i, j] = rotation_Matrix[i, j];
                        }
                    }
                    tasks.Dims = dimension;
                    #endregion
                    break;
                case "TSP":
                    #region TSP
                    tasks.Weight_Matrix = read_TSP_Instance(tasks.Data_file_path, out dimension);
                    tasks.Dims = dimension;
                    #endregion
                    break;
                case "PRUFER_CODE":
                    #region Prufer_Tree
                    tasks.Weight_Matrix = read_TSP_Instance(tasks.Data_file_path, out dimension);
                    tasks.Dims = dimension - 2;
                    #endregion
                    break;
                case "BLOB_CODE":
                     #region BLOB_CODE
                    tasks.Weight_Matrix = read_TSP_Instance(tasks.Data_file_path, out dimension);
                    tasks.Dims = dimension - 2;
                    #endregion
                    break;
                case "EDGES_SET":
                case "EDGES_SET_NO_DECODING":
                case "EDGES_SET_VERTEX_IN_SUBGRAPH":
                    #region EDGES_SET
                    tasks.Weight_Matrix = read_TSP_Instance(tasks.Data_file_path, out dimension);
                    tasks.Dims = dimension;
                    #endregion
                    break;
                case "CLUSTERED_TREE":
                case "MAX_GROUP_IN_CLUSTERED_TREE":
                    #region CLUSTERED_TREE
                    double[,] weight_Matrix;
                    int[][] vertexInCluster;
                    int numCluster;
                    int sourceVertex;

                    read_File_Clusted_Tree(tasks.Data_file_path, out weight_Matrix, out vertexInCluster, out dimension, out numCluster, out sourceVertex);
                    tasks.Dims = dimension;
                    tasks.Num_Cluster = numCluster;
                    tasks.Source_Vertex = sourceVertex;
                    tasks.Weight_Matrix = new double[dimension, dimension];
                    tasks.Vertex_In_Cluster = new int[numCluster][];
                    for (int i = 0; i < dimension; i ++)
                    {
                        for(int j = 0; j < dimension; j++)
                        {
                            tasks.Weight_Matrix[i, j] = weight_Matrix[i, j];
                        }
                    }
                    for (int i = 0; i < numCluster; i++)
                    {
                        tasks.Vertex_In_Cluster[i] = new int[(vertexInCluster[i]).Length];
                        for (int j = 0; j < (vertexInCluster[i]).Length; j++)
                        {
                            tasks.Vertex_In_Cluster[i][j] = vertexInCluster[i][j];
                        }
                    }


                    #endregion
                    break;
            }
        }

        public string[] read_Parameters_File(string file_Name)
        {
            if(!File.Exists(file_Name))
            {
                Console.WriteLine(string.Format("{0} do not exist", file_Name));
                return null;
            }
            return DAL_rwFile.read_Parameters_File(file_Name);
        }

        //Ghi ket qua toi uu ra file
        public void write_Results(string file_Name, int seed, Tasks tsk, int idx_task, Chromosome best_Ind, string running_Time)
        {
            Evaluate eval_class = new Evaluate();
            int[] tour_Decode;
            int[,] tree_Solution;
            int[] prufer_Tour_Decode;
            int[,] opt_tree;
            switch (tsk.Function_Cost_Name)
            {
                case "Sphere":
                case "Rosenbrok":
                case "Ackley":
                case "Rastrigin":
                case "Griewank":
                case "Weierstrass":
                case "Schwefel":
                    write_Solution_to_File(file_Name, seed, best_Ind.Rnvec, tsk.Dims, best_Ind.Obj_value[idx_task]);
                    break;
                case "TSP":
                    #region TSP
                    tour_Decode = eval_class.decoding_TSP(best_Ind.Invec, tsk.MaxDims, tsk.Dims);
                    write_TSP_Solution_to_File(file_Name, seed, tour_Decode, tsk.Dims, best_Ind.Obj_value[idx_task]);
                    #endregion
                    break;
                case "PRUFER_CODE":
                    #region PRUFER_CODE
                    tour_Decode = eval_class.decoding_MFO_Prufer_Simple(best_Ind.Invec, tsk.MaxDims, tsk.Dims);
                    int[,] edge_Weight = eval_class.convert_Prufer_to_Tree(tour_Decode, tsk.Dims + 2);
                    write_Opt_Solution_File(file_Name, seed, edge_Weight, tsk.Dims + 2, best_Ind.Obj_value[idx_task], running_Time, false);
                    draw_Plot_in_Matlab(Path.GetDirectoryName(file_Name) + @"\" + Path.GetFileNameWithoutExtension(file_Name) + @".m", tsk.Dims + 2, edge_Weight);
                    #endregion
                    break;
                case "BLOB_CODE":
                    #region BLOB_CODE
                    int[,] tree_temp = new int[(tsk.Dims + 2) + 1, (tsk.Dims + 2) + 1];
                    int[] blod_arr = new int[(tsk.Dims + 2) + 1];
                    blod_arr[0] = -1; //A Blob string B = (b2, b3, . . . , bn−1) ∈ Cn.
                    blod_arr[1] = -1; //Blod code: 1 -> n
                    blod_arr[ tsk.Dims + 2] = -1;
                    prufer_Tour_Decode = eval_class.decoding_MFO_Prufer_Simple(best_Ind.Invec, tsk.MaxDims, tsk.Dims);

                    for (int i = 2; i <= (tsk.Dims + 2) - 1; i++)
                    {
                        blod_arr[i] = prufer_Tour_Decode[i - 2] + 1;
                    }
                    tree_temp = eval_class.convert_Blod_to_Tree(blod_arr, (tsk.Dims + 2));
                    tree_Solution = new int[tsk.Dims + 2, tsk.Dims + 2];

                    for (int i = 0; i < (tsk.Dims + 2); i++)
                    {
                        for (int j = 0; j < (tsk.Dims + 2); j++)
                        {
                            tree_Solution[i, j] = tree_temp[i + 1, j + 1];
                        }
                    }
                    write_Opt_Solution_File(file_Name, seed, tree_Solution, tsk.Dims + 2, best_Ind.Obj_value[idx_task], running_Time, false);
                    draw_Plot_in_Matlab(Path.GetDirectoryName(file_Name) + @"\" + Path.GetFileNameWithoutExtension(file_Name) + @".m", tsk.Dims + 2, tree_Solution);
                    #endregion
                    break;
                case "EDGES_SET":
                case "EDGES_SET_VERTEX_IN_SUBGRAPH":
                    #region EDGES_SET
                    //tree_Solution = new int[tsk.MaxDims, tsk.MaxDims];
                    //for (int i = 0; i < tsk.MaxDims; i++)
                    //{
                    //    for (int j = 0; j < tsk.MaxDims; j++)
                    //    {
                    //        tree_Solution[i, j] = (int)best_Ind.Edges_Matrix[i, j];
                    //    }
                    //}
                    //draw_Plot_in_Matlab(Path.GetDirectoryName(file_Name) + @"\" + "AAAA" + tsk.Dims.ToString() + @".m", tsk.MaxDims, tree_Solution);

                    tree_Solution = eval_class.decoding_MFO_Edge_Set_1(best_Ind.Edges_Matrix, tsk.MaxDims, tsk.Dims);

                    write_Opt_Solution_File(file_Name, seed, tree_Solution, tsk.Dims, best_Ind.Obj_value[idx_task], running_Time, false);
                    draw_Plot_in_Matlab(Path.GetDirectoryName(file_Name) + @"\" + Path.GetFileNameWithoutExtension(file_Name) + @".m", tsk.Dims, tree_Solution);
                   
                    #endregion
                    break;
                case "CLUSTERED_TREE":
                    #region CLUSTERED_TREE
                    tree_Solution = new int[tsk.MaxDims, tsk.MaxDims];
                    for (int i = 0; i < tsk.MaxDims; i++)
                    {
                        for (int j = 0; j < tsk.MaxDims; j++)
                        {
                            tree_Solution[i, j] = (int)best_Ind.Edges_Matrix[i, j];
                        }
                    }
                    //draw_Plot_in_Matlab(Path.GetDirectoryName(file_Name) + @"\" + "AAAA" + tsk.Dims.ToString() + @".m", tsk.MaxDims, tree_Solution);

                    //?????????????????Decoding
                    //tree_Solution = eval_class.decoding_MFO_Edge_Set_1(best_Ind.Edges_Matrix, tsk.MaxDims, tsk.Dims);

                    write_Opt_Solution_File(file_Name, seed, tree_Solution, tsk.Dims, best_Ind.Obj_value[idx_task], running_Time, false);
                    draw_Plot_in_Matlab(Path.GetDirectoryName(file_Name) + @"\" + Path.GetFileNameWithoutExtension(file_Name) + @".m", tsk.Dims, tree_Solution);

                    #endregion
                    break;
                case "MAX_GROUP_IN_CLUSTERED_TREE":
                #region MAX_GROUP_IN_CLUSTERED_TREE
                    int idx_Max_Clus = 0;
                    int num_Vertex_In_Cluster = tsk.Vertex_In_Cluster[0].Length;
                    for (int i = 1; i < tsk.Num_Cluster; i++)
                    {
                        if (tsk.Vertex_In_Cluster[i].Length > num_Vertex_In_Cluster)
                        {
                            num_Vertex_In_Cluster = tsk.Vertex_In_Cluster[i].Length;
                            idx_Max_Clus = i;
                        }
                    }
                    double[,] weight_Cluster_Matrix = init_Chromo.create_Weight_Matrix_for_Cluster(tsk.MaxDims, num_Vertex_In_Cluster, best_Ind.Edges_Matrix,
                        tsk.Vertex_In_Cluster[idx_Max_Clus]);

                    tree_Solution = new int[num_Vertex_In_Cluster, num_Vertex_In_Cluster];
                    for (int i = 0; i < num_Vertex_In_Cluster; i++)
                    {
                        for (int j = 0; j < num_Vertex_In_Cluster; j++)
                        {
                            tree_Solution[i, j] = (int)weight_Cluster_Matrix[i, j];
                        }
                    }

                    write_Opt_Solution_File(file_Name, seed, tree_Solution, num_Vertex_In_Cluster, best_Ind.Obj_value[idx_task], running_Time, false);
                    draw_Plot_in_Matlab(Path.GetDirectoryName(file_Name) + @"\" + Path.GetFileNameWithoutExtension(file_Name) + @".m", num_Vertex_In_Cluster, tree_Solution);

                #endregion
                    break;

            }
        }

        //Ghi ket qua tot nhat cua cac task o moi the he
        public void write_Results_In_Generations(string file_Name, List<double[]> ev_Best_Fitness, int num_Tasks)
        {
            DAL_rwFile.write_Results_In_Generations(file_Name, ev_Best_Fitness, num_Tasks);
        }

        //Ghi kết quả list ra file với cột tiêu đề là tham số: column_Title
        public void write_Parameters_In_Generations(string file_Name, List<double> lst_Para, string column_Title)
        {
            DAL_rwFile.write_Parameters_In_Generations(file_Name, lst_Para, column_Title);
        }

        public void write_Optimal_of_One_Max_Tree(string file_Name, int num_Vertex, int[,] weight_Matrix)
        {
            DAL_rwFile.write_Optimal_of_One_Max_Tree(file_Name, num_Vertex, weight_Matrix);
        }


         //Tạo ra *.m file để vẽ đồ thị là luu thành ảnh trong matlbe
        public void draw_Plot_in_Matlab(string file_Name, int num_Vertex, int[,] weight_Matrix)
        {
            DAL_rwFile.draw_Plot_in_Matlab(file_Name, num_Vertex, weight_Matrix);
        }

        #endregion
        

        #region Clutered Tree
        public void create_Clustered_Intance(string tspFileName, string clusteredTreeFileName, int[] numVertexInGroup, Random rnd)
        {
            DAL_rwFile.create_Clustered_Intance(tspFileName, clusteredTreeFileName, numVertexInGroup, rnd);
        }

        public void read_File_Clusted_Tree(string filePath, out double[,] weightMatrix, out int[][] vertexInCluster, out int numVertex, out int numCluster, out int sourceVertex)
        {
            DAL_rwFile.read_File_Clusted_Tree(filePath, out weightMatrix, out vertexInCluster, out numVertex, out numCluster, out sourceVertex);
        }

        /*********************************************************************************************************************************************
         *  Tao file *.par tu file source_file;
         *  Ten cua Final_File la Ten cua begin_Of_Name_Of_Final_File va file du lieu trong file_data
         *  
         *  Parmaters: alg(0); source_File(1); file_data(2); begin_Of_Name_Of_Final_File(3)_
         *  
         ********************************************************************************************************************************************/
        public void creatate_Parameter_File_For_Cluster_Tree_GA(string sourceFile, string Data_File, string begin_Name)
        {
            DAL_rwFile.creatate_Parameter_File_For_Cluster_Tree_GA(sourceFile, Data_File, begin_Name);
        }

        /*********************************************************************************************************************************************
         *  Tạo bản sao của file *.PAR cho thuật toán Clustered Tree Problem Từ file *.PAR sẽ thay thế instance bằng instance khác được lấy từ danh sách file nguồn: 
         *  
         *  Parmaters: alg(0); source_File(1); file_data(2); begin_Of_Name_Of_Final_File(3)_
         *  
         ********************************************************************************************************************************************/
        public void creatate_Parameter_File_For_Cluster_Tree_MFO(string sourceFile, string Data_File, string begin_Name)
        {
            DAL_rwFile.creatate_Parameter_File_For_Cluster_Tree_MFO(sourceFile, Data_File, begin_Name);
        }

        public void create_Tree_Instance(string file_Name, int num_Vertex,  double[,] weight_Matrix)
        {
            DAL_rwFile.create_Tree_Instance(file_Name, num_Vertex, weight_Matrix);
        }

        public void create_Clustered_Tree_Instances_From_Clustered_TSP(string in_File_CTSP, string out_File)
        {
            DAL_rwFile.create_Clustered_Tree_Instances_From_Clustered_TSP(in_File_CTSP, out_File);
        }

        public void create_NON_Euclidean_Clustered_Tree_Instances_From_Clustered_TSP(string in_File_CTSP, int lowNum, int upperNum, string out_File)
        {
            DAL_rwFile.create_NON_Euclidean_Clustered_Tree_Instances_From_Clustered_TSP(in_File_CTSP, lowNum, upperNum, out_File);
        }

        #endregion
        

        #region Tổng hợp kết quả
        //Thong ke ket qua tu cac file *.tour va *.opt trong thu muc
        public void statistic_Results(string director_Path, string extention_File, string out_File)
        {
            DAL_rwFile.statistic_Results(director_Path, extention_File, out_File);
        }

        /*********************************************************************************************************************************************
        *  Thống kê các instance của bài clustered tree, cluster lớn nhất có bao nhiêu đỉnh, chiếm bao nhiêu % số đỉnh
        *  Ghi ra: ten_instance     id_largest_Cluster      num_Vertex_in_Largest_Cluster       percentage
        *  Parmaters: alg(0); list_Instances(1); statistic_File(2);
        *  
        ********************************************************************************************************************************************/
        public void count_Number_Of_Largest_Cluster_in_Instance(string list_Instances, string statistic_File)
        {
            DAL_rwFile.count_Number_Of_Largest_Cluster_in_Instance(list_Instances, statistic_File);
        }

        #endregion

    }
}
