
rem Parameters: giai_thuat(0); ten_file_TSP(1);  ten_file_Out(2); seed(3); so_group(4); cac_gia_tri_group(...)

rem @MFO.exe 104 TSP\eil51.tsp 5eil51.clt 1 5 5	10	20	1	15
@MFO.exe 106 GA\GA_Clustered_Tree.par GA\Type_1_Small.txt GA_Clus_Tree
@MFO.exe 106 GA\GA_Clustered_Tree.par GA\Type_1_Large.txt GA_Clus_Tree
@MFO.exe 106 GA\GA_Clustered_Tree.par GA\Type_2.txt GA_Clus_Tree
@MFO.exe 106 GA\GA_Clustered_Tree.par GA\Type_3_Large.txt GA_Clus_Tree
@MFO.exe 106 GA\GA_Clustered_Tree.par GA\Type_4_Large.txt GA_Clus_Tree
@MFO.exe 106 GA\GA_Clustered_Tree.par GA\Type_5_Large.txt GA_Clus_Tree
@MFO.exe 106 GA\GA_Clustered_Tree.par GA\Type_5_Small.txt GA_Clus_Tree
@MFO.exe 106 GA\GA_Clustered_Tree.par GA\Type_6_Large.txt GA_Clus_Tree
@MFO.exe 106 GA\GA_Clustered_Tree.par GA\Type_6_Small.txt GA_Clus_Tree

