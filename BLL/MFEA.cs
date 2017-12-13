using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DTO;
using BLL;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;


namespace BLL
{
    public class MFEA
    {
        Crossovers crossosver = new Crossovers();
        Mutation mutation = new Mutation(); 
        ReadWriteFile ioFile = new ReadWriteFile();
        Stopwatch stGA = new Stopwatch();
        Random rnd;
        FormatCmdOut frmCmdOut = new FormatCmdOut();
        Crossover_Parameters crss_Para = new Crossover_Parameters();
        Mutation_Parameters mtt_Para = new Mutation_Parameters();
        TerminationCondition termination_Condition = new TerminationCondition();

        long    maximum_number_of_evaluations,                  /* The maximum number of evaluations. */
                number_of_evaluations = 1;                     /* The current number of times a function evaluation was performed. */

        void print_Usage()
        {
            Console.Write("Usage: LTGA [-?] [-P] [-s] [-w] [-v] [-l] [-r] pro dim pop eva vtr tol\n");
            Console.Write(" -?: Prints out this usage information.\n");
            Console.Write(" -P: Prints out a list of all installed optimization problems.\n");
            Console.Write(" -s: Enables computing and writing of statistics every generation.\n");
            Console.Write(" -w: Enables writing of solutions and their fitnesses every generation.\n");
            Console.Write(" -v: Enables verbose mode. Prints the settings before starting the run.\n");
            Console.Write(" -l: Enables printing the contents of the LT every generation.\n");
            Console.Write(" -r: Enables use of vtr in termination condition (value-to-reach).\n");
            Console.Write("\n");
            Console.Write("  pro: Index of optimization problem to be solved (minimization).\n");
            Console.Write("  dim: Number of parameters.\n");
            Console.Write("  pop: Population size per normal.\n");
            Console.Write("  eva: Maximum number of evaluations allowed.\n");
            Console.Write("  vtr: The value to reach. If the objective value of the best feasible solution reaches\n");
            Console.Write("       this value, termination is enforced (if -r is specified).\n");
            Console.Write("  tol: The tolerance level for fitness variance (i.e. minimum fitness variance)\n");

            return;
        }


        //Hàm lấy chuỗi con từ vị trí xuất hiện của của chuổi strDetermination  tới cuối
        public string getSubStringToEnd(string str, string strDeter)
        {
            return str.Substring(str.LastIndexOf(strDeter) + 1, str.Length - str.LastIndexOf(strDeter) - 1);
        }

        public void copy_Individual(Chromosome from_Ind, ref Chromosome to_Ind, int numGenes, int numTasks)
        {
            for (int i = 0; i < numTasks; i++)
            {
                to_Ind.Obj_value[i] = from_Ind.Obj_value[i];
                to_Ind.Constraint_violation[i] = from_Ind.Constraint_violation[i];
                to_Ind.Factorial_Cost[i] = from_Ind.Factorial_Cost[i];
                to_Ind.Factorial_Rank[i] = from_Ind.Factorial_Rank[i];
            }
            to_Ind.Scalar_Fitness = from_Ind.Scalar_Fitness;
            to_Ind.Skill_Factor = from_Ind.Skill_Factor;

            for (int i = 0; i < numGenes; i++)
            {
                to_Ind.Rnvec[i] = from_Ind.Rnvec[i];
                to_Ind.Invec[i] = from_Ind.Invec[i];
            }

            foreach(Edge tmp in from_Ind.Envec)
            {
                to_Ind.Envec.Add(tmp);
            }

            for (int i = 0; i < numGenes; i++)
            {
                for (int j = 0; j < numGenes; j++)
                {
                     to_Ind.Edges_Matrix[i, j] = from_Ind.Edges_Matrix[i, j];
                }
            }
        }

        /*********************************************************************************************************************************************
        *  Tính Factorial rank của các các thể trong quần thể với TASK_J
        *  Output:  + Index của cá thể tốt nhất
         *          + rank của các cá thể trong quần thể
        ********************************************************************************************************************************************/
        public int factorial_Rank_Task_J(ref Chromosome[] population, int pop_Size, int task_j)
        {
            double[] factorial_cost = new double[pop_Size];
            int[] idx = new int[pop_Size];
            for (int i = 0; i < pop_Size; i++)
            {
                idx[i] = i;
                factorial_cost[i] = population[i].Factorial_Cost[task_j];
            }
            Array.Sort(factorial_cost, idx, 0, pop_Size);

            for(int i = 0; i < pop_Size; i++)
            {
                population[idx[i]].Factorial_Rank[task_j] = i;
            }
            return idx[0];
        }

        public void skill_Factor_Ind(ref Chromosome ind, int num_Task, Random rnd)
        {
            int min_value = ind.Factorial_Rank.Min();
            //double[] min_array = Array.FindAll(ind.Rnvec, x => x == min_value);
            List<int> index_Min_Array = new List<int>();
            for (int i = 0; i < num_Task; i++)
            {
                if (ind.Factorial_Rank[i] == min_value)
                {
                    index_Min_Array.Add(i);
                }
            }

            if(index_Min_Array.Count > 1)
            {
                ind.Skill_Factor = index_Min_Array[rnd.Next(index_Min_Array.Count)];
            }
            else
            {
                ind.Skill_Factor = index_Min_Array[0];
            }
        }

        public void apply_Genetic_Operators(Chromosome par_1, Chromosome par_2, int num_Genens, int num_Tasks, Crossover_Parameters cross_Para,
           Mutation_Parameters mutation_Para, ref List<Chromosome> child, Tasks tsk, Random rnd)
        {
            double r_rand = rnd.NextDouble();
            double sf1, sf2;
            if(par_1.Skill_Factor == par_2.Skill_Factor)
            {
                List<Chromosome> tmp = new List<Chromosome>();
                crossosver.MFO_Crossover(cross_Para, par_1, par_2, num_Genens, num_Tasks, ref tmp, tsk, rnd);
                switch (tmp.Count)
                {
                    case 1:
                        #region Lai ghep tao ra 1 ca the con
                        sf1 = rnd.NextDouble();
                        if (sf1 < 0.5)
                        {
                            tmp[0].Skill_Factor = par_1.Skill_Factor;
                        }
                        else
                        {
                            tmp[0].Skill_Factor = par_2.Skill_Factor;
                        }
                        Chromosome tmp1 = new Chromosome(num_Genens, num_Tasks);
                        copy_Individual(tmp[0], ref tmp1, num_Genens, num_Tasks);
                        mutation.MFO_Mutation(mutation_Para, ref tmp1, num_Genens, tsk, rnd);
                        //child[0].copy_From_Individual(tmp1, num_Genens, num_Tasks);
                        child.Add(tmp1);
                        #endregion
                        break;

                    case 2:
                        #region Lai ghep tao ra 2 ca the con
                        sf1 = rnd.NextDouble();
                        if (sf1 < 0.5)
                        {
                            tmp[0].Skill_Factor = par_1.Skill_Factor;
                        }
                        else
                        {
                            tmp[0].Skill_Factor = par_2.Skill_Factor;
                        }
                        Chromosome tmp2 = new Chromosome(num_Genens, num_Tasks);
                        copy_Individual(tmp[0], ref tmp2, num_Genens, num_Tasks);
                        mutation.MFO_Mutation(mutation_Para, ref tmp2, num_Genens, tsk, rnd);
                        //child[0].copy_From_Individual(tmp1, num_Genens, num_Tasks);
                        child.Add(tmp2);

                        sf2 = rnd.NextDouble();
                        if (sf2 < 0.5)
                        {
                            tmp[1].Skill_Factor = par_1.Skill_Factor;
                        }
                        else
                        {
                            tmp[1].Skill_Factor = par_2.Skill_Factor;
                        }
                        Chromosome tmp3 = new Chromosome(num_Genens, num_Tasks);
                        copy_Individual(tmp[0], ref tmp3, num_Genens, num_Tasks);
                        mutation.MFO_Mutation(mutation_Para, ref tmp3, num_Genens, tsk, rnd);
                        //child[0].copy_From_Individual(tmp1, num_Genens, num_Tasks);
                        child.Add(tmp3);
                        #endregion
                        break;
                }
            }
            else
            {
                if ((r_rand < cross_Para.Rmp))
                {
                    List<Chromosome> tmp = new List<Chromosome>();
                    crossosver.MFO_Crossover(cross_Para, par_1, par_2, num_Genens, num_Tasks, ref tmp, tsk, rnd);
                    switch (tmp.Count)
                    {
                        case 1:
                            #region Lai ghep tao ra 1 ca the con
                            sf1 = rnd.NextDouble();
                            if (sf1 < 0.5)
                            {
                                tmp[0].Skill_Factor = par_1.Skill_Factor;
                            }
                            else
                            {
                                tmp[0].Skill_Factor = par_2.Skill_Factor;
                            }
                            Chromosome tmp1 = new Chromosome(num_Genens, num_Tasks);
                            copy_Individual(tmp[0], ref tmp1, num_Genens, num_Tasks);
                            mutation.MFO_Mutation(mutation_Para, ref tmp1, num_Genens, tsk, rnd);
                            //child[0].copy_From_Individual(tmp1, num_Genens, num_Tasks);
                            child.Add(tmp1);
                            #endregion
                            break;

                        case 2:
                            #region Lai ghep tao ra 2 ca the con
                            sf1 = rnd.NextDouble();
                            if (sf1 < 0.5)
                            {
                                tmp[0].Skill_Factor = par_1.Skill_Factor;
                            }
                            else
                            {
                                tmp[0].Skill_Factor = par_2.Skill_Factor;
                            }
                            Chromosome tmp2 = new Chromosome(num_Genens, num_Tasks);
                            copy_Individual(tmp[0], ref tmp2, num_Genens, num_Tasks);
                            mutation.MFO_Mutation(mutation_Para, ref tmp2, num_Genens, tsk, rnd);
                            //child[0].copy_From_Individual(tmp1, num_Genens, num_Tasks);
                            child.Add(tmp2);

                            sf2 = rnd.NextDouble();
                            if (sf2 < 0.5)
                            {
                                tmp[1].Skill_Factor = par_1.Skill_Factor;
                            }
                            else
                            {
                                tmp[1].Skill_Factor = par_2.Skill_Factor;
                            }
                            Chromosome tmp3 = new Chromosome(num_Genens, num_Tasks);
                            copy_Individual(tmp[0], ref tmp3, num_Genens, num_Tasks);
                            mutation.MFO_Mutation(mutation_Para, ref tmp3, num_Genens, tsk, rnd);
                            //child[0].copy_From_Individual(tmp1, num_Genens, num_Tasks);
                            child.Add(tmp3);
                            #endregion
                            break;
                    }
                }
                else
                {
                    Chromosome tmp1 = new Chromosome(num_Genens, num_Tasks);
                    copy_Individual(par_1, ref tmp1, num_Genens, num_Tasks);
                    mutation.MFO_Mutation(mutation_Para, ref tmp1, num_Genens, tsk, rnd);
                    child.Add(tmp1);

                    Chromosome tmp2 = new Chromosome(num_Genens, num_Tasks);
                    copy_Individual(par_2, ref tmp2, num_Genens, num_Tasks);
                    mutation.MFO_Mutation(mutation_Para, ref tmp2, num_Genens, tsk, rnd);
                    child.Add(tmp2);
                }
            }
        
        }

   
        /*********************************************************************************************************************************************
      *  Y. Yuan, Y.-S. Ong, A. Gupta, P. S. Tan, and H. Xu, “Evolutionary Multitasking in Permutation-Based Combinatorial Optimization Problems: Realization with TSP, QAP, LOP, and JSP.”
      *  Input: quần thể 2N cá thể;
      *  Output: mảng các các thể L
      * 
      *                       
     ********************************************************************************************************************************************/
        public void level_based_selection(Chromosome[] pop, int popSize, int numTask, ref int[][] L, ref int[] countElements)
        {
            double[] obj_values;
            int[] idx;

            for (int i = 0; i < numTask; i++)
            {
                countElements[i] = 0;
            }

            for (int i = 0; i < 2 * popSize; i++)
            {
                L[pop[i].Skill_Factor][countElements[pop[i].Skill_Factor]] = i;//index cua ca the trong quan the
                countElements[pop[i].Skill_Factor]++;
            }

            for (int i = 0; i < numTask; i++)
            {
                obj_values = new double[countElements[i]];
                idx = new int[countElements[i]];
                for (int j = 0; j < countElements[i]; j++)
                {
                    obj_values[j] = pop[L[i][j]].Obj_value[i];
                    idx[j] = L[i][j];
                }
                Array.Sort(obj_values, idx);

                for (int j = 0; j < countElements[i]; j++)
                {
                    L[i][j] = idx[j];
                }
            }


        }


        //Ham đếm các phần tử trong mỗi hàng Fi -> |Fi|
        public int count_Elements_Fi(int[][] L, int numTask, int level_i)
        {
            int count = 0;
            for (int i = 0; i < numTask; i++)
            {
                if (L[i][level_i] > 0)
                {
                    count++;
                }
            }
            return count;
        }

        public void Level_Based_Selection_Procedure(Chromosome[] pop_Q, int[][] L, int popSize, int maxGenes, int numTask,
            ref Chromosome[] selectPop, Random rnd)
        {
            int idx_F = 0;
            int u = 0;
            int num_Ind_In_Fi = 0;

            num_Ind_In_Fi = count_Elements_Fi(L, numTask, idx_F);
            while (u + num_Ind_In_Fi < popSize)
            {
                for (int i = 0; i < numTask; i++)
                {
                    if (L[i][idx_F] > 0)
                    {
                        copy_Individual(pop_Q[L[i][idx_F]], ref selectPop[u + i], maxGenes, numTask);
                    }
                }
                u = u + num_Ind_In_Fi;
                idx_F++;
                num_Ind_In_Fi = count_Elements_Fi(L, numTask, idx_F);
            }

            int rnd_idx = 0;

            num_Ind_In_Fi = count_Elements_Fi(L, numTask, idx_F);
            for (int i = 0; i < popSize - u; i++)
            {
                rnd_idx = rnd.Next(num_Ind_In_Fi);
                int tmp_idx = 0;
                for (int j = 0; j < numTask; j++)
                {
                    if (L[j][idx_F] > 0)
                    {
                        if (tmp_idx == rnd_idx)
                        {
                            copy_Individual(pop_Q[L[j][idx_F]], ref selectPop[u + i], maxGenes, numTask);
                            break;
                        }
                        tmp_idx++;
                    }
                }
            }
        }

        /*********************************************************************************************************************************************
         *  MFEA 
         *  + num_Generations: generation count
         *  + pop_Size: population size
         *  + selection_pressure: choose either 'elitist' or 'roulette wheel'
         *  
         *  + rmp: random mating probability
         *  + p_il: probability of individual learning (BFGA quasi-Newton Algorithm) --> Indiviudal Learning is an IMPORTANT component of the MFEA.
         *  + index_Tj: index of task Tj
         *                      
         *  MFO: giai_thuat(0); seed(1); popSize(2); numGeneration(3); rmp(4); mutation_rate(5); lambda(6); selection_pressure(7); 
         *  type_encoding_in_union_search_space(8); num_TASKS(8);...<problem index>-<nameInstance>;
         *  
         ********************************************************************************************************************************************/

        public Data_MFEA main_MFEA_1(string init_Individual_Alg, TerminationCondition termination_Condition, int pop_Size, Tasks[] tasks, int num_Tasks, double lambda,
           Crossover_Parameters cros_Para, Mutation_Parameters mu_Para, string selection_pressure,  string local_Search_Method, 
            string population_Diversity_Method, Random rnd)
        {
            Initialize_Chromosome init_Chromosome = new Initialize_Chromosome();
            double[] best_Obj = new double[num_Tasks];                              //Cá thể tốt nhất của mỗi TASKS
            List<double[]> ev_Best_Fitness = new List<double[]>();                  //Lưu best fitness tốt nhất qua các thế hệ
            List<double[]> lst_Ind_In_Task_In_Generation = new List<double[]>();    //Lưu số các thể thuộc mỗi task của mỗi thế hệ
            List<double> diversity_of_pop_in_Gers = new List<double>();            //Lưu độ đa dạng quần thể qua các thế hệ
            double diversity_of_Pop = 0.0f;
            Chromosome[] best_Ind_data = new Chromosome[num_Tasks];
            Evaluate evaluate = new Evaluate();
            Chromosome[] current_pop = new Chromosome[pop_Size];
            Chromosome[] offspring_pop = new Chromosome[pop_Size];
            Chromosome[] c_pop = new Chromosome[2 * pop_Size];
            double[] best_Fitness_In_Generation = new double[num_Tasks];
            double[] num_Ind_In_Task_In_Generation = new double[num_Tasks];

            int max_Genens = 0,//D_multitask
                idx_Max_Task = 0,
                count_Evaluate = 0;//Biến trung gian đếm số lần đánh giá khi gọi hàm evaluate

            if (num_Tasks <= 1)
            {
                Console.WriteLine("At least 2 tasks required for MFEA");
            }

            for (int i = 0; i < num_Tasks; i++)
            {
                best_Obj[i] = double.MaxValue;
                if (tasks[i].Dims > max_Genens)
                {
                    max_Genens = tasks[i].Dims;
                    idx_Max_Task = i;
                }
            }

            for (int i = 0; i < num_Tasks; i++)
            {
                tasks[i].MaxDims = max_Genens;
                best_Ind_data[i] = new Chromosome(max_Genens, num_Tasks);
            }

            for (int i = 0; i < pop_Size; i++)
            {
                current_pop[i] = new Chromosome(max_Genens, num_Tasks);
                offspring_pop[i] = new Chromosome(max_Genens, num_Tasks);
                c_pop[i] = new Chromosome(max_Genens, num_Tasks);
                c_pop[i + pop_Size] = new Chromosome(max_Genens, num_Tasks);
            }

            //Initial Population
            for (int i = 0; i < pop_Size; i++)
            {
                //current_pop[i].Rnvec = init_Chromosome.initialize_Chromosome(1, max_Genens, rnd);
                init_Chromosome.initialize_Chromosome(init_Individual_Alg, max_Genens, tasks[idx_Max_Task], rnd, ref current_pop[i]);
                current_pop[i].Skill_Factor = -1;
                for (int j = 0; j < num_Tasks; j++)
                {
                    current_pop[i].Constraint_violation[j] = 0;
                }               
            }

            number_of_evaluations = 0;
            //Evaluate population
            for (int i = 0; i < pop_Size; i++)
            {
                //Gán ngẫu nhiên skill factor cho các cá thể ở quần thể khởi tạo để đỡ tính nhiều lần
                //Theo algorithm 1: A. Gupta and Y.-S. Ong, “Genetic transfer or population diversification? deciphering the secret ingredients of evolutionary multitask optimization,” in Computational Intelligence (SSCI), 2016 IEEE Symposium Series on, 2016, pp. 1–7.
                current_pop[i].Skill_Factor = rnd.Next(num_Tasks);

                evaluate.evaluate(tasks, num_Tasks, lambda, ref current_pop[i], ref count_Evaluate);
                number_of_evaluations = number_of_evaluations + count_Evaluate;
            }

            //Compute factorial rank of initial population
            for (int i = 0; i < num_Tasks; i++)
            {
                int bst_Idx = factorial_Rank_Task_J(ref current_pop, pop_Size, i);
                best_Obj[i] = current_pop[bst_Idx].Factorial_Cost[i];
                best_Fitness_In_Generation[i] = best_Obj[i];
                //ev_Best_Fitness[i, 0] = best_Obj[i];
                copy_Individual(current_pop[bst_Idx], ref  best_Ind_data[i], max_Genens, num_Tasks);
            }
            ev_Best_Fitness.Add(best_Fitness_In_Generation);
            diversity_of_Pop = evaluate.compute_Population_Diversity(current_pop, pop_Size, max_Genens, population_Diversity_Method);
            diversity_of_pop_in_Gers.Add(diversity_of_Pop);

            //Compute skill Factor
            for (int i = 0; i < pop_Size; i++)
            {
                skill_Factor_Ind(ref current_pop[i], num_Tasks, rnd);
                double tmp = current_pop[i].Factorial_Cost[current_pop[i].Skill_Factor];
                for (int j = 0; j < num_Tasks; j++)
                {
                    current_pop[i].Factorial_Cost[j] = double.MaxValue;
                }
                current_pop[i].Factorial_Cost[current_pop[i].Skill_Factor] = tmp;
            }

            int generation = 1;
            termination_Condition.Number_of_evaluations = number_of_evaluations;

            while (termination_Condition.checkTerminationCondition())
            {
                //i. Apply genetic operators on current-pop to generate an offspring-pop (C). Refer to Algorithm 2.
                int count_Child = 0;
                while (count_Child < pop_Size)
                {
                    int pos1 = rnd.Next(pop_Size);
                    int pos2 = rnd.Next(pop_Size);
                    while (pos1 == pos2)
                    {
                        pos2 = rnd.Next(pop_Size);
                    }
                    List<Chromosome> lst_Child = new List<Chromosome>();
                    apply_Genetic_Operators(current_pop[pos1], current_pop[pos2], max_Genens, num_Tasks, cros_Para, mu_Para, ref lst_Child, tasks[idx_Max_Task], rnd);
                    for (int i = 0; i < lst_Child.Count; i++)
                    {
                        if (count_Child + (i+1) <= pop_Size)
                        {
                            copy_Individual(lst_Child[i], ref offspring_pop[count_Child + i], max_Genens, num_Tasks);
                            //count_Child++;
                        }
                    }
                    count_Child = count_Child + lst_Child.Count;
                }

                //ii. Evaluate the individuals in offspring-pop for selected optimization tasks only (see Algorithm 3)
                for (int i = 0; i < pop_Size; i++)
                {
                    evaluate.evaluate(tasks, num_Tasks, lambda, ref offspring_pop[i], ref count_Evaluate);
                    number_of_evaluations = number_of_evaluations + count_Evaluate;
                }

                //iii. Concatenate offspring-pop and current-pop to form an intermediate-pop (P ∪ C).
                for (int i = 0; i < pop_Size; i++)
                {
                    copy_Individual(current_pop[i], ref c_pop[i], max_Genens, num_Tasks);
                    copy_Individual(offspring_pop[i], ref c_pop[i + pop_Size], max_Genens, num_Tasks);
                }

                //iv. Update the scalar fitness (φ) and skill factor (τ) of every individual in intermediate-pop.
                //+ Compute factorial rank of initial population
                best_Fitness_In_Generation = new double[num_Tasks];
                for (int i = 0; i < num_Tasks; i++)
                {
                    int bst_Idx = factorial_Rank_Task_J(ref c_pop, 2 * pop_Size, i);
                    if (c_pop[bst_Idx].Factorial_Cost[i] < best_Obj[i])
                    {
                        best_Obj[i] = c_pop[bst_Idx].Factorial_Cost[i];
                        copy_Individual(c_pop[bst_Idx], ref best_Ind_data[i], max_Genens, num_Tasks);
                    }
                    best_Fitness_In_Generation[i] = best_Obj[i];
                }
                ev_Best_Fitness.Add(best_Fitness_In_Generation);

                //+ Compute skill Factor
                for (int i = 0; i < 2 * pop_Size; i++)
                {
                    skill_Factor_Ind(ref c_pop[i], num_Tasks, rnd);
                    c_pop[i].Scalar_Fitness = 1.0f / (c_pop[i].Factorial_Rank[c_pop[i].Skill_Factor] + 1);//do co rank = 0
                }

                //v. Select the fittest individuals from intermediate-pop to form the next current-pop (P).
                switch(selection_pressure.Trim())
                { 
                    case "elitist":
                        #region elitist
                       
                        Array.Sort(c_pop, new SortDecsScalarFitness());
                        for (int j = 0; j < pop_Size; j++)
                        {
                            copy_Individual(c_pop[j], ref current_pop[j], max_Genens, num_Tasks);
                        }
                        #endregion
                        break;
                    case "roulette_wheel":
                        #region roulette wheel


                        #endregion
                        break;
                    case "level_based":
                        #region level_based
                        int[] countElements = new int[num_Tasks];
                        int[][] L = new int[num_Tasks][];
                        for (int i = 0; i < num_Tasks; i++)
                        {
                            L[i] = new int[pop_Size * 2];
                        }

                        for (int i = 0; i < num_Tasks; i++)
                        {
                            for (int j = 0; j < pop_Size * 2; j++)
                            {
                                L[i][j] = -1;
                            }
                        }
                        level_based_selection(c_pop, pop_Size, num_Tasks, ref L, ref countElements);
                        Level_Based_Selection_Procedure(c_pop, L, pop_Size, max_Genens, num_Tasks, ref current_pop, rnd);

                        #endregion
                        break;
                }
                //Thống kê các các thể trong quần thể thuộc task nào
                num_Ind_In_Task_In_Generation = new double[num_Tasks];
                for (int j = 0; j < pop_Size; j++)
                {
                    num_Ind_In_Task_In_Generation[current_pop[j].Skill_Factor] = num_Ind_In_Task_In_Generation[current_pop[j].Skill_Factor] + 1;
                }
                lst_Ind_In_Task_In_Generation.Add(num_Ind_In_Task_In_Generation);

                diversity_of_Pop = evaluate.compute_Population_Diversity(current_pop, pop_Size, max_Genens, population_Diversity_Method);
                diversity_of_pop_in_Gers.Add(diversity_of_Pop);

                generation++;
                frmCmdOut.ClearFromPosToEndOfCurrentLine(50);
                Console.Write(String.Format("{0, 7} |   {1, 13} |", generation, ""));
                termination_Condition.Number_of_evaluations = number_of_evaluations;
            } //while

            Data_MFEA data_MFEA = new Data_MFEA(max_Genens, num_Tasks);
            for (int i = 0; i < num_Tasks; i++)
            {
                copy_Individual(best_Ind_data[i], ref data_MFEA.Best_Ind_Data[i], max_Genens, num_Tasks);
            }
            foreach (double[] items in ev_Best_Fitness)
            {
                data_MFEA.Ev_Best_Fitness.Add(items);
            }
            foreach (double[] items in lst_Ind_In_Task_In_Generation)
            {
                data_MFEA.Num_Ind_in_Task.Add(items);
            }
            foreach (double items in diversity_of_pop_in_Gers)
            {
                data_MFEA.Diversity_of_Pop.Add(items);
            }
            return data_MFEA;
        }

        /*********************************************************************************************************************************************
        * Tham số trong file *.PAR
        *                       
        ********************************************************************************************************************************************/

        public void MFEA_Alg(string[] args)
        {

            #region Khai bao
            string  tmpStr = "",
                    all_Instances = "",
                    tempPath,
                    folderResults,
                    instance_name = "",
                    selection_pressure = "",
                    problem_Instance_Name,                      /* The name of instance of the optimization problem. */
                    saveFile,
                    folder_Results,
                    initial_Individual_Alg = "",
                    local_Search_Alg = "",
                    population_Diversity_Alg = "",
                    algorithm_Index = "",
                    parameter_File = "";

            int population_size = 0,                            /* The size of the population. */
                selection_size = 0,                             /* The size of the selection. */
                random_seed = 0,                                /* The seed used for the random-number generator. */
                number_of_Vertex = 0,                           /* The number of vertex in graph.*/
                problem_index = 0,                              /* The index of the optimization problem. */
                num_Tasks = 1,
                max_Vertex = 0,                                 /* Số gene tối đa của các TASK <-> Số gen cua nhiễm sắc thể*/
                type_encoding_in_union_search_space = 0,        /* Kiểu mã hóa được sử dụng trong không gian chung giữa các task (sử dụng khi khở tạo quần thể) => có thể khác problem_index của các task*/
                number_Runs = 0;                                /* Số lần chạy <-> số seed */

            double  fitness_variance_tolerance,             /* The minimum fitness variance level that is allowed. */
                    vtr,                                    /* The value-to-reach (function value of best solution that is feasible). */
                    best_ever_evaluated_objective_value,    /* The best ever evaluated objective value. */
                    best_ever_evaluated_constraint_value,   /* The best ever evaluated constraint value. */
                    best_prevgen_objective_value,           /* Objective value of best solution in all previous generations. */
                    best_prevgen_constraint_value,          /* Constraint value of best solution in all previous generations. */
                    bstFitness,
                    lambda = 0.0f,
                    crossover_Rate,
                    mutation_Rate;

            byte use_vtr;                                   /* Whether to terminate at the value-to-reach (VTR) (0 = no). */

            int[] bstTour;

            double[] bestFitness;

            Tasks[] tasks = new Tasks[num_Tasks];                                    /* Information about data of each tasks */
            string pattern = " ";
            Regex myRegex = new Regex(pattern);
            Regex rg = new Regex(@"\s+");
            #endregion
            //Prufer_Instance_Data[] instances_Weights;     /* Edges weight matrix of an instance*/

            #region Read parameters
            try
            {
                if (args.Length <= 0)
                {
                    Console.Write("Number of parameters is incorrect, require > 9 parameters (you provided %d).\n\n", args.Length);
                    print_Usage();
                }
                
                for(int i = 0; i < args.Length; i++)
                {
                    args[i] = args[i].Trim();
                    args[i] = rg.Replace(args[i], @" ");
                    args[i] = args[i].Replace(" ", "");

                    if (string.IsNullOrEmpty(args[i]))
                    {
                        continue;
                    }
                    
                    #region PARAMETER_FILE
                    if (args[i].IndexOf("PARAMETER_FILE") != -1)
                    {
                        parameter_File = getSubStringToEnd(args[i], "=");
                        continue;
                    }

                    #endregion

                    #region ALGORITHM_INDEX
                    if (args[i].IndexOf("ALGORITHM_INDEX") != -1)
                    {
                        algorithm_Index = getSubStringToEnd(args[i], "=");
                        continue;
                    }
                    #endregion

                    #region RUNS
                    if (args[i].IndexOf("RUNS") != -1)
                    {
                        number_Runs = Convert.ToInt32(getSubStringToEnd(args[i], "="));
                        if (number_Runs < 1)
                        {
                            Console.WriteLine(" Entered seed value is wrong, value of number of runs must > 0 \n");
                            return;
                        }
                        continue;
                    }

                    #endregion

                    #region SEED
                    if (args[i].IndexOf("SEED") != -1)
                    {
                        random_seed = Convert.ToInt32(getSubStringToEnd(args[i], "="));
                        if (random_seed <= 0.0)
                        {
                            Console.WriteLine(" Entered seed value is wrong, seed value must > 0 \n");
                            return;
                        }
                        
                        //rnd = new Random(random_seed);
                        continue;
                    }
                    #endregion

                    #region POPULATION_SIZE
                    if (args[i].IndexOf("POPULATION_SIZE") != -1)
                    {
                        population_size = Convert.ToInt32(getSubStringToEnd(args[i], "="));
                        if (population_size < 4 || (population_size % 2) != 0)
                        {
                            if (population_size < 4)
                            {
                                Console.WriteLine(" population size read is : %d", population_size);
                                Console.WriteLine("\n Wrong population size entered, hence exist \n");
                                return;
                            }

                            if (population_size % 2 == 1)
                            {
                                Console.WriteLine(" population size read is : %d", population_size);
                                Console.WriteLine("\n Wrong population size entered, hence fix \n");
                                population_size = population_size + 1;
                            }
                        }
                        continue;
                    }
                    #endregion

                    #region NUMBER_OF_EVALUATE
                    if (args[i].IndexOf("NUMBER_OF_EVALUATE") != -1)
                    {
                        maximum_number_of_evaluations = Convert.ToInt64(getSubStringToEnd(args[i], "="));
                        if (number_of_evaluations < 1)
                        {
                            Console.WriteLine("\n number of generations read is : %d", maximum_number_of_evaluations);
                            Console.WriteLine("\n Wrong number of generations entered, hence exiting \n");
                            return;
                        }
                        continue;
                    }
                    #endregion

                    #region CROSSOVER_RATE
                    if (args[i].IndexOf("CROSSOVER_RATE") != -1)
                    {
                        crossover_Rate = Convert.ToDouble(getSubStringToEnd(args[i], "="));

                        if (crossover_Rate <= 0.0 || crossover_Rate >= 1.0)
                        {
                            Console.WriteLine(" Entered rmp value is wrong, rmp value must be in (0,1) \n");
                            return;
                        }
                        crss_Para.Rmp = crossover_Rate;
                        continue;
                    }
                    #endregion

                    #region MUTATION_RATE
                    if (args[i].IndexOf("MUTATION_RATE") != -1)
                    {
                        mutation_Rate = Convert.ToDouble(getSubStringToEnd(args[i], "="));
                        if (mutation_Rate <= 0.0 || mutation_Rate >= 1.0)
                        {
                            Console.WriteLine(" Entered mutation rate value is wrong,  mutation rate value must be in (0,1) \n");
                            return;
                        }
                        mtt_Para.Mutation_Rate = mutation_Rate;
                        continue;
                    }
                    #endregion

                    #region LAMBDA
                    if (args[i].IndexOf("LAMBDA") != -1)
                    {
                        lambda = Convert.ToDouble(getSubStringToEnd(args[i], "="));
                        if (lambda < 0.0)
                        {
                            Console.WriteLine("\n Wrong number of Lambda, hence exiting \n");
                            return;
                        }
                        continue;
                    }
                    #endregion

                    #region SELECTION_PRESSURE
                    if (args[i].IndexOf("SELECTION_PRESSURE") != -1)
                    {
                        selection_pressure = getSubStringToEnd(args[i], "=");
                        if (string.IsNullOrEmpty(selection_pressure))
                        {
                            Console.WriteLine("\n Wrong selection pressure \n");
                            return;
                        }
                        continue;
                    }
                    #endregion

                    #region INITIAL_INDIVIDUAL_ALGORITHM
                    if (args[i].IndexOf("INITIAL_INDIVIDUAL_ALGORITHM") != -1)
                    {
                        initial_Individual_Alg = getSubStringToEnd(args[i], "=");
                        continue;
                    }
                    #endregion

                    #region CROSSOVER_NAME
                    if (args[i].IndexOf("CROSSOVER_NAME") != -1)
                    {
                        crss_Para.Crossver_Name = getSubStringToEnd(args[i], "=");
                        continue;
                    }
                    #endregion

                    #region MUTATION_NAME
                    if (args[i].IndexOf("MUTATION_NAME") != -1)
                    {
                        mtt_Para.Mutation_Name = getSubStringToEnd(args[i], "=");
                        continue;
                    }
                    #endregion

                    #region LOCAL_SEARCH_ALGORITHM
                    if (args[i].IndexOf("LOCAL_SEARCH_ALGORITHM") != -1)
                    {
                        local_Search_Alg = getSubStringToEnd(args[i], "=");
                        continue;
                    }
                    #endregion

                    #region POPULATION_DIVERSITY_ALGORITHM
                    if (args[i].IndexOf("POPULATION_DIVERSITY_ALGORITHM") != -1)
                    {
                        population_Diversity_Alg = getSubStringToEnd(args[i], "=");
                        continue;
                    }
                    #endregion

                    #region NUMBER_OF_TASKS
                    if (args[i].IndexOf("NUMBER_OF_TASKS") != -1)
                    {
                        num_Tasks = Convert.ToInt32(getSubStringToEnd(args[i], "="));
                        if (num_Tasks <= 1)
                        {
                            Console.WriteLine("\n At least 2 tasks required for MFEA \n");
                            return;
                        }
                        tasks = new Tasks[num_Tasks];
                        for (int j = 0; j < num_Tasks; j++)
                        {
                            tasks[j] = new Tasks();
                        }
                        continue;
                    }
                    #endregion

                    #region Read_Info_Of_Task
                    if (args[i].IndexOf("INSTANCE_NAME") != -1)
                    {
                        //int idx = Convert.ToInt32(args[i].Substring(args[i].LastIndexOf("_") + 1, 1))-1;
                        //int idx = Convert.ToInt32(args[i].Substring(args[i].LastIndexOf("_") + 1, args[i].LastIndexOf("=") - args[i].LastIndexOf("_") - 1)) - 1;
                        //string ssss  = args[i].Substring(args[i].LastIndexOf("_") + 1, args[i].LastIndexOf("=") - args[i].LastIndexOf("_") - 1);
                        int idx = Convert.ToInt32(args[i].Substring(args[i].LastIndexOf("=") - 2, 2)) - 1;

                        instance_name = getSubStringToEnd(args[i], "=");
                        tasks[idx].Data_file_path = instance_name;
                        //Tach lay ten file bo duong dan
                        instance_name = Path.GetFileNameWithoutExtension(Path.GetFileName(instance_name));
                        instance_name = Char.ToUpperInvariant(instance_name[0]) + instance_name.Substring(1);
                        if (idx == 0)
                        {
                            all_Instances = instance_name;//Tên các bộ dữ liệu Test -> tạo thư mục lưu kết quả
                        }
                        else
                        {
                            all_Instances = all_Instances + "_" + instance_name;
                        }
                        continue;
                    }
                    //Lấy thông tin về kiểu file dữ liệu input
                    if (args[i].IndexOf("INSTANCE_TYPE") != -1)
                    {
                        //int idx = Convert.ToInt32(args[i].Substring(args[i].LastIndexOf("_") + 1, 1))-1;
                        //int idx = Convert.ToInt32(args[i].Substring(args[i].LastIndexOf("=") - 2, 2)) - 1;
                        //int idx = Convert.ToInt32(args[i].Substring(args[i].IndexOf("_",0,2) + 1, args[i].LastIndexOf("=") - args[i].IndexOf("_",0,2) - 1)) - 1;

                        int idx = Convert.ToInt32(args[i].Substring(args[i].LastIndexOf("=") - 2, 2)) - 1;
                        tasks[idx].Function_Cost_Name = getSubStringToEnd(args[i], "=");
                        continue;
                    }
                    #endregion
                }//for
            }
            catch
            {
                Console.Write("Error parsing parameters.\n\n");
                print_Usage();
            }

            //Đọc dữ liệu từng bài toán
            //Index đầu tiên là: problem_index (kiểu bài toán) ---> index tiếp theo là:  tên file dữ liệu -> bai toan 1 - 8 thi bo qua ten file du lieu
            for (int i = 0; i < num_Tasks; i++)
            {
                if (!File.Exists(tasks[i].Data_file_path))
                {
                    Console.WriteLine("Do not exist file: " + tasks[i].Data_file_path);
                    return;
                }
                ioFile.read_Data(ref tasks[i]);
            }

            #endregion
            tmpStr = all_Instances;
            //Ghi thong tin ra file

            folderResults = @"Results";
            Directory.CreateDirectory(folderResults);

            if(all_Instances.Length > 30)
            {
                all_Instances = all_Instances.Substring(0, 30);
            }
            all_Instances = string.Format("Para_File({0})_NumTask({1})_{2}", Path.GetFileNameWithoutExtension(parameter_File), num_Tasks, all_Instances);
            folderResults = @"Results\" + all_Instances;
            Directory.CreateDirectory(folderResults);

            //Neu da co file ket qua thi ko chay tiep
            //tempPath = folderResults + @"\" + all_Instances + @".opt";
            //if (File.Exists(tempPath))
            //{
            //    Console.WriteLine();
            //    return;
            //}

            //Điều kiện dừng
            termination_Condition.Maximum_number_of_evaluations = maximum_number_of_evaluations;
            termination_Condition.Number_of_evaluations = 0;

            if(number_Runs <= 1)
            {
                if (random_seed == 1)
                {
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    Console.WriteLine("| Algorithm |      Instances            | Seed |  Geners  |        Best Cost|");
                    Console.WriteLine("-----------------------------------------------------------------------------");
                }

                Console.Write(String.Format("| {0,-9} | {1,-25} | {2,4} |", "MFO_" + algorithm_Index.ToString(), "", random_seed));
               
                Console.SetCursorPosition(13, Console.CursorTop);
                Console.Write(String.Format(" {0, 0} ", tmpStr));

                instance_name = Path.GetFileNameWithoutExtension(Path.GetFileName(tasks[0].Data_file_path));
                tempPath = String.Format(@"{0}\{1}Task_{2}({3})_Seed({4}).opt", folderResults, string.Format("Para_File({0})_", Path.GetFileNameWithoutExtension(parameter_File)), 1, instance_name, random_seed);
                if(File.Exists(tempPath))
                {
                    return;
                }
                stGA.Reset();
                stGA.Start();
                rnd = new Random(random_seed);
                Data_MFEA final_Results = main_MFEA_1(initial_Individual_Alg, termination_Condition, population_size, tasks, num_Tasks, lambda, crss_Para, mtt_Para, 
                    selection_pressure, local_Search_Alg, population_Diversity_Alg, rnd);
                stGA.Stop();
                Console.WriteLine();
                for (int i = 0; i < num_Tasks; i++)
                {
                    instance_name = Path.GetFileNameWithoutExtension(Path.GetFileName(tasks[i].Data_file_path));
                    instance_name = Char.ToUpperInvariant(instance_name[0]) + instance_name.Substring(1);

                    tempPath = folderResults + @"\" + string.Format("Para_File({0})_", Path.GetFileNameWithoutExtension(parameter_File)) + "Task_" + (i + 1).ToString() + "(" + instance_name + @")_Seed(" + random_seed.ToString() + ").opt";

                    //ioFile.write_Solution_to_File(tempPath, random_seed, final_Results.Best_Ind_Data[i].Rnvec, tasks[i].Dims, final_Results.Best_Ind_Data[i].Obj_value[i]);

                    ioFile.write_Results(tempPath, random_seed, tasks[i], i, final_Results.Best_Ind_Data[i], stGA.Elapsed.ToString());

                    Console.WriteLine(string.Format("| {0,-9} | {1,-25} | {2,4} | {3,9}| {4,15} |", "", "+ " + instance_name, "", "", final_Results.Best_Ind_Data[i].Obj_value[i].ToString("0.00")));
                }
                tempPath = folderResults + @"\" + all_Instances + "_Seed(" + random_seed.ToString() + ").gen";
                ioFile.write_Results_In_Generations(tempPath, final_Results.Ev_Best_Fitness, num_Tasks);
                tempPath = folderResults + @"\" + all_Instances + "_Seed(" + random_seed.ToString() + ").div";
                ioFile.write_Parameters_In_Generations(tempPath, final_Results.Diversity_of_Pop, "Pop_Diversity");
            }
            else
            {
                instance_name = Path.GetFileNameWithoutExtension(Path.GetFileName(tasks[0].Data_file_path));
                //tempPath = folderResults + @"\" + string.Format("Para_File({0})_", Path.GetFileNameWithoutExtension(parameter_File)) + "Task_" + (1).ToString() + "(" + instance_name + @")_Seed(" + (1).ToString() + ").opt";
                //if (File.Exists(tempPath))
                //{
                //    return;
                //}
                double[] sum_Best_Fitness = new double[num_Tasks];
                for (int k = 0; k < num_Tasks; k++)
                {
                    sum_Best_Fitness[k] = 0.0f;
                }

                for (int j = 1; j <= number_Runs; j++)
                {
                    if (j == 1)
                    {
                        Console.WriteLine("-----------------------------------------------------------------------------");
                        Console.WriteLine("| Algorithm |      Instances            | Seed |  Geners  |        Best Cost|");
                        Console.WriteLine("-----------------------------------------------------------------------------");
                    }
                    Console.Write(String.Format("| {0,-9} | {1,-25} | {2,4} |", "MFO_" + algorithm_Index.ToString(), "", j));
                    Console.SetCursorPosition(13, Console.CursorTop);
                    Console.Write(String.Format(" {0, 0} ", tmpStr));

                    tempPath = folderResults + @"\" + string.Format("Para_File({0})_", Path.GetFileNameWithoutExtension(parameter_File)) + "Task_" + (1).ToString() + "(" + instance_name + @")_Seed(" + j.ToString() + ").opt";
                    if (File.Exists(tempPath))
                    {
                        Console.WriteLine();
                        continue;
                    }
                                        
                    //stGA = new Stopwatch();
                    stGA.Reset();
                    stGA.Start();
                    rnd = new Random(j);
                    Data_MFEA final_Results = main_MFEA_1(initial_Individual_Alg, termination_Condition, population_size, tasks, num_Tasks, lambda, crss_Para, 
                        mtt_Para, selection_pressure, local_Search_Alg, population_Diversity_Alg, rnd);
                    stGA.Stop();
                    
                    Console.SetCursorPosition(60, Console.CursorTop);
                    Console.Write(string.Format("{0,15}", "---------------"));
                    Console.WriteLine();

                    for (int i = 0; i < num_Tasks; i++)
                    {
                        instance_name = Path.GetFileNameWithoutExtension(Path.GetFileName(tasks[i].Data_file_path));
                        instance_name = Char.ToUpperInvariant(instance_name[0]) + instance_name.Substring(1);

                        tempPath = folderResults + @"\" + string.Format("Para_File({0})_", Path.GetFileNameWithoutExtension(parameter_File)) + "Task_" + (i + 1).ToString() + "(" + instance_name + @")_Seed(" + j.ToString() + ").opt";
                      
                        ioFile.write_Results(tempPath, j, tasks[i], i, final_Results.Best_Ind_Data[i], stGA.Elapsed.ToString());

                        sum_Best_Fitness[i] = sum_Best_Fitness[i] + final_Results.Best_Ind_Data[i].Obj_value[i];
                        tempPath = folderResults + @"\" + string.Format("Para_File({0})_", Path.GetFileNameWithoutExtension(parameter_File)) + "Task_" + (i + 1).ToString() + "(" + instance_name + @").opt";
                      
                        ioFile.writeOptimalSolutionofSeed(tempPath, j, final_Results.Best_Ind_Data[i].Obj_value[i], "");

                        Console.WriteLine(string.Format("| {0,-9} | {1,-25} | {2,4} | {3,9}| {4,15} |", "", "+ " + instance_name, "", "", final_Results.Best_Ind_Data[i].Obj_value[i].ToString("0.00")));
                    }

                    tempPath = folderResults + @"\" + all_Instances + "_Seed(" + j.ToString() + ").gen";
                    ioFile.write_Results_In_Generations(tempPath, final_Results.Ev_Best_Fitness, num_Tasks);

                    //In thống kê số lượng các thể thuộc các task trong mỗi thế hệ
                    //tempPath = folderResults + @"\" + all_Instances + "_Seed(" + j.ToString() + ").num";
                    //ioFile.write_Results_In_Generations(tempPath, final_Results.Num_Ind_in_Task, num_Tasks);

                    tempPath = folderResults + @"\" + all_Instances + "_Seed(" + j.ToString() + ").div";
                    ioFile.write_Parameters_In_Generations(tempPath, final_Results.Diversity_of_Pop, "Pop_Diversity");
                }//for
                for (int j = 0; j < num_Tasks; j++)
                {
                    sum_Best_Fitness[j] = sum_Best_Fitness[j] / number_Runs;
                    instance_name = Path.GetFileNameWithoutExtension(Path.GetFileName(tasks[j].Data_file_path));
                    instance_name = Char.ToUpperInvariant(instance_name[0]) + instance_name.Substring(1);
                    tempPath = folderResults + @"\" + string.Format("Para_File({0})_", Path.GetFileNameWithoutExtension(parameter_File)) + "Task_" + (j + 1).ToString() + "(" + instance_name + @").opt";
                   
                    ioFile.writeOptimalSolutionofSeed(tempPath, -1, sum_Best_Fitness[j], "");
                }
            }//if


        }


        #region Prufer code





        #endregion 


    }
}
