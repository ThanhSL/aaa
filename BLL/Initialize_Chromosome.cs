using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using DTO;
using System.Collections;

namespace BLL
{
    public class Initialize_Chromosome
    {
        private double[] initialize_Double(int array_Length, double lower_Bound, double upper_Bound, Random rnd)
        {
            double[] tmp = new double[array_Length];
            for(int i = 0; i < array_Length; i++)
            {
                tmp[i] = lower_Bound + (upper_Bound - lower_Bound)*rnd.NextDouble();
            }
            return tmp;
        }

        /*********************************************************************************************************************************************
         * Create random permutation: 0 --> n-1 
         * Return permutation array
         ********************************************************************************************************************************************/
        private int[] initialize_Permutation(int array_Length, Random rnd)
        {
            int[] tmp = new int[array_Length];
            int tmp_swap;
            int pos1, pos2;
            for (int i = 0; i < array_Length; i++)
            {
                tmp[i] = i;
            }

            for (int i = 0; i < array_Length; i++)
            {
                pos1 = rnd.Next(array_Length);
                pos2 = rnd.Next(array_Length);
                while (pos1 == pos2)
                {
                    pos2 = rnd.Next(array_Length);
                }
                tmp_swap = tmp[pos1];
                tmp[pos1] = tmp[pos2];
                tmp[pos2] = tmp_swap;
            }
            return tmp;
        }


       /*********************************************************************************************************************************************
        * Create real individual
        * 
        ********************************************************************************************************************************************/
        public double[] initialize_Chromosome(int problem_Index, int array_Length, Random rnd)
        {
            double[] tmp = new double[array_Length];
            switch (problem_Index)
            {
                case 1: tmp = initialize_Double(array_Length, 0, 1, rnd); break;
                case 2: tmp = initialize_Double(array_Length, 0, 1, rnd); break;
                case 3: tmp = initialize_Double(array_Length, 0, 1, rnd); break;
                case 4: tmp = initialize_Double(array_Length, 0, 1, rnd); break;
                case 5: tmp = initialize_Double(array_Length, 0, 1, rnd); break;
                case 6: tmp = initialize_Double(array_Length, 0, 1, rnd); break;
                case 7: tmp = initialize_Double(array_Length, 0, 1, rnd); break;
                //case 8: tmp = initialize_Permutation(array_Length, rnd); break;
            }
            return tmp;
        }


        /*********************************************************************************************************************************************
        * Create prufer array of tree which has n vertex
        * Return an array has n - 2 elements
        ********************************************************************************************************************************************/
        public int[] initialize_Prufer(int num_Vertex, Random rnd)
        {
            int[] tmp = new int[num_Vertex];
            int tmp_swap;
            int pos1, pos2;
            for (int i = 0; i < num_Vertex; i++)
            {
                tmp[i] = i;
            }

            for (int i = 0; i < num_Vertex; i++)
            {
                pos1 = rnd.Next(num_Vertex);
                pos2 = rnd.Next(num_Vertex);
                while (pos1 == pos2)
                {
                    pos2 = rnd.Next(num_Vertex);
                }
                tmp_swap = tmp[pos1];
                tmp[pos1] = tmp[pos2];
                tmp[pos2] = tmp_swap;
            }

            int[] prefer_Arr = new int[num_Vertex - 2];
            for (int i = 0; i < num_Vertex - 2; i++)
            {
                prefer_Arr[i] = tmp[i];
            }
            return prefer_Arr;
        }

      
        /*********************************************************************************************************************************************
         * Create real individual
         * type_encoding_in_union_search_space: kiểu mã hóa được sử dụng trong không gian chung giữa các task => có thể khác problem_index của các task
         ********************************************************************************************************************************************/

        #region Thuật toán tạo cây khung
        //Kiem tra ma tran co doi xung hay khong
        public bool check_Symetric_Matrix(int num_Verties, double[,] weight_Matrix)
        {
            bool ok = true;
            for (int i = 0; i < num_Verties; i++)
            {
                for (int j = i+1; j < num_Verties; j++)
                {
                    if(weight_Matrix[i,j] != weight_Matrix[j,i])
                    {
                        ok = false;
                        break;
                    }
                }
            }
            return ok;
        }

        
        /*********************************************************************************************************************************************
        *  Tìm danh sách các cạnh có đỉnh kề với đỉnh v mà không thuộc tập C
        *  
        ********************************************************************************************************************************************/
        public List<Edge> find_Edges(int num_Verties, int v, double[,] weight_Matrix, List<int> C)
        {
            List<Edge> lst_Edge = new List<Edge>();
            for (int i = 0; i < num_Verties; i++)
            {
                if (i != v)
                {
                    if (!C.Contains(i))
                    {
                        if (weight_Matrix[v, i] > 0)
                        {
                            Edge edge_tmp = new Edge(v, i);
                            lst_Edge.Add(edge_tmp);
                        }
                    }
                }
            }
            return lst_Edge;
        }

        /*********************************************************************************************************************************************
        *  G. R. Raidl and B. A. Julstrom, “Edge sets: an effective evolutionary coding of spanning trees,” IEEE Transactions on evolutionary computation, vol. 7, no. 3, pp. 225–239, 2003.
        *  Thuật toán tạo cây khung ngẫu nhiên mô phỏng thuật toán PRIM
        *       Trả về NULL nếu không tìm được cây khung
        ********************************************************************************************************************************************/
        public double[,] primRST(int num_Verties, double[,] weight_Matrix, Random rnd)
        {
            int rnd_Verties = -1,
                idx_Rnd_Edge = -1;
            List<int> C = new List<int>();
            List<Edge> A = new List<Edge>();

            double[,] T = new double[num_Verties, num_Verties];
            for (int i = 0; i < num_Verties; i++)
            {
                for (int j = i; j < num_Verties; j++)
                {
                    T[i, j] = 0;
                    T[j, i] = 0;
                }
            }

            rnd_Verties = rnd.Next(num_Verties);
            C.Add(rnd_Verties);
            A.AddRange(find_Edges(num_Verties, rnd_Verties, weight_Matrix, C));

           
            while (C.Count < num_Verties)
            {
                if (A.Count == 0) //Không tìm được cây khung
                {
                    return null;
                }

                idx_Rnd_Edge = rnd.Next(A.Count);     //Do các cạnh thuộc A có ít nhất 1 đỉnh thuộc C -> thuật toán tạo thì là start_point
                Edge edge_Tmp = new Edge(A[idx_Rnd_Edge].Start_Point, A[idx_Rnd_Edge].End_Point);
                A.RemoveAt(idx_Rnd_Edge);
                if (!C.Contains(edge_Tmp.End_Point))
                {
                    T[edge_Tmp.Start_Point, edge_Tmp.End_Point] = 1.0f;
                    T[edge_Tmp.End_Point, edge_Tmp.Start_Point] = 1.0f;

                    //ReadWriteFile ioFile = new ReadWriteFile();
                    //ioFile.writeMatrixToFile("aaa.tx", T, num_Verties, false);
                    
                    C.Add(edge_Tmp.End_Point);
                    A.AddRange(find_Edges(num_Verties, edge_Tmp.End_Point, weight_Matrix, C));
                }
            }
            return T;
        }

        public double[,] kruskalRST(int num_Verties, double[,] weight_Matrix, Random rnd)
        {
            int rnd_Verties = -1,
                idx_Rnd_Edge = -1,
                num_Edges_In_T = 0;
            List<int> C = new List<int>();
            List<Edge> A = new List<Edge>();

            double[,] T = new double[num_Verties, num_Verties];
            for (int i = 0; i < num_Verties; i++)
            {
                for (int j = 0; j < num_Verties; j++)
                {
                    T[i, j] = 0;
                }
            }

            //Create A
            for (int i = 0; i < num_Verties; i++)
            {
                for (int j = i + 1; j < num_Verties; j++)
                {
                    if (weight_Matrix[i, j] > 0)
                    {
                        A.Add(new Edge(i, j));
                    }
                }
            }
            while (num_Edges_In_T < num_Edges_In_T - 1)
            {
                idx_Rnd_Edge = rnd.Next(A.Count);
                Edge edge_Tmp = new Edge(A[idx_Rnd_Edge].Start_Point, A[idx_Rnd_Edge].End_Point);
                A.RemoveAt(idx_Rnd_Edge);

            }

            //----->chua xong
            return T;
        }


        #endregion
        public void initialize_Chromosome(string init_Individual_Alg, int array_Length, Tasks tsk, Random rnd, ref Chromosome ind)
        {
            double[,] weight_Matrix;
            switch (init_Individual_Alg)
            {
                case "REAL_RANDOM": //01-07. hàm số thực
                    ind.Rnvec = initialize_Double(array_Length, 0, 1, rnd); 
                    break;
                case "PERMUTATION": //08. TSP
                    ind.Invec = initialize_Permutation(array_Length, rnd);
                    break;
                case "PRUFER_CODE":  //09. prufer -> Chuỗi là độ dài n - 2
                    ind.Invec = initialize_Prufer(array_Length + 2, rnd);  
                    break;
                case "PRIMRST":  //10. PrimRST -> Kích thước ma trận là số đỉnh đồ thị
                    #region PRIMRST
                    weight_Matrix = new double[array_Length, array_Length];
                    for (int i = 0; i < array_Length; i++)
                    {
                        for(int j = 0; j < array_Length; j++)
                        {
                            if(i == j)
                            {
                                weight_Matrix[i, j] = 0.0f;
                            }
                            else
                            {
                                weight_Matrix[i, j] = 1.0f;
                            }
                        }
                    }
                    ind.Edges_Matrix = primRST(array_Length, weight_Matrix, rnd);

                    //if (!check_Symetric_Matrix(array_Length, ind.Edges_Matrix))
                    //{
                    //    Console.WriteLine("Ma tran cay ko doi xung");
                    //    Console.ReadKey();
                    //    return;
                    //}

                    //ReadWriteFile ioFile = new ReadWriteFile();
                    //int[,] aaa = new int[array_Length, array_Length];
                    //for (int i = 0; i < array_Length; i++)
                    //{
                    //    for (int j = 0; j < array_Length; j++)
                    //    {
                    //        aaa[i, j] = (int)ind.Edges_Matrix[i, j];
                    //    }
                    //}
                    //ioFile.draw_Plot_in_Matlab("init" + @".m", array_Length, aaa);
                    #endregion
                    break;
                case "KRUSKALRST":  //10. PrimRST -> Kích thước ma trận là số đỉnh đồ thị
                    #region KRUSKALRST
                    weight_Matrix = new double[array_Length, array_Length];
                    for (int i = 0; i < array_Length; i++)
                    {
                        for (int j = 0; j < array_Length; j++)
                        {
                            weight_Matrix[i, j] = 1.0f;
                        }
                    }
                    //ind.Edges_Matrix = kruskalRST(array_Length, weight_Matrix, rnd);
                    #endregion
                    break;
                case "PRIMRST_CLUSTERED_TREE":  //10. PrimRST -> Kích thước ma trận là số đỉnh đồ thị
                    #region PRIMRST_CLUSTERED_TREE
                    ind.Edges_Matrix = primRST_for_Clustered_Tree(array_Length, tsk.Num_Cluster, tsk.Weight_Matrix, tsk.Vertex_In_Cluster, rnd);

                    //if (!check_Symetric_Matrix(array_Length, ind.Edges_Matrix))
                    //{
                    //    Console.WriteLine("Ma tran cay ko doi xung");
                    //    Console.ReadKey();
                    //    return;
                    //}

                    //ReadWriteFile ioFile = new ReadWriteFile();
                    //int[,] aaa = new int[array_Length, array_Length];
                    //for (int i = 0; i < array_Length; i++)
                    //{
                    //    for (int j = 0; j < array_Length; j++)
                    //    {
                    //        aaa[i, j] = (int)ind.Edges_Matrix[i, j];
                    //    }
                    //}
                    //ioFile.draw_Plot_in_Matlab("init_clustered_tree_" + @".m", array_Length, aaa);
                    #endregion
                    break;
                case "PRUFER_CODE_CLUSTERED_TREE":
                    #region PRUFER_CODE_CLUSTERED_TREE
                    //Lấy số cluster của task lớn nhất
                    

                    #endregion
                    break;
                case "eeee":
                    #region PRUFER_CODE_CLUSTERED_TREE


                    #endregion
                    break;
                //case 8: tmp = initialize_Permutation(array_Length, rnd); break;
            }
            return;
        }

        
        #region Create optimal solution for ONE MAX TREE PROBLEM
      

        public void create_Optimal_of_One_Max_Tree(string file_Name, int num_Vertex)
        {
            Random rnd = new Random();
            Evaluate eva_class = new Evaluate();
            int[] prufer_Number = initialize_Prufer(num_Vertex, rnd);
            int[,] edge_Weight = eva_class.convert_Prufer_to_Tree(prufer_Number, num_Vertex);

            ReadWriteFile ioFile = new ReadWriteFile();
            ioFile.write_Optimal_of_One_Max_Tree(file_Name, num_Vertex, edge_Weight);
            ioFile.draw_Plot_in_Matlab(Path.GetFileNameWithoutExtension(file_Name)+@".m", num_Vertex, edge_Weight);
            
        }


        #endregion


        #region Cluster Tree
        /*********************************************************************************************************************************************
        *  Tạo ma trận trọng số cho cluster từ ma trận của đồ thị G ban đầu
        *  Ma trận kết quả có kích thước: num_Vertex_in_Cluster * num_Vertex_in_Cluster
        *  
        ********************************************************************************************************************************************/
        public double[,] create_Weight_Matrix_for_Cluster(int num_Vertex, int num_Vertex_in_Cluster, double[,] weight_Matrix, int[] vertex_In_Cluster)
        {
            double[,] weight_Cluster_Matrix = new double[num_Vertex_in_Cluster, num_Vertex_in_Cluster];
            //int[] idx = new int[num_Vertex];
            //for(int i = 0; i < num_Vertex_in_Cluster; i++)
            //{
            //    idx[i] = vertex_In_Cluster[i];
            //}
            for(int i = 0; i < num_Vertex_in_Cluster; i++)
            {
                for(int j = 0; j < num_Vertex_in_Cluster; j++)
                {
                    weight_Cluster_Matrix[i, j] = weight_Matrix[vertex_In_Cluster[i], vertex_In_Cluster[j]];
                }
            }
            return weight_Cluster_Matrix;
        }
        
        /*********************************************************************************************************************************************
        *  Tạo cây khung theo thuật toán primRST: G. R. Raidl and B. A. Julstrom, “Edge sets: an effective evolutionary coding of spanning trees,” IEEE Transactions on evolutionary computation, vol. 7, no. 3, pp. 225–239, 2003.
        *  Tạo cây khung cho tập các đỉnh trong: vertex_In_Cluster
        ********************************************************************************************************************************************/
        public double[,] primRST_for_Clustered_Tree(int num_Vertex, int num_Cluster, double[,] weight_Matrix, int[][] vertex_In_Cluster, Random rnd)
        {
            int rnd_Verties = -1,
                idx_Rnd_Edge = -1,
                num_Verties;
            double[,] T = new double[num_Vertex, num_Vertex];
            //Khoi tao
            for (int i = 0; i < num_Vertex; i++)
            {
                for (int j = i; j < num_Vertex; j++)
                {
                    T[i, j] = 0;
                    T[j, i] = 0;
                }
            }

            //Tao cay khung cho tung cluster
            double[,] weight_Cluster_Matrix;
            double[,] spanning_Tree_of_Cluster;
            for(int i = 0; i < num_Cluster; i++)
            {
                int num_Vertex_in_Cluster = vertex_In_Cluster[i].Length;
                weight_Cluster_Matrix = create_Weight_Matrix_for_Cluster(num_Vertex, num_Vertex_in_Cluster, weight_Matrix, vertex_In_Cluster[i]);
                spanning_Tree_of_Cluster = primRST(num_Vertex_in_Cluster, weight_Cluster_Matrix, rnd);
                //Chuyen ra cay khung cua do thi G
                for (int k = 0; k < num_Vertex_in_Cluster; k++)
                {
                    for (int j = 0; j < num_Vertex_in_Cluster; j++)
                    {
                        if (spanning_Tree_of_Cluster[k, j] > 0)
                        {
                            T[vertex_In_Cluster[i][k], vertex_In_Cluster[i][j]] = spanning_Tree_of_Cluster[k, j];
                        }
                    }
                }
            }
            //Tạo cây khung cho đại diện các nhóm
            //Chọn mỗi nhóm 1 đỉnh
            int[] idx_Cluster = new int[num_Cluster];
            for(int i = 0; i < num_Cluster; i++)
            {
                int vt = rnd.Next(vertex_In_Cluster[i].Length);
                idx_Cluster[i] = vertex_In_Cluster[i][vt];
            }

            weight_Cluster_Matrix = create_Weight_Matrix_for_Cluster(num_Vertex, num_Cluster, weight_Matrix, idx_Cluster);//ko dung khi do thi la do thi thua
            spanning_Tree_of_Cluster = primRST(num_Cluster, weight_Cluster_Matrix, rnd);

            //int[,] aaa = new int[num_Vertex, num_Vertex];
            //for (int i = 0; i < num_Vertex; i++)
            //{
            //    for (int j = 0; j < num_Vertex; j++)
            //    {
            //        aaa[i, j] = (int)T[i, j];
            //    }
            //}
            //ReadWriteFile ioFile = new ReadWriteFile();
            //ioFile.draw_Plot_in_Matlab("Init_Befor_Full" + @".m", num_Vertex, aaa);

            //Chuyen ra cay khung cua do thi G
            for (int k = 0; k < num_Cluster; k++)
            {
                for (int j = 0; j < num_Cluster; j++)
                {
                    if (spanning_Tree_of_Cluster[k, j] > 0)
                    {
                        T[idx_Cluster[k], idx_Cluster[j]] = spanning_Tree_of_Cluster[k, j];
                    }
                }
            }
            //int[,] aaa = new int[num_Vertex, num_Vertex];
            //for (int i = 0; i < num_Vertex; i++)
            //{
            //    for (int j = 0; j < num_Vertex; j++)
            //    {
            //        aaa[i, j] = (int)T[i, j];
            //    }
            //}
            ////ReadWriteFile ioFile = new ReadWriteFile();
            //ioFile.draw_Plot_in_Matlab("Init_full" + @".m", num_Vertex, aaa);
            return T;
        }

        /*********************************************************************************************************************************************
         *  - Biểu diễn cá thể với:
         *  + Số cluster là max cluster của 2 instanes
         *  + Số đỉnh trong mỗi cluster là max số đỉnh trong cluster của 2 instancess
         *  
         ********************************************************************************************************************************************/


        #endregion


        #region Bài toán Cluster tree sử dụng biểu diễn Prufer code
        /*********************************************************************************************************************************************
         *  - Biểu diễn cá thể với:
         *  + Số cluster là max cluster của 2 instanes
         *  + Số đỉnh trong mỗi cluster là max số đỉnh trong cluster của 2 instancess
         *  - Kết quả trả về là cả thể với số đoạn là:
         *  + 1 đoạn biểu độ thì mà mỗi cluster là 1 đỉnh
         *  + k đoạn tiếp theo biểu diễn k cluster với k là max số cluster của các task
         *  Tham số:
         *  + max_Num_Vertex: tổng số đỉnh bao gồm cả số max số cluster và tổng số đỉnh trong mỗi max cluster
         ********************************************************************************************************************************************/
        public int[] prufer_Code_for_Clustered_Tree(int max_Num_Vertex, int max_Num_Cluster, int[] num_Vertex_In_Cluster, Random rnd)
        {
            int[] ind = new int[max_Num_Cluster];
            //Tạo hoán vị cho đoạn đầu tiên mà mỗi cluster là 1 đỉnh
            int[] tmp = initialize_Permutation(max_Num_Cluster, rnd);
            //+ Chuyển vào cá thể
            for (int i = 0; i < max_Num_Cluster; i++)
            {
                ind[i] = tmp[i];
            }
            //Tạo hoán vị cho các đoạn tiếp theo, mỗi đoạn có số phần tử là max số phần tử trong cluster tương ứng.
            int num_Elements = max_Num_Cluster;
            for(int i = 0; i < max_Num_Cluster; i++)
            {
                tmp = initialize_Permutation(num_Vertex_In_Cluster[i], rnd);
                for(int j = 0; j < num_Vertex_In_Cluster[i]; j++)
                {
                    ind[j + num_Elements] = tmp[j];
                }
                num_Elements = num_Elements + num_Vertex_In_Cluster[i];
            }
            return ind;
        }

        /*********************************************************************************************************************************************
        *  - Biểu diễn cá thể với:
        *  + Số cluster là max cluster của 2 instanes
        *  + Số đỉnh trong mỗi cluster là max số đỉnh trong cluster của 2 instancess
        *  + segment_of_a_Cluster: Xác định vị trí bắt đầu và kết thúc của cluster trên ind
        *  - Kết quả trả về là cả thể với số đoạn là:
        *  + 1 đoạn biểu độ thì mà mỗi cluster là 1 đỉnh
        *  + k đoạn tiếp theo biểu diễn k cluster với k là max số cluster của các task
        *  Tham số:
        *  + max_Num_Vertex: tổng số đỉnh bao gồm cả số max số cluster và tổng số đỉnh trong mỗi max cluster
        ********************************************************************************************************************************************/
        public int[] prufer_Code_for_Clustered_Tree(int max_Num_Vertex, int max_Num_Cluster, int[] segment_of_a_Cluster, Random rnd)
        {
            int[] ind = new int[max_Num_Cluster];
            //Tạo hoán vị cho đoạn đầu tiên mà mỗi cluster là 1 đỉnh
            int[] tmp = initialize_Permutation(max_Num_Cluster, rnd);
            //+ Chuyển vào cá thể
            for (int i = 0; i < max_Num_Cluster; i++)
            {
                ind[i] = tmp[i];
            }
            //Tạo hoán vị cho các đoạn tiếp theo, mỗi đoạn có số phần tử là max số phần tử trong cluster tương ứng.
            int num_Elements = max_Num_Cluster;
            for (int i = 0; i < max_Num_Cluster; i++)
            {
                tmp = initialize_Permutation(num_Vertex_In_Cluster[i + 1] - num_Vertex_In_Cluster[i] + 1, rnd);
                for (int j = 0; j < num_Vertex_In_Cluster[i]; j++)
                {
                    ind[j + num_Elements] = tmp[j];
                }
                num_Elements = num_Elements + num_Vertex_In_Cluster[i];
            }
            return ind;
        }



        #endregion

    }
}
