using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO;
using BLL;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;

namespace MFO
{
    public class MainClass
    {
        ReadWriteFile ioFile = new ReadWriteFile();
        MFEA mfea_instance = new MFEA();
        GA ga_instance = new GA();
        Clustered_Tree clustered_Tree = new Clustered_Tree();
        Initialize_Chromosome init_Chrom = new Initialize_Chromosome();
        Evaluate eva = new Evaluate();
        private string getSubStringToEnd(string str, string strDeter)
        {
            return str.Substring(str.LastIndexOf(strDeter) + 1, str.Length - str.LastIndexOf(strDeter) - 1);
        }
        private void MFO_All_Method(string[] args)
        {
            if ((args == null) || (args.Length < 1))
            {
                Console.WriteLine("Khong co tham so");
            }
            else
            {
                Random ran;
                int numCluster;
                int numCity;
                int depot;
                string nameInstance;
                string folderResults;

                string saveFile = "";
                string tempPath = "";

                int[] lhkTour;
                double bestCost = 0.0f;
                int[] tour;
                double[] mi;
                double m;
                double[,] tmpWeight;
                string[] args_In;
                string str_Path;

                Stopwatch stGA = new Stopwatch();
                //string alg_Index = getSubStringToEnd(args[1].Trim(), "=");
                switch (Convert.ToInt32(args[0]))
                {
                    case 1:
                        #region MFO
                        /*********************************************************************************************************************************************
                         *  MFO: giai_thuat(0); File_Parameters(1);
                         *  num_KP_TASK(k);...
                         *                       
                         ********************************************************************************************************************************************/

                        //if (int.Parse(args[1]) == 1)
                        //{
                        //    Console.WriteLine("-----------------------------------------------------------------------------");
                        //    Console.WriteLine("| Algorithm |      Instances            | Seed |  Geners  |        Best Cost|");
                        //    Console.WriteLine("-----------------------------------------------------------------------------");
                        //}

                        //Console.Write(String.Format("| {0,-9} | {1,-25} | {2,4} |", "Real_MFO", "", args[1]));

                        args_In = ioFile.read_Parameters_File(args[1]);
                        mfea_instance.MFEA_Alg(args_In);
                        break;
                        #endregion

                    case 2:
                        #region GA
                        /*********************************************************************************************************************************************
                         * MFO: giai_thuat(0); seed(1); popSize(2); numGeneration(3); rmp(4); mutation_rate(5); lambda(6); selection_pressure(7); num_TASKS(8);...<problem index>-<nameInstance>;
                         *  MFO: giai_thuat(0); seed(1); popSize(2); numGeneration(3); rmp(4); lambda(5); num_TSP_TASK(6);...<nameInstance>;
                         *  num_KP_TASK(k);...
                         *                       
                         ********************************************************************************************************************************************/

                        args_In = ioFile.read_Parameters_File(args[1]);
                        ga_instance.GA_Alg(args_In);
                        break;
                        #endregion
                    
                    case 3:
                    case 4:
                        #region Clustered_Tree
                        /*********************************************************************************************************************************************
                         * Giải thuật gần đúng cho bài toán Cluster Tree  trong bài báo: M. D’Emidio, L. Forlizzi, D. Frigioni, S. Leucci, and G. Proietti, 
                         * “On the Clustered Shortest-Path Tree Problem,” ICTCS 2016, p. 263, 2016.
                         * 
                         *  Parametes: Giai_thuat(0); Input_File_Name(1); 
                         *  Alg = 3: BFS+Dijstra
                         *  Alg = 4: Prim + Prim
                         ********************************************************************************************************************************************/
                        clustered_Tree.Heuristic_Clustered_Tree(args);
                       
                        break;
                        #endregion

                    case 100:
                        #region Test
                        /*********************************************************************************************************************************************
                         * 
                         * Test
                         *                       
                         ********************************************************************************************************************************************/

                        

                        int[] blob_Arr = new int[18] { 16, 4, 6, 2, 12, 0, 7, 0, 19, 3, 5, 17, 3, 16, 6, 12, 11, 9 };
                        int[] blob_Arr_1 = new int[21] {-1, -1, 17, 5, 7, 3, 13, 1, 8, 1, 20, 4, 6, 18, 4, 17, 7, 13, 12, 10, -1};
                        int[,] tree = new int[21, 21];
                        int[,] tree_1 = new int[20, 20];

                      
                        break;
                        #endregion

                    case 101:
                        #region TSP Test
                        /*********************************************************************************************************************************************
                         *   MFO: giai_thuat(0); seed(1); popSize(2); numGeneration(3); rmp(4); mutation_rate(5); lambda(6); selection_pressure(7); 
                         *  type_encoding_in_union_search_space(8); num_TASKS(8);...<problem index>-<nameInstance>;
                         *                       
                         ********************************************************************************************************************************************/

                        if (int.Parse(args[1]) == 1)
                        {
                            Console.WriteLine("-----------------------------------------------------------------------------");
                            Console.WriteLine("| Algorithm |      Instances            | Seed |  Geners  |        Best Cost|");
                            Console.WriteLine("-----------------------------------------------------------------------------");
                        }
                        Console.Write(String.Format("| {0,-9} | {1,-25} | {2,4} |", "Real_MFO", "", args[1]));

                        mfea_instance.MFEA_Alg(args);

                        #endregion
                        break;

                    case 102:
                        #region TSP Test
                        /*********************************************************************************************************************************************
                         *  Tạo ngẫu nhiên optimal solution cho bài toán ONE MAX TREE
                         *  MFO: giai_thuat(0); File_Name(1); Number_of_Vertex(2); 
                         *                       
                         ********************************************************************************************************************************************/
                        Initialize_Chromosome init_Chrome = new Initialize_Chromosome();
                        init_Chrome.create_Optimal_of_One_Max_Tree(args[1], Convert.ToInt32(args[2]));

                        #endregion
                        break;

                    case 103:
                        #region Chuyen do thi bieu dien duoi dang ma tran sang lenh Maplab
                        /*********************************************************************************************************************************************
                         * Đọc dữ liệu từ file input của bài toán One-Max-Tree: *.in -> vẽ đồ thị của cây của ma trận trong số
                         * giai_thuat(0); File_Name(1); Number_of_Vertex(2); => 
                         *                       
                         ********************************************************************************************************************************************/

                        tmpWeight = ioFile.read_TSP_Instance(args[1], out numCity);
                        int[,] opt_tree = new int[numCity, numCity];
                        for (int i = 0; i < numCity; i++)
                        {
                            for (int j = 0; j < numCity; j++)
                            {
                                opt_tree[i, j] = (int)tmpWeight[i, j];
                            }
                        }

                        ioFile.draw_Plot_in_Matlab(Path.GetFileNameWithoutExtension(args[1]) + @".m", numCity, opt_tree);
                        #endregion
                        break;

                    case 104:
                        #region Tao instance cho bai toan CLUSTERED TREEE
                        /*********************************************************************************************************************************************
                         * Tao instance cho Clustered tree tu file TSP instance
                         * Parameters: giai_thuat(0); ten_file_TSP(1);  ten_file_HTSP(2); seed(3); so_group(4); cac_gia_tri_group(...)
                         *                       
                         ********************************************************************************************************************************************/
                        ran = new Random(Convert.ToInt32(args[3]));
                        int[] numCitiesInCluster = new int[Convert.ToInt32(args[4])];
                        for(int i = 0; i < Convert.ToInt32(args[4]); i++)
                        {
                            numCitiesInCluster[i] = Convert.ToInt32(args[4 + i + 1]);
                        }
                        
                        ioFile.create_Clustered_Intance(args[1], args[2], numCitiesInCluster, ran);
                        #endregion
                        break;

                    case 105:
                        #region Tao instance cho bai toan CLUSTERED TREE từ bài toán Clustered TSP 
                        /*********************************************************************************************************************************************
                         * Tao instance cho Clustered tree tu file Cluster TSP instance bằng cách thêm đỉnh nguồn
                         * Parameters: giai_thuat(0); ten_file_CTSP(1); Path_for_Save_Clustered_Tree_File(2); 
                         *                        
                         ********************************************************************************************************************************************/
                        str_Path = Path.GetFileName(args[1]);
                        if(string.IsNullOrWhiteSpace(args[2].Trim()))
                        {
                            str_Path = Path.GetFileNameWithoutExtension(str_Path) + @".clt";
                        }
                        else
                        {
                            if (!Directory.Exists(args[2]))
                            {
                                Directory.CreateDirectory(args[2]);
                            }
                            str_Path = args[2].Trim() + @"\" + Path.GetFileNameWithoutExtension(str_Path) + @".clt";
                        }

                        ioFile.create_Clustered_Tree_Instances_From_Clustered_TSP(args[1], str_Path);

                        #endregion
                        break;

                    case 106:
                        #region Tao file *.par tu file source_file cho GA giai bai toan Clustered Tree
                        
                        /*********************************************************************************************************************************************
                         *  Tao file *.par tu file source_file;
                         *  Ten cua Final_File la Ten cua begin_Of_Name_Of_Final_File va file du lieu trong file_data
                         *  
                         *  Parmaters: alg(0); source_File(1); file_data(2); begin_Of_Name_Of_Final_File(3)
                         ********************************************************************************************************************************************/

                        ioFile.creatate_Parameter_File_For_Cluster_Tree_GA(args[1], args[2], args[3]);

                        break;
                        #endregion

                    case 107:
                        #region Tao file *.par tu file source_file cho MFO giai bai toan Clustered Tree

                        /*********************************************************************************************************************************************
                         *  Tao file *.par tu file source_file;
                         *  Ten cua Final_File la Ten cua begin_Of_Name_Of_Final_File va file du lieu trong file_data
                         *  
                         *  Parmaters: alg(0); source_File(1); file_data(2); begin_Of_Name_Of_Final_File(3)
                         ********************************************************************************************************************************************/

                        ioFile.creatate_Parameter_File_For_Cluster_Tree_MFO(args[1], args[2], args[3]);

                        break;
                        #endregion

                    case 108:
                        #region Tạo ma trận trọng số cho cluster có kích thước lớn nhất từ dữ liệu bài toán Clustered Tree 
                        /*********************************************************************************************************************************************
                         * Parameters: alg(0); source_File(1); out_File(2); 
                         *                       
                         ********************************************************************************************************************************************/
                        double[,] weight_Matrix;
                        int[][] vertexInCluster;
                        int sourceVertex;

                        ioFile.read_File_Clusted_Tree(args[1], out weight_Matrix, out vertexInCluster, out numCity, out numCluster, out sourceVertex);
                        int idx_Max_Clus = 0;
                        int num_Vertex_In_Cluster = vertexInCluster[0].Length;
                        for (int i = 1; i < numCluster; i++)
                        {
                            if (vertexInCluster[i].Length > num_Vertex_In_Cluster)
                            {
                                num_Vertex_In_Cluster = vertexInCluster[i].Length;
                                idx_Max_Clus = i;
                            }
                        }
                        double[,] weight_Cluster_Matrix = init_Chrom.create_Weight_Matrix_for_Cluster(numCity, num_Vertex_In_Cluster, weight_Matrix, vertexInCluster[idx_Max_Clus]);
                        ioFile.create_Tree_Instance(args[2], num_Vertex_In_Cluster, weight_Cluster_Matrix);

                        break;
                        #endregion

                    case 109:
                        #region Tạo ma trận trọng số tu bai toan cluster de chay bai toan One-max tree cung
                        /*********************************************************************************************************************************************
                         * Parameters: alg(0); source_File(1); out_File(2); 
                         * Example: 107 4Eil10.clt 4Eil10.in                
                         ********************************************************************************************************************************************/
                        double[,] weight_Matrix_1;
                        int[][] vertexInCluster_1;
                        int sourceVertex_1;

                        ioFile.read_File_Clusted_Tree(args[1], out weight_Matrix_1, out vertexInCluster_1, out numCity, out numCluster, out sourceVertex_1);

                        ioFile.create_Tree_Instance(args[2], numCity, weight_Matrix_1);

                        break;
                        #endregion

                    case 110:
                        #region Tong hop ket qua tu file *.opt; *.tour tu cac file trong thu muc (sử dụng cho thuật toán PRIM_PRIM (AAL))
                       /*********************************************************************************************************************************************
                        * Parameters: alg(0); Thu_muc_chua_ket_qua(1); Phan_mo_rong_cua_cac_file_ket_qua(2); ten_file_tong_hop(3) 
                        * Example: 107 4Eil10.clt 4Eil10.in                
                        ********************************************************************************************************************************************/
                        Console.WriteLine("Dang doc du lieu tu thu muc: ");
                        Console.WriteLine(args[1]);

                        ioFile.statistic_Results(args[1], args[2], args[3]);
                        Console.WriteLine();
                        #endregion
                        break;

                    case 111:
                        #region Thống kê các instance của bài clustered tree, cluster lớn nhất có bao nhiêu đỉnh, chiếm bao nhiêu % số đỉnh
                        /*********************************************************************************************************************************************
                        * Parameters: alg(0); File_Danh_Sach_Instances(1); File_Thong_Ke(2); ten_file_tong_hop(3) 
                        * Example: 107 4Eil10.clt 4Eil10.in                
                        ********************************************************************************************************************************************/

                        ioFile.count_Number_Of_Largest_Cluster_in_Instance(args[1], args[2]);

                        #endregion
                    break;

                    case 112:
                        #region Tao instance NON_EUCLIDEAN cho bai toan CLUSTERED TREE từ bài toán Clustered TSP
                        /*********************************************************************************************************************************************
                         * Tao instance NON_EUCLIDEAN cho Clustered tree tu file Cluster TSP instance bằng cách thêm đỉnh nguồn & tạo ma trân trọng số mới
                         * Parameters: giai_thuat(0); ten_file_CTSP(1); lowerNum(2); uppperNum(3); Path(Directory)_for_Save_Clustered_Tree_File(4); 
                         *                        
                         ********************************************************************************************************************************************/
                        str_Path = Path.GetFileName(args[1]);
                        if (string.IsNullOrWhiteSpace(args[2].Trim()))
                        {
                            str_Path = Path.GetFileNameWithoutExtension(str_Path) + @".clt";
                        }
                        else
                        {
                            if (!Directory.Exists(args[4]))
                            {
                                Directory.CreateDirectory(args[4]);
                            }
                            str_Path = args[4].Trim() + @"\" + Path.GetFileNameWithoutExtension(str_Path) + @".clt";
                        }

                        ioFile.create_NON_Euclidean_Clustered_Tree_Instances_From_Clustered_TSP(args[1], Convert.ToInt32(args[2]), Convert.ToInt32(args[3]), str_Path);

                        #endregion
                    break;

                    case 113:
                        #region Tính lại cost optimal solution của bài toán cluster shortest-path tree theo hàm mục tiêu đã sửa
                        /*********************************************************************************************************************************************
                        * Tình lại cost của lời giải bài toán cluster shortest-path tree theo hàm mục tiêu chuẩn
                        * Parameters: giai_thuat(0); file_Optmal_Solution(1); ;file_Out(3); 
                        *                        
                        ********************************************************************************************************************************************/
                        recompute_Cost_Opt_Solution_In_Folder(args[1], args[2], args[3]);

                        #endregion
                    break;

                    case 186:
                    #region aaa
                        //arg[1]: *.clt
                        //arg[2]: ma tran canh cua optimal solution
                        int dimension;
                        ioFile.read_File_Clusted_Tree(args[1], out weight_Matrix, out vertexInCluster, out dimension, out numCluster, out sourceVertex);
                        int[,] edge_matrix;
                        List<int> path_1;
                        double path_Length = 0;

                        read_Matrix_2D(args[2], dimension, out edge_matrix);

                        double[,] temp_Matrix = new double[dimension, dimension];
                         for (int i = 0; i < dimension; i++)
                        {
                            for (int j = 0; j < dimension; j++)
                            {
                                if ((edge_matrix[i, j] > 0) && (edge_matrix[i, j] < double.MaxValue))//neu co canh
                                {
                                    temp_Matrix[i, j] = weight_Matrix[i, j];
                                }
                                else
                                {
                                    temp_Matrix[i, j] = 0;
                                }
                            }
                        }


                        Graph_Method graph_Method_Class = new Graph_Method();
                        int[] pre = graph_Method_Class.dijkstra(temp_Matrix, dimension, sourceVertex);
                        for (int i = 0; i < dimension; i++)
                        {
                            path_1 = graph_Method_Class.print_Path(sourceVertex, i, pre);
                            for (int l = path_1.Count - 1; l > 0; l--)
                            {
                                path_Length = path_Length + weight_Matrix[path_1[l], path_1[l - 1]];
                            }
                        }

                        double aaa = evaluation_Tree(weight_Matrix, edge_matrix, dimension, sourceVertex);


                        Console.WriteLine("Dijstra: " + path_Length);
                        Console.WriteLine("Mo: " + aaa);

                        Console.ReadKey();
                        //return path_Length;
                   
                    #endregion
                    break;
                }
                Console.WriteLine();
            }
        }

        public void run(string[] args)
        {
            if ((args == null) || (args.Length < 1))
            {
                Console.WriteLine("Khong co tham so");
            }

            MFO_All_Method(args);
        }//public void run(int idx)


        public void read_Matrix_2D(string filePath, int Dimention, out int[,] edge_Weight)
        {
            StreamReader doc = new StreamReader(filePath);
            int maxIndex = 1;
            string line = "";
            string pattern = " ";
            Regex myRegex = new Regex(pattern);
            Regex rg = new Regex(@"\s+");

            maxIndex = Dimention;
            edge_Weight = new int[maxIndex, maxIndex];

            int num_Vertex = maxIndex;

            bool read_Vertex_Wegith = false;
            int index = 0;
            while ((line = doc.ReadLine()) != null)
            {
                line = line.Trim();
                line = rg.Replace(line, " ");

                if (string.IsNullOrWhiteSpace(line) || (string.IsNullOrEmpty(line)) || (line.Equals("EOF")))
                {
                    continue;
                }

                if (line.IndexOf("VERTEX_WEIGHT_SECTION") != -1)
                {
                    read_Vertex_Wegith = true;
                    continue;
                }
                             
                string[] temp = myRegex.Split(line);
                for (int i = 0; i < maxIndex; i++)//temp[0] la chi so cua cluster, temp[n-1] = -1
                {
                    edge_Weight[index, i] = Convert.ToInt32(temp[i]);
                }
                index++;
            }
            doc.Dispose();
            doc.Close();
        }


        /*********************************************************************************************************************************************
         *  Tính lai giá tri cua opt solution của bài toán cluster shortest path tree do 
         *  
         *  Parmaters: alg(0); ten_file_opt_solution(1); file_du_lieu_clt(2); file_luu_thong_tin_moi(3)
         *  
         ********************************************************************************************************************************************/
        public void recompute_Cost_Opt_Solution(string fileOptSolution, string fileClt, string outFile)
        {
            double[,] weight_Matrix;
            int[][] vertexInCluster;
            int dimension;
            int numCluster;
            int sourceVertex;

            StreamReader doc = new StreamReader(fileOptSolution);
           
            StreamWriter wr = new StreamWriter(outFile);

            ioFile.read_File_Clusted_Tree(fileClt, out weight_Matrix, out vertexInCluster, out dimension, out numCluster, out sourceVertex);

            int maxIndex = 1;
            string line = "";
            string pattern = " ";
            Regex myRegex = new Regex(pattern);
            Regex rg = new Regex(@"\s+");

            int[,] edge_Weight = new int[dimension, dimension];

            string str1 = "",
                    str2 = "";

            bool read_Vertex_Wegith = false;
            int index = 0;
            while ((line = doc.ReadLine()) != null)
            {
                line = line.Trim();
                line = rg.Replace(line, " ");

                if (string.IsNullOrWhiteSpace(line) || (string.IsNullOrEmpty(line)) || (line.Equals("EOF")))
                {
                    continue;
                }

                if (line.IndexOf("fileName") != -1)
                {
                    wr.WriteLine(line);
                    continue;
                }
                if (line.IndexOf("Seed") != -1)
                {
                    wr.WriteLine(line);
                    continue;
                }
                if (line.IndexOf("Fitness") != -1)
                {
                    //str1 = line;
                    continue;
                }
                if (line.IndexOf("Time") != -1)
                {
                    str2 = line;
                    continue;
                }

                string[] temp = myRegex.Split(line);
                for (int i = 0; i < dimension; i++)//temp[0] la chi so cua cluster, temp[n-1] = -1
                {
                    edge_Weight[index, i] = Convert.ToInt32(temp[i]);
                    //edge_Weight[i, index] = edge_Weight[index, i];
                }
                index++;
            }

            double cost = eva.compute_Cost_of_Tree(weight_Matrix, edge_Weight, dimension, sourceVertex);


            //List<int> path_1;
            //double path_Length = 0;
            //double[,] temp_Matrix = new double[dimension, dimension];
            //for (int i = 0; i < dimension; i++)
            //{
            //    for (int j = 0; j < dimension; j++)
            //    {
            //        if ((edge_Weight[i, j] > 0) && (edge_Weight[i, j] < double.MaxValue))//neu co canh
            //        {
            //            temp_Matrix[i, j] = weight_Matrix[i, j];
            //        }
            //        else
            //        {
            //            temp_Matrix[i, j] = 0;
            //        }
            //    }
            //}
            //Graph_Method graph_Method_Class = new Graph_Method();
            //int[] pre = graph_Method_Class.dijkstra(temp_Matrix, dimension, sourceVertex);
            //for (int i = 0; i < dimension; i++)
            //{
            //    path_1 = graph_Method_Class.print_Path(sourceVertex, i, pre);
            //    for (int l = path_1.Count - 1; l > 0; l--)
            //    {
            //        path_Length = path_Length + weight_Matrix[path_1[l], path_1[l - 1]];
            //    }
            //}

            wr.WriteLine("Fitness: " + cost.ToString());
            wr.WriteLine(str2);

            doc.Dispose();
            doc.Close();
            wr.Dispose();
            wr.Close();
        }


        /*********************************************************************************************************************************************
        * Tính lại giá trị của opt solution tu danh sach file
        * Parameters: giai_thuat(0); Ten_Thu_Muc_Chu_Opt_File(1); extention_File(2); file_Out(3); 
        *                        
        ********************************************************************************************************************************************/
        public void recompute_Cost_Opt_Solution_In_Folder(string folderIn, string extention_File, string folderOut)
        {
            string[] folders = Directory.GetDirectories(folderIn);
           
            string fileOut = "",
                   fileClt = "",
                   pathOut = "",
                   tmp = "";
            int pos = 0,
                pos_1 = 0;
            for (int k = 0; k < folders.Length; k++)
            {
                string[] files = Directory.GetFiles(folders[k], extention_File, SearchOption.AllDirectories);

                pos = folders[k].LastIndexOf(@"\");

                pathOut = folderOut + @"\" + folders[k].Substring(pos + 1);

                if (!Directory.Exists(pathOut))
                {
                    Directory.CreateDirectory(pathOut);
                }

                for (int i = 0; i < files.Length; i++)
                {
                    tmp = Path.GetFileNameWithoutExtension(Path.GetFileName(files[i]));
                    fileOut = pathOut + @"\" + tmp + @".opt";
                    pos = tmp.IndexOf("Instance");
                    if (pos != -1)//Dinh dang file ket qua cua GA
                    {
                        pos_1 = tmp.LastIndexOf("_");
                        if (pos_1 > pos)//loai bo  file *.opt file tong hop
                        {
                            fileClt = tmp.Substring(pos + 9, pos_1 - pos - 9 - 1) + @".clt";
                            recompute_Cost_Opt_Solution(files[i], fileClt, fileOut);
                        }

                    }
                    pos = -1;
                    pos = tmp.IndexOf("_1(");
                    if (pos != -1)//Dinh dang file ket qua cua GA
                    {
                        pos_1 = tmp.LastIndexOf("_");
                        if (pos_1 > pos)//loai bo  file *.opt file tong hop
                        {
                            fileClt = tmp.Substring(pos + 3, pos_1 - pos - 3 - 1) + @".clt";
                            recompute_Cost_Opt_Solution(files[i], fileClt, fileOut);
                        }

                    }

                    //
                }


            }



           

        }

        public double evaluation_Tree(double[,] weightMatrix, int[,] tree, int num_vertex, int startVertex)
        {
	        double[]  distances  =  new double[num_vertex];// distance between root and the others
	        double sum  = 0;
	        distances[startVertex] = 0;
	        Boolean[] mark = new Boolean[num_vertex];
            Queue queue = new Queue();
	        for( int i = 0; i < num_vertex; i ++)
            {
		        mark[i] = true;
	        }
            queue.Enqueue(startVertex);
            while (queue.Count>0)
            {
                int u = (int)queue.Dequeue();
		        mark[u] = false;
		        for( int i = 0; i < num_vertex; i++)
                {
			        if(tree[u,i] > 0 && mark[i] )
                    {
                        queue.Enqueue(i);
				        mark[i] = false;
				        distances[i] =  distances[u] + weightMatrix[u,i];
				        sum += distances[i]; 
			        }
		        }
	        }
	        return sum;
        }


    }
}
