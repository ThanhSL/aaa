using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DTO;
using System.Collections;

namespace BLL
{
    public class Crossovers
    {
        ReadWriteFile ioFile = new ReadWriteFile();
        Initialize_Chromosome init_Chrom = new Initialize_Chromosome();

        #region Method cu - khong con su dung

        public void MFO_Crossover(Crossover_Parameters crss_Para, Chromosome par_1, Chromosome par_2, int num_Genens, ref Chromosome child_1,
            ref Chromosome child_2, Random rnd)
        {
            switch (crss_Para.Crossver_Name)
            {
                case "REAL_CROSSVER":
                    child_1.Rnvec = real_Crossover(par_1.Rnvec, par_2.Rnvec, num_Genens, crss_Para.Cf);
                    child_2.Rnvec = real_Crossover(par_1.Rnvec, par_2.Rnvec, num_Genens, crss_Para.Cf);
                    break;
                case "ONE_POINT_CROSSOVER":
                    one_Point_Crossover(par_1.Rnvec, par_2.Rnvec, num_Genens, child_1.Rnvec, child_2.Rnvec, rnd);
                    break;
                case "PMX":
                    PMX(par_1.Invec, par_2.Invec, num_Genens, child_1.Invec, child_2.Invec, rnd);
                    break;
                case "PRUFER_ONE_POINT_CROSSOVER":
                    prufer_One_Point_Crossover(par_1.Invec, par_2.Invec, num_Genens, child_1.Invec, child_2.Invec, rnd);
                    break;
                case "PRIMRST_CROSSOVER":
                    child_1.Edges_Matrix = primRST_Crossover(par_1.Edges_Matrix, par_2.Edges_Matrix, num_Genens, rnd);
                    //???child_2
                    break;
            }
        }





        #endregion

        //1. 
        private double[] real_Crossover(double[] p1, double[] p2, int num_Genens, double cf)
        {
            double[] child = new double[num_Genens];
            for (int i = 0; i < num_Genens; i++)
            {
                child[i] = 0.5 * ((1 + cf) * p1[i] + (1 - cf) * p2[i]);
                if (child[i] > 1)
                {
                    child[i] = 1;
                }
                if (child[i] < 0)
                {
                    child[i] = 0;
                }
            }
            return child;
        }

        //2. 
        private void one_Point_Crossover(double[] par1, double[] par2, int num_Genens, double[] child1, double[] child2, Random rnd)
        {
            int point = rnd.Next(num_Genens);
            for (int i = 0; i < point; i++)
            {
                child1[i] = par1[i];
                child2[i] = par2[i];
            }
            for (int i = point; i < num_Genens; i++)
            {
                child1[i] = par2[i];
                child2[i] = par1[i];
            }
        }


        #region Crossovers for TSP

        private void PMX(int[] par1, int[] par2, int numGenes, int[] child1, int[] child2, Random ran)
        {
            int pos1 = ran.Next(numGenes - 1);
            int pos2 = ran.Next(numGenes - pos1) + pos1;

            ArrayList s1 = new ArrayList();
            ArrayList s2 = new ArrayList();

            for (int i = pos1; i <= pos2; i++)
            {
                child1[i] = par1[i];
                s1.Add(par1[i]);
                child2[i] = par2[i];
                s2.Add(par2[i]);
            }

            int index1 = 1;
            int index2 = 1;
            int temp = 0;
            for (int i = 1; i <= numGenes - (pos2 - pos1 + 1); i++)
            {
                temp = i + pos2;
                if (temp >= numGenes)
                {
                    temp = temp - numGenes;
                }
                int ps1 = index1 + pos2;
                if (ps1 >= numGenes)
                {
                    ps1 = ps1 - numGenes;
                }
                int ps2 = index2 + pos2;
                if (ps2 >= numGenes)
                {
                    ps2 = ps2 - numGenes;
                }

                int tt = par2[temp];
                int pp1 = -1;
                while (s1.Contains(tt))
                {
                    pp1 = s1.IndexOf(tt);
                    tt = (int)s2[pp1];
                }

                child1[ps1] = tt;
                index1++;

                tt = par1[temp];
                pp1 = -1;
                while (s2.Contains(tt))
                {
                    pp1 = s2.IndexOf(tt);
                    tt = (int)s1[pp1];
                }
                child2[ps2] = tt;
                index2++;
            }

        }

        //Eiben, Introduction to Evolutionary Computing. Berlin, Springer-Verlag.
        private void cycleCrossover(int[] par1, int[] par2, int numCity, ref int[] child1, ref int[] child2)
        {
            //child1 = new int[numGenes];
            //child2 = new int[numGenes];

            int count = 0;
            int idx = -1;
            bool ok = true; //Xác định xem vị trí trên Par1 sẽ được gán cho Child1 hay Child2
            List<int> lstCycle = new List<int>();
            while (count < numCity)
            {
                idx = -1;
                //Start with the First unused position and allele of P1
                for (int i = 0; i < numCity; i++)
                {
                    if (Array.IndexOf(child1, par1[i]) < 0)
                    {
                        idx = i;
                        break;
                    }
                }
                //1. Start with the first unused position and allele of P1
                //2. Look at the allele in the same position in P2
                //3. Go to the position with the same allele in P1
                //4. Add this allele to the cycle
                //5. Repeat steps 2 through 4 until you arrive at the first allele of P1


                //the offspring are created by selecting alternate cycles from each parent
                lstCycle.Clear();
                while (!lstCycle.Contains(par1[idx]))
                {
                    lstCycle.Add(par1[idx]);
                    idx = Array.IndexOf(par1, par2[idx]);
                }
                if (ok)
                {
                    for (int j = 0; j < lstCycle.Count; j++)
                    {
                        child1[Array.IndexOf(par1, lstCycle[j])] = lstCycle[j];
                        child2[Array.IndexOf(par2, lstCycle[j])] = lstCycle[j];
                        count++;
                    }
                    ok = !ok;
                }
                else
                {
                    for (int j = 0; j < lstCycle.Count; j++)
                    {
                        child1[Array.IndexOf(par2, lstCycle[j])] = lstCycle[j];
                        child2[Array.IndexOf(par1, lstCycle[j])] = lstCycle[j];
                        count++;
                    }
                    ok = !ok;
                }
            }//while            
        }//method

        /************************************************   Order Crossover  ******************************************************************************************
         * 
         * 1. Choose two crossover points at random, and copy the segment between them from the ¯rst parent (P1) into the ¯rst o®spring.
         * 2. Starting from the second crossover point in the second parent, copy the remaining unused numbers into the ¯rst child in the order that they appear in the second parent, wrapping around at the end of the list.
         * 3. Create the second o®spring in an analogous manner, with the parent roles reversed.
         * 
         ************************************************************************************************************************************************************/

        //Thanh pho co gia tri: 1 -> numCity ???
        private void orderCrossover(int[] par1, int[] par2, int numCity, ref int[] child1, ref int[] child2, Random ran)
        {
            //int numCity = Par1.OrderCity.Length;
            //child1 = new PathTour(numCity);
            //child2 = new PathTour(numCity);
            int pos1 = ran.Next(numCity - 1);
            int pos2 = ran.Next(numCity - pos1) + pos1;

            ArrayList s1 = new ArrayList();
            ArrayList s2 = new ArrayList();
            for (int i = pos1; i <= pos2; i++)
            {
                child1[i] = par1[i];
                s1.Add(par1[i]);
                child2[i] = par2[i];
                s2.Add(par2[i]);
            }
            int index1 = 1;
            int index2 = 1;
            int temp = 0;
            for (int i = 1; i <= numCity; i++) //for (int i = pos2+1; i < pos2 + this.chromoLength - (pos2 - pos1 + 1); i++)
            {
                temp = i + pos2;
                if (temp >= numCity)
                {
                    temp = temp - numCity;
                }
                int ps1 = index1 + pos2;
                if (ps1 >= numCity)
                {
                    ps1 = ps1 - numCity;
                }
                int ps2 = index2 + pos2;
                if (ps2 >= numCity)
                {
                    ps2 = ps2 - numCity;
                }
                if (!s1.Contains(par2[temp]))
                {
                    child1[ps1] = par2[temp];
                    index1++;
                }
                if (!s2.Contains(par1[temp]))
                {
                    child2[ps2] = par1[temp];
                    index2++;
                }
            }
        }


        //Thanh pho co gia tri: 1 -> numCity
        private void edge_recombination(int[] par1, int[] par2, int numCity, ref int[] child, Random ran)
        {
            int[][] adjacency_matrix = new int[numCity][];
            for (int i = 0; i < numCity; i++)
            {
                adjacency_matrix[i] = new int[5];//phan tu cuoi - adjacency_matrix[4] luu chieu dai con lai cua mang trong khi lai ghep
                for (int j = 0; j < 4; j++)
                {
                    adjacency_matrix[i][j] = -1;
                }
            }
            int[] rowLeng = new int[numCity]; //mang luu chieu dai cua cac neighborhood 

            bool[] flag = new bool[numCity]; //Mang danh dau cac thanh pho da duoc chon
            for (int i = 0; i < numCity; i++)
            {
                flag[i] = true;
                rowLeng[i] = 0;
            }

            //Tim cac dinh ke trong p1

            int idx = -1;
            for (int i = 0; i < numCity; i++)
            {
                idx = i + 1;
                if (idx >= numCity)
                {
                    idx = idx - numCity;
                }

                if (Array.IndexOf(adjacency_matrix[par1[i] - 1], par1[idx]) == -1)
                {
                    adjacency_matrix[par1[i] - 1][rowLeng[par1[i] - 1]] = par1[idx];//them neighborhood vao lan can
                    rowLeng[par1[i] - 1]++;
                }

                idx = i - 1;
                if (idx < 0)
                {
                    idx = idx + numCity;
                }

                if (Array.IndexOf(adjacency_matrix[par1[i] - 1], par1[idx]) == -1)
                {
                    adjacency_matrix[par1[i] - 1][rowLeng[par1[i] - 1]] = par1[idx];//them neighborhood vao lan can
                    rowLeng[par1[i] - 1]++;
                }
            }

            //Tim cac dinh ke trong p2
            for (int i = 0; i < numCity; i++)
            {
                idx = i + 1;
                if (idx >= numCity)
                {
                    idx = idx - numCity;
                }

                if (Array.IndexOf(adjacency_matrix[par2[i] - 1], par2[idx]) == -1)
                {
                    adjacency_matrix[par2[i] - 1][rowLeng[par2[i] - 1]] = par2[idx];//them neighborhood vao lan can
                    rowLeng[par2[i] - 1]++;
                }


                idx = i - 1;
                if (idx < 0)
                {
                    idx = idx + numCity;
                }
                if (Array.IndexOf(adjacency_matrix[par2[i] - 1], par2[idx]) == -1)
                {
                    adjacency_matrix[par2[i] - 1][rowLeng[par2[i] - 1]] = par2[idx];//them neighborhood vao lan can
                    rowLeng[par2[i] - 1]++;
                }
            }

            child[0] = par2[0];
            //}

            idx = 0;
            for (int i = 1; i < numCity; i++)
            {
                //01. Xoa phan tu trong mang trang lien ke
                flag[child[i - 1] - 1] = false;

                for (int j = 0; j < numCity; j++)
                {
                    if (Array.IndexOf(adjacency_matrix[j], child[i - 1]) != -1)
                    {
                        rowLeng[j]--;
                    }
                }
                //02. Tim Neighborhood co chieu dai ngan nhat
                if (rowLeng[child[i - 1] - 1] > 0)
                {
                    int minLength = 5;
                    for (int j = 0; j < rowLeng[child[i - 1] - 1]; j++)
                    {
                        if ((rowLeng[adjacency_matrix[child[i - 1] - 1][j] - 1] < minLength) && (rowLeng[adjacency_matrix[child[i - 1] - 1][j] - 1] > 0) &&
                            (flag[adjacency_matrix[child[i - 1] - 1][j] - 1]))
                        {
                            minLength = rowLeng[adjacency_matrix[child[i - 1] - 1][j] - 1];
                        }
                    }
                    //chon ngau nhien neu co nhieu Neighborhood co cung chieu dai
                    int countMin = 0;
                    for (int j = 0; j < rowLeng[child[i - 1] - 1]; j++)
                    {
                        if ((rowLeng[adjacency_matrix[child[i - 1] - 1][j] - 1] == minLength) && (flag[adjacency_matrix[child[i - 1] - 1][j] - 1]))
                        {
                            countMin++;
                        }
                    }
                    //chon ngau nhien list ngan nhat
                    int randIdxNeighborhood = 0;
                    if (countMin > 1)
                    {
                        randIdxNeighborhood = ran.Next(countMin);
                    }
                    else
                    {
                        randIdxNeighborhood = countMin;
                    }

                    countMin = 0;
                    for (int j = 0; j < rowLeng[child[i - 1] - 1]; j++)
                    {
                        if ((rowLeng[adjacency_matrix[child[i - 1] - 1][j] - 1] == minLength) && (flag[adjacency_matrix[child[i - 1] - 1][j] - 1]))
                        {
                            if (countMin == randIdxNeighborhood)
                            {
                                idx = j;
                                break;
                            }
                            countMin++;
                        }
                    }
                }
                else
                {
                    //Khong con Neighborhood -> chon ngau nhien mot trong cac thanh pho chua co.
                    int count = 0;
                    for (int j = 0; j < numCity; j++)
                    {
                        if (flag[j])
                        {
                            count++;
                        }
                    }
                    idx = ran.Next(count);
                    count = 0;
                    for (int j = 0; j < numCity; j++)
                    {
                        if (flag[j])
                        {
                            if (count == idx)
                            {
                                idx = j;
                                break;
                            }
                            count++;
                        }
                    }
                }

                //gan cho ca the con
                child[i] = adjacency_matrix[child[i - 1] - 1][idx];
            }

        }


        #endregion


        #region Prufer Tree
        // one_Point_Crossover trong code cu
        private void prufer_One_Point_Crossover(int[] par1, int[] par2, int numGenes, int[] child1, int[] child2, Random rnd)
        {
            int point = rnd.Next(numGenes);
            for (int i = 0; i < point; i++)
            {
                child1[i] = par1[i];
                child2[i] = par2[i];
            }
            for (int i = point; i < numGenes; i++)
            {
                child1[i] = par2[i];
                child2[i] = par1[i];
            }
        }


        #endregion


        #region Edges set
        /*********************************************************************************************************************************************
        *  G. R. Raidl and B. A. Julstrom, “Edge sets: an effective evolutionary coding of spanning trees,” IEEE Transactions on evolutionary computation, vol. 7, no. 3, pp. 225–239, 2003.
        *  Lai ghép cho biểu diễn cây sử dụng thuậ toán PrimRST để tạo cây
        *  
        ********************************************************************************************************************************************/
        private double[,] primRST_Crossover(double[,] par1, double[,] par2, int num_Vertex, Random rnd)
        {
            double[,] G_cr = new double[num_Vertex, num_Vertex];
            int[,] aaa = new int[num_Vertex, num_Vertex];
            Initialize_Chromosome init_Chrome = new Initialize_Chromosome();
            //01. Tao do thi Gcr
            for (int i = 0; i < num_Vertex; i++)
            {
                for (int j = 0; j < num_Vertex; j++)
                {
                    G_cr[i, j] = par1[i, j] + par2[i, j];
                }
            }


            ReadWriteFile ioFile = new ReadWriteFile();
            //for (int i = 0; i < num_Vertex; i++)
            //{
            //    for (int j = 0; j < num_Vertex; j++)
            //    {
            //        aaa[i, j] = (int)par1[i, j];
            //    }
            //}
            //ioFile.draw_Plot_in_Matlab("par_1" + @".m", num_Vertex, aaa);
            //for (int i = 0; i < num_Vertex; i++)
            //{
            //    for (int j = 0; j < num_Vertex; j++)
            //    {
            //        aaa[i, j] = (int)par2[i, j];
            //    }
            //}
            //ioFile.draw_Plot_in_Matlab("par_2" + @".m", num_Vertex, aaa);

            //for (int i = 0; i < num_Vertex; i++)
            //{
            //    for (int j = 0; j < num_Vertex; j++)
            //    {
            //        aaa[i, j] = (int)par1[i, j] + (int)par2[i, j];
            //    }
            //}
            //ioFile.draw_Plot_in_Matlab("aaa" + @".m", num_Vertex, aaa);

            //02. Ap dung PrimRST de tao ca the con
            double[,] aaaaa = init_Chrome.primRST(num_Vertex, G_cr, rnd);

            //if (aaaaa == null)
            //{
            //    //ReadWriteFile ioFile = new ReadWriteFile();
            //    ioFile.writeMatrixToFile("G_cr.txt", G_cr, num_Vertex, false);
            //    Console.WriteLine("null child");
               
            //    for (int i = 0; i < num_Vertex; i++)
            //    {
            //        for (int j = 0; j < num_Vertex; j++)
            //        {
            //            aaa[i, j] = (int)par1[i, j];
            //        }
            //    }
            //    ioFile.draw_Plot_in_Matlab("par_1" + @".m", num_Vertex, aaa);
            //    for (int i = 0; i < num_Vertex; i++)
            //    {
            //        for (int j = 0; j < num_Vertex; j++)
            //        {
            //            aaa[i, j] = (int)par2[i, j];
            //        }
            //    }
            //    ioFile.draw_Plot_in_Matlab("par_2" + @".m", num_Vertex, aaa);

            //    for (int i = 0; i < num_Vertex; i++)
            //    {
            //        for (int j = 0; j < num_Vertex; j++)
            //        {
            //            aaa[i, j] = (int)par1[i, j] + (int)par2[i, j];
            //        }
            //    }
            //    ioFile.draw_Plot_in_Matlab("aaa" + @".m", num_Vertex, aaa);
            //    Console.ReadKey();
            //}

            //for (int i = 0; i < num_Vertex; i++)
            //{
            //    for (int j = 0; j < num_Vertex; j++)
            //    {
            //        aaa[i, j] = (int)aaaaa[i, j];
            //    }
            //}
            //ioFile.draw_Plot_in_Matlab("child" + @".m", num_Vertex, aaa);
           
            return aaaaa;
        }


        #endregion


        #region Clustered Tree
    

        /*********************************************************************************************************************************************
         *  Tìm ma trận liên kết các cluster từ cây khung
         * + Hướng 1: Chỉ tìm liên kết không quan tâm tới trọng số <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
         * + Hướng 2: Tìm liên kết có lưu trọng số để thực hiện lai ghép về sau
         ********************************************************************************************************************************************/
        private double[,] find_Spanning_Tree_Between_Clusters(double[,] par_1, int num_Vertex, int num_Cluster, int[][] vertex_In_Cluster)
        {
            double[,] T = new double[num_Cluster, num_Cluster];
            //Khoi tao
            for (int i = 0; i < num_Cluster; i++)
            {
                for (int j = i; j < num_Cluster; j++)
                {
                    T[i, j] = 0;
                    T[j, i] = 0;
                }
            }

            int num_Vertex_in_Cluster_1 = 0;
            int num_Vertex_in_Cluster_2 = 0;
            for (int i = 0; i < num_Cluster; i++)
            {
                num_Vertex_in_Cluster_1 = vertex_In_Cluster[i].Length;
                for(int j = 0; j < num_Vertex_in_Cluster_1; j++)
                {
                    for (int k = i + 1; k < num_Cluster; k++)
                    {
                        num_Vertex_in_Cluster_2 = vertex_In_Cluster[k].Length;
                        for(int t = 0; t < num_Vertex_in_Cluster_2; t++)
                        {
                            if(par_1[vertex_In_Cluster[i][j],vertex_In_Cluster[k][t]] > 0)
                            {
                                T[i, k] = par_1[vertex_In_Cluster[i][j], vertex_In_Cluster[k][t]];
                                T[k, i] = T[i, k]; 
                            }
                        }
                    }
                }
            }
            return T;
        }

        /*********************************************************************************************************************************************
         *  Tìm ma trận liên kết các cluster từ cây khung
         * + Hướng 1: Chỉ tìm liên kết không quan tâm tới trọng số <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
         * + Hướng 2: Tìm liên kết có lưu trọng số để thực hiện lai ghép về sau
         * 
         * =>Thay đổi so với method trên: chọn ngẫu nhiên 2 đỉnh thuộc 2 cluster khác nhau, nếu có cạnh thì dừng
         ********************************************************************************************************************************************/
        private double[,] find_Spanning_Tree_Between_Clusters_1(double[,] par_1, int num_Vertex, int num_Cluster, int[][] vertex_In_Cluster)
        {
            double[,] T = new double[num_Cluster, num_Cluster];
            //Khoi tao
            for (int i = 0; i < num_Cluster; i++)
            {
                for (int j = i; j < num_Cluster; j++)
                {
                    T[i, j] = 0;
                    T[j, i] = 0;
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
                            if (par_1[vertex_In_Cluster[i][j], vertex_In_Cluster[k][t]] > 0)
                            {
                                T[i, k] = par_1[vertex_In_Cluster[i][j], vertex_In_Cluster[k][t]];
                                T[k, i] = T[i, k];
                            }
                        }
                    }
                }
            }
            return T;
        }

        /*********************************************************************************************************************************************
        * Tìm ma trận liên kết các cluster từ cây khung
        * + Hướng 1: Chỉ tìm liên kết không quan tâm tới trọng số 
        * + Hướng 2: Tìm liên kết có lưu trọng số, và các cạnh nối giữa các cluster để thực hiện lai ghép về sau <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
        **********************************************************************************************************************************************/
      
        private double[,] primRST_Cluster_Crossover(double[,] par_1, double[,] par_2, int num_Vertex, int num_Cluster, int[][] vertex_In_Cluster, Random rnd)
        {
            double[,] G_cr = new double[num_Vertex, num_Vertex];
            double[,] T = new double[num_Vertex, num_Vertex];
            Initialize_Chromosome init_Chrome = new Initialize_Chromosome();

            //Khoi tao
            for (int i = 0; i < num_Vertex; i++)
            {
                for (int j = i; j < num_Vertex; j++)
                {
                    T[i, j] = 0;
                    T[j, i] = 0;
                }
            }


            //Ap dung PrimRST crossover cho tung cluster
            double[,] weight_Cluster_Matrix_Par_1, weight_Cluster_Matrix_Par_2;
            double[,] spanning_Tree_of_Cluster;
            int num_Vertex_in_Cluster = 0;
            for(int i = 0; i < num_Cluster; i ++)
            {
                num_Vertex_in_Cluster = vertex_In_Cluster[i].Length;
                weight_Cluster_Matrix_Par_1 = init_Chrome.create_Weight_Matrix_for_Cluster(num_Vertex, num_Vertex_in_Cluster, par_1, vertex_In_Cluster[i]);
                weight_Cluster_Matrix_Par_2 = init_Chrome.create_Weight_Matrix_for_Cluster(num_Vertex, num_Vertex_in_Cluster, par_2, vertex_In_Cluster[i]);
                spanning_Tree_of_Cluster = primRST_Crossover(weight_Cluster_Matrix_Par_1, weight_Cluster_Matrix_Par_2, num_Vertex_in_Cluster, rnd);
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
            //01. Tim cạnh liên kết các nhóm của cha mẹ => các cạnh của cha mẹ có thể liên kết tới các đỉnh khác nhau của cùng 1 nhóm =>  
            weight_Cluster_Matrix_Par_1 = find_Spanning_Tree_Between_Clusters(par_1, num_Vertex, num_Cluster, vertex_In_Cluster);
            weight_Cluster_Matrix_Par_2 = find_Spanning_Tree_Between_Clusters(par_2, num_Vertex, num_Cluster, vertex_In_Cluster);
            
            //02. Tạo các thể con cho các cạnh nay
            spanning_Tree_of_Cluster = primRST_Crossover(weight_Cluster_Matrix_Par_1, weight_Cluster_Matrix_Par_2, num_Cluster, rnd);
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
                        T[idx_Cluster[k], idx_Cluster[j]] = spanning_Tree_of_Cluster[k, j];
                    }
                }
            }
            return T;
        }

        #endregion

        /*********************************************************************************************************************************************
         *  - PrimRST_Crossover: Chi tao ra 1 ca the con
         *  - num_Genens:
         *      + PRUFER_ONE_POINT_CROSSOVER: Num_Vertex - 2
         *      + PRIMRST_CROSSOVER: Num_Vertex
         ********************************************************************************************************************************************/

        public void MFO_Crossover(Crossover_Parameters crss_Para, Chromosome par_1, Chromosome par_2, int num_Genens, int num_Tasks, ref List<Chromosome> child, Tasks tsk, Random rnd)
        {
            Chromosome child_1;
            Chromosome child_2;
            switch (crss_Para.Crossver_Name)
            {
                case "REAL_CROSSVER":
                    #region REAL_CROSSVER
                    child_1 = new Chromosome(num_Genens, num_Tasks);
                    child_1.Rnvec = real_Crossover(par_1.Rnvec, par_2.Rnvec, num_Genens, crss_Para.Cf);
                    child.Add(child_1);
                    child_2 = new Chromosome(num_Genens, num_Tasks);
                    child_2.Rnvec = real_Crossover(par_1.Rnvec, par_2.Rnvec, num_Genens, crss_Para.Cf);
                    child.Add(child_2);
                    #endregion
                    break;
                case "ONE_POINT_CROSSOVER":
                    #region ONE_POINT_CROSSOVER
                    child_1 = new Chromosome(num_Genens, num_Tasks);
                    child_2 = new Chromosome(num_Genens, num_Tasks);
                    one_Point_Crossover(par_1.Rnvec, par_2.Rnvec, num_Genens, child_1.Rnvec, child_2.Rnvec, rnd);
                    child.Add(child_1);
                    child.Add(child_2);
                    #endregion
                    break;
                case "PMX":
                    #region PMX
                    child_1 = new Chromosome(num_Genens, num_Tasks);
                    child_2 = new Chromosome(num_Genens, num_Tasks);
                    PMX(par_1.Invec, par_2.Invec, num_Genens, child_1.Invec, child_2.Invec, rnd);
                    child.Add(child_1);
                    child.Add(child_2);
                    #endregion
                    break;
                case "PRUFER_ONE_POINT_CROSSOVER":
                    #region PRUFER_ONE_POINT_CROSSOVER
                    child_1 = new Chromosome(num_Genens, num_Tasks);
                    child_2 = new Chromosome(num_Genens, num_Tasks);
                    prufer_One_Point_Crossover(par_1.Invec, par_2.Invec, num_Genens, child_1.Invec, child_2.Invec, rnd);
                    child.Add(child_1);
                    child.Add(child_2);
                    #endregion
                    break;
                case "PRIMRST_CROSSOVER":
                    #region PRIMRST_CROSSOVER
                    //if (!init_Chrom.check_Symetric_Matrix(num_Genens, par_1.Edges_Matrix))
                    //{
                    //    Console.WriteLine("Par_1 ko doi xung");
                    //    ioFile.writeMatrixToFile("Par_1_truoc_lai_ghep.txt", par_1.Edges_Matrix, num_Genens, false);
                    //    Console.ReadKey();
                    //    return;
                    //}
                    //if (!init_Chrom.check_Symetric_Matrix(num_Genens, par_2.Edges_Matrix))
                    //{
                    //    Console.WriteLine("Par_2 ko doi xung");
                    //    ioFile.writeMatrixToFile("Par_2_truoc_lai_ghep.txt", par_1.Edges_Matrix, num_Genens, false);
                    //    Console.ReadKey();
                    //    return;
                    //}

                    child_1 = new Chromosome(num_Genens, num_Tasks);
                    child_1.Edges_Matrix = primRST_Crossover(par_1.Edges_Matrix, par_2.Edges_Matrix, num_Genens, rnd);
                    child.Add(child_1);
                    #endregion
                    break;
                case "PRIMRST_CLUSTER_CROSSOVER":
                    #region PRIMRST_CLUSTER_CROSSOVER

                    //if (!init_Chrom.check_Symetric_Matrix(num_Genens, par_1.Edges_Matrix))
                    //{
                    //    Console.WriteLine("Par_1 ko doi xung");
                    //    ioFile.writeMatrixToFile("Par_1_truoc_lai_ghep.txt", par_1.Edges_Matrix, num_Genens, false);
                    //    Console.ReadKey();
                    //    return;
                    //}

                    //int[,] aaa = new int[num_Genens, num_Genens];
                    //for (int i = 0; i < num_Genens; i++)
                    //{
                    //    for (int j = 0; j < num_Genens; j++)
                    //    {
                    //        aaa[i, j] = (int)par_1.Edges_Matrix[i, j];
                    //    }
                    //}
                    //ioFile.draw_Plot_in_Matlab("Par_1" + @".m", num_Genens, aaa);

                    //if (!init_Chrom.check_Symetric_Matrix(num_Genens, par_2.Edges_Matrix))
                    //{
                    //    Console.WriteLine("Par_2 ko doi xung");
                    //    ioFile.writeMatrixToFile("Par_2_truoc_lai_ghep.txt", par_1.Edges_Matrix, num_Genens, false);
                    //    Console.ReadKey();
                    //    return;
                    //}

                    //for (int i = 0; i < num_Genens; i++)
                    //{
                    //    for (int j = 0; j < num_Genens; j++)
                    //    {
                    //        aaa[i, j] = (int)par_2.Edges_Matrix[i, j];
                    //    }
                    //}
                    //ioFile.draw_Plot_in_Matlab("Par_2" + @".m", num_Genens, aaa);

                    child_1 = new Chromosome(num_Genens, num_Tasks);
                    child_1.Edges_Matrix = primRST_Cluster_Crossover(par_1.Edges_Matrix, par_2.Edges_Matrix, num_Genens, tsk.Num_Cluster, tsk.Vertex_In_Cluster, rnd);
                    child.Add(child_1);

                    //double[,] temp_Matrix = new double[num_Genens, num_Genens];
                    //for (int i = 0; i < num_Genens; i++)
                    //{
                    //    for (int j = 0; j < num_Genens; j++)
                    //    {
                    //        if ((child_1.Edges_Matrix[i, j] > 0) && (child_1.Edges_Matrix[i, j] < double.MaxValue))//neu co canh
                    //        {
                    //            temp_Matrix[i, j] = child_1.Edges_Matrix[i, j];
                    //        }
                    //        else
                    //        {
                    //            temp_Matrix[i, j] = 0;
                    //        }
                    //    }
                    //}
                   

                    //int[,] aaa = new int[num_Genens, num_Genens];
                    //for (int i = 0; i < num_Genens; i++)
                    //{
                    //    for (int j = 0; j < num_Genens; j++)
                    //    {
                    //        aaa[i, j] = (int)child_1.Edges_Matrix[i, j];
                    //    }
                    //}
                    //ioFile.draw_Plot_in_Matlab("Child" + @".m", num_Genens, aaa);
                    
                    //Graph_Method graph_Method_Class = new Graph_Method();
                    //graph_Method_Class.dijkstra(temp_Matrix, num_Genens, 0);

                    #endregion
                    break;
            }
        }


    }
}
