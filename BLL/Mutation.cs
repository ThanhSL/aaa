using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using DTO;


namespace BLL
{
    public class Mutation
    {
        Graph_Method graph_Method_Class = new Graph_Method();
        //1. Mutation
        private double[] slight_Mutate(double[] parent, int num_Genes, double sigma, Random rnd)
        {
            double[] child = new double[num_Genes];
            for (int i = 0; i < num_Genes; i++)
            {
                child[i] = parent[i] + rnd.NextDouble()*sigma;
                if(child[i] > 1)
                {
                    child[i] = 1;
                }
                if(child[i] < 0)
                {
                    child[i] = 0;
                }
            }
            return child;
        }

        //2. Mutation Index
        private int[] swap_Mutation(int[] parent, int num_Genes, double mutation_Rate, Random rnd)
        {
            int temp;
            int[] child = new int[num_Genes];
            for(int i = 0; i < num_Genes; i++)
            {
                child[i] = parent[i];
            }

            for (int i = 0; i < num_Genes; i++)
            {
                if (rnd.NextDouble() < mutation_Rate)
                {
                    int pos1 = rnd.Next(num_Genes);
                    int pos2 = rnd.Next(num_Genes);
                    while (pos1 == pos2)
                    {
                        pos2 = rnd.Next(num_Genes);
                    }
                    temp = child[pos1];
                    child[pos1] = child[pos2];
                    child[pos2] = temp;
                }
            }
            return child;
        }



        #region Edge representation
        /*********************************************************************************************************************************************
         * 
         *  http://www.geeksforgeeks.org/detect-cycle-undirected-graph/
         *  A recursive function that uses visited[] and parent to detect cycle in subgraph reachable from vertex v.
         * 
         * https://voer.edu.vn/c/tim-duong-di-va-kiem-tra-tinh-lien-thong/9c021e14/d2dc68dc
         *  
         ********************************************************************************************************************************************/

        private void find_Cyclic(int start_Vertex, int end_Vertex, double[,] weight_Matrix, int num_Vertex, bool[] visited, int[] pre)
        {
            visited[start_Vertex] = true;
            for (int u = 0; u < num_Vertex; u++)
            {
                if ((weight_Matrix[start_Vertex, u] > 0) && (u == end_Vertex))
                {
                    pre[u] = start_Vertex;
                    return;
                }
                if ((weight_Matrix[start_Vertex, u] > 0) && (!visited[u]))
                {
                    pre[u] = start_Vertex;
                    find_Cyclic(u, end_Vertex, weight_Matrix, num_Vertex, visited, pre);

                }
            }
        }

    
        /*********************************************************************************************************************************************
        * 
        * G. R. Raidl and B. A. Julstrom, “Edge sets: an effective evolutionary coding of spanning trees,” IEEE Transactions on evolutionary computation, 
         * vol. 7, no. 3, pp. 225–239, 2003.
        *  
        ********************************************************************************************************************************************/

        public double[,] edge_Mutation(double[,] par, int num_Vertex, double mutation_Rate, Random rnd)
        {
            double[,] child = new double[num_Vertex, num_Vertex];
            for (int i = 0; i < num_Vertex; i++)
            {
                for (int j = 0; j < num_Vertex; j++)
                {
                    child[i, j] = par[i, j];
                }
            }
            int start_Vertex, end_Vertex; //2 dinh cua canh ngau nhien them vao
            start_Vertex = rnd.Next(num_Vertex);
            end_Vertex = rnd.Next(num_Vertex);
            while ((start_Vertex == end_Vertex) || (child[start_Vertex, end_Vertex] > 0)) //2 đỉnh có phải kề nhau hoc trung nhau
            {
                end_Vertex = rnd.Next(num_Vertex);
                start_Vertex = rnd.Next(num_Vertex);
            }

            if(rnd.NextDouble() < mutation_Rate)
            {
                //Tìm duong ti tu dinh start_Vertex -> end_Vertex
                bool[] visited = new bool[num_Vertex];
                int[] pre = new int[num_Vertex];
                for (int i = 0; i < num_Vertex; i++)
                {
                    visited[i] = false;
                    pre[i] = -1;
                }
                //Tìm đường đi nối từ đỉnh start_Vertex --> end_Vertex
                find_Cyclic(start_Vertex, end_Vertex, child, num_Vertex, visited, pre);
                List<int> path = graph_Method_Class.print_Path(start_Vertex, end_Vertex, pre);
                //Xóa cạnh bất kỳ trên đường đi.
                int del_idx_1 = rnd.Next(path.Count - 1);
                //int del_idx_2 = rnd.Next(path.Count);
                int del_idx_2 = del_idx_1 + 1;
                //while (del_idx_1 == del_idx_2) 
                //{
                //    del_idx_2 = rnd.Next(path.Count);
                //}
                child[path[del_idx_1], path[del_idx_2]] = 0f;
                child[path[del_idx_2], path[del_idx_1]] = 0f;
               //Dat canh moi
                child[start_Vertex, end_Vertex] = 1f;
                child[end_Vertex, start_Vertex] = 1f;
            }
            return child;
        }


        #endregion


        #region Clustered Tree
        public double[,] edge_Clustered_Tree_Mutation(double[,] par, int num_Vertex, double mutation_Rate, int num_Cluster, int[][] vertex_In_Cluster, Random rnd)
        {
            Initialize_Chromosome init_Chrome = new Initialize_Chromosome();

            double[,] child = new double[num_Vertex, num_Vertex];
            for (int i = 0; i < num_Vertex; i++)
            {
                for (int j = 0; j < num_Vertex; j++)
                {
                    child[i, j] = par[i, j];
                }
            }

            for(int ii = 0; ii < num_Vertex; ii++)
            {
                int idx_Cluster = rnd.Next(num_Cluster);
                while (vertex_In_Cluster[idx_Cluster].Length < 3)
                {
                    idx_Cluster = rnd.Next(num_Cluster);

                }
                //01. Chuyển ma trận của cluster thành ma trận cây để áp dụng đột biến
                double[,] weight_Cluster_Matrix;
                double[,] spanning_Tree_of_Cluster;
                int num_Vertex_in_Cluster = vertex_In_Cluster[idx_Cluster].Length;
                weight_Cluster_Matrix = init_Chrome.create_Weight_Matrix_for_Cluster(num_Vertex, num_Vertex_in_Cluster, par, vertex_In_Cluster[idx_Cluster]);
                spanning_Tree_of_Cluster = edge_Mutation(weight_Cluster_Matrix, num_Vertex_in_Cluster, mutation_Rate, rnd);

                //Chuyen ra cay khung cua do thi G
                int[] cluster = new int[num_Vertex_in_Cluster];
                for (int i = 0; i < num_Vertex_in_Cluster; i++)
                {
                    cluster[i] = vertex_In_Cluster[idx_Cluster][i];
                }
                for (int k = 0; k < num_Vertex_in_Cluster; k++)
                {
                    for (int j = 0; j < num_Vertex_in_Cluster; j++)
                    {
                        //child[cluster[k], cluster[j]] = 0;
                        child[cluster[k], cluster[j]] = spanning_Tree_of_Cluster[k, j];
                    }
                }

            }

           
            return child;
        }

      
        #endregion

        
        public void MFO_Mutation(Mutation_Parameters mutation_Para, ref Chromosome ind, int num_Genes, Tasks tsk, Random rnd)
        {
            switch (mutation_Para.Mutation_Name)
            {
                case "SLIGHT_MUTATE":
                    ind.Rnvec = slight_Mutate(ind.Rnvec, num_Genes, mutation_Para.Sigma, rnd);
                    break;
                case "SWAP_MUTATION":
                    ind.Invec = swap_Mutation(ind.Invec, num_Genes, mutation_Para.Mutation_Rate, rnd);
                    break;
                case "PRUFER_SWAP_MUTATION":
                    ind.Invec = swap_Mutation(ind.Invec, num_Genes, mutation_Para.Mutation_Rate, rnd);
                    break;
                case "EDGE_MUTATION":
                    ind.Edges_Matrix = edge_Mutation(ind.Edges_Matrix, num_Genes, mutation_Para.Mutation_Rate, rnd);
                    break;
                case "EDGE_CLUSTERED_TREE_MUTATION":
                    #region  EDGE_CLUSTERED_TREE_MUTATION
                    ind.Edges_Matrix = edge_Clustered_Tree_Mutation(ind.Edges_Matrix, num_Genes, mutation_Para.Mutation_Rate, tsk.Num_Cluster, tsk.Vertex_In_Cluster, rnd);
                    #endregion
                    break;
            }
            return;
        }

       
    }



}
