using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DTO;
using System.Collections;


namespace BLL
{
    public class Graph_Method
    {
        Initialize_Chromosome init_Chrom_Class = new Initialize_Chromosome();

        //Thuật toán duyệt theo chiều rộng bắt đầu từ đỉnh: start_Vertex
        public int[] BFS(double[,] weigh_Matrix, int num_Vertex, int start_Vertex)
        {
            int[] QUEUE = new int[num_Vertex];
            bool[] chua_Xet = new bool[num_Vertex];
            int[] truoc = new int[num_Vertex];//ko can vi ko can tim duoc di
            int u, dauQ, cuoiQ;

            for (int i = 0; i < num_Vertex; i++)
            {
                chua_Xet[i] = true;
                truoc[i] = start_Vertex;
                QUEUE[i] = -1;
            }
            truoc[start_Vertex] = -1;
            dauQ = 0;
            cuoiQ = 0;
            QUEUE[cuoiQ] = start_Vertex;
            chua_Xet[start_Vertex] = false;

            while (dauQ <= cuoiQ)
            {
                u = QUEUE[dauQ];
                dauQ++;
                for (int i = 0; i < num_Vertex; i++)
                {

                    if (weigh_Matrix[u, i] == double.MaxValue)
                    {
                        continue;
                    }
                    else
                    {
                        if ((weigh_Matrix[u, i] > 0) && (chua_Xet[i]))
                        {
                            cuoiQ++;
                            QUEUE[cuoiQ] = i;
                            chua_Xet[i] = false;
                            truoc[i] = u;
                        }
                    }
                }//for
            }//while
            return truoc;
        }

        //Thuat toan in duong di su dung trong Dijstra va DFS, BFS,..
        public List<int> print_Path(int start_Vertex, int end_Vertex, int[] pre)
        {
            List<int> tmp_Path = new List<int>();

            if (pre[end_Vertex] == -1)
            {
                //Khong co duong di tu start_Vertex -> end_Vertex
            }
            else
            {
                int j = end_Vertex;
                while (j != start_Vertex)
                {
                    tmp_Path.Add(j);
                    j = pre[j];
                }
                tmp_Path.Add(start_Vertex);
            }
            return tmp_Path;
        }

        //Thuat toan Dijkstra tim duong di ngan nhat tu dinh start_Vertex toi dinh end_Vertex
        //Cai dat theo giao trinh: Toan roi rac (Nguyen Duc Nghia - Nguyen To Thanh)
        public int[] dijkstra_Between_Two_Vertex(double[,] weigh_Matrix, int num_Vertex, int start_Vertex, int end_Vertex)
        {
            int[] truoc = new int[num_Vertex];
            double[] d = new double[num_Vertex];
            bool[] final_Vertex = new bool[num_Vertex];
            double minp = double.MaxValue;
            int u = 0;
            //Khoi tao nhan
            for (int v = 0; v < num_Vertex; v++)
            {
                if (weigh_Matrix[start_Vertex, v] <= 0)
                {
                    d[v] = double.MaxValue;
                }
                else
                {
                    d[v] = weigh_Matrix[start_Vertex, v];
                }
                truoc[v] = start_Vertex;
                final_Vertex[v] = false;
            }

            truoc[start_Vertex] = -1;
            d[start_Vertex] = 0;
            final_Vertex[start_Vertex] = true;

            while (!final_Vertex[end_Vertex])
            {
                //Tim u la dinh co nhan tam thoi nho nhat
                minp = double.MaxValue;
                u = -1;
                for (int v = 0; v < num_Vertex; v++)
                {
                    if ((!final_Vertex[v]) && (minp > d[v]))
                    {
                        u = v;
                        minp = d[v];
                    }
                }
                final_Vertex[u] = true;
                if (!final_Vertex[end_Vertex])
                {
                    for (int v = 0; v < num_Vertex; v++)
                    {
                        if ((d[u] == double.MaxValue) || (weigh_Matrix[u, v] == double.MaxValue))
                        {
                            break;
                        }
                        else
                        {
                            if ((!final_Vertex[v]) && (d[u] + weigh_Matrix[u, v] < d[v]) && (weigh_Matrix[u, v] > 0))
                            {
                                d[v] = d[u] + weigh_Matrix[u, v];
                                truoc[v] = u;
                            }
                        }

                    }
                }
            }
            return truoc;
        }
        
        //Thuat toan Dijkstra tim duong di ngan nhat
        public int[] dijkstra(double[,] weigh_Matrix, int num_Vertex, int start_Vertex)
        {
            int[] truoc = new int[num_Vertex];
            double[] d = new double[num_Vertex];
            //bool[] final_Vertex = new bool[num_Vertex];
            List<int> T = new List<int>();
            double minp = double.MaxValue;
            int u = 0;

            //int[,] aaa = new int[num_Vertex, num_Vertex];
            //for (int i = 0; i < num_Vertex; i++)
            //{
            //    for (int j = 0; j < num_Vertex; j++)
            //    {
            //        aaa[i, j] = (int)weigh_Matrix[i, j];
            //    }
            //}
            //ReadWriteFile ioFile = new ReadWriteFile();
            //ioFile.draw_Plot_in_Matlab("in_Distra" + @".m", num_Vertex, aaa);


            //Khoi tao nhan
            for (int v = 0; v < num_Vertex; v++)
            {
                if ((weigh_Matrix[start_Vertex, v] <= 0) || (weigh_Matrix[start_Vertex, v] == double.MaxValue))
                {
                    d[v] = double.MaxValue;
                }
                else
                {
                    d[v] = weigh_Matrix[start_Vertex, v];
                }
                truoc[v] = start_Vertex;
                T.Add(v);
            }

            truoc[start_Vertex] = -1;
            d[start_Vertex] = 0;
            T.Remove(start_Vertex);

            while (T.Count > 0)
            {
                //Tim u la dinh co nhan tam thoi nho nhat
                minp = double.MaxValue;
                u = -1;

                foreach (int v in T)
                {
                    if (minp > d[v])
                    {
                        u = v;
                        minp = d[v];
                    }
                }
                T.Remove(u);
                foreach (int v in T)
                {
                    if ((d[u] == double.MaxValue) || (weigh_Matrix[u, v] == double.MaxValue))
                    {
                        break;
                    }
                    else
                    {
                        if ((d[u] + weigh_Matrix[u, v] < d[v]) && (weigh_Matrix[u, v] > 0))
                        {
                            d[v] = d[u] + weigh_Matrix[u, v];
                            truoc[v] = u;
                        }
                    }

                }
            }
            return truoc;
        }

        //Theo TOAN ROI RAC (Nguyen Duc Nghia - Nguyen To Thanh) - Trang 212
        public double[,] prim(double[,] weight_Matrix, int num_Verties, Random rnd)
        {
            int rnd_Verties = -1,
                idx_Rnd_Edge = -1;
            List<int> Vh = new List<int>();
            List<int> V_Vh = new List<int>();       //Tập đỉnh V\Vh
            double[] d = new double[num_Verties];   //Độ dài nhỏ nhất từ đỉnh không thuộc cây khung tới cây khung
            int[] near = new int[num_Verties];      //Đỉnh gần cây khung nhất
            double[,] T = new double[num_Verties, num_Verties];
            for (int i = 0; i < num_Verties; i++)
            {
                for (int j = i; j < num_Verties; j++)
                {
                    T[i, j] = 0;
                    T[j, i] = 0;
                }
                V_Vh.Add(i);
                d[i] = double.MaxValue;
                near[i] = -1;
            }

            if(num_Verties==1)
            {
                return T;
            }

            //Khoi tao
            rnd_Verties = rnd.Next(num_Verties);
            Vh.Add(rnd_Verties);
            d[rnd_Verties] = 0;
            near[rnd_Verties] = rnd_Verties;
            V_Vh.Remove(rnd_Verties);

            foreach (int v in V_Vh)
            {
                if (weight_Matrix[rnd_Verties, v] == 0)
                {
                    d[v] = double.MaxValue;
                }
                else
                {
                    d[v] = weight_Matrix[rnd_Verties, v];
                }

                near[v] = rnd_Verties;
            }

            //Buoc lap
            bool stop = false;
            while (!stop)
            {
                //01. Tim dinh u co khoang cach toi cay khung nho nhat
                double tmp = double.MaxValue;
                int idx_Min_Vertex = -1;
                foreach (int u in V_Vh)
                {
                    if ((d[u] < tmp) && (d[u] > 0))
                    {
                        idx_Min_Vertex = u;
                        tmp = d[u];
                    }
                }
                //02. 
                Vh.Add(idx_Min_Vertex);
                V_Vh.Remove(idx_Min_Vertex);
                T[idx_Min_Vertex, near[idx_Min_Vertex]] = 1.0f;
                T[near[idx_Min_Vertex], idx_Min_Vertex] = 1.0f;

                if (Vh.Count == num_Verties)
                {
                    //Tim duoc cay khung nho nhat
                    stop = true;
                }
                else
                {
                    foreach (int v in V_Vh)
                    {
                        if ((d[v] > weight_Matrix[idx_Min_Vertex, v]) && (weight_Matrix[idx_Min_Vertex, v] > 0))
                        {
                            d[v] = weight_Matrix[idx_Min_Vertex, v];
                            near[v] = idx_Min_Vertex;
                        }
                    }
                }
            }
            return T;
        }

        private int[] BFS(double[,] weigh_Matrix, int num_Vertex, ref int[] chua_Xet, int solt, int start_Vertex)
        {
            int[] QUEUE = new int[num_Vertex];
            //bool[] chua_Xet = new bool[num_Vertex];
            int[] truoc = new int[num_Vertex];//ko can vi ko can tim duoc di
            int u, dauQ, cuoiQ;

            for (int i = 0; i < num_Vertex; i++)
            {
                //chua_Xet[i] = true;
                truoc[i] = start_Vertex;
                QUEUE[i] = -1;
            }
            truoc[start_Vertex] = -1;
            dauQ = 0;
            cuoiQ = 0;
            QUEUE[cuoiQ] = start_Vertex;
            chua_Xet[start_Vertex] = solt;

            while (dauQ <= cuoiQ)
            {
                u = QUEUE[dauQ];
                dauQ++;
                for (int i = 0; i < num_Vertex; i++)
                {

                    if (weigh_Matrix[u, i] == double.MaxValue)
                    {
                        continue;
                    }
                    else
                    {
                        if ((weigh_Matrix[u, i] > 0) && (chua_Xet[i] == -1))
                        {
                            cuoiQ++;
                            QUEUE[cuoiQ] = i;
                            chua_Xet[i] = solt;
                            truoc[i] = u;
                        }
                    }
                }//for
            }//while
            return truoc;
        }

        //
        private void dem_So_Thanh_Phan_Lien_Thong(double[,] weigh_Matrix, int num_Vertex)
        {
            int[] chua_Xet = new int[num_Vertex];
            int solt = 0;
            for (int i = 0; i < num_Vertex; i++)
            {
                chua_Xet[i] = -1;
            }
            for (int i = 0; i < num_Vertex; i++)
            {
                if (chua_Xet[i] == -1)
                {

                    BFS(weigh_Matrix, num_Vertex, ref chua_Xet, solt, i);
                    solt++;
                }
            }
            //for (int i = 0; i < solt; i++)
            //{
            //    Console.WriteLine("Thanh phan lien thong " + i + " gom cac dinh: ");
            //    for (int j = 0; j < num_Vertex; j++)
            //    {
            //        if (chua_Xet[j] == i)
            //        {
            //            Console.WriteLine(j + "    ");
            //        }
            //    }
            //    Console.WriteLine();
            //}
           
            //return chua_Xet;
        }

        //Hàm trả về số thành phần liên thông, và lấy mỗi thành phần liên thông 1 đỉnh đầu tiên đưa ra mảng. ===> lay dinh cuoi cung theo thu tu duyet theo chieu dong cua tp lien thong
        public int[] get_Vertex_In_Each_SubGraph(double[,] weigh_Matrix, int num_Vertex)
        {
            int[] chua_Xet = new int[num_Vertex];
            int solt = 0;
            for (int i = 0; i < num_Vertex; i++)
            {
                chua_Xet[i] = -1;
            }
            for (int i = 0; i < num_Vertex; i++)
            {
                if (chua_Xet[i] == -1)
                {

                    BFS(weigh_Matrix, num_Vertex, ref chua_Xet, solt, i);
                    solt++;
                }
            }

            int[] vertex_In_SubGraph = new int[solt];
            for (int i = 0; i < solt; i++)
            {
                for (int j = 0; j < num_Vertex; j++)
                {
                    if (chua_Xet[j] == i)
                    {
                        vertex_In_SubGraph[i] = j;
                    }
                }
            }

            return vertex_In_SubGraph;
        }


    }
}
