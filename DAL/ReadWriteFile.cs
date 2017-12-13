using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DTO;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Diagnostics;
using System.Reflection;

namespace DAL
{
    public class ReadWriteFile
    {

        public double calculateDistance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
        }

        public double[,] read_TSP_Instance(string filePath, out int numOfCity)
        {
            numOfCity = -1;
            if(!File.Exists(filePath))
            {
                return null;
            }
            StreamReader doc = new StreamReader(filePath);
            int maxIndex = 1;
            string line = "";
            string pattern = " ";
            Regex myRegex = new Regex(pattern);
            Regex rg = new Regex(@"\s+");

            bool readCluster = false;
            bool readWeight = false;
            string[] temp;
            int index = 0;
            double[,] weightMatrix;
            double[] x;             //Toa do x trong truong hop input la toa do cac thanh pho
            double[] y;             //Toa do y trong truong hop input la toa do cac thanh pho

            while ((line = doc.ReadLine()) != null)
            {
                if (line.IndexOf("DIMENSION") != -1)
                {
                    maxIndex = Convert.ToInt32(line.Substring(line.LastIndexOf(" "), line.Length - line.LastIndexOf(" ")));
                    break;
                }
            }

            numOfCity = maxIndex;
            weightMatrix = new double[maxIndex, maxIndex];
            x = new double[maxIndex];
            y = new double[maxIndex];

            string edge_weight_format = "";
            while ((line = doc.ReadLine()) != null)
            {
                line = line.Trim();
                line = rg.Replace(line, " ");

                if (string.IsNullOrWhiteSpace(line) || (string.IsNullOrEmpty(line)))
                {
                    continue;
                }
              
                if (line.IndexOf("EDGE_WEIGHT_FORMAT") != -1)
                {
                    edge_weight_format = line.Substring(line.LastIndexOf(" "), line.Length - line.LastIndexOf(" ")).Trim();
                    continue;
                }

                if ((line.IndexOf("EDGE_WEIGHT_SECTION") != -1) || (line.IndexOf("NODE_COORD_SECTION") != -1))
                {
                    readWeight = true;
                    continue;
                }

                //Doc du lieu la ma tran trong so hoac toa do cac dinh
                if (readWeight)
                {
                    switch (edge_weight_format)
                    {
                        case "FULL_MATRIX":
                            #region Full matrix
                            //weightMatrix = new double[numOfCity, numOfCity];
                            if (line == @"EOF")
                            {
                                break;
                            }
                            temp = myRegex.Split(line);
                            for (int j = 0; j < numOfCity; j++)
                            {
                                weightMatrix[index, j] = Convert.ToDouble(temp[j]);
                            }
                            index++;
                            break;
                            #endregion

                        default:
                            #region Toa do cac dinh + Uclid 2D
                            //doc toa do cac dinh
                            //citiesCoordinate = new CityObject[maxIndex];
                        
                            if (Char.IsNumber(line[0]))
                            {
                                temp = myRegex.Split(line);
                                index = Convert.ToInt32(temp[0]);
                                x[index - 1] = Double.Parse(temp[1], System.Globalization.NumberStyles.Float);
                                y[index - 1] = Double.Parse(temp[2], System.Globalization.NumberStyles.Float);
                            }
                            break;
                            #endregion
                    }
                }
            }//while

            doc.Dispose();
            doc.Close();

            //Chuyển thành ma trận trọng số trong trường hợp dữ liệu là tọa độ các đỉnh
            if (string.IsNullOrEmpty(edge_weight_format))
            {
                for (int i = 0; i < maxIndex; i++)
                {
                    for (int j = 0; j < maxIndex; j++)
                    {
                        weightMatrix[i, j] = calculateDistance(x[i], y[i], x[j], y[j]);
                    }
                }
            }            
            return weightMatrix;
        }

        //In ket qua cac seed ra file tong hop
        //Trung binh cac seed bat  dau bang: -1
        public void writeOptimalSolutionofSeed(string fileName, int seed, double best_Ind_Fitness, string running_Time)
        {
            StreamWriter wr = new StreamWriter(fileName, true);
            try
            {
                if (seed == 1)
                {
                    wr.WriteLine(string.Format("Seed \t Values \t Times "));
                }
                wr.WriteLine(String.Format("{0}{1}{2}{3}", seed, "\t", best_Ind_Fitness, "\t", running_Time));
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                wr.Flush();
                wr.Close();
            }
        }

        //Edit from read_File_HTSP(..)
      
        #region Create instance for Clutered Tree
        //Create Clutered Tree intance from TSP-> tao bo du lieu Clutered Tree voi cac nhom ngau nhien tu file TSP
        public void create_Clustered_Intance(string tspFileName, string clusteredTreeFileName, int[] numCitiesInGroup, Random ran)
        {
            StreamReader doc = new StreamReader(tspFileName);
            StreamWriter wr = new StreamWriter(clusteredTreeFileName);

            int numOfCity = 1;
            string line = "";
            string pattern = " ";
            Regex myRegex = new Regex(pattern);
            Regex rg = new Regex(@"\s+");

            //line = Path.GetFileNameWithoutExtension(tspFileName);
            wr.WriteLine("NAME: " + clusteredTreeFileName);
            wr.WriteLine("TYPE: CLUSTERED_TREE");
            wr.WriteLine("COMMENT: ");

            string strTmp = "";
            for (int i = 0; i < line.Length; i++)
            {
                if (char.IsDigit(line[i]))
                {
                    strTmp = strTmp + line[i];
                }
            }
                               

            //bo thong tin khong can thiet
            while ((line = doc.ReadLine()) != null)
            {
                if (line.IndexOf("DIMENSION") != -1)
                {
                    numOfCity = Convert.ToInt32(line.Substring(line.LastIndexOf(" "), line.Length - line.LastIndexOf(" ")));
                    wr.WriteLine("DIMENSION: " + numOfCity);
                    continue;
                }

                if (line.IndexOf("NODE_COORD_SECTION") != -1)
                {
                    break;
                }
            }
            wr.WriteLine("NUMBER_OF_CLUSTER: " + numCitiesInGroup.Length.ToString());
            wr.WriteLine("EDGE_WEIGHT_TYPE: EUC_2D");
            wr.WriteLine("NODE_COORD_SECTION");

            while ((line = doc.ReadLine()) != "EOF")
            {
                if (string.IsNullOrWhiteSpace(line) || (string.IsNullOrEmpty(line)))
                {
                    continue;
                }
                wr.WriteLine(line);
            }

            //Ghi thong tin cac group
            int cities = 0;
            for (int i = 0; i < numCitiesInGroup.Length; i++)
            {
                cities = cities + numCitiesInGroup[i];
            }

            if (cities != numOfCity)
            {
                doc.Dispose();
                doc.Close();
                Console.WriteLine("TONG SO THANH PHO TRONG GROUP PHAI BANG: NUMCITIES");

                wr.Dispose();
                wr.Close();
                Console.ReadKey();
                return;
            }

            //Tp 1 la DEPOT
            wr.WriteLine("CLUSTER_SECTION:");
            wr.WriteLine("SOURCE_VERTEX: " + ran.Next(numOfCity));
            int[] tourTemp = new int[numOfCity];
            for (int i = 0; i < numOfCity; i++)
            {
                tourTemp[i] = i;
            }
            int count = numOfCity;
            int pos = 0;

            for (int i = 0; i < numCitiesInGroup.Length - 1; i++)
            {
                strTmp = (i + 1).ToString();//Index cua cluster
                for (int j = 0; j < numCitiesInGroup[i]; j++)
                {
                    pos = ran.Next(count);
                    strTmp = strTmp + " " + tourTemp[pos];
                    for (int k = pos; k < count - 1; k++)
                    {
                        tourTemp[k] = tourTemp[k + 1];
                    }
                    count--;
                }
                strTmp = strTmp + " -1";
                wr.WriteLine(strTmp);
            }

            //group cuoi
            strTmp = numCitiesInGroup.Length.ToString();
            for (int i = 0; i < count; i++)
            {
                strTmp = strTmp + " " + tourTemp[i];
            }
            strTmp = strTmp + " -1";
            wr.WriteLine(strTmp);

            wr.WriteLine("EOF");
            doc.Dispose();
            doc.Close();
            wr.Dispose();
            wr.Close();
        }
        
        public void read_File_Clusted_Tree(string filePath, out double[,] weightMatrix, out int[][] vertexInCluster, out int numVertex, out int numCluster, out int sourceVertex)
        {
            StreamReader doc = new StreamReader(filePath);
            int maxIndex = 1;
            string line = "";
            string pattern = " ";
            Regex myRegex = new Regex(pattern);
            Regex rg = new Regex(@"\s+");

            bool readCluster = false;
            bool readWeight = false;
            bool[] findDepot; //Su dung de tim dinh Depot
            string[] temp;
            int index = 0;


            numCluster = 0;
            sourceVertex = 0;

            while ((line = doc.ReadLine()) != null)
            {
                if (line.IndexOf("DIMENSION") != -1)
                {
                    maxIndex = Convert.ToInt32(line.Substring(line.LastIndexOf(" "), line.Length - line.LastIndexOf(" ")));
                    //break;
                    continue;
                }

                if ((line.IndexOf("GTSP_SETS") != -1) || (line.IndexOf("HTSP_SETS") != -1) || (line.IndexOf("NUMBER_OF_CLUSTERS") != -1))
                {
                    numCluster = Convert.ToInt32(line.Substring(line.LastIndexOf(" "), line.Length - line.LastIndexOf(" ")));
                    break;
                }
            }

            vertexInCluster = new int[numCluster][];

            numVertex = maxIndex;
            findDepot = new bool[numVertex];
            for (int i = 0; i < numVertex; i++)
            {
                findDepot[i] = true;
            }
            weightMatrix = new double[numVertex, numVertex];
            double[] x_coordinate = new double[maxIndex];
            double[] y_coordinate = new double[maxIndex];

            string edge_weight_format = "";

            while ((line = doc.ReadLine()) != null)
            {
                line = line.Trim();
                line = rg.Replace(line, " ");

                if (string.IsNullOrWhiteSpace(line) || (string.IsNullOrEmpty(line)))
                {
                    continue;
                }

                if (line.IndexOf("CLUSTER_SECTION") != -1)
                {
                    readCluster = true;
                    readWeight = false;
                    continue;
                }

                if (line.IndexOf("EDGE_WEIGHT_FORMAT") != -1)
                {
                    edge_weight_format = line.Substring(line.LastIndexOf(" "), line.Length - line.LastIndexOf(" ")).Trim();
                    continue;
                }

                if (line.IndexOf("EDGE_WEIGHT_SECTION") != -1)
                {
                    readWeight = true;
                    readCluster = false;
                    edge_weight_format = "FULL_MATRIX";//Nếu không có xác định "edge_weight_format" thì mặc định là "FULL_MATRIX"
                    continue;
                }

                if (line.IndexOf("NODE_COORD_SECTION") != -1)
                {
                    readWeight = true;
                    readCluster = false;
                    continue;
                }

                //doc cac dinh cua moi cluster
                if (readCluster)
                {
                    if (line.IndexOf("SOURCE_VERTEX") != -1)
                    {
                        sourceVertex = Convert.ToInt32(line.Substring(line.LastIndexOf(" "), line.Length - line.LastIndexOf(" ")));
                        continue;
                    }

                    if (Char.IsNumber(line[0]))
                    {
                        temp = myRegex.Split(line);
                        index = Convert.ToInt32(temp[0]);
                        vertexInCluster[index - 1] = new int[temp.Length - 2];
                        for (int i = 1; i < temp.Length - 1; i++)//temp[0] la chi so cua cluster, temp[n-1] = -1
                        {
                            vertexInCluster[index - 1][i - 1] = Convert.ToInt32(temp[i]);
                        }
                    }
                }

                //Doc du lieu la ma tran trong so hoac toa do cac dinh
                if (readWeight)
                {
                    switch (edge_weight_format)
                    {
                        case "FULL_MATRIX":
                            #region Full matrix
                            //weightMatrix = new double[numOfCity, numOfCity];
                            temp = myRegex.Split(line);
                            for (int j = 0; j < numVertex; j++)
                            {
                                weightMatrix[index, j] = Convert.ToDouble(temp[j]);
                            }
                            index++;
                            break;
                            #endregion

                        default:
                            #region Toa do cac dinh + Uclid 2D
                            //doc toa do cac dinh
                            //citiesCoordinate = new CityObject[maxIndex];
                            if (Char.IsNumber(line[0]))
                            {
                                temp = myRegex.Split(line);
                                index = Convert.ToInt32(temp[0]);
                                x_coordinate[index - 1] = Double.Parse(temp[1]);
                                y_coordinate[index - 1] = Double.Parse(temp[2]);
                            }
                            break;
                            #endregion

                    }
                }
            }
            //Chuyen toa do cac dinh thanh ma trang trong so
            if (edge_weight_format != "FULL_MATRIX")
            {
                for (int i = 0; i < numVertex; i++)
                {
                    for (int j = i + 1; j < numVertex; j++)
                    {
                        weightMatrix[i, j] = Math.Sqrt((x_coordinate[i] - x_coordinate[j]) * (x_coordinate[i] - x_coordinate[j])
                            + (y_coordinate[i] - y_coordinate[j]) * (y_coordinate[i] - y_coordinate[j]));
                        if(weightMatrix[i, j] == 0.0f)
                        {
                            weightMatrix[i, j] = 0.0000001f;
                        }
                        weightMatrix[j, i] = weightMatrix[i, j];  
                    }
                    weightMatrix[i, i] = 0;
                    //weightMatrix[i, i] = -1;
                }
            }
            doc.Dispose();
            doc.Close();
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
            StreamReader read_Data_File = new StreamReader(Data_File);
            StreamWriter wr;

            string line = "";
            string new_Instance = "";
            string pattern = " ";
            Regex myRegex = new Regex(pattern);
            Regex rg = new Regex(@"\s+");

            while ((new_Instance = read_Data_File.ReadLine()) != null)//doc file de lay file instance mo
            {
                new_Instance = new_Instance.Trim();
                new_Instance = rg.Replace(new_Instance, " ");
                if (string.IsNullOrWhiteSpace(new_Instance) || (string.IsNullOrEmpty(new_Instance)))
                {
                    continue;
                }
                StreamReader read_Source_File = new StreamReader(sourceFile);
                string new_Para_File = begin_Name + "_" + Path.GetFileNameWithoutExtension(Path.GetFileName(new_Instance)) + @".par";
                wr = new StreamWriter(new_Para_File);

                while ((line = read_Source_File.ReadLine()) != null)//doc file du lieu cu de copy sang file moi
                {
                    line = line.Trim();
                    line = rg.Replace(line, " ");

                    if (string.IsNullOrWhiteSpace(line) || (string.IsNullOrEmpty(line)))
                    {
                        continue;
                    }



                    if (line.IndexOf("PARAMETER_FILE") != -1)
                    {
                        wr.WriteLine("PARAMETER_FILE = " + new_Para_File);
                        continue;
                    }

                    if (line.IndexOf("INSTANCE_NAME") != -1)
                    {
                        wr.WriteLine("INSTANCE_NAME = " + new_Instance);
                        continue;
                    }

                    wr.WriteLine(line);
                }
                read_Source_File.Dispose();
                read_Source_File.Close();

                wr.Dispose();
                wr.Close();
            }

            read_Data_File.Dispose();
            read_Data_File.Close();
        }

        /*********************************************************************************************************************************************
         *  Tạo bản sao của file *.PAR cho thuật toán Clustered Tree Problem Từ file *.PAR sẽ thay thế instance bằng instance khác được lấy từ danh sách file nguồn: 
         *  
         *  Parmaters: alg(0); source_File(1); file_data(2); begin_Of_Name_Of_Final_File(3)_
         *  
         ********************************************************************************************************************************************/

        public void creatate_Parameter_File_For_Cluster_Tree_MFO(string sourceFile, string Data_File, string begin_Name)
        {
            StreamReader read_Data_File = new StreamReader(Data_File);
            StreamWriter wr;

            string line = "";
            string new_Instance = "";
            string pattern = " ";
            Regex myRegex = new Regex(pattern);
            Regex rg = new Regex(@"\s+");

            while ((new_Instance = read_Data_File.ReadLine()) != null)//doc file de lay file instance mo
            {
                new_Instance = new_Instance.Trim();
                new_Instance = rg.Replace(new_Instance, " ");
                if (string.IsNullOrWhiteSpace(new_Instance) || (string.IsNullOrEmpty(new_Instance)))
                {
                    continue;
                }
                StreamReader read_Source_File = new StreamReader(sourceFile);
                string new_Para_File = begin_Name + "_" + Path.GetFileNameWithoutExtension(Path.GetFileName(new_Instance)) + @".par";
                wr = new StreamWriter(new_Para_File);

                while ((line = read_Source_File.ReadLine()) != null)//doc file du lieu cu de copy sang file moi
                {
                    line = line.Trim();
                    line = rg.Replace(line, " ");

                    if (string.IsNullOrWhiteSpace(line) || (string.IsNullOrEmpty(line)))
                    {
                        continue;
                    }

                    if (line.IndexOf("PARAMETER_FILE") != -1)
                    {
                        wr.WriteLine("PARAMETER_FILE = " + new_Para_File);
                        continue;
                    }

                    if (line.IndexOf("INSTANCE_NAME") != -1)
                    {
                        //Xoa bo phan ben phai sau "=" 
                        string str_Tmp = line.Substring(0, line.IndexOf("=") + 1);
                        wr.WriteLine(str_Tmp + " " + new_Instance);
                        continue;
                    }

                    wr.WriteLine(line);
                }
                read_Source_File.Dispose();
                read_Source_File.Close();

                wr.Dispose();
                wr.Close();
            }

            read_Data_File.Dispose();
            read_Data_File.Close();
        }


        /*********************************************************************************************************************************************
         *  Tạo Cluster_Tree instance từ dữ liệu file cluster traveling salesman problem
         *  http://webhotel4.ruc.dk/~keld/research/CLKH/
         *  http://labic.ic.uff.br/Instance/index.php
         *  
         ********************************************************************************************************************************************/
        public void create_Clustered_Tree_Instances_From_Clustered_TSP(string in_File_CTSP, string out_File)
        {
            StreamReader doc = new StreamReader(in_File_CTSP);
            StreamWriter wr = new StreamWriter(out_File);

            Random rnd = new Random();
            string line = "";
            string pattern = " ";
            Regex myRegex = new Regex(pattern);
            Regex rg = new Regex(@"\s+");

            int numCluster = -1,
                numOfCity = -1;
            bool readCluster = false;

            while ((line = doc.ReadLine()) != null)
            {
                line = line.Trim();
                //line = rg.Replace(line, " ");

                if (string.IsNullOrWhiteSpace(line) || (string.IsNullOrEmpty(line)))
                {
                    continue;
                }

                if (line.IndexOf("DIMENSION") != -1)
                {
                    string line_Tmp = rg.Replace(line, " ");
                    numOfCity = Convert.ToInt32(line_Tmp.Substring(line.LastIndexOf(" "), line_Tmp.Length - line_Tmp.LastIndexOf(" ")));
                    wr.WriteLine(line);
                    continue;
                }

                if (line.IndexOf("TYPE") != -1)
                {
                    wr.WriteLine("TYPE: " + "CLUSTERED_TREE");
                    continue;
                }

                if ((line.IndexOf("GTSP_SETS") != -1) || (line.IndexOf("HTSP_SETS") != -1) || (line.IndexOf("NUMBER_OF_CLUSTERS") != -1))
                {
                    string line_Tmp = rg.Replace(line, " ");
                    numCluster = Convert.ToInt32(line_Tmp.Substring(line.LastIndexOf(" "), line_Tmp.Length - line_Tmp.LastIndexOf(" ")));
                    wr.WriteLine("NUMBER_OF_CLUSTERS: " + numCluster);
                    continue;
                }

                if (line.IndexOf("GTSP_SET_SECTION") != -1)
                {
                    wr.WriteLine("CLUSTER_SECTION:");
                    wr.WriteLine("SOURCE_VERTEX: " + rnd.Next(numOfCity).ToString());
                    readCluster = true;
                    break;
                }

                if (line.IndexOf("EDGE_WEIGHT_FORMAT") != -1)
                {
                    string line_Tmp = rg.Replace(line, " ");
                    wr.WriteLine(line);
                    continue;
                }

                if ((line.IndexOf("EDGE_WEIGHT_SECTION") != -1) || (line.IndexOf("NODE_COORD_SECTION") != -1))
                {
                    wr.WriteLine(line);
                    continue;
                }

                wr.WriteLine(line);
            }

            string[] temp;
            string str_Tmp = "";
            while ((line = doc.ReadLine()) != null)
            {
                line = line.Trim();
                line = rg.Replace(line, " ");
                if (line == "EOF")
                {
                    continue;
                }
                if (string.IsNullOrWhiteSpace(line) || (string.IsNullOrEmpty(line)))
                {
                    continue;
                }

                //doc cac dinh cua moi cluster
                if (readCluster)
                {
                    
                    if (Char.IsNumber(line[0]))
                    {
                        temp = myRegex.Split(line);
                        str_Tmp = temp[0];
                        for (int i = 1; i < temp.Length - 1; i++)//temp[0] la chi so cua cluster, temp[n-1] = -1
                        {
                            str_Tmp = str_Tmp + " " + (Convert.ToInt32(temp[i]) - 1).ToString();  
                        }
                    }
                    str_Tmp = str_Tmp + " " + "-1";
                    wr.WriteLine(str_Tmp);
                }
            }
            wr.WriteLine("EOF");
            wr.Dispose();
            wr.Close();

            doc.Dispose();
            doc.Close();
        }

        /*********************************************************************************************************************************************
        *  Tạo Cluster_Tree instance dạng NON EUCLID dưa trên thông tin về số đỉnh, số đỉnh trên mỗi cluster của bài toán cluster traveling salesman problem
        *  => Chương trình chỉ sinh thêm 
        *  http://webhotel4.ruc.dk/~keld/research/CLKH/
        *  http://labic.ic.uff.br/Instance/index.php
        *  
        ********************************************************************************************************************************************/
        public void create_NON_Euclidean_Clustered_Tree_Instances_From_Clustered_TSP(string in_File_CTSP, int lowNum, int upperNum, string out_File)
        {
            StreamReader doc = new StreamReader(in_File_CTSP);
            StreamWriter wr = new StreamWriter(out_File);

            Random rnd = new Random();
            string line = "";
            string pattern = " ";
            Regex myRegex = new Regex(pattern);
            Regex rg = new Regex(@"\s+");

            int numCluster = -1,
                numOfCity = -1;
            bool readCluster = false,
                 isClusteredTree = false;//Tạo none euclide từ instance là clustered tree   

            //Đọc thông tin chung file và ma trận trọng số hoặc tọa độ các đỉnh
            while ((line = doc.ReadLine()) != null)
            {
                line = line.Trim();
                //line = rg.Replace(line, " ");

                if (string.IsNullOrWhiteSpace(line) || (string.IsNullOrEmpty(line)))
                {
                    continue;
                }

                if (line.IndexOf("Name") != -1)
                {
                    wr.WriteLine(line);
                    continue;
                }
                

                if (line.IndexOf("DIMENSION") != -1)
                {
                    string line_Tmp = rg.Replace(line, " ");
                    numOfCity = Convert.ToInt32(line_Tmp.Substring(line.LastIndexOf(" "), line_Tmp.Length - line_Tmp.LastIndexOf(" ")));
                    wr.WriteLine(line);
                    continue;
                }

                if (line.IndexOf("TYPE") != -1)
                {
                    wr.WriteLine("TYPE: " + "NON_EUC_CLUSTERED_TREE");
                    continue;
                }

                if ((line.IndexOf("GTSP_SETS") != -1) || (line.IndexOf("HTSP_SETS") != -1) || (line.IndexOf("NUMBER_OF_CLUSTERS") != -1))
                {
                    string line_Tmp = rg.Replace(line, " ");
                    numCluster = Convert.ToInt32(line_Tmp.Substring(line.LastIndexOf(" "), line_Tmp.Length - line_Tmp.LastIndexOf(" ")));
                    wr.WriteLine("NUMBER_OF_CLUSTERS: " + numCluster);
                    continue;
                }

                if (line.IndexOf("GTSP_SET_SECTION") != -1)
                {
                    wr.WriteLine("CLUSTER_SECTION:");
                    wr.WriteLine("SOURCE_VERTEX: " + rnd.Next(numOfCity).ToString());
                    readCluster = true;
                    break;
                }
                //Đọc các cluster từ instane của clustered tree rồi
                if (line.IndexOf("CLUSTER_SECTION") != -1)
                {
                    wr.WriteLine(line);
                    readCluster = true;
                    isClusteredTree = true;
                    break;
                }


                //Tới vùng chứa thông tin là tọa đổ các đỉnh thì bỏ qua
                if (line.IndexOf("NODE_COORD_SECTION") != -1)
                {
                    wr.WriteLine("EDGE_WEIGHT_SECTION:");
                    int[,] w = new int[numOfCity, numOfCity];
                    for (int i = 0; i < numOfCity; i++)
                    {
                        for (int j = i + 1; j < numOfCity; j++)
                        {
                            w[i, j] = lowNum + (int)((upperNum - lowNum) * rnd.NextDouble());
                            w[j, i] = w[i, j];
                        }
                        w[i, i] = 0;
                    }

                    for (int i = 0; i < numOfCity; i++)
                    {
                        for (int j = 0; j < numOfCity; j++)
                        {
                            wr.Write(String.Format("{0,10}", w[i, j]));
                            wr.Write("\t");
                        }
                        wr.WriteLine();
                    }
                    continue;
                }


                //if (line.IndexOf("EDGE_WEIGHT_FORMAT") != -1)
                //{
                //    string line_Tmp = rg.Replace(line, " ");
                //    wr.WriteLine(line);
                //    continue;
                //}

                //if ((line.IndexOf("EDGE_WEIGHT_SECTION") != -1) || (line.IndexOf("NODE_COORD_SECTION") != -1))
                //{
                //    wr.WriteLine(line);
                //    continue;
                //}

                //wr.WriteLine(line);
            }
            
            string[] temp;
            string str_Tmp = "";
            while ((line = doc.ReadLine()) != null)
            {
                line = line.Trim();
                line = rg.Replace(line, " ");
                if (line == "EOF")
                {
                    continue;
                }
                if (string.IsNullOrWhiteSpace(line) || (string.IsNullOrEmpty(line)))
                {
                    continue;
                }

                //doc cac dinh cua moi cluster
                if (readCluster)
                {
                    if (isClusteredTree)
                    {
                        wr.WriteLine(line);
                        continue;
                    }

                    if (Char.IsNumber(line[0]))
                    {
                        temp = myRegex.Split(line);
                        str_Tmp = temp[0];
                        for (int i = 1; i < temp.Length - 1; i++)//temp[0] la chi so cua cluster, temp[n-1] = -1
                        {
                            str_Tmp = str_Tmp + " " + (Convert.ToInt32(temp[i]) - 1).ToString();
                        }
                    }
                    str_Tmp = str_Tmp + " " + "-1";
                    wr.WriteLine(str_Tmp);
                }
            }
            wr.WriteLine("EOF");
            wr.Dispose();
            wr.Close();

            doc.Dispose();
            doc.Close();
        }

        #endregion


        #region MFO
        public void read_01KP(string fileName, ref double caption, out double[] weight_arr, out double[] values_arr, out int numGenes)
        {
            string nameData = "";
            string line = "";
            string pattern = " ";
            Regex myRegex = new Regex(pattern);
            Regex rg = new Regex(@"\s+");

            nameData = fileName + @"_c.txt";

            StreamReader doc = new StreamReader(nameData);
            //read caption
            while ((line = doc.ReadLine()) != null)
            {
                line = line.Trim();
                line = rg.Replace(line, " ");

                if (string.IsNullOrWhiteSpace(line) || (string.IsNullOrEmpty(line)))
                {
                    continue;
                }

                caption = Convert.ToDouble(line);
            }
            doc.Dispose();
            doc.Close();

            //read weight
            nameData = fileName + @"_w.txt";
            doc = new StreamReader(nameData);

            ArrayList arrLst = new ArrayList();

            while ((line = doc.ReadLine()) != null)
            {
                line = line.Trim();
                line = rg.Replace(line, " ");

                if (string.IsNullOrWhiteSpace(line) || (string.IsNullOrEmpty(line)))
                {
                    continue;
                }

                arrLst.Add(Convert.ToDouble(line));
            }

            weight_arr = arrLst.ToArray(typeof(double)) as double[];
            numGenes = weight_arr.Length;
            doc.Dispose();
            doc.Close();

            //read value
            nameData = fileName + @"_p.txt";
            doc = new StreamReader(nameData);
            arrLst.Clear();
            line = "";
            while ((line = doc.ReadLine()) != null)
            {
                line = line.Trim();
                line = rg.Replace(line, " ");

                if (string.IsNullOrWhiteSpace(line) || (string.IsNullOrEmpty(line)))
                {
                    continue;
                }
                arrLst.Add(Convert.ToDouble(line));

            }

            values_arr = arrLst.ToArray(typeof(double)) as double[];

            doc.Dispose();
            doc.Close();
        }

        public void write_Solution_to_File(string fileName, int seed, double[] array, int num_Genes, double fitness)
        {
            StreamWriter wr = new StreamWriter(fileName);
            wr.WriteLine("fileName: " + fileName);
            wr.WriteLine("Seed: " + seed);
            wr.WriteLine("Fitness: " + fitness);
            for (int i = 0; i < num_Genes; i++)
            {
                wr.Write(String.Format("{0,10:0.00}", array[i]));
            }
            wr.WriteLine();
            wr.Flush();
            wr.Close();
        }

        public void write_TSP_Solution_to_File(string fileName, int seed, int[] array, int num_Genes, double fitness)
        {
            StreamWriter wr = new StreamWriter(fileName);
            wr.WriteLine("fileName: " + fileName);
            wr.WriteLine("Seed: " + seed);
            wr.WriteLine("Fitness: " + fitness);
            for (int i = 0; i < num_Genes; i++)
            {
                wr.Write(array[i] + "\t");
            }
            wr.WriteLine();
            wr.Flush();
            wr.Close();
        }

        public void writeBestFitnessInPop(string fileName, int ord_Generations, double best_Ind_Fitness)
        {
            StreamWriter wr = new StreamWriter(fileName, true);
            try
            {
                wr.WriteLine(String.Format("{0,10:0}: {1,20:0.00}", ord_Generations, best_Ind_Fitness));
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                wr.Flush();
                wr.Close();
            }
        }

        private void writeMatrix(StreamWriter wr, double[,] matrixWeight, int matrix_Size)
        {
            for (int i = 0; i < matrix_Size; i++)
            {
                for (int j = 0; j < matrix_Size; j++)
                {
                    wr.Write(String.Format("{0,10:0.00}", matrixWeight[i, j]));
                }
                wr.WriteLine();
            }
        }

        public void writeMatrixToFile(string file_Path, double[,] matrixWeight, int matrix_Size, bool append)
        {
            StreamWriter wr = new StreamWriter(file_Path, append);
            writeMatrix(wr, matrixWeight, matrix_Size);
            wr.Flush();
            wr.Close();
        }

        private void writeMatrix(StreamWriter wr, int[,] matrixWeight, int matrix_Size)
        {
            for (int i = 0; i < matrix_Size; i++)
            {
                for (int j = 0; j < matrix_Size; j++)
                {
                    wr.Write(String.Format("{0}{1}", matrixWeight[i, j], "\t"));
                }
                wr.WriteLine();
            }
        }

        public void writeMatrixToFile(string file_Path, int[,] matrixWeight, int matrix_Size, bool append)
        {
            StreamWriter wr = new StreamWriter(file_Path, append);
            writeMatrix(wr, matrixWeight, matrix_Size);
            wr.Flush();
            wr.Close();
        }

        //Write opt solution of tree to file
        public void write_Opt_Solution_File(string file_Path, int seed, int[,] matrixWeight, int matrix_Size, double fitness, string running_Time, bool append)
        {
            StreamWriter wr = new StreamWriter(file_Path, append);
            wr.WriteLine("fileName: " + file_Path);
            wr.WriteLine("Seed: " + seed);
            wr.WriteLine("Fitness: " + fitness);
            wr.WriteLine("Time: " + running_Time);
            writeMatrix(wr, matrixWeight, matrix_Size);
            wr.Flush();
            wr.Close();
        }

        public void writeArrayToFile(string file_Path, double[] arr, int array_Size, bool append)
        {
            StreamWriter wr = new StreamWriter(file_Path, append);
            for (int i = 0; i < array_Size; i++)
            {
                wr.Write(String.Format("{0,10:0.00}", arr[i]));
            }
            wr.WriteLine();
            wr.Flush();
            wr.Close();
        }

        public void create_Tree_Instance_Random(string file_Name, int num_Vertex, double low_Weight_Edge, double height_Weight_Edge, double low_Weight_Vertex,
            double height_Weight_Vertex, Random rnd)
        {
            StreamWriter wr = new StreamWriter(file_Name);
            double[,] edge_Weight = new double[num_Vertex, num_Vertex];
            double[] vertex_Weight = new double[num_Vertex];

            file_Name = Path.GetFileNameWithoutExtension(file_Name);

            wr.WriteLine("NAME: " + file_Name);
            wr.WriteLine("TYPE: Tree");
            wr.WriteLine("COMMENT: ");
            wr.WriteLine("DIMENSION: " + num_Vertex.ToString());
            wr.WriteLine("EDGE_WEIGHT_TYPE: EXPLICIT");
            wr.WriteLine("EDGE_WEIGHT_FORMAT: FULL_MATRIX");
            wr.WriteLine("EDGE_WEIGHT_SECTION");

            for (int i = 0; i < num_Vertex; i++)
            {
                for (int j = i + 1; j < num_Vertex; j++)
                {
                    edge_Weight[i, j] = low_Weight_Edge + (height_Weight_Edge - low_Weight_Edge) * rnd.NextDouble();
                    edge_Weight[j, i] = edge_Weight[i, j];
                }
                edge_Weight[i, i] = 0;
            }

            for (int i = 0; i < num_Vertex; i++)
            {
                for (int j = 0; j < num_Vertex; j++)
                {
                    wr.Write(String.Format("{0,10:0.00}", edge_Weight[i, j]));
                    wr.Write("\t");
                }
                wr.WriteLine();
            }

            wr.WriteLine("VERTEX_WEIGHT_SECTION");
            for (int i = 0; i < num_Vertex; i++)
            {
                vertex_Weight[i] = low_Weight_Vertex + (height_Weight_Vertex - low_Weight_Vertex) * rnd.NextDouble();
            }
            for (int i = 0; i < num_Vertex; i++)
            {
                wr.Write(String.Format("{0,10:0.00}", vertex_Weight[i]));
            }
            wr.WriteLine();
            wr.WriteLine("EOF");
            wr.Dispose();
            wr.Close();
        }

        //Tạo dữ liệu cho cây từ ma trận trọng số
        public void create_Tree_Instance(string file_Name, int num_Vertex,  double[,] weight_Matrix)
        {
            StreamWriter wr = new StreamWriter(file_Name);
            double[,] edge_Weight = new double[num_Vertex, num_Vertex];
            double[] vertex_Weight = new double[num_Vertex];

            file_Name = Path.GetFileNameWithoutExtension(file_Name);

            wr.WriteLine("NAME: " + file_Name);
            wr.WriteLine("TYPE: Tree");
            wr.WriteLine("COMMENT: ");
            wr.WriteLine("DIMENSION: " + num_Vertex.ToString());
            wr.WriteLine("EDGE_WEIGHT_TYPE: EXPLICIT");
            wr.WriteLine("EDGE_WEIGHT_FORMAT: FULL_MATRIX");
            wr.WriteLine("EDGE_WEIGHT_SECTION");
          
            for (int i = 0; i < num_Vertex; i++)
            {
                for (int j = 0; j < num_Vertex; j++)
                {
                    wr.Write(String.Format("{0,10:0.00}", weight_Matrix[i, j]));
                    wr.Write("\t");
                }
                wr.WriteLine();
            }
            //wr.WriteLine();
            wr.WriteLine("EOF");
            wr.Dispose();
            wr.Close();
        }

        public void read_Tree_Instance(string filePath, out int num_Vertex, out double[,] edge_Weight, out double[] vertex_Weight)
        {
            StreamReader doc = new StreamReader(filePath);
            int maxIndex = 1;
            string line = "";
            string pattern = " ";
            Regex myRegex = new Regex(pattern);
            Regex rg = new Regex(@"\s+");

            while ((line = doc.ReadLine()) != null)
            {
                if (line.IndexOf("DIMENSION") != -1)
                {
                    maxIndex = Convert.ToInt32(line.Substring(line.LastIndexOf(" "), line.Length - line.LastIndexOf(" ")));
                    continue;
                }
                if (line.IndexOf("EDGE_WEIGHT_SECTION") != -1)
                {
                    break;
                }
            }
            edge_Weight = new double[maxIndex, maxIndex];
            vertex_Weight = new double[maxIndex];

            num_Vertex = maxIndex;

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

                if (read_Vertex_Wegith)
                {
                    //doc trong so cua vertex
                    string[] temp = myRegex.Split(line);
                    for (int i = 0; i < maxIndex; i++)//temp[0] la chi so cua cluster, temp[n-1] = -1
                    {
                        vertex_Weight[i] = Convert.ToDouble(temp[i]);
                    }
                }
                else
                {
                    //doc trong so
                    string[] temp = myRegex.Split(line);
                    for (int i = 0; i < maxIndex; i++)//temp[0] la chi so cua cluster, temp[n-1] = -1
                    {
                        edge_Weight[index, i] = Convert.ToDouble(temp[i]);
                    }
                    index++;
                }
            }
            doc.Dispose();
            doc.Close();
        }

        //Doc du lieu cua ham benchmark: 1 - 7
        public void read_Data_For_Real_Functions(string filePath, out double[] global_Optima, out double[,] rotation_Matrix, ref int dimention)
        {
            StreamReader doc = new StreamReader(filePath);
            string line = "";
            string pattern = " ";
            Regex myRegex = new Regex(pattern);
            Regex rg = new Regex(@"\s+");

            bool read_Rotation_Matrix = false;
            bool read_Global_Optima = false;
            string[] temp;
            int index = 0;
            dimention = 1;

            while ((line = doc.ReadLine()) != null)
            {
                if (line.IndexOf("DIMENSION") != -1)
                {
                    dimention = Convert.ToInt32(line.Substring(line.LastIndexOf(" "), line.Length - line.LastIndexOf(" ")));
                    //break;
                    break;
                }


            }

            rotation_Matrix = new double[dimention, dimention];
            global_Optima = new double[dimention];

            string edge_weight_format = "";

            while ((line = doc.ReadLine()) != null)
            {
                line = line.Trim();
                line = rg.Replace(line, " ");

                if (string.IsNullOrWhiteSpace(line) || (string.IsNullOrEmpty(line)))
                {
                    continue;
                }

                if (line.IndexOf("GLOBAL_OPTIMA") != -1)
                {
                    read_Global_Optima = true;
                    read_Rotation_Matrix = false;
                    continue;
                }

                if (line.IndexOf("ROTATION_MATRIX") != -1)
                {
                    read_Global_Optima = false;
                    read_Rotation_Matrix = true;
                    continue;
                }

                //doc cac dinh cua moi cluster
                if (read_Global_Optima)
                {
                    temp = myRegex.Split(line);
                    for (int i = 0; i < temp.Length; i++)
                    {
                        global_Optima[i] = Convert.ToDouble(temp[i]); ;
                    }
                }

                //Doc du lieu la ma tran trong so hoac toa do cac dinh
                if (read_Rotation_Matrix)
                {
                    temp = myRegex.Split(line);
                    for (int j = 0; j < dimention; j++)
                    {
                        rotation_Matrix[index, j] = Convert.ToDouble(temp[j]);
                    }
                    index++;
                }

            }

            doc.Dispose();
            doc.Close();
        }

        //Đọc file các tham số của thuật toán
        public string[] read_Parameters_File(string file_Name)
        {
            StreamReader doc = new StreamReader(file_Name);
            string line = "";
            string pattern = " ";
            Regex myRegex = new Regex(pattern);
            Regex rg = new Regex(@"\s+");
            List<string> lst = new List<string>();

            while ((line = doc.ReadLine()) != null)
            {
                line = line.Trim();
                line = rg.Replace(line, " ");
                lst.Add(line);
            }

            doc.Dispose();
            doc.Close();
            string[] pars = lst.ToArray();
            return pars;
        }

        //Ghi ket qua tot nhat cua cac task o moi the he
        public void write_Results_In_Generations(string file_Name, List<double[]> ev_Best_Fitness, int num_Tasks)
        {
            StreamWriter wr = new StreamWriter(file_Name);
            wr.Write("Generations" + "\t");
            string str = "";
            for(int j = 0; j < num_Tasks; j++)
            {
                    str = "Task_" + (j+1).ToString() + "\t"; 
                    wr.Write(str);
            }
            wr.WriteLine();
            for (int i = 0; i < ev_Best_Fitness.Count; i++)
            {
                wr.Write(i.ToString() + "\t");
                for(int j = 0; j < num_Tasks; j++)
                {
                    wr.Write(ev_Best_Fitness[i][j] + "\t");
                }
                 wr.WriteLine();
            }
            wr.WriteLine();
            wr.Flush();
            wr.Close();

        }

        //Ghi kết quả list ra file với cột tiêu đề là tham số: column_Title
        public void write_Parameters_In_Generations(string file_Name, List<double> lst_Para, string column_Title)
        {
            StreamWriter wr = new StreamWriter(file_Name);
            wr.Write("Generations" + "\t");
            string str = "";

            str = column_Title + "\t";
            wr.Write(str);
            wr.WriteLine();
            for (int i = 0; i < lst_Para.Count; i++)
            {
                wr.Write(i.ToString() + "\t");
                wr.Write(lst_Para[i] + "\t");
                wr.WriteLine();
            }
            wr.WriteLine();
            wr.Flush();
            wr.Close();

        }

        /*********************************************************************************************************************************************
        *  Tạo Cluster_Tree instance dạng NON EUCLID dưa trên thông tin về số đỉnh, số đỉnh trên mỗi cluster của bài toán cluster traveling salesman problem
        *  => Chương trình chỉ sinh thêm 
        *  http://webhotel4.ruc.dk/~keld/research/CLKH/
        *  http://labic.ic.uff.br/Instance/index.php
        *  
        ********************************************************************************************************************************************/


        #endregion


        #region Write optimal solution for ONE MAX TREE PROBLEM
        public void write_Optimal_of_One_Max_Tree(string file_Name, int num_Vertex, int[,] weight_Matrix)
        {
            StreamWriter wr = new StreamWriter(file_Name);

            file_Name = Path.GetFileNameWithoutExtension(file_Name);

            wr.WriteLine("NAME: Optimal Solution of One MAX Tree");
            wr.WriteLine("TYPE: Tree");
            wr.WriteLine("COMMENT: ");
            wr.WriteLine("DIMENSION: " + num_Vertex.ToString());
            wr.WriteLine("EDGE_WEIGHT_TYPE: EXPLICIT");
            wr.WriteLine("EDGE_WEIGHT_FORMAT: FULL_MATRIX");
            wr.WriteLine("EDGE_WEIGHT_SECTION");
            for (int i = 0; i < num_Vertex; i++)
            {
                for (int j = 0; j < num_Vertex; j++)
                {
                    wr.Write(String.Format("{0}", weight_Matrix[i, j]));
                    wr.Write("\t");
                }
                wr.WriteLine();
            }
            wr.WriteLine();
            wr.WriteLine("EOF");
            wr.Dispose();
            wr.Close();
        }

        //Tạo ra *.m file để vẽ đồ thị là luu thành ảnh trong matlbe
        public void draw_Plot_in_Matlab(string file_Name, int num_Vertex, int[,] weight_Matrix)
        {
           
            StreamWriter wr = new StreamWriter(file_Name);
            file_Name = Path.GetFileNameWithoutExtension(file_Name);
            wr.WriteLine(@"clear all;");
            wr.WriteLine(@"if(~isdeployed)");
            wr.WriteLine(@"cd(fileparts(which(mfilename)));");
            wr.WriteLine(@"end");
            wr.Write(@"A =[");
            for (int i = 0; i < num_Vertex; i++)
            {
                for (int j = 0; j < num_Vertex; j++)
                {
                    wr.Write(String.Format("{0}", weight_Matrix[i, j]));
                    wr.Write("\t");
                }
                wr.Write(@";");
            }
            wr.WriteLine(@"];");
            wr.WriteLine(@"G = graph(A);");
            wr.WriteLine(@"pl = plot(G);");
            wr.WriteLine(@"axis off;");
            wr.WriteLine(string.Format("saveas(pl,\'{0}\','png')", file_Name));
            wr.Dispose();
            wr.Close();

            
        }

        #endregion


        #region Tổng hợp kết quả
        //Thong ke ket qua tu cac file *.tour va *.opt trong thu muc
        public void statistic_Results(string director_Path, string extention_File, string out_File)
        {
            StreamReader doc;// = new StreamReader(tspFileName);
            StreamWriter wr = new StreamWriter(director_Path + @"\" + out_File);
            string[] files = Directory.GetFiles(director_Path, extention_File, SearchOption.AllDirectories);
            string line = "";
            string pattern = " ";
            Regex myRegex = new Regex(pattern);
            Regex rg = new Regex(@"\s+");

            wr.WriteLine("Instance name" + "\t" + "Cost" + "\t" + "Time");
            string tmp = "";
            for (int i = 0; i < files.Length; i++)
            {
                doc = new StreamReader(files[i]);
                tmp = Path.GetFileNameWithoutExtension(Path.GetFileName(files[i]));
                line = doc.ReadLine();
                line = line.Trim();
                line = rg.Replace(line, " ");

                while ((line != "EOF") && (!string.IsNullOrEmpty(line)))
                {
                    line = line.Trim();
                    line = rg.Replace(line, " ");

                    //if (string.IsNullOrWhiteSpace(line) || (string.IsNullOrEmpty(line)))
                    //{
                    //    continue;
                    //}

                    if (line.IndexOf("Fitness") != -1)
                    {
                        tmp = tmp + "\t" + line.Substring(line.LastIndexOf(":") + 1, line.Length - line.LastIndexOf(":") - 1);
                    }

                    if (line.IndexOf("Time") != -1)
                    {
                        tmp = tmp + "\t" + line.Substring(line.IndexOf(":") + 1, line.Length - line.IndexOf(":") - 1);
                        break;
                    }
                    line = doc.ReadLine();
                }
                wr.WriteLine(tmp);
                doc.Dispose();
                doc.Close();
            }//for
            wr.Dispose();
            wr.Close();
        }

        /*********************************************************************************************************************************************
         *  Thống kê các instance của bài clustered tree, cluster lớn nhất có bao nhiêu đỉnh, chiếm bao nhiêu % số đỉnh
         *  Ghi ra: ten_instance     id_largest_Cluster      num_Vertex_in_Largest_Cluster       percentage
         *  Parmaters: alg(0); list_Instances(1); statistic_File(2);
         *  
         ********************************************************************************************************************************************/
        public void count_Number_Of_Largest_Cluster_in_Instance(string list_Instances, string statistic_File)
        {
            StreamReader doc = new StreamReader(list_Instances);
            StreamReader doc_Instance;
            StreamWriter wr;

            double[,] weight_Matrix;
            int[][] vertex_In_Cluster;
            int num_Cluster, 
                source_Vertex,
                num_Vertex;

            Random rnd = new Random();
            string line = "";
            string pattern = " ";
            Regex myRegex = new Regex(pattern);
            Regex rg = new Regex(@"\s+");

            bool readCluster = false;

          

            string tmp = "",
                   tmp_tmp = "";
            line = doc.ReadLine();
            line = line.Trim();
            line = rg.Replace(line, " ");

            tmp = Path.GetFileNameWithoutExtension(Path.GetFileName(statistic_File));
            if (!Directory.Exists(tmp))
            {
                Directory.CreateDirectory(tmp);
            }
            wr = new StreamWriter(tmp + @"\" + statistic_File);
            wr.WriteLine("Instance name" + "\t" + "id_largest_Cluster" + "\t" + "num_Vertex_in_Largest_Cluster" + "\t" + "percentage");

            while ((line != "EOF") && (!string.IsNullOrEmpty(line)))
            {
                line = line.Trim();
                line = rg.Replace(line, " ");

                read_File_Clusted_Tree(line, out weight_Matrix, out vertex_In_Cluster, out num_Vertex, out num_Cluster, out source_Vertex);

                int idx_Max_Clus = 0;
                int num_Vertex_In_Cluster = vertex_In_Cluster[0].Length;
                for (int i = 1; i < num_Cluster; i++)
                {
                    if (vertex_In_Cluster[i].Length > num_Vertex_In_Cluster)
                    {
                        num_Vertex_In_Cluster = vertex_In_Cluster[i].Length;
                        idx_Max_Clus = i;
                    }
                }

                tmp = Path.GetFileNameWithoutExtension(Path.GetFileName(line));
                tmp_tmp = tmp + "\t" + idx_Max_Clus + "\t" + num_Vertex_In_Cluster + "\t" + ((num_Vertex_In_Cluster*1.0f)/ num_Vertex) * 100;

                wr.WriteLine(tmp_tmp);
                
                line = doc.ReadLine();
            }//for
            doc.Dispose();
            doc.Close();
            wr.Dispose();
            wr.Close();


        }

        
       

        #endregion


    }
}
