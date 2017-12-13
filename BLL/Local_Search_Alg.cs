using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DTO;
using System.Collections;


namespace BLL
{
    public class Local_Search_Alg
    {
        Evaluate eva = new Evaluate();
        Initialize_Chromosome init_Chrome = new Initialize_Chromosome();
        Graph_Method graph_Method_Class = new Graph_Method();
        ReadWriteFile ioFile = new ReadWriteFile();


        public void local_search_alg(Tasks tsk, string local_Search_Method_Name, int max_Genens, Random rnd, ref Chromosome ind, ref int func_call)
        {
            func_call = 1;
            switch (local_Search_Method_Name)
            {
                case "PRIM_2nd_TASK":
                    #region áp dụng giải thuật PRIM tìm cây khung nhỏ nhất cho cluster
                    int idx_Max_Clus = 0;
                    int num_Vertex_In_Cluster = 0;
                    Evaluate eva = new Evaluate();
                    eva.find_Largest_Cluster(tsk, ref idx_Max_Clus, ref num_Vertex_In_Cluster);

                    double[,] cluster_Weight = init_Chrome.create_Weight_Matrix_for_Cluster(max_Genens, num_Vertex_In_Cluster, tsk.Weight_Matrix, tsk.Vertex_In_Cluster[idx_Max_Clus]);
                    double[,] spanning_Tree_of_Cluster = graph_Method_Class.prim(cluster_Weight, num_Vertex_In_Cluster, rnd);

                    //Chuyen ra cay khung cua do thi G
                    for (int k = 0; k < num_Vertex_In_Cluster; k++)
                    {
                        for (int j = 0; j < num_Vertex_In_Cluster; j++)
                        {
                            if (spanning_Tree_of_Cluster[k, j] > 0)
                            {
                                ind.Edges_Matrix[tsk.Vertex_In_Cluster[idx_Max_Clus][k], tsk.Vertex_In_Cluster[idx_Max_Clus][j]] = spanning_Tree_of_Cluster[k, j];
                            }
                        }
                    }
                    
                    //int[,] aaa = new int[max_Genens, max_Genens];
                    //for (int i = 0; i < max_Genens; i++)
                    //{
                    //    for (int j = 0; j < max_Genens; j++)
                    //    {
                    //        aaa[i, j] = (int)ind.Edges_Matrix[i, j];
                    //    }
                    //}
                    //ioFile.draw_Plot_in_Matlab("Local_Search_GA" + @".m", max_Genens, aaa);

                    //Evaluate???

                    #endregion
                    break;
                case "1":
                    #region 1
                    //11111;
                    #endregion
                    break;
            }
        }
    }
}
