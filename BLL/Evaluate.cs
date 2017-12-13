using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DTO;
using System.Collections;

namespace BLL
{
    public class Evaluate
    {
        Graph_Method graph_Method_Class = new Graph_Method();
        Mutation mutation_Class = new Mutation();
        Initialize_Chromosome init_Chromo = new Initialize_Chromosome();
        ReadWriteFile ioFile = new ReadWriteFile();

        #region Các hàm số thực
        //01. Function 01: sphere
        private double sphere(double[] var, int dimension)
        {
            double sum;
            sum = 0;
            for (int i = 1; i <= dimension; i++)
            {
                sum = sum + var[i-1] * var[i-1];
            }
            return sum;

        }

        //01. Function 01: sphere
        //   - var: design variable vector
        //   - opt: shift vector
        private double sphere(double[] var, double[] opt, int dimension)
        {
            double[] tmp = shiftVariables(var, opt, dimension);
            return sphere(tmp, dimension);
        }

        //02. Function 02: rosenbrock
        private double rosenbrock(double[] var, int dimension)
        {
            double sum1,
                   sum2;
            sum1 = 0;
            sum2 = 0;
            for (int i = 1; i <= dimension - 1; i++)
            {
                sum1 = sum1 + 100 * (var[i-1] * var[i-1] - var[i + 1-1]);
                sum2 = sum2 + (var[i-1] - 1) * (var[i-1] - 1);
            }
            return sum1 + sum2;
        }

        //03. Function 03: ackley
        private double ackley(double[] var, int dimension)
        {
            double sum1,
                    sum2;
            sum1 = 0;
            sum2 = 0;
            for (int i = 1; i <= dimension; i++)
            {
                sum1 = sum1 + var[i-1] * var[i-1];
                sum2 = sum2 + Math.Cos(2 * Math.PI * var[i-1]);
            }
            sum1 = sum1 / dimension;
            sum2 = sum2 / dimension;

            return -20 * Math.Exp(-0.2 * Math.Sqrt(sum1)) - Math.Exp(sum2) + 20 + Math.Exp(1);

        }

        //03. Function 03: ackley
        //   - var: design variable vector
        //   - M: rotation matrix
        //   - opt: shift vector
        private double ackley(double[] var, double[,] rotation_Matrix, double[] shiftValues, int dimension)
        {
            double[] x = shiftVariables(var, shiftValues, dimension);
            x = rotateVariables(x, rotation_Matrix, dimension);
            return ackley(x, dimension);
        }
        
        //04. Function 04: rastrigin
        private double rastrigin(double[] var, int dimension)
        {
            double sum;
            sum = 0;
            for (int i = 1; i <= dimension; i++)
            {
                sum = sum + var[i-1] * var[i-1] - 10 * Math.Cos(2 * Math.PI * var[i-1]) + 10;
            }
            return sum;
        }

        //04. Function 04: rastrigin
        //   - var: design variable vector
        //   - M: rotation matrix
        //   - opt: shift vector
        private double rastrigin(double[] var, double[,] rotation_Matrix, double[] shiftValues, int dimension)
        {
            double[] x = shiftVariables(var, shiftValues, dimension);
            x = rotateVariables(x, rotation_Matrix, dimension);
            return rastrigin(x, dimension);
        }
       
        //05. Function 05: griewank
        private double griewank(double[] var, int dimension)
        {
            double sum,
                   pro;
            sum = 0;
            pro = 0;
            for (int i = 1; i <= dimension; i++)
            {
                sum = sum + var[i-1] * var[i-1];
                pro = pro *  Math.Cos(var[i-1]/(Math.Sqrt(i)));
            }
            sum = sum / 4000;
            return 1 + sum + pro;
        }

        //05. Function 05: griewank
        //   - var: design variable vector
        //   - M: rotation matrix
        //   - opt: shift vector
        private double griewank(double[] var, double[,] rotation_Matrix, double[] shiftValues, int dimension)
        {
            double[] x = shiftVariables(var, shiftValues, dimension);
            x = rotateVariables(x, rotation_Matrix, dimension);
            return griewank(x, dimension);
        }

        //06. Function 06: weierstrass
        private double weierstrass(double[] var, int dimension)
        {
            double a = 0.5, 
                   b = 3, 
                   result = 0;
            int kmax = 20;
                       
            for (int i = 1; i <= dimension; i++)
            {
                for (int k = 0; k < kmax; k++)
                {
                    result = result + Math.Pow(a, k) * Math.Cos(2 * Math.PI * Math.Pow(b, k) * (var[i-1] + 0.5));
                }
            }

            for (int k = 0; k <= kmax; k++)
            {
                result = result - dimension * Math.Pow(a, k) * Math.Cos(2 * Math.PI * Math.Pow(b, k) * 0.5);
            }
            return result;
        }

        //06. Function 06: weierstrass
        //   - var: design variable vector
        //   - M: rotation matrix
        //   - opt: shift vector
        private double weierstrass(double[] var, double[,] rotation_Matrix, double[] shiftValues, int dimension)
        {
            double[] x = shiftVariables(var, shiftValues, dimension);
            x = rotateVariables(x, rotation_Matrix, dimension);
            return weierstrass(x, dimension);
        }

        //07. Function 07: schwefel
        private double schwefel(double[] var, int dimension)
        {
            double sum;
            sum = 0;
            for (int i = 1; i <= dimension; i++)
            {
                sum = sum + var[i-1] * Math.Sin(Math.Sqrt(Math.Abs(var[i-1])));
            }
            return 418.9829 * dimension - sum;
        }

        #endregion

        //08. Function 08: TSP
        public double compute_Cost_TSP_Tour(int[] tour, int num_genes, double[,] weight_Matrix)
        {
            double s = 0;

            for (int i = 0; i < num_genes - 1; i++)
            {
                s = s + weight_Matrix[tour[i], tour[i + 1]];
            }
            s = s + weight_Matrix[tour[num_genes - 1], tour[0]];
            return s;
        }

        //Chỉ sử dụng được khi các bài toán có cùng một bài toán (nhưng khác nhau về số dimention)
        private double problem_Evaluation(Tasks tsk, int problem_Index, double[] var)
        {
            double results = 0;
            switch (problem_Index)
            {
                case 1: results = sphere(var, tsk.Dims);        break;
                case 2: results = rosenbrock(var, tsk.Dims);    break;
                case 3: results = ackley(var, tsk.Dims);        break;
                case 4: results = rastrigin(var, tsk.Dims);     break;
                case 5: results = griewank(var, tsk.Dims);      break;
                case 6: results = weierstrass(var, tsk.Dims);   break;
                case 7: results = schwefel(var, tsk.Dims);      break;
                //case 8: results = compute_Cost_TSP_Tour(var, tsk.Dims, tsk.Weight_Matrix); break;
            }
            return results;
        }

        //Chỉ sử dụng được khi các bài toán có cùng một bài toán (nhưng khác nhau về số dimention)
        public void evaluate(Tasks[] tsks, int num_Tasks, int problem_Index, double lambda, ref Chromosome obj)
        {
            if(obj.Skill_Factor == -1)
            {
                for(int i = 0; i < num_Tasks; i++)
                {
                    obj.Obj_value[i] = problem_Evaluation(tsks[i], problem_Index, obj.Rnvec);
                    obj.Factorial_Cost[i] = obj.Constraint_violation[i] * lambda + obj.Obj_value[i];
                }
            }
            else
            {
                for(int i = 0; i < num_Tasks; i++)
                {
                    if(obj.Skill_Factor == i)
                    {
                        obj.Obj_value[i] = problem_Evaluation(tsks[i], problem_Index, obj.Rnvec);
                    }
                    else
                    {
                        obj.Obj_value[i] = double.MaxValue;
                    }
                    obj.Factorial_Cost[i] = obj.Constraint_violation[i] * lambda + obj.Obj_value[i];
                }
            }
        }

        //Chuyển solution từ giá trị [0,1] về [lower, upper] của hàm và tính giá trị.
        //Trả về giá trị của hàm sau khi đưa về [lower, upper]
        private double fnceval(Tasks task, double[] rnvec, ref int num_Evaluate)
        {
            int d = task.Dims;
            double[] nvars = new double[d];
            double[] minrange = new double[d];
            double[] maxrange = new double[d];
            double[] y = new double[d];
            double[] vars = new double[d];


            for (int i = 0; i < d; i++)
            {
                nvars[i] = rnvec[i];
                minrange[i] = task.Lb[i];
                maxrange[i] = task.Ub[i];
                y[i] = maxrange[i] - minrange[i];
                vars[i] = y[i] * nvars[i] + minrange[i];
            }
            num_Evaluate = 1;
            return problem_Evaluation(task, vars);
        }
        
        private double problem_Evaluation(Tasks tsk, double[] var)
        {
            double results = 0;
            switch (tsk.Function_Cost_Name)
            {
                //case 1: results = sphere(var, tsk.Dims);        break;
                //case 2: results = rosenbrock(var, tsk.Dims);    break;
                //case 3: results = ackley(var, tsk.Dims);        break;
                //case 4: results = rastrigin(var, tsk.Dims);     break;
                //case 5: results = griewank(var, tsk.Dims);      break;
                //case 6: results = weierstrass(var, tsk.Dims);   break;
                //case 7: results = schwefel(var, tsk.Dims);      break;
                case "Sphere":      results = sphere(var, tsk.Global_Optima, tsk.Dims);                         break;
                case "Rosenbrock":  results = rosenbrock(var, tsk.Dims);                                        break;
                case "Ackley":      results = ackley(var, tsk.Weight_Matrix, tsk.Global_Optima, tsk.Dims);      break;
                case "Rastrigin":   results = rastrigin(var, tsk.Weight_Matrix, tsk.Global_Optima, tsk.Dims);   break;
                case "Griewank":    results = griewank(var, tsk.Weight_Matrix, tsk.Global_Optima, tsk.Dims);    break;
                case "Weierstrass": results = weierstrass(var, tsk.Weight_Matrix, tsk.Global_Optima, tsk.Dims); break;
                case "Schwefel":    results = schwefel(var, tsk.Dims);                                          break;
                case "TSP":           break;
            }
            return results;
        }

        //Nhân 2 ma trận n*m vaf m*r
        public double[,] MultiplyMatrix(double[,] A, double[,] B, int n, int m, int r)
        {
            double si;
            double[,] C = new double[n, r];
            
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < r; j++)
                {
                    si = 0;
                    for (int k = 0; k < m; k++)
                    {
                        si += A[i, k] + B[k, j];
                    }
                    C[i, j] = si;
                }
            }
            return C;
        }

        //Ma trận chuyển vị
        public double[,] Transpose(double[,] matrix, int num_Rows, int num_Columns)
        {
            double[,] result = new double[num_Columns, num_Rows];
            for (int i = 0; i < num_Rows; i++)
            {
                for (int j = 0; j < num_Columns; j++)
                {
                    result[j, i] = matrix[i, j];
                }
            }
            return result;
        }

        /*********************************************************************************************************************************************
        * Algorithm: Decoding - chuyển mã hóa hoán vị có độ dài D_max về mã hóa hoán vị của từng bài toán
        *  + Y. Yuan, Y.-S. Ong, A. Gupta, P. S. Tan, and H. Xu, “Evolutionary Multitasking in Permutation-Based Combinatorial Optimization Problems: 
        *  Realization with TSP, QAP, LOP, and JSP.”
        *                       
        ***********************************************************************************************************************************************/
        public int[] decoding_TSP(int[] ind_tour, int maxGenes, int num_Gen_in_Task_j)
        {
            int[] tmpArr = new int[num_Gen_in_Task_j];
            int idx = 0;
            for (int i = 0; i < maxGenes; i++)
            {
                if (ind_tour[i] < num_Gen_in_Task_j)
                {
                    tmpArr[idx] = ind_tour[i];
                    idx++;
                }
            }
            return tmpArr;
        }

        /*********************************************************************************************************************************************
         * Tìm index của cluster lớn nhất
         * Output: idx của cluster lớn nhất và số đỉnh trong cluster lớn nhất
         * 
         ***********************************************************************************************************************************************/
        public void find_Largest_Cluster(Tasks tsk, ref int idx_Largest_Cluster, ref int num_Vertex_In_Largest_Cluster)
        {
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
            idx_Largest_Cluster = idx_Max_Clus;
            num_Vertex_In_Largest_Cluster = num_Vertex_In_Cluster;
        }

        private double problem_Evaluation(Tasks tsk, Chromosome obj)
        {
            double results = 0;
            int d = tsk.Dims;
            double[] nvars = new double[d];
            double[] minrange = new double[d];
            double[] maxrange = new double[d];
            double[] y = new double[d];
            double[] vars = new double[d];
            int[,] tree_Solution;
            int[] prufer_Tour_Decode;
            int[,] opt_tree;
            int[] tour_Decode;
            switch (tsk.Function_Cost_Name)
            {
                case "Sphere":
                    #region sphere
                    //Deconding
                    for (int i = 0; i < d; i++)
                    {
                        nvars[i] = obj.Rnvec[i];
                        minrange[i] = tsk.Lb[i];
                        maxrange[i] = tsk.Ub[i];
                        y[i] = maxrange[i] - minrange[i];
                        vars[i] = y[i] * nvars[i] + minrange[i];
                    }
                    results = sphere(vars, tsk.Global_Optima, tsk.Dims); 
                    #endregion
                    break;
                case "Rosenbrok":
                    #region rosenbrock
                    //Deconding
                    for (int i = 0; i < d; i++)
                    {
                        nvars[i] = obj.Rnvec[i];
                        minrange[i] = tsk.Lb[i];
                        maxrange[i] = tsk.Ub[i];
                        y[i] = maxrange[i] - minrange[i];
                        vars[i] = y[i] * nvars[i] + minrange[i];
                    }
                    results = rosenbrock(vars, tsk.Dims); 
                    #endregion
                    break;
                case "Ackley":
                    #region ackley
                    //Deconding
                    for (int i = 0; i < d; i++)
                    {
                        nvars[i] = obj.Rnvec[i];
                        minrange[i] = tsk.Lb[i];
                        maxrange[i] = tsk.Ub[i];
                        y[i] = maxrange[i] - minrange[i];
                        vars[i] = y[i] * nvars[i] + minrange[i];
                    }
                    results = ackley(vars, tsk.Weight_Matrix, tsk.Global_Optima, tsk.Dims); 
                    #endregion
                    break;
                case "Rastrigin":
                    #region rastrigin
                    //Deconding
                    for (int i = 0; i < d; i++)
                    {
                        nvars[i] = obj.Rnvec[i];
                        minrange[i] = tsk.Lb[i];
                        maxrange[i] = tsk.Ub[i];
                        y[i] = maxrange[i] - minrange[i];
                        vars[i] = y[i] * nvars[i] + minrange[i];
                    }
                    results = rastrigin(vars, tsk.Weight_Matrix, tsk.Global_Optima, tsk.Dims); 
                    #endregion
                    break;
                case "Griewank":
                    #region griewank
                    //Deconding
                    for (int i = 0; i < d; i++)
                    {
                        nvars[i] = obj.Rnvec[i];
                        minrange[i] = tsk.Lb[i];
                        maxrange[i] = tsk.Ub[i];
                        y[i] = maxrange[i] - minrange[i];
                        vars[i] = y[i] * nvars[i] + minrange[i];
                    }
                    results = griewank(vars, tsk.Weight_Matrix, tsk.Global_Optima, tsk.Dims); 
                    #endregion
                    break;
                case "Weierstrass":
                    #region weierstrass
                    //Deconding
                    for (int i = 0; i < d; i++)
                    {
                        nvars[i] = obj.Rnvec[i];
                        minrange[i] = tsk.Lb[i];
                        maxrange[i] = tsk.Ub[i];
                        y[i] = maxrange[i] - minrange[i];
                        vars[i] = y[i] * nvars[i] + minrange[i];
                    }
                    results = weierstrass(vars, tsk.Weight_Matrix, tsk.Global_Optima, tsk.Dims); 
                    #endregion
                    break;
                case "Schwefel":
                    #region schwefel
                    //Deconding
                    for (int i = 0; i < d; i++)
                    {
                        nvars[i] = obj.Rnvec[i];
                        minrange[i] = tsk.Lb[i];
                        maxrange[i] = tsk.Ub[i];
                        y[i] = maxrange[i] - minrange[i];
                        vars[i] = y[i] * nvars[i] + minrange[i];
                    }
                    results = schwefel(vars, tsk.Dims); 
                    #endregion
                    break;
                case "TSP":
                    #region TSP
                    //Deconding
                    tour_Decode = decoding_TSP(obj.Invec, tsk.MaxDims, d);
                    results = compute_Cost_TSP_Tour(tour_Decode, tsk.Dims, tsk.Weight_Matrix); 
                    #endregion
                    break;
                case "PRUFER_CODE":
                    #region PRUFER_TREE
                    prufer_Tour_Decode = decoding_MFO_Prufer_Simple(obj.Invec, tsk.MaxDims, d);
                    tree_Solution = convert_Prufer_to_Tree(prufer_Tour_Decode, d + 2);
                    //Optimal solution luu trong weight_Matrix trong Task.
                    opt_tree = new int[d + 2, d + 2];
                    for (int i = 0; i < d + 2; i++)
                    {
                        for(int j = 0; j < d + 2; j++)
                        {
                            opt_tree[i, j] = (int)tsk.Weight_Matrix[i, j];
                        }
                    }
                    results = one_Max_Tree_Evaluate(tree_Solution, opt_tree, d + 2);
                    #endregion
                    break;
                case "BLOB_CODE":
                    #region BLOB_CODE
                    int[,] tree_temp = new int[(d + 2) + 1, (d + 2) + 1];
                    int[] blod_arr = new int[(d + 2) + 1];
                    blod_arr[0] = -1; //A Blob string B = (b2, b3, . . . , bn−1) ∈ Cn.
                    blod_arr[1] = -1; //Blod code: 1 -> n
                    blod_arr[d + 2] = -1;
                    prufer_Tour_Decode = decoding_MFO_Prufer_Simple(obj.Invec, tsk.MaxDims, d);

                    for (int i = 2; i <= (d + 2) - 1; i++)
                    {
                        blod_arr[i] = prufer_Tour_Decode[i - 2] + 1;
                    }
                    tree_temp = convert_Blod_to_Tree(blod_arr, (d + 2));
                    tree_Solution = new int[d + 2, d + 2];

                    for (int i = 0; i < (d + 2); i++)
                    {
                        for (int j = 0; j < (d + 2); j++)
                        {
                            tree_Solution[i, j] = tree_temp[i + 1, j + 1];
                        }
                    }
                    opt_tree = new int[d + 2, d + 2];
                    for (int i = 0; i < d + 2; i++)
                    {
                        for(int j = 0; j < d + 2; j++)
                        {
                            opt_tree[i, j] = (int)tsk.Weight_Matrix[i, j];
                        }
                    }
                    results = one_Max_Tree_Evaluate(tree_Solution, opt_tree, d + 2);
                    #endregion
                    break;
                case "EDGES_SET":
                    #region EDGES_SET
                    tree_Solution = decoding_MFO_Edge_Set_1(obj.Edges_Matrix, tsk.MaxDims, d);

                    //Chuyen ma tran canh ve so nguyen
                    opt_tree = new int[d, d];
                    for (int i = 0; i < d; i++)
                    {
                        for(int j = 0; j < d; j++)
                        {
                            opt_tree[i, j] = (int)tsk.Weight_Matrix[i, j];
                        }
                    }
                    results = one_Max_Tree_Evaluate(tree_Solution, opt_tree, d);
                    #endregion
                    break;
                case "EDGES_SET_NO_DECODING":
                    #region EDGES_SET_NO_DECODING
                    int num_Vertex_In_Small_Graph = 20;
                    //tree_Solution = new int[num_Vertex_In_Small_Graph, num_Vertex_In_Small_Graph];
                    //for (int i = 0; i < num_Vertex_In_Small_Graph; i++)
                    //{
                    //    for (int j = 0; j < num_Vertex_In_Small_Graph; j++)
                    //    {
                    //        tree_Solution[i, j] = (int)obj.Edges_Matrix[i, j];
                    //    }
                    //}

                    tree_Solution = decoding_MFO_Edge_Set_1(obj.Edges_Matrix, tsk.MaxDims, num_Vertex_In_Small_Graph);

                    //Chuyen ma tran canh ve so nguyen
                    opt_tree = decoding_MFO_Edge_Set_1(tsk.Weight_Matrix, tsk.MaxDims, num_Vertex_In_Small_Graph);
                    //for (int i = 0; i < num_Vertex_In_Small_Graph; i++)
                    //{
                    //    for (int j = 0; j < num_Vertex_In_Small_Graph; j++)
                    //    {
                    //        opt_tree[i, j] = (int)opt_tree_double[i, j];
                    //    }
                    //}
                    results = one_Max_Tree_Evaluate(tree_Solution, opt_tree, num_Vertex_In_Small_Graph);
                    #endregion
                    break;
                case "EDGES_SET_VERTEX_IN_SUBGRAPH":
                    #region EDGES_SET_VERTEX_IN_SUBGRAHP
                    tree_Solution = decoding_MFO_Vertex_In_SubGraph(obj.Edges_Matrix, tsk.MaxDims, d);
                    //Chuyen ma tran canh ve so nguyen
                    opt_tree = new int[d, d];
                    for (int i = 0; i < d; i++)
                    {
                        for(int j = 0; j < d; j++)
                        {
                            opt_tree[i, j] = (int)tsk.Weight_Matrix[i, j];
                        }
                    }


                    //int[,] aaa = new int[d, d];
                    //for (int i = 0; i < d; i++)
                    //{
                    //    for (int j = 0; j < d; j++)
                    //    {
                    //        aaa[i, j] = (int)opt_tree[i, j];
                    //    }
                    //}
                    //ioFile.draw_Plot_in_Matlab("aaaa" + @".m", d, aaa);

                    results = one_Max_Tree_Evaluate(tree_Solution, opt_tree, d);
                    #endregion
                    break;
                case "CLUSTERED_TREE":
                    #region CLUSTERED_TREE
                    //decoding ????
                    results = clustered_Tree_Evaluate(obj.Edges_Matrix, tsk.Weight_Matrix, tsk.MaxDims, tsk.Source_Vertex);
                    #endregion
                    break;
                case "MAX_GROUP_IN_CLUSTERED_TREE":
                    #region MAX_GROUP_IN_CLUSTERED_TREE
                    int idx_Max_Clus = 0;
                    int num_Vertex_In_Cluster = 0;
               
                    find_Largest_Cluster(tsk, ref idx_Max_Clus, ref num_Vertex_In_Cluster);
                    double[,] weight_Cluster_Matrix = init_Chromo.create_Weight_Matrix_for_Cluster(tsk.MaxDims, num_Vertex_In_Cluster, tsk.Weight_Matrix,
                        tsk.Vertex_In_Cluster[idx_Max_Clus]);
                    results = 0;
                  
                    for (int i = 0; i < num_Vertex_In_Cluster; i++)
                    {
                        for (int j = 0; j < num_Vertex_In_Cluster; j++)
                        {
                            if ((obj.Edges_Matrix[tsk.Vertex_In_Cluster[idx_Max_Clus][i], tsk.Vertex_In_Cluster[idx_Max_Clus][j]] > 0) 
                                && (obj.Edges_Matrix[tsk.Vertex_In_Cluster[idx_Max_Clus][i], tsk.Vertex_In_Cluster[idx_Max_Clus][j]] < Double.MaxValue))
                            {
                                results = results + tsk.Weight_Matrix[tsk.Vertex_In_Cluster[idx_Max_Clus][i], tsk.Vertex_In_Cluster[idx_Max_Clus][j]];
                            }
                            
                        }
                    }
                    return results;
                    #endregion
                    break;
                case "1":
                    #region 1


                    #endregion
                    break;
            }
            return results;
        }
    
        //Ham shift dùng cho thi tại hội thảo CEC 2017
        public double[] shiftVariables(double[] x, double[] shiftValues, int dimension) 
        {
            double[] vars = new double[dimension];
		    for (int i = 0; i < dimension; i++)
            {
                  x[i] -= shiftValues[i];
            }
            return vars;
	    }

        //Ham rotate dùng cho thi tại hội thảo CEC 2017
        //Trả về mạng một chiều sau khi đã thực hienj rotate
        private double[] rotateVariables(double[] x, double[,] rotation_Matrix, int dimension) 
        {
		    double[] res = new double[dimension];
            double[] y = new double[dimension];

		    for (int i = 0; i < dimension; i++) 
            {
                for(int j = 0; j < dimension; j++)
                {
                    y[j] = rotation_Matrix[i,j];
                }

			    double sum = 0;
			    for (int j = 0; j < dimension; j++)
                {
                     sum += x[j] * y[j];
                }
			    res[i] = sum;
		    }
		    return res;
	    }
	   
        private double fnceval(Tasks task, Chromosome obj, ref int num_Evaluate)
        {
            num_Evaluate = 1;
            return problem_Evaluation(task, obj);
        }
        
        public void evaluate(Tasks[] tsks, int num_Tasks, double lambda, ref Chromosome obj, ref int func_call)
        {
            int num_Evaluate = 0;
            if (obj.Skill_Factor == -1)
            {
                func_call = 0;
                for (int i = 0; i < num_Tasks; i++)
                {
                    //obj.Obj_value[i] = fnceval(tsks[i], obj.Rnvec, ref num_Evaluate);
                    obj.Obj_value[i] = fnceval(tsks[i], obj, ref num_Evaluate);
                    obj.Factorial_Cost[i] = obj.Constraint_violation[i] * lambda + obj.Obj_value[i];
                    func_call = func_call + num_Evaluate;
                }
            }
            else
            {
                for (int i = 0; i < num_Tasks; i++)
                {
                    obj.Obj_value[i] = double.MaxValue;
                    obj.Factorial_Cost[i] = obj.Constraint_violation[i] * lambda + obj.Obj_value[i];
                }

                for (int i = 0; i < num_Tasks; i++)
                {
                    if (obj.Skill_Factor == i)
                    {
                        //obj.Obj_value[i] = fnceval(tsks[i], obj.Rnvec, ref num_Evaluate);
                        obj.Obj_value[i] = fnceval(tsks[i], obj, ref num_Evaluate);
                        obj.Factorial_Cost[i] = obj.Constraint_violation[i] * lambda + obj.Obj_value[i];
                        func_call = num_Evaluate;
                        break;
                    }
                }
            }//else
        }

        #region Thuật toán tạo cây khung
        /*********************************************************************************************************************************************
        *  Tìm danh sách các cạnh có đỉnh kề với đỉnh v mà không thuộc tập C
        *  
        ********************************************************************************************************************************************/
        private List<Edge> find_Edges(int num_Verties, int v, double[,] weight_Matrix, List<int> C)
        {
            List<Edge> lst_Edge = new List<Edge>();
            for(int i = 0; i < num_Verties; i++)
            {
                if(i != v)
                {
                    if(!C.Contains(i))
                    {
                        if(weight_Matrix[v,i] > 0)
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
        private double[,] primRST(int num_Verties, double[,] weight_Matrix, Random rnd)
        {
            int rnd_Verties = -1,
                idx_Rnd_Edge = -1;
            List<int> C = new List<int>();
            List<Edge> A = new List<Edge>();

            double[,] T = new double[num_Verties, num_Verties];
            for(int i = 0; i < num_Verties; i++)
            {
                for(int j = 0; j < num_Verties; j++)
                {
                    T[i, j] = 0;
                }
            }

            rnd_Verties = rnd.Next(num_Verties);
            C.Add(rnd_Verties);
            A.AddRange(find_Edges(num_Verties, rnd_Verties, weight_Matrix, C));

            while(C.Count < num_Verties)
            {
                if (A.Count == 0) //Không tìm được cây khung
                {
                    return null;
                }

                idx_Rnd_Edge = rnd.Next(A.Count);     //Do các cạnh thuộc A có ít nhất 1 đỉnh thuộc C -> thuật toán tạo thì là start_point
                Edge edge_Tmp = new Edge(A[idx_Rnd_Edge].Start_Point, A[idx_Rnd_Edge].End_Point);
                A.RemoveAt(idx_Rnd_Edge);
                if(!C.Contains(edge_Tmp.End_Point))
                {
                    T[edge_Tmp.Start_Point, edge_Tmp.End_Point] = 1;
                    C.Add(edge_Tmp.End_Point);
                    A.AddRange(find_Edges(num_Verties, edge_Tmp.End_Point, weight_Matrix, C));
                }
            }
            return T;
        }

        private double[,] kruskalRST(int num_Verties, double[,] weight_Matrix, Random rnd)
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
                for (int j = i+1; j < num_Verties; j++)
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

        #region Prufer
        
        /*********************************************************************************************************************************************
         * 1. Chuan hoa cac so trong day ve doan 1..Solution_Length
         * 2. Lay Solution_Length ky tu dau tien 
         * 
         * => ko can dung maxGenes vi cu lay num_Gen_in_Task_j dau tien la duoc
         ********************************************************************************************************************************************/
        public int[] decoding_MFO_Prufer_Simple(int[] ind_tour, int maxGenes, int num_Gen_of_Task_j)
        {
            int[] tmpArr = new int[num_Gen_of_Task_j];
            //1. Chuan hoa cac so trong day ve doan 1..Solution_Length
            for (int i = 0; i < num_Gen_of_Task_j; i++)
            {
                ind_tour[i] = ind_tour[i] % num_Gen_of_Task_j;
            }
            //2. Lay Solution_Length ky tu dau tien 
            for (int i = 0; i < num_Gen_of_Task_j; i++)
            {
                tmpArr[i] = ind_tour[i];
            }
            return tmpArr;
        }

        //T. Paulden and D. K. Smith, “Developing new locality results for the Prüfer Code using a remarkable linear-time decoding algorithm,” the electronic journal of combinatorics, vol. 14, no. 1, p. R55, 2007.
        //F. Rothlauf and D. E. Goldberg, “Prüfer numbers and genetic algorithms: A lesson on how the low locality of an encoding can harm the performance of GAs,” in International Conference on Parallel Problem Solving from Nature, 2000, pp. 395–404.

        public int[,] convert_Prufer_to_Tree(int[] pruferArr, int num_Vertex_In_Tree)
        {
            int pruferLength = num_Vertex_In_Tree - 2;
            int[,] tree = new int[num_Vertex_In_Tree, num_Vertex_In_Tree];
            int eligible_i = 0;
            int prufer_j = 0;

            //Cay chua co canh
            for (int i = 0; i < num_Vertex_In_Tree; i++)
            {
                for (int j = 0; j < num_Vertex_In_Tree; j++)
                {
                    tree[i, j] = 0;
                }
            }

            //step 1
            ArrayList lst_Prufer = new ArrayList(pruferArr);
            ArrayList lst_Eligible = new ArrayList();

            for (int i = 0; i < num_Vertex_In_Tree; i++)
            {
                if (!lst_Prufer.Contains(i))
                {
                    lst_Eligible.Add(i);
                }
            }
        //step 2
        step2_label:

            lst_Eligible.Sort();
            eligible_i = (int)lst_Eligible[0];
            prufer_j = (int)lst_Prufer[0];

            //step 3
            tree[eligible_i, prufer_j] = 1;
            tree[prufer_j, eligible_i] = 1;

            //step 4
            lst_Eligible.Remove(eligible_i);
            lst_Prufer.Remove(prufer_j);

            //step 5
            if (!lst_Prufer.Contains(prufer_j))
            {
                lst_Eligible.Add(prufer_j);
            }

            //step 6
            if (lst_Prufer.Count >= 1)
            {
                goto step2_label;
            }
            tree[(int)lst_Eligible[0], (int)lst_Eligible[1]] = 1;
            tree[(int)lst_Eligible[1], (int)lst_Eligible[0]] = 1;
            return tree;
        }

        //Index tu 1 den n
        private bool check_Assign_Color_To_Vertex_1(int[] color_array, int array_length)
        {
            bool ok = true;
            for (int i = 1; i < array_length + 1; i++)
            {
                if (color_array[i] == 0)
                {
                    ok = false;
                    break;
                }
            }
            return ok;
        }

        //Index tu 1 -> n
        private int find_Largest_Uncolored_Vertex_1(int[] color_array, int array_length)
        {
            int max = -1;
            for (int i = 1; i < array_length + 1; i++)
            {
                if ((color_array[i] == 0) && (i > max))
                {
                    max = i;
                }
            }
            return max;
        }


        /*********************************************************************************************************************************************
         * F. Rothlauf and D. E. Goldberg, “Prüfer numbers and genetic algorithms: A lesson on how the low locality of an encoding can harm the performance of GAs,” in International Conference on Parallel Problem Solving from Nature, 2000, pp. 395–404.
         * C. C. Palmer and A. Kershenbaum, “Representing trees in genetic algorithms,” Proceedings of the First IEEE Conference on Evolutionary Computation. IEEE World Congress on Computational Intelligence, pp. 379–384 vol.1, Jun. 1994.
         * Chuyen tu tree sang ma prufer
         * 
         ********************************************************************************************************************************************/
        //private int[] convert_Tree_to_Pufer(int[,] edge_Matrix, int num_Vertex)
        //{
        //    int[] degree = new int[num_Vertex];
        //    List<int> vertex_Unused = new List<int>();
        //    int[,] tmp_Edge_Matrix = new int[num_Vertex, num_Vertex];

        //    for (int i = 0; i < num_Vertex; i++)
        //    {
        //        degree[i] = 0;
        //        vertex_Unused.Add(i);
        //        for (int j = 0; j < num_Vertex; j++)
        //        {
        //            tmp_Edge_Matrix[i, j] = edge_Matrix[i, j];
        //        }
        //    }

            
        //    while(vertex_Unused.Count > 2)
        //    {
        //        Compute degree of vertex
        //        for (int i = 0; i < vertex_Unused.Count; i++)
        //        {
        //            degree[i] = 0;
        //            for (int j = i + 1; j < num_Vertex; j++)
        //            {

        //            }
        //        }

        //    }






         

        //}



        #endregion

        #region Blod code

        /*********************************************************************************************************************************************
        * ---------------- Recent Advances in the Study of the Dandelion Code, Happy Code, and Blob Code Spanning Tree Representations ---------------
        * Blod code decoding (string-to-tree)
        * Input: A Blob string B = (b2, b3, . . . , bn−1) ∈ Cn.
        * Blod code: 1 -> n
        * Output: The tree T ∈ Tn corresponding to B.=> tree co kich thuoc: n+1   
        * B. A. Julstrom, “The blob code is competitive with edge-sets in genetic algorithms for the minimum routing cost spanning tree problem,” in Proceedings of the 7th annual conference on Genetic and evolutionary computation, 2005, pp. 585–590.
        * T. Paulden and D. K. Smith, “Some novel locality results for the Blob Code spanning tree representation,” in Proceedings of the 9th annual conference on Genetic and evolutionary computation, 2007, pp. 1320–1327.

        ********************************************************************************************************************************************/
        public int[,] convert_Blod_to_Tree(int[] blob_Arr, int num_Vertex_In_Tree)
        {
            //Step 1: Form an auxiliary array χ in which row j contains all values of i such that bi = j. Let all of the vertices in [1, n]
            List<int>[] x_array = new List<int>[num_Vertex_In_Tree + 1];
            int[,] tree = new int[num_Vertex_In_Tree + 1, num_Vertex_In_Tree + 1];
            List<int> queue_Q = new List<int>();
            int[] color_Vertex = new int[num_Vertex_In_Tree + 1]; //0 - ko mau, 1 - den, 2 - trang
            int vertex_v;

            //Cay chua co canh
            for (int i = 0; i < num_Vertex_In_Tree + 1; i++)
            {
                for (int j = 0; j < num_Vertex_In_Tree + 1; j++)
                {
                    tree[i, j] = 0;
                }
            }

            for (int i = 1; i < num_Vertex_In_Tree + 1; i++)
            {
                x_array[i] = new List<int>();
            }

            for (int i = 2; i <= num_Vertex_In_Tree - 1; i++)//chieu dai mang blob la: num_Vertex_In_Tree - 2
            {
                x_array[blob_Arr[i]].Add(i);
            }

            //Let all of the vertices in [1, n] be uncoloured
            for (int i = 1; i < num_Vertex_In_Tree + 1; i++)
            {
                color_Vertex[i] = 0;
            }

            vertex_v = num_Vertex_In_Tree; //blod code: 0 -> n-1

        step2_label:
            //Step 2: Assign the colour black to vertex v.
            queue_Q.Clear();
            color_Vertex[vertex_v] = 1;

            //Step 3: Form a queue Q containing each uncoloured vertex i such that bi = v (using row v of χ). Then, while Q is nonempty, repeat the following four-step procedure: (a) Let 
            //w be the first element of Q; (b) Assign the colour white to vertex w; (c) Append each uncoloured vertex i satisfying bi = w to the end of Q; (d) Delete w from the front of Q.
            int tmp = 0;
            for (int i = 0; i < x_array[vertex_v].Count; i++)
            {
                tmp = (x_array[vertex_v])[i];
                if (color_Vertex[tmp] == 0)
                {
                    queue_Q.Add(tmp);
                }
            }

            int w = 0;
            while (queue_Q.Count > 0)
            {
                //Step 3.a.
                w = queue_Q[0];
                //Step 3.b.
                color_Vertex[w] = 2;//trang
                //Step 3.c.
                for (int i = 0; i < x_array[w].Count; i++)
                {
                    tmp = (x_array[w])[i];
                    if (color_Vertex[tmp] == 0)
                    {
                        queue_Q.Add(tmp);
                    }
                }
                //Step 3.d
                queue_Q.RemoveAt(0);
            }

            //Step 4: If all vertices in [1, n] are coloured, go to step 5. Otherwise, let v be the largest uncoloured vertex in [1, n], and go to step 2.
            if (!check_Assign_Color_To_Vertex_1(color_Vertex, num_Vertex_In_Tree))
            {
                vertex_v = find_Largest_Uncolored_Vertex_1(color_Vertex, num_Vertex_In_Tree);
                goto step2_label;
            }

            //Step 5: Suppose the black vertices are x1 < x2 < . . . < xt, where t ∈ [2, n] is the total number of black vertices; observe that x1 = 1 and xt = n. To construct the tree T ∈ Tn
            //corresponding to B, take a set of n isolated vertices (labelled with the integers from 1 to n), create the edge (i, bi) for each white vertex i ∈ [2, n − 1], create the edge (xi, bxi−1) for
            //each i ∈ [3, t], and finally create the edge (x2, 1).

            List<int> black_Vertex = new List<int>();// array x
            for (int i = 1; i < num_Vertex_In_Tree + 1; i++)
            {
                if (color_Vertex[i] == 1)
                {
                    black_Vertex.Add(i);
                }
            }

            black_Vertex.Sort();//sắp xếp tăng dần?

            //Gán màu cho các đỉnh mau trang
            for (int i = 2; i < num_Vertex_In_Tree + 1; i++)
            {
                if (color_Vertex[i] == 2)
                {
                    tree[i, blob_Arr[i]] = 1;
                    tree[blob_Arr[i], i] = 1;
                }
            }

            int x_i_1 = 0;
            for (int i = 3; i < black_Vertex.Count + 1; i++)//index tu 0 nen i = 2 chu ko phai bang 3
            {
                tmp = black_Vertex[i - 1];//vi index mang tu 0
                x_i_1 = black_Vertex[i - 1 - 1];
                tree[tmp, blob_Arr[x_i_1]] = 1;
                tree[blob_Arr[x_i_1], tmp] = 1;
            }
            //create the edge (x2, 1)
            tmp = black_Vertex[1];
            tree[tmp, 1] = 1;
            tree[1, tmp] = 1;

            return tree;
        }

        #endregion

        #region Edge set
        /*********************************************************************************************************************************************
         * phương thức này tạo ra cây không hợp lệ khi có nhiều đường đi qua 1 đỉnh bị xóa, thì phương thức chỉ tìm và nối 1 đường, các đường khác   * 
         * không nối
         * 
         ********************************************************************************************************************************************/
        public int[,] decoding_MFO_Edge_Set(double[,] ind_Matrix, int max_Genes, int num_Gen_of_Task_j)
        {
            int[,] tmp_Matrix = new int[max_Genes, max_Genes];
            for (int i = 0; i < max_Genes; i++)
            {
                for (int j = 0; j < max_Genes; j++)
                {
                    tmp_Matrix[i, j] = (int)ind_Matrix[i, j];
                }
            }
            //01. Tìm các đỉnh lớn hơn num_Gen_of_Task_j và là nút lá.
            bool ok = false;
            do
            {
                ok = false;
                for (int i = num_Gen_of_Task_j; i < max_Genes; i++)
                {
                    int deg_Vertex = 0;     //dem bac
                    int inter_Vertex = -1;  //Đỉnh nối với đỉnh bỏ đi là đỉnh lá.
                    for (int j = 0; j < max_Genes; j++)
                    {
                        if ((tmp_Matrix[i, j] > 0) && (i != j))
                        {
                            deg_Vertex++;
                            inter_Vertex = j;
                        }
                    }
                    //nếu i là nút lá thì xóa bỏ
                    if (deg_Vertex == 1)
                    {
                        tmp_Matrix[i, inter_Vertex] = 0;
                        tmp_Matrix[inter_Vertex, i] = 0;
                        ok = true;
                    }
                }
            } while (ok);

            //02. Xóa đỉnh trung gian nối 2 đỉnh

            do
            {
                ok = false;
                for (int i = 0; i < max_Genes; i++)
                {
                    for (int j = i+1; j < max_Genes; j++)
                    {
                        for (int k = num_Gen_of_Task_j; k < max_Genes; k++)
                        {
                            if ((k != i) && (k != j))
                            {
                                if ((tmp_Matrix[i, k] > 0) && (tmp_Matrix[k, j] > 0))
                                {
                                    ok = true;
                                    //

                                    //xoa canh cu
                                    tmp_Matrix[i, k] = 0;
                                    tmp_Matrix[k, i] = 0;
                                    tmp_Matrix[k, j] = 0;
                                    tmp_Matrix[j, k] = 0;
                                    //tao canh moi
                                    tmp_Matrix[i, j] = 1;
                                    tmp_Matrix[j, i] = 1;
                                }
                            }
                        }
                    }
                }
            } while (ok);

            //Tao ra ma tran ket qua
            int[,] final_Matrix = new int[num_Gen_of_Task_j, num_Gen_of_Task_j];
            for (int i = 0; i < num_Gen_of_Task_j; i++)
            {
                for (int j = 0; j < num_Gen_of_Task_j; j++)
                {
                    final_Matrix[i, j] = tmp_Matrix[i, j];
                }
            }
            return final_Matrix;
        }


        /*********************************************************************************************************************************************
         * Khắc phục hạn chế của phương thức trên                                                                                                    * 
         * 01. Với mỗi i thuộc [1, max_vertex]
         * 02. Xét các đỉnh cần bỏ k
         * 03. Tìm các đỉnh nối với k mà khác i
         * 04. Xóa k và nối các đỉnh nối với k thành nối với i
         ********************************************************************************************************************************************/
        public int[,] decoding_MFO_Edge_Set_1(double[,] ind_Matrix, int max_Genes, int num_Gen_of_Task_j)
        {
            int[,] tmp_Matrix = new int[max_Genes, max_Genes];
            for (int i = 0; i < max_Genes; i++)
            {
                for (int j = 0; j < max_Genes; j++)
                {
                    tmp_Matrix[i, j] = (int)ind_Matrix[i, j];
                }
            }
            //01. Tìm các đỉnh lớn hơn num_Gen_of_Task_j và là nút lá.
            bool ok = false;
            do
            {
                ok = false;
                for (int i = num_Gen_of_Task_j; i < max_Genes; i++)
                {
                    int deg_Vertex = 0;     //dem bac
                    int inter_Vertex = -1;  //Đỉnh nối với đỉnh bỏ đi là đỉnh lá.
                    for (int j = 0; j < max_Genes; j++)
                    {
                        if ((tmp_Matrix[i, j] > 0) && (i != j))
                        {
                            deg_Vertex++;
                            inter_Vertex = j;
                        }
                    }
                    //nếu i là nút lá thì xóa bỏ
                    if (deg_Vertex == 1)
                    {
                        tmp_Matrix[i, inter_Vertex] = 0;
                        tmp_Matrix[inter_Vertex, i] = 0;
                        ok = true;
                    }
                }
            } while (ok);

            //02. Xóa đỉnh trung gian nối 2 đỉnh
            do
            {
                ok = false;
                for (int i = 0; i < max_Genes; i++)
                {
                    for (int k = num_Gen_of_Task_j; k < max_Genes; k++)
                    {
                        if ((k != i) && (tmp_Matrix[i, k] > 0))
                        {
                            List<int> bride_Vertex = new List<int>();
                            for (int j = 0; j < max_Genes; j++)
                            {
                                if ((k != j) && (i != j) && (tmp_Matrix[k, j] > 0))
                                {
                                    bride_Vertex.Add(j);
                                }
                            }

                            if(bride_Vertex.Count > 0)
                            {
                                ok = true;
                                foreach(int v in bride_Vertex)
                                {
                                    //xoa canh cu
                                    tmp_Matrix[v, k] = 0;
                                    tmp_Matrix[k, v] = 0;
                                    //tao canh moi
                                    tmp_Matrix[i, v] = 1;
                                    tmp_Matrix[v, i] = 1;
                                }
                            }
                        }//if
                    }//for(k..)
                }
            } while (ok);

            //Tao ra ma tran ket qua
            int[,] final_Matrix = new int[num_Gen_of_Task_j, num_Gen_of_Task_j];
            for (int i = 0; i < num_Gen_of_Task_j; i++)
            {
                for (int j = 0; j < num_Gen_of_Task_j; j++)
                {
                    final_Matrix[i, j] = tmp_Matrix[i, j];
                }
            }
            return final_Matrix;
        }


        /*********************************************************************************************************************************************
         * Decoding cho bai toan cay khung
         * Giam tu n dinh ve thanh m dinh
         * 01. Xóa các đỉnh và cạnh liên thuộc với nó > m
         * 02. Tìm các thành phần liên thông
         * 03. Nối các thành phần liên thông thứ i -> i + 1
         * + Chọn ngẫu nhiên ở mỗi thành phần liên thông 1 đỉnh
         * + Chọn đỉnh đầu tiên
         ********************************************************************************************************************************************/
        public int[,] decoding_MFO_Vertex_In_SubGraph(double[,] ind_Matrix, int max_Genes, int num_Gen_of_Task_j)
        {
            double[,] tmp_Matrix = new double[max_Genes, max_Genes];
            //01. Tìm các đỉnh lớn hơn num_Gen_of_Task_j và xóa nó cùng cạnh liên thuộc
            for (int i = 0; i < num_Gen_of_Task_j; i++)
            {
                for (int j = 0; j < num_Gen_of_Task_j; j++)
                {
                    if ((ind_Matrix[i, j] > 0) && (ind_Matrix[i,j] < double.MaxValue))
                    {
                        tmp_Matrix[i, j] = 1.0f;
                    }
                    else
                    {
                         tmp_Matrix[i, j] = 0.0f;
                    }
                }
            }
            //02. Tìm các thành phần liên thông va lay moi tp lien thong 1 dinh
            int[] tp_LT = graph_Method_Class.get_Vertex_In_Each_SubGraph(tmp_Matrix, num_Gen_of_Task_j);

            //03. Nối các thành phần liên thông thứ i -> i + 1
            for (int i = 0; i < tp_LT.Length - 1; i++)
            {
                tmp_Matrix[tp_LT[i], tp_LT[i+1]] = 1.0f;
                tmp_Matrix[tp_LT[i+1], tp_LT[i]] = 1.0f;
            }
          
            //Tao ra ma tran ket qua
            int[,] final_Matrix = new int[num_Gen_of_Task_j, num_Gen_of_Task_j];
            for (int i = 0; i < num_Gen_of_Task_j; i++)
            {
                for (int j = 0; j < num_Gen_of_Task_j; j++)
                {
                    final_Matrix[i, j] = (int)tmp_Matrix[i, j];
                }
            }
            return final_Matrix;
        }



        #endregion

        #region CLUSTERED_TREE
        public double clustered_Tree_Evaluate(double[,] edge_Matrix, double[,] weigh_Matrix, int num_Vertex, int start_Vertex)
        {
            double path_Length = 0;
            int[] pre = new int[num_Vertex];
           
            List<int> path_1;
            //Tạo ma trận trọng của cây khung từ ma trận canh và ma trận trọng số để tìm đường đi ngắn nhất bằng dijstra
            double[,] temp_Matrix = new double[num_Vertex, num_Vertex];
            for (int i = 0; i < num_Vertex; i++)
            {
                for(int j = 0; j < num_Vertex; j++)
                {
                    if ((edge_Matrix[i, j] > 0) && (edge_Matrix[i, j] < double.MaxValue))//neu co canh
                    {
                        temp_Matrix[i, j] = weigh_Matrix[i, j];
                    }
                    else
                    {
                        temp_Matrix[i, j] = 0;
                    }
                }
            }

            //int[,] aaa = new int[num_Vertex, num_Vertex];
            //for (int i = 0; i < num_Vertex; i++)
            //{
            //    for (int j = 0; j < num_Vertex; j++)
            //    {
            //        aaa[i, j] = (int)edge_Matrix[i, j];
            //    }
            //}
            //ioFile.draw_Plot_in_Matlab("clustered_Tree_Evaluate_edge_Matrix" + @".m", num_Vertex, aaa);

            //for (int i = 0; i < num_Vertex; i++)
            //{
            //    for (int j = 0; j < num_Vertex; j++)
            //    {
            //        aaa[i, j] = (int)temp_Matrix[i, j];
            //    }
            //}
            //ioFile.draw_Plot_in_Matlab("clustered_Tree_Evaluate_temp_Matrix" + @".m", num_Vertex, aaa);

            //for (int i = 0; i < num_Vertex; i++)
            //{
            //    for (int j = 0; j < num_Vertex; j++)
            //    {
            //        aaa[i, j] = (int)weigh_Matrix[i, j];
            //    }
            //}
            //ioFile.draw_Plot_in_Matlab("clustered_Tree_Evaluate_weigh_Matrix" + @".m", num_Vertex, aaa);

            //pre = graph_Method_Class.dijkstra(temp_Matrix, num_Vertex, start_Vertex);
            //for (int i = 0; i < num_Vertex; i++)
            //{
            //    path_1 = graph_Method_Class.print_Path(start_Vertex, i, pre);
            //    for (int l = path_1.Count - 1; l > 0; l--)
            //    {
            //        path_Length = path_Length + temp_Matrix[path_1[l], path_1[l - 1]];
            //    }
            //}

            path_Length = compute_Cost_of_Tree(temp_Matrix, edge_Matrix, num_Vertex, start_Vertex);

            return path_Length;
        }

        //Tính tổng đường đi ngắn nhất từ các đỉnh tới đỉnh nguồn
        public double compute_Cost_of_Tree(double[,] weightMatrix, double[,] tree, int num_vertex, int startVertex)
        {
            double[] distances = new double[num_vertex];// distance between root and the others
            double sum = 0;
            distances[startVertex] = 0;
            Boolean[] mark = new Boolean[num_vertex];
            Queue queue = new Queue();
            for (int i = 0; i < num_vertex; i++)
            {
                mark[i] = true;
            }
            queue.Enqueue(startVertex);
            while (queue.Count > 0)
            {
                int u = (int)queue.Dequeue();
                mark[u] = false;
                for (int i = 0; i < num_vertex; i++)
                {
                    if (tree[u, i] > 0 && mark[i])
                    {
                        queue.Enqueue(i);
                        mark[i] = false;
                        distances[i] = distances[u] + weightMatrix[u, i];
                        sum += distances[i];
                    }
                }
            }
            return sum;
        }

        public double compute_Cost_of_Tree(double[,] weightMatrix, int[,] tree, int num_vertex, int startVertex)
        {
            double[] distances = new double[num_vertex];// distance between root and the others
            double sum = 0;
            distances[startVertex] = 0;
            Boolean[] mark = new Boolean[num_vertex];
            Queue queue = new Queue();
            for (int i = 0; i < num_vertex; i++)
            {
                mark[i] = true;
            }
            queue.Enqueue(startVertex);
            while (queue.Count > 0)
            {
                int u = (int)queue.Dequeue();
                mark[u] = false;
                for (int i = 0; i < num_vertex; i++)
                {
                    if (tree[u, i] > 0 && mark[i])
                    {
                        queue.Enqueue(i);
                        mark[i] = false;
                        distances[i] = distances[u] + weightMatrix[u, i];
                        sum += distances[i];
                    }
                }
            }
            return sum;
        }

        /*********************************************************************************************************************************************
         * 
         * 
         * 
         ***********************************************************************************************************************************************/

        #endregion


        //The distance dab between two trees evaluate_Tree and opt_Tree
        //B. Schindler, F. Rothlauf, and H.-J. Pesch, “Evolution strategies, network random keys, and the one-max tree problem,” in Workshops on Applications of Evolutionary Computation, 2002, pp. 143–152.

        private int one_Max_Tree_Evaluate(int[,] evaluate_Tree, int[,] opt_Tree, int num_Vertex)
        {
            int dis = 0;
            int l_Eval_Tree = 0;
            int l_Opt_Tree = 0;
            for (int i = 1; i <= num_Vertex - 1; i++)
            {
                for (int j = 0; j <= i - 1; j++)
                {
                    if (evaluate_Tree[i, j] > 0)
                    {
                        l_Eval_Tree = 1;
                    }
                    else
                    {
                        l_Eval_Tree = 0;
                    }
                    if (opt_Tree[i, j] > 0)
                    {
                        l_Opt_Tree = 1;
                    }
                    else
                    {
                        l_Opt_Tree = 0;
                    }
                    dis = dis + Math.Abs(l_Eval_Tree - l_Opt_Tree);
                }
            }
            return dis / 2;
        }

        #region Population Diversity
        /*********************************************************************************************************************************************
         * Algorithm: Tính độ đa dạng của quần thể dựa trên Entropy
         * 
         * Y. Tsujimura and M. Gen, “Entropy-based genetic algorithm for solving TSP,” in Knowledge-Based Intelligent Electronic Systems, 1998. Proceedings KES’98. 1998 Second International Conference on, 1998, vol. 2, pp. 285–290.
         * J. N. Kapur and H. K. Kesavan, “Entropy optimization principles and their applications,” in Entropy and Energy Dissipation in Water Resources, Springer, 1992, pp. 3–20.
         * A. Gupta and Y.-S. Ong, “Genetic transfer or population diversification? deciphering the secret ingredients of evolutionary multitask optimization,” in Computational Intelligence (SSCI), 2016 IEEE Symposium Series on, 2016, pp. 1–7.
         * 
         **********************************************************************************************************************************************/
        private double entropy_Pop(Chromosome[] pop, int pop_Size, int num_Vertex)
        {
            double E_pop = 0.0f,       //Entropy of population
                    E_rs_1 = 0.0f,
                    E_rs_0 = 0.0f;
            //01. Tinh locus(r,s) -> đếm số cạnh xuất hiện tại vị trí (r,s) của các cá thể trong quần thể
            for(int i = 0; i < num_Vertex; i++)
            {
                for(int j = 0; j < num_Vertex; j++)
                {
                    E_rs_1 = 0.0f;
                    E_rs_0 = 0.0f;
                    for(int k = 0; k < pop_Size; k++)
                    {
                        if((pop[k].Edges_Matrix[i,j] > 0) && (pop[k].Edges_Matrix[i,j] < double.MaxValue))
                        {
                            E_rs_1 = E_rs_1 + 1;
                        }
                        else
                        {
                            E_rs_0 = E_rs_0 + 1;
                        }
                    }
                    if(E_rs_1>0)
                    {
                        E_pop = E_pop + -((E_rs_1 / pop_Size) * Math.Log(E_rs_1 / pop_Size));
                        //E_pop = E_pop + -((E_rs) * Math.Log(E_rs));
                    }
                    if (E_rs_0 > 0)
                    {
                        E_pop = E_pop + -((E_rs_0 / pop_Size) * Math.Log(E_rs_0 / pop_Size));
                        //E_pop = E_pop + -((E_rs) * Math.Log(E_rs));
                    }
                }
            }
            return E_pop / (num_Vertex * num_Vertex);
        }

        
        /*********************************************************************************************************************************************
         * Algorithm: Tính độ đa dạng của quần thể dựa trên Entropy ==> chỉ tính các phần tử trên đường chéo chính
         * 
         * Y. Tsujimura and M. Gen, “Entropy-based genetic algorithm for solving TSP,” in Knowledge-Based Intelligent Electronic Systems, 1998. Proceedings KES’98. 1998 Second International Conference on, 1998, vol. 2, pp. 285–290.
         * J. N. Kapur and H. K. Kesavan, “Entropy optimization principles and their applications,” in Entropy and Energy Dissipation in Water Resources, Springer, 1992, pp. 3–20.
         * A. Gupta and Y.-S. Ong, “Genetic transfer or population diversification? deciphering the secret ingredients of evolutionary multitask optimization,” in Computational Intelligence (SSCI), 2016 IEEE Symposium Series on, 2016, pp. 1–7.
         * 
         **********************************************************************************************************************************************/
        private double entropy_Pop_1(Chromosome[] pop, int pop_Size, int num_Vertex)
        {
            double E_pop = 0.0f,       //Entropy of population
                   E_rs_1 = 0.0f,
                   E_rs_0 = 0.0f;
            //01. Tinh locus(r,s) -> đếm số cạnh xuất hiện tại vị trí (r,s) của các cá thể trong quần thể
            for (int i = 0; i < num_Vertex; i++)
            {
                for (int j = i+1; j < num_Vertex; j++)
                {
                    E_rs_1 = 0.0f;
                    E_rs_0 = 0.0f;
                    for (int k = 0; k < pop_Size; k++)
                    {
                        if ((pop[k].Edges_Matrix[i, j] > 0) && (pop[k].Edges_Matrix[i, j] < double.MaxValue))
                        {
                            E_rs_1 = E_rs_1 + 1;
                        }
                        else
                        {
                            E_rs_0 = E_rs_0 + 1;
                        }
                    }
                    if (E_rs_1 > 0)
                    {
                        E_pop = E_pop + -((E_rs_1 / pop_Size) * Math.Log(E_rs_1 / pop_Size));
                        //E_pop = E_pop + -((E_rs) * Math.Log(E_rs));
                    }
                    if (E_rs_0 > 0)
                    {
                        E_pop = E_pop + -((E_rs_0 / pop_Size) * Math.Log(E_rs_0 / pop_Size));
                        //E_pop = E_pop + -((E_rs) * Math.Log(E_rs));
                    }
                }
            }
            return E_pop / ((num_Vertex * num_Vertex * 1.0f) / 2 - num_Vertex);
        }



        /*********************************************************************************************************************************************
         * Algorithm: Tính độ đa dạng của quần thể dựa trên Entropy
         * R. W. Morrison and K. A. De Jong, “Measurement of population diversity,” in International Conference on Artificial Evolution (Evolution Artificielle), 2001, pp. 31–41.
         * 
         **********************************************************************************************************************************************/
        private double moment_of_Inertia(Chromosome[] pop, int pop_Size, int num_Vertex)
        {
            double[,] c = new double[num_Vertex, num_Vertex];
            double pop_Diver = 0.0f;

            //01. Compute centroid of P
            for(int i = 0; i < num_Vertex; i++)
            {
                for(int j = 0; j < num_Vertex; j++)
                {
                    c[i, j] = 0.0f;
                    for(int k = 0; k < pop_Size; k++)
                    {
                        c[i, j] = c[i, j] + pop[k].Edges_Matrix[i, j];
                    }
                    c[i, j] = c[i, j] / pop_Size;
                }
            }

            //02. Compute moment-of-inertia
            for (int i = 0; i < num_Vertex; i++)
            {
                for (int j = 0; j < num_Vertex; j++)
                {
                    for (int k = 0; k < pop_Size; k++)
                    {
                        pop_Diver = pop_Diver + (c[i, j] - pop[k].Edges_Matrix[i, j]) * (c[i, j] - pop[k].Edges_Matrix[i, j]);
                    }
                }
            }
            return pop_Diver;
        }

        public double compute_Population_Diversity(Chromosome[] pop, int pop_Size, int num_Vertex, string population_Diversity_Method)
        {
            double pop_Diver = 0.0f;
            switch (population_Diversity_Method.Trim())
            {
                case "ENTROPY_CLUSTERED_TREE":
                    #region ENTROPY_CLUSTERED_TREE
                    pop_Diver =  entropy_Pop(pop, pop_Size, num_Vertex);
                    #endregion
                    break;
                case "ENTROPY_CLUSTERED_TREE_1":
                    #region ENTROPY_CLUSTERED_TREE
                    pop_Diver = entropy_Pop_1(pop, pop_Size, num_Vertex);
                    #endregion
                    break;
                case "MOMENT_OF_INERTIA":
                    #region MOMENT_OF_INERTIA
                    pop_Diver = moment_of_Inertia(pop, pop_Size, num_Vertex);
                    #endregion
                    break;
                case "NONE":
                    #region NONE
                    pop_Diver = 0.0f;
                    #endregion
                    break;
            }
            return pop_Diver;
        }

        #endregion

    }
}
