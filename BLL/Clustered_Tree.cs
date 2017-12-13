using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DTO;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;



namespace BLL
{
    public class Clustered_Tree
    {
        Graph_Method graph_Method_Class = new Graph_Method();
        Initialize_Chromosome init_Chrome = new Initialize_Chromosome();
        ReadWriteFile ioFile = new ReadWriteFile();
        Evaluate eval_Class = new Evaluate();
        Stopwatch stGA = new Stopwatch();

        /*********************************************************************************************************************************************
        * Tìm ma trận liên kết các cluster từ cây khung
        * + Hướng 1: Chỉ tìm liên kết không quan tâm tới trọng số 
        * + Hướng 2: Tìm liên kết có lưu trọng số, và các cạnh nối giữa các cluster để thực hiện lai ghép về sau <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
        **********************************************************************************************************************************************/
        private void find_Spanning_Tree_Between_Clusters_Determine_Vertex(double[,] weight_Matrix, int num_Vertex, int num_Cluster, int[][] vertex_In_Cluster,
            out double[,] cluster_Weight, out  int[,] connect_Cluster)
        {
            cluster_Weight = new double[num_Cluster, num_Cluster];
            connect_Cluster = new int[num_Cluster, num_Cluster];

            //Khoi tao
            for (int i = 0; i < num_Cluster; i++)
            {
                for (int j = i; j < num_Cluster; j++)
                {
                    cluster_Weight[i, j] = 0;
                    cluster_Weight[j, i] = 0;
                    connect_Cluster[i, j] = -1;
                    connect_Cluster[j, i] = -1;
                }
            }

            int num_Vertex_in_Cluster_1 = 0;
            int num_Vertex_in_Cluster_2 = 0;
            for (int i = 0; i < num_Cluster; i++)
            {
                num_Vertex_in_Cluster_1 = vertex_In_Cluster[i].Length;
                for (int j = 0; j < num_Vertex_in_Cluster_1; j++)
                {
                    for (int k = i + 1; k < num_Cluster; k++)
                    {
                        num_Vertex_in_Cluster_2 = vertex_In_Cluster[k].Length;
                        for (int t = 0; t < num_Vertex_in_Cluster_2; t++)
                        {
                            if (weight_Matrix[vertex_In_Cluster[i][j], vertex_In_Cluster[k][t]] > 0)
                            {
                                cluster_Weight[i, k] = weight_Matrix[vertex_In_Cluster[i][j], vertex_In_Cluster[k][t]];
                                cluster_Weight[k, i] = cluster_Weight[i, k];
                                connect_Cluster[i, k] = vertex_In_Cluster[i][j];//dinh noi tu cluster i -> k cua cluster i
                                connect_Cluster[k, i] = vertex_In_Cluster[k][t];//dinh noi tu cluster i -> k cua cluster k
                                break;
                            }
                        }
                    }
                }
            }
        }



        /*********************************************************************************************************************************************
         * Thủ tục gần đúng tìm lời giải bài toán: Clustered Shortest-Path Tree Problem 
         * Thuật toán áp dụng DFS cho cây khung với mỗi nút là 1 cluster; sau đó áp dụng Dijstra cho các đỉnh trong cluster
         * Output: ma tran trong so bieu dien cay khung: edge_Matrix                      
         ********************************************************************************************************************************************/

        private void BFS_Dijstra_Clustered_Tree(double[,] weigh_Matrix, int num_Vertex, int num_Cluster, int[][] vertex_In_Cluster, int start_Vertex, 
            out double[,] edge_Matrix)
        {
            int[,] connect_Cluster; //Lưu các đỉnh thuộc các cluster nào được sử dụng để nối tới đỉnh khác.
            double[,] cluster_Weight;
            int[] pre;
            List<int> path_1;

            edge_Matrix = new double[num_Vertex, num_Vertex];
            for (int i = 0; i < num_Vertex; i++)
            {
                for (int j = i; j < num_Vertex; j++)
                {
                    edge_Matrix[i, j] = 0;
                }
            }

            //01. Chuyển đồ thị thành độ thị với mỗi cluster là một đỉnh
            find_Spanning_Tree_Between_Clusters_Determine_Vertex(weigh_Matrix, num_Vertex, num_Cluster, vertex_In_Cluster, out cluster_Weight, out connect_Cluster);

            //02. Áp dụng BFS cho đồ thị được tạo từ cluster
            //+ Kiểm tra xem start_Vertex thuộc cluster nào, để áp dụng BFS từ cluster đó.
            int idx_Cluster = -1;
            for (int i = 0; i < num_Cluster; i++)
            {
                if (vertex_In_Cluster[i].Contains(start_Vertex))
                {
                    idx_Cluster = i;
                    break;
                }
            }
            int[] preBFS = graph_Method_Class.BFS(cluster_Weight, num_Cluster, idx_Cluster);

            //+ Tạo đồ thị theo duyệt cây BFS
            //double[,] aaa = new double[num_Cluster, num_Cluster];
            for (int i = 0; i < num_Cluster; i++)
            {
                path_1 = graph_Method_Class.print_Path(idx_Cluster, i, preBFS);
                for (int j = path_1.Count - 1; j > 0; j--)
                {
                    //aaa[path_1[j], path_1[j - 1]] = cluster_Weight[path_1[j], path_1[j - 1]];
                    //aaa[path_1[j - 1], path_1[j]] = aaa[path_1[j], path_1[j - 1]];

                    edge_Matrix[connect_Cluster[path_1[j], path_1[j - 1]], connect_Cluster[path_1[j - 1], path_1[j]]] = cluster_Weight[path_1[j], path_1[j - 1]];
                    edge_Matrix[connect_Cluster[path_1[j - 1], path_1[j]], connect_Cluster[path_1[j], path_1[j - 1]]] = cluster_Weight[path_1[j], path_1[j - 1]];
                }
            }

            //int[,] map = new int[num_Cluster, num_Cluster];
            //for (int i = 0; i < num_Cluster; i++)
            //{
            //    for (int j = 0; j < num_Cluster; j++)
            //    {
            //        map[i, j] = (int)aaa[i, j];
            //    }
            //}
            //ioFile.draw_Plot_in_Matlab("DFS_cluster_Tree.m", num_Cluster, map);

            //02. Du dung Dijstra cho cac Cluster
            double[,] weight_Cluster_Matrix;
            double[,] spanning_Tree_of_Cluster;
            int num_Vertex_in_Cluster = 0;
            int start_Vertex_In_Cluster = -1; //Đỉnh kết nối với cluster ngoài sau khi chuyển về ma trận clustert
            int vertex_Connect_Other_Cluster = -1; //Đỉnh kết nối với ma trận ngoài
            for (int i = 0; i < num_Cluster; i++)
            {
                start_Vertex_In_Cluster = -1;
                num_Vertex_in_Cluster = vertex_In_Cluster[i].Length;
                weight_Cluster_Matrix = init_Chrome.create_Weight_Matrix_for_Cluster(num_Vertex, num_Vertex_in_Cluster, weigh_Matrix, vertex_In_Cluster[i]);
                if (vertex_In_Cluster[i].Contains(start_Vertex))//cluster nao chua dinh: start_Vertex thi se duyet distra tu dinh do
                {
                    vertex_Connect_Other_Cluster = start_Vertex;
                    //Tìm xem: start_Vertex là đỉnh thứ bao nhiều trong đồ thị sau khi chuyển cluster
                    for (int j = 0; j < num_Vertex_in_Cluster; j++)
                    {
                        if (vertex_In_Cluster[i][j] == start_Vertex)
                        {
                            start_Vertex_In_Cluster = j;
                            break;
                        }
                    }
                    pre = graph_Method_Class.dijkstra(weight_Cluster_Matrix, num_Vertex_in_Cluster, start_Vertex_In_Cluster);
                }
                else
                {
                    //Xác định đỉnh trong cluster liên kết với đỉnh ở cluster khác
                    path_1 = graph_Method_Class.print_Path(idx_Cluster, i, preBFS);
                    //++ Xác cluster cha trong đường đi tới cluster chứa đỉnh nguồn
                    int idx_Cluster_Parent = path_1[1];//path_1[0] la dinh i

                    //Xác định xem đỉnh trên là đỉnh nào trong ma trận chuyển la dinh se bat dau ap dung dijstra
                    vertex_Connect_Other_Cluster = connect_Cluster[i, path_1[1]];

                    //Tu dinh do xac dinh xem la dinh nao trong ma tran chuyen cluster
                    for (int j = 0; j < num_Vertex_in_Cluster; j++)
                    {
                        if (vertex_In_Cluster[i][j] == vertex_Connect_Other_Cluster)
                        {
                            start_Vertex_In_Cluster = j;
                            break;
                        }
                    }

                    //+ Ap dung Dijstra cho cluster
                    pre = graph_Method_Class.dijkstra(weight_Cluster_Matrix, num_Vertex_in_Cluster, start_Vertex_In_Cluster);
                }
                //Tao cay khung cho Dijstra                
                spanning_Tree_of_Cluster = new double[num_Vertex_in_Cluster, num_Vertex_in_Cluster];
                for (int ii = 0; ii < num_Vertex_in_Cluster; ii++)
                {
                    path_1 = graph_Method_Class.print_Path(start_Vertex_In_Cluster, ii, pre);
                    for (int l = path_1.Count - 1; l > 0; l--)
                    {
                        spanning_Tree_of_Cluster[path_1[l], path_1[l - 1]] = weight_Cluster_Matrix[path_1[l], path_1[l - 1]];
                        spanning_Tree_of_Cluster[path_1[l - 1], path_1[l]] = weight_Cluster_Matrix[path_1[l - 1], path_1[l]];
                    }
                }

                //map = new int[num_Vertex_in_Cluster, num_Vertex_in_Cluster];
                //for (int iii = 0; iii < num_Vertex_in_Cluster; iii++)
                //{
                //    for (int jjj = 0; jjj < num_Vertex_in_Cluster; jjj++)
                //    {
                //        map[iii, jjj] = (int)spanning_Tree_of_Cluster[iii, jjj];
                //    }
                //}
                //ioFile.draw_Plot_in_Matlab("Dijstra_for_cluster_Weight_" + i.ToString() + ".m", num_Vertex_in_Cluster, map);

                //Chuyen ra cay khung cua do thi G
                for (int k = 0; k < num_Vertex_in_Cluster; k++)
                {
                    for (int j = 0; j < num_Vertex_in_Cluster; j++)
                    {
                        if (spanning_Tree_of_Cluster[k, j] > 0)
                        {
                            edge_Matrix[vertex_In_Cluster[i][k], vertex_In_Cluster[i][j]] = spanning_Tree_of_Cluster[k, j];
                        }
                    }
                }
            }//for
        }


        public void Heuristic_Clustered_Tree(string[] args)
        {

            #region Khai bao
            string  instance_Name,
                    input_File_Data = "",
                    out_File_Data,
                    tempPath,
                    folder_Results =" ";
            int[] bstTour;
            double[,] edge_Matrix;  //Ma tran ket qua

            double[] bestFitness;

            string pattern = " ";
            Regex myRegex = new Regex(pattern);
            Regex rg = new Regex(@"\s+");
            #endregion

            #region Read parameters
            try
            {
                if (args.Length <= 0)
                {
                    Console.Write("Number of parameters is incorrect, require > 9 parameters (you provided %d).\n\n", args.Length);
                    //print_Usage();
                    Console.ReadKey();
                }

                input_File_Data = args[1].Trim();
               

            }
            catch
            {
                Console.Write("Error parsing parameters.\n\n");
                //print_Usage();
            }

            //Đọc dữ liệu từng bài toán
            //Index đầu tiên là: problem_index (kiểu bài toán) ---> index tiếp theo là:  tên file dữ liệu -> bai toan 1 - 8 thi bo qua ten file du lieu

            if (!File.Exists(input_File_Data))
            {
                Console.WriteLine("Do not exist file: " + input_File_Data);
                return;
            }
          

            #endregion

            double[,] weight_Matrix;
            int[][] vertex_In_Cluster;
            int num_Cluster,
                num_Vertex,
                source_Vertex;

            ioFile.read_File_Clusted_Tree(input_File_Data, out weight_Matrix, out vertex_In_Cluster, out num_Vertex, out num_Cluster, out source_Vertex);
            
            instance_Name = Path.GetFileName(input_File_Data);
            int alg_Index = Convert.ToInt32(args[0]);
          
            switch (alg_Index)
            {
                case 3:
                    folder_Results = @"Results\BFS_Dijstra_Clustered_Tree";
                    break;
                case 4:
                    folder_Results = @"Results\Prim_Prim_Clustered_Tree";
                    break;
            }
          
            Directory.CreateDirectory(folder_Results);

            folder_Results = folder_Results + @"\" + Path.GetFileNameWithoutExtension(instance_Name);
            Directory.CreateDirectory(folder_Results);

            //Neu da co file ket qua thi ko chay tiep
            out_File_Data = folder_Results + @"\" + Path.GetFileNameWithoutExtension(instance_Name) + @".opt";
            if (File.Exists(out_File_Data))
            {
                Console.WriteLine();
                return;
            }

            //Điều kiện dừng
       
        
          
           edge_Matrix = new double[1, 1];
           stGA.Start();
           switch(alg_Index)
           {
               case 3:
                   Console.WriteLine("-----------------------------------------------------------------------------");
                   Console.WriteLine("|   Algorithm        |           Instances            |            Best Cost|");
                   Console.WriteLine("-----------------------------------------------------------------------------");

                   Console.Write(String.Format("| {0,-18} | {1,-30} | {2, 19} |", "BFS_Dijstra", instance_Name, "" ));
                   BFS_Dijstra_Clustered_Tree(weight_Matrix, num_Vertex, num_Cluster, vertex_In_Cluster, source_Vertex, out edge_Matrix);
                   break;
               case 4:
                   Console.WriteLine("-----------------------------------------------------------------------------");
                   Console.WriteLine("|   Algorithm        |           Instances            |            Best Cost|");
                   Console.WriteLine("-----------------------------------------------------------------------------");

                   Console.Write(String.Format("| {0,-18} | {1,-30} | {2, 19} |", "Prim_Prim", instance_Name, "" ));
                   Prim_Clustered_Tree(weight_Matrix, num_Vertex, num_Cluster, vertex_In_Cluster, source_Vertex, out edge_Matrix);
                   break;
           }
           stGA.Stop();

           //Tinh cost
           double cost_Opt = eval_Class.clustered_Tree_Evaluate(edge_Matrix, weight_Matrix, num_Vertex, source_Vertex);

           int[,] aaa = new int[num_Vertex, num_Vertex];
           for (int i = 0; i < num_Vertex; i++)
           {
               for (int j = 0; j < num_Vertex; j++)
               {
                   aaa[i, j] = (int)edge_Matrix[i, j];
               }
           }
           ioFile.write_Opt_Solution_File(out_File_Data, -1, aaa, num_Vertex, cost_Opt, stGA.Elapsed.ToString(), false);

           ioFile.draw_Plot_in_Matlab(folder_Results + @"\" + Path.GetFileNameWithoutExtension(instance_Name) + @".m", num_Vertex, aaa);

            Console.SetCursorPosition(60, Console.CursorTop);
            Console.Write(string.Format("{0,18}", cost_Opt.ToString("0.00")));
            Console.WriteLine();
        }


        /*********************************************************************************************************************************************
        * Thủ tục gần đúng tìm lời giải bài toán: Clustered Shortest-Path Tree Problem 
        * Thuật toán áp dụng Prim cho cây khung với mỗi nút là 1 cluster; sau đó áp dụng Prim cho các đỉnh trong cluster
        * Output: ma tran trong so bieu dien cay khung: edge_Matrix                      
        ********************************************************************************************************************************************/

        private void Prim_Clustered_Tree(double[,] weigh_Matrix, int num_Vertex, int num_Cluster, int[][] vertex_In_Cluster, int start_Vertex,
            out double[,] edge_Matrix)
        {
            int[,] connect_Cluster; //Lưu các đỉnh thuộc các cluster nào được sử dụng để nối tới đỉnh khác.
            double[,] cluster_Weight;
            int[] pre;
            List<int> path_1;
            double[,] spanning_Tree_of_Cluster;
            Random rnd = new Random();

            edge_Matrix = new double[num_Vertex, num_Vertex];
            for (int i = 0; i < num_Vertex; i++)
            {
                for (int j = i; j < num_Vertex; j++)
                {
                    edge_Matrix[i, j] = 0;
                }
            }

            //01. Chuyển đồ thị thành độ thị với mỗi cluster là một đỉnh
            find_Spanning_Tree_Between_Clusters_Determine_Vertex(weigh_Matrix, num_Vertex, num_Cluster, vertex_In_Cluster, out cluster_Weight, out connect_Cluster);

            //02. Áp dụng BFS cho đồ thị được tạo từ cluster
            spanning_Tree_of_Cluster = graph_Method_Class.prim(cluster_Weight, num_Cluster, rnd);

            //+ Tạo đồ thị tu ket qua Prim
            //Chuyen ra cay khung cua do thi G
            int[] idx_Cluster = new int[num_Cluster];
            for (int i = 0; i < num_Cluster; i++)
            {
                int vt = rnd.Next(vertex_In_Cluster[i].Length);
                idx_Cluster[i] = vertex_In_Cluster[i][vt];
            }
            for (int k = 0; k < num_Cluster; k++)
            {
                for (int j = 0; j < num_Cluster; j++)
                {
                    if (spanning_Tree_of_Cluster[k, j] > 0)
                    {
                        edge_Matrix[idx_Cluster[k], idx_Cluster[j]] = spanning_Tree_of_Cluster[k, j];
                    }
                }
            }

            //int[,] map = new int[num_Cluster, num_Cluster];
            //for (int i = 0; i < num_Cluster; i++)
            //{
            //    for (int j = 0; j < num_Cluster; j++)
            //    {
            //        map[i, j] = (int)spanning_Tree_of_Cluster[i, j];
            //    }
            //}
            //ioFile.draw_Plot_in_Matlab("Prim_Clust.m", num_Cluster, map);

            //map = new int[num_Vertex, num_Vertex];
            //for (int i = 0; i < num_Vertex; i++)
            //{
            //    for (int j = 0; j < num_Vertex; j++)
            //    {
            //        map[i, j] = (int)edge_Matrix[i, j];
            //    }
            //}
            //ioFile.draw_Plot_in_Matlab("Prim_Large_Clust.m", num_Vertex, map);

            //02. Du dung Prim cho cac Cluster
            int num_Vertex_in_Cluster = 0;
            for (int i = 0; i < num_Cluster; i++)
            {
                num_Vertex_in_Cluster = vertex_In_Cluster[i].Length;
                cluster_Weight = init_Chrome.create_Weight_Matrix_for_Cluster(num_Vertex, num_Vertex_in_Cluster, weigh_Matrix, vertex_In_Cluster[i]);
                spanning_Tree_of_Cluster = graph_Method_Class.prim(cluster_Weight, num_Vertex_in_Cluster, rnd);
                //Chuyen ra cay khung cua do thi G
                for (int k = 0; k < num_Vertex_in_Cluster; k++)
                {
                    for (int j = 0; j < num_Vertex_in_Cluster; j++)
                    {
                        if (spanning_Tree_of_Cluster[k, j] > 0)
                        {
                            edge_Matrix[vertex_In_Cluster[i][k], vertex_In_Cluster[i][j]] = spanning_Tree_of_Cluster[k, j];
                        }
                    }
                }
                //map = new int[num_Vertex_in_Cluster, num_Vertex_in_Cluster];
                //for (int iii = 0; iii < num_Vertex_in_Cluster; iii++)
                //{
                //    for (int jjj = 0; jjj < num_Vertex_in_Cluster; jjj++)
                //    {
                //        map[iii, jjj] = (int)spanning_Tree_of_Cluster[iii, jjj];
                //    }
                //}
                //ioFile.draw_Plot_in_Matlab("Prim_for_cluster_Weight_" + i.ToString() + ".m", num_Vertex_in_Cluster, map);

            }

            //map = new int[num_Vertex, num_Vertex];
            //for (int i = 0; i < num_Vertex; i++)
            //{
            //    for (int j = 0; j < num_Vertex; j++)
            //    {
            //        map[i, j] = (int)edge_Matrix[i, j];
            //    }
            //}
            //ioFile.draw_Plot_in_Matlab("Prim_Large_Clust.m", num_Vertex, map);



        }//method


    }
}
