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

    public class GA
    {
        Crossovers crossosver = new Crossovers();
        Mutation mutation = new Mutation();
        ReadWriteFile ioFile = new ReadWriteFile();
        MFEA mfea_Class = new MFEA();
        Stopwatch stGA = new Stopwatch();
        Random rnd;
        FormatCmdOut frmCmdOut = new FormatCmdOut();
        Crossover_Parameters crss_Para = new Crossover_Parameters();
        Mutation_Parameters mtt_Para = new Mutation_Parameters();
        TerminationCondition termination_Condition = new TerminationCondition();
        Local_Search_Alg local_Search = new Local_Search_Alg();

        long maximum_number_of_evaluations,                     /* The maximum number of evaluations. */
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

        public Data_MFEA main_GA_1(string init_Individual_Alg, TerminationCondition termination_Condition, int pop_Size, Tasks[] tsk, double lambda,
      Crossover_Parameters cros_Para, Mutation_Parameters mu_Para, string selection_pressure, string local_Search_Method, string population_Diversity_Method, Random rnd)
        {
            Initialize_Chromosome init_Chromosome = new Initialize_Chromosome();
            double best_Fitness_In_Generation = double.MaxValue,            //Cá thể tốt nhất của mỗi TAS
                   diversity_of_Pop = 0.0f;
            List<double> ev_Best_Fitness = new List<double>();              //Lưu best fitness tốt nhất qua các thế hệ
            List<double> diversity_of_pop_in_Gers = new List<double>();    //Lưu độ đa dạng quần thể qua các thế hệ

            Chromosome best_Ind_data;
            Evaluate evaluate = new Evaluate();
            Chromosome[] current_pop = new Chromosome[pop_Size];
            Chromosome[] offspring_pop = new Chromosome[pop_Size];
            Chromosome[] c_pop = new Chromosome[2 * pop_Size];
            int num_Genens = tsk[0].Dims,//D_multitask
                count_Evaluate = 0,//Biến trung gian đếm số lần đánh giá khi gọi hàm evaluate
                bst_Idx;

            best_Ind_data = new Chromosome(num_Genens, 1);

            for (int i = 0; i < pop_Size; i++)
            {
                current_pop[i] = new Chromosome(num_Genens, 1);
                offspring_pop[i] = new Chromosome(num_Genens, 1);
                c_pop[i] = new Chromosome(num_Genens, 1);
                c_pop[i + pop_Size] = new Chromosome(num_Genens, 1);
            }

            //Initial Population
            for (int i = 0; i < pop_Size; i++)
            {
                init_Chromosome.initialize_Chromosome(init_Individual_Alg, num_Genens, tsk[0], rnd, ref current_pop[i]);
                current_pop[i].Skill_Factor = -1;
                current_pop[i].Constraint_violation[0] = 0;//DO CHI CO 1 TASSK
            }

            number_of_evaluations = 0;
            //Evaluate population
            bst_Idx = -1;
            for (int i = 0; i < pop_Size; i++)
            {
                evaluate.evaluate(tsk, 1, lambda, ref current_pop[i], ref count_Evaluate);
                number_of_evaluations = number_of_evaluations + count_Evaluate;
                if (current_pop[i].Factorial_Cost[0] < best_Fitness_In_Generation)
                {
                    best_Fitness_In_Generation = current_pop[i].Factorial_Cost[0];//do chi co 1 task
                    bst_Idx = i;
                }
            }
            mfea_Class.copy_Individual(current_pop[bst_Idx], ref  best_Ind_data, num_Genens, 1);
            ev_Best_Fitness.Add(best_Fitness_In_Generation);

            diversity_of_Pop = evaluate.compute_Population_Diversity(current_pop, pop_Size, num_Genens, population_Diversity_Method);
            diversity_of_pop_in_Gers.Add(diversity_of_Pop);

            int generation = 1;
            termination_Condition.Number_of_evaluations = number_of_evaluations;

            //while (generation < max_Generations)
            while (termination_Condition.checkTerminationCondition())
            {
                //i. Apply genetic operators on current-pop to generate an offspring-pop (C). Refer to Algorithm 2.
                int count_Child = 0;
                while(count_Child < pop_Size)
                {
                    int pos1 = rnd.Next(pop_Size);
                    int pos2 = rnd.Next(pop_Size);
                    while (pos1 == pos2)
                    {
                        pos2 = rnd.Next(pop_Size);
                    }
                    List<Chromosome> lst_Child = new List<Chromosome>();
                    mfea_Class.apply_Genetic_Operators(current_pop[pos1], current_pop[pos2], num_Genens, 1, cros_Para, mu_Para, ref lst_Child, tsk[0], rnd);
                    for(int i = 0; i < lst_Child.Count; i++)
                    {
                         mfea_Class.copy_Individual(lst_Child[i], ref offspring_pop[count_Child+i], num_Genens, 1);
                    }
                    count_Child = count_Child + lst_Child.Count;
                }
                
                //ii. Evaluate the individuals in offspring-pop for selected optimization tasks only (see Algorithm 3)
                bst_Idx = -1;
                for (int i = 0; i < pop_Size; i++)
                {
                    evaluate.evaluate(tsk, 1, lambda, ref offspring_pop[i], ref count_Evaluate);
                    number_of_evaluations = number_of_evaluations + count_Evaluate;
                    if (offspring_pop[i].Factorial_Cost[0] < best_Fitness_In_Generation)
                    {
                        best_Fitness_In_Generation = offspring_pop[i].Factorial_Cost[0];//do chi co 1 task
                        bst_Idx = i;
                    }
                    termination_Condition.Number_of_evaluations = number_of_evaluations;
                    //Nếu đủ điều kiện thì dừng
                    if (!termination_Condition.checkTerminationCondition())
                    {
                        break;
                    }
                }
                if (bst_Idx != -1)
                {
                    mfea_Class.copy_Individual(offspring_pop[bst_Idx], ref  best_Ind_data, num_Genens, 1);
                }

                ev_Best_Fitness.Add(best_Fitness_In_Generation);

                //iii. Concatenate offspring-pop and current-pop to form an intermediate-pop (P ∪ C).
                for (int i = 0; i < pop_Size; i++)
                {
                    mfea_Class.copy_Individual(current_pop[i], ref c_pop[i], num_Genens, 1);
                    mfea_Class.copy_Individual(offspring_pop[i], ref c_pop[i + pop_Size], num_Genens, 1);
                }

                //v. Select the fittest individuals from intermediate-pop to form the next current-pop (P).
                if (string.Equals(selection_pressure, "elitist"))
                {
                    Array.Sort(c_pop, new SortAscFactorialCostIdx());
                    for (int j = 0; j < pop_Size; j++)
                    {
                        mfea_Class.copy_Individual(c_pop[j], ref current_pop[j], num_Genens, 1);
                    }
                }
                else
                {
                    if (string.Equals(selection_pressure, "roulette wheel"))
                    {

                    }
                }

                //Local search
                if (!string.IsNullOrEmpty(local_Search_Method) && (local_Search_Method.Trim() != "NONE"))
                {
                    for (int i = 0; i < pop_Size; i++)
                    {
                        local_Search.local_search_alg(tsk[0], local_Search_Method, num_Genens, rnd, ref offspring_pop[i], ref count_Evaluate);
                        number_of_evaluations = number_of_evaluations + count_Evaluate;
                        termination_Condition.Number_of_evaluations = number_of_evaluations;
                        //Nếu đủ điều kiện thì dừng
                        if (!termination_Condition.checkTerminationCondition())
                        {
                            break;
                        }

                    }
                }

                diversity_of_Pop = evaluate.compute_Population_Diversity(current_pop, pop_Size, num_Genens, population_Diversity_Method);
                diversity_of_pop_in_Gers.Add(diversity_of_Pop);

                generation++;
                frmCmdOut.ClearFromPosToEndOfCurrentLine(50);
                Console.Write(String.Format("{0, 7} |   {1, 13} |", generation, ""));
                termination_Condition.Number_of_evaluations = number_of_evaluations;
            } //while

            Data_MFEA data_MFEA = new Data_MFEA(num_Genens, 1);
            mfea_Class.copy_Individual(best_Ind_data, ref data_MFEA.Best_Ind_Data[0], num_Genens, 1);

            foreach (double items in ev_Best_Fitness)
            {
                double[] tmp = new double[1];
                tmp[0] = items;
                data_MFEA.Ev_Best_Fitness.Add(tmp);
            }
            foreach (double items in diversity_of_pop_in_Gers)
            {
                data_MFEA.Diversity_of_Pop.Add(items);
            }
            return data_MFEA;
        }

        public void GA_Alg(string[] args)
        {

            #region Khai bao
            string tmpStr = "",
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
                max_Vertex = 0,                                 /* Số gene tối đa của các TASK <-> Số gen cua nhiễm sắc thể*/
                type_encoding_in_union_search_space = 0,        /* Kiểu mã hóa được sử dụng trong không gian chung giữa các task (sử dụng khi khở tạo quần thể) => có thể khác problem_index của các task*/
                number_Runs = 0;                                /* Số lần chạy <-> số seed */

            double fitness_variance_tolerance,             /* The minimum fitness variance level that is allowed. */
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

            Tasks[] tasks = new Tasks[1];                                    /* Information about data of each tasks */
            tasks[0] = new Tasks();
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

                for (int i = 0; i < args.Length; i++)
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
                        parameter_File = mfea_Class.getSubStringToEnd(args[i], "=");
                        continue;
                    }

                    #endregion

                    #region ALGORITHM_INDEX
                    if (args[i].IndexOf("ALGORITHM_INDEX") != -1)
                    {
                        algorithm_Index = mfea_Class.getSubStringToEnd(args[i], "=");
                        continue;
                    }
                    #endregion

                    #region RUNS
                    if (args[i].IndexOf("RUNS") != -1)
                    {
                        number_Runs = Convert.ToInt32(mfea_Class.getSubStringToEnd(args[i], "="));
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
                        random_seed = Convert.ToInt32(mfea_Class.getSubStringToEnd(args[i], "="));
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
                        population_size = Convert.ToInt32(mfea_Class.getSubStringToEnd(args[i], "="));
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
                        maximum_number_of_evaluations = Convert.ToInt64(mfea_Class.getSubStringToEnd(args[i], "="));
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
                        crossover_Rate = Convert.ToDouble(mfea_Class.getSubStringToEnd(args[i], "="));

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
                        mutation_Rate = Convert.ToDouble(mfea_Class.getSubStringToEnd(args[i], "="));
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
                        lambda = Convert.ToDouble(mfea_Class.getSubStringToEnd(args[i], "="));
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
                        selection_pressure = mfea_Class.getSubStringToEnd(args[i], "=");
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
                        initial_Individual_Alg = mfea_Class.getSubStringToEnd(args[i], "=");
                        continue;
                    }
                    #endregion

                    #region CROSSOVER_NAME
                    if (args[i].IndexOf("CROSSOVER_NAME") != -1)
                    {
                        crss_Para.Crossver_Name = mfea_Class.getSubStringToEnd(args[i], "=");
                        continue;
                    }
                    #endregion

                    #region MUTATION_NAME
                    if (args[i].IndexOf("MUTATION_NAME") != -1)
                    {
                        mtt_Para.Mutation_Name = mfea_Class.getSubStringToEnd(args[i], "=");
                        continue;
                    }
                    #endregion

                    #region LOCAL_SEARCH_ALGORITHM
                    if (args[i].IndexOf("LOCAL_SEARCH_ALGORITHM") != -1)
                    {
                        local_Search_Alg = mfea_Class.getSubStringToEnd(args[i], "=");
                        continue;
                    }
                    #endregion

                    #region POPULATION_DIVERSITY_ALGORITHM
                    if (args[i].IndexOf("POPULATION_DIVERSITY_ALGORITHM") != -1)
                    {
                        population_Diversity_Alg = mfea_Class.getSubStringToEnd(args[i], "=");
                        continue;
                    }
                    #endregion

                    #region Read_Info_Of_Task
                    if (args[i].IndexOf("INSTANCE_NAME") != -1)
                    {
                        //int idx = Convert.ToInt32(args[i].Substring(args[i].LastIndexOf("_") + 1, 1))-1;
                        instance_name = mfea_Class.getSubStringToEnd(args[i], "=");
                        tasks[0].Data_file_path = instance_name;

                        //Tach lay ten file bo duong dan
                        instance_name = Path.GetFileNameWithoutExtension(Path.GetFileName(instance_name));
                        instance_name = Char.ToUpperInvariant(instance_name[0]) + instance_name.Substring(1);
                        all_Instances = instance_name;//Tên các bộ dữ liệu Test -> tạo thư mục lưu kết quả
                     
                        continue;
                    }
                    //Lấy thông tin về kiểu file dữ liệu input
                    if (args[i].IndexOf("INSTANCE_TYPE") != -1)
                    {
                        tasks[0].Function_Cost_Name = mfea_Class.getSubStringToEnd(args[i], "=");
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

            if (!File.Exists(tasks[0].Data_file_path))
            {
                Console.WriteLine("Do not exist file: " + tasks[0].Data_file_path);
                return;
            }
            ioFile.read_Data(ref tasks[0]);
            tasks[0].MaxDims = tasks[0].Dims;

            #endregion

            tmpStr = all_Instances;
            //Ghi thong tin ra file

            folderResults = @"Results";
            Directory.CreateDirectory(folderResults);

            all_Instances = string.Format("Para_File({0})_Instance({1})", Path.GetFileNameWithoutExtension(parameter_File), all_Instances);
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

            if (number_Runs <= 1)
            {
                if (random_seed == 1)
                {
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    Console.WriteLine("| Algorithm |      Instances            | Seed |  Geners  |        Best Cost|");
                    Console.WriteLine("-----------------------------------------------------------------------------");
                }

                Console.Write(String.Format("| {0,-9} | {1,-25} | {2,4} |", "GA_" + algorithm_Index.ToString(), "", random_seed));

                //Console.SetCursorPosition(13, Console.CursorTop);
                //Console.Write(String.Format(" {0, 0} ", tmpStr));

                instance_name = Path.GetFileNameWithoutExtension(Path.GetFileName(tasks[0].Data_file_path));
                tempPath = folderResults + @"\" + string.Format("Para_File({0})_", Path.GetFileNameWithoutExtension(parameter_File)) + "Instance_" + "(" + instance_name + @")_Seed(" + random_seed.ToString() + ").opt";
                if (File.Exists(tempPath))
                {
                    return;
                }
                stGA.Reset();
                stGA.Start();
                rnd = new Random(random_seed);
                Data_MFEA final_Results = main_GA_1(initial_Individual_Alg, termination_Condition, population_size, tasks, lambda, crss_Para, mtt_Para, 
                    selection_pressure, local_Search_Alg, population_Diversity_Alg, rnd);
                Console.WriteLine();
                stGA.Stop();
                instance_name = Path.GetFileNameWithoutExtension(Path.GetFileName(tasks[0].Data_file_path));
                instance_name = Char.ToUpperInvariant(instance_name[0]) + instance_name.Substring(1);

                tempPath = folderResults + @"\" + string.Format("Para_File({0})_", Path.GetFileNameWithoutExtension(parameter_File)) + "Instance" + "(" + instance_name + @")_Seed(" + random_seed.ToString() + ").opt";

                //ioFile.write_Solution_to_File(tempPath, random_seed, final_Results.Best_Ind_Data[i].Rnvec, tasks[i].Dims, final_Results.Best_Ind_Data[i].Obj_value[i]);

                ioFile.write_Results(tempPath, random_seed, tasks[0], 0, final_Results.Best_Ind_Data[0], stGA.Elapsed.ToString());

                //Console.WriteLine(string.Format("| {0,-9} | {1,-25} | {2,4} | {3,9}| {4,15} |", "", "+ " + instance_name, "", "", final_Results.Best_Ind_Data[0].Obj_value[0].ToString("#.##")));
                Console.SetCursorPosition(60, Console.CursorTop);
                Console.Write(string.Format("{0,15}", final_Results.Best_Ind_Data[0].Obj_value[0].ToString("0.00")));
                Console.WriteLine();

                tempPath = folderResults + @"\" + all_Instances + "_Seed(" + random_seed.ToString() + ").gen";
                ioFile.write_Results_In_Generations(tempPath, final_Results.Ev_Best_Fitness, 1);
                tempPath = folderResults + @"\" + all_Instances + "_Seed(" + random_seed.ToString() + ").div";
                ioFile.write_Parameters_In_Generations(tempPath, final_Results.Diversity_of_Pop, "Pop_Diversity");
            }
            else
            {
                instance_name = Path.GetFileNameWithoutExtension(Path.GetFileName(tasks[0].Data_file_path));
                //tempPath = folderResults + @"\" + string.Format("Para_File({0})_", Path.GetFileNameWithoutExtension(parameter_File)) + "Instance" + "(" + instance_name + @")_Seed(" + (1).ToString() + ").opt";
                //if (File.Exists(tempPath))
                //{
                //    return;
                //}

                double sum_Best_Fitness = 0.0f;

                for (int j = 1; j <= number_Runs; j++)
                {
                    if (j == 1)
                    {
                        Console.WriteLine("-----------------------------------------------------------------------------");
                        Console.WriteLine("| Algorithm |      Instances            | Seed |  Geners  |        Best Cost|");
                        Console.WriteLine("-----------------------------------------------------------------------------");
                    }
                    tempPath = folderResults + @"\" + string.Format("Para_File({0})_", Path.GetFileNameWithoutExtension(parameter_File)) + "Instance" + "(" + instance_name + @")_Seed(" + j.ToString() + ").opt";
                    Console.Write(String.Format("| {0,-9} | {1,-25} | {2,4} |", "GA_" + algorithm_Index.ToString(), "", j));

                    Console.SetCursorPosition(13, Console.CursorTop);
                    Console.Write(String.Format(" {0, 0} ", tmpStr));

                    if (File.Exists(tempPath))
                    {
                        Console.WriteLine();
                        continue;
                    }

                   
                    stGA.Reset();
                    stGA.Start();
                    rnd = new Random(j);
                    Data_MFEA final_Results = main_GA_1(initial_Individual_Alg, termination_Condition, population_size, tasks, lambda, crss_Para, mtt_Para,
                        selection_pressure, local_Search_Alg, population_Diversity_Alg, rnd);
                    stGA.Stop();
                    //Console.SetCursorPosition(60, Console.CursorTop);
                    //Console.Write(string.Format("{0,15}", "---------------"));
                    //Console.WriteLine();

                    instance_name = Path.GetFileNameWithoutExtension(Path.GetFileName(tasks[0].Data_file_path));
                    instance_name = Char.ToUpperInvariant(instance_name[0]) + instance_name.Substring(1);

                    tempPath = folderResults + @"\" + string.Format("Para_File({0})_", Path.GetFileNameWithoutExtension(parameter_File)) + "Instance" + "(" + instance_name + @")_Seed(" + j.ToString() + ").opt";

                    //ioFile.write_Solution_to_File(tempPath, random_seed, final_Results.Best_Ind_Data[i].Rnvec, tasks[i].Dims, final_Results.Best_Ind_Data[i].Obj_value[i]);

                    ioFile.write_Results(tempPath, j, tasks[0], 0, final_Results.Best_Ind_Data[0], stGA.Elapsed.ToString());

                    Console.SetCursorPosition(60, Console.CursorTop);
                    Console.Write(string.Format("{0,15}", final_Results.Best_Ind_Data[0].Obj_value[0].ToString("0.00")));
                    Console.WriteLine();

                    tempPath = folderResults + @"\" + all_Instances + "_Seed(" + j.ToString() + ").gen";
                    ioFile.write_Results_In_Generations(tempPath, final_Results.Ev_Best_Fitness, 1);

                    tempPath = folderResults + @"\" + all_Instances + "_Seed(" + j.ToString() + ").div";
                    ioFile.write_Parameters_In_Generations(tempPath, final_Results.Diversity_of_Pop, "Pop_Diversity");

                    sum_Best_Fitness = sum_Best_Fitness + final_Results.Best_Ind_Data[0].Obj_value[0];
                    tempPath = folderResults + @"\" + string.Format("Para_File({0})_", Path.GetFileNameWithoutExtension(parameter_File)) + "Instance" + "(" + instance_name + ").opt";
                    ioFile.writeOptimalSolutionofSeed(tempPath, j, final_Results.Best_Ind_Data[0].Obj_value[0], "");


                }//for
                sum_Best_Fitness = sum_Best_Fitness / number_Runs;
                tempPath = folderResults + @"\" + string.Format("Para_File({0})_", Path.GetFileNameWithoutExtension(parameter_File)) + "Instance" + "(" + instance_name + ").opt";
                ioFile.writeOptimalSolutionofSeed(tempPath, -1, sum_Best_Fitness, "");

            }//if


        }
        


    }
}
