using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pensje_XIX_IT_Olympics
{
    class Program
    {
        //IN file directory
        static string fileDirectory = @"";

        static void Main(string[] args)
        {
            BFO bfo = new BFO();

            //bfo.LoadInitialDataByFile(fileDestination);
            bfo.LoadInitialDataByUser();
            bfo.CalculateVariables();
            bfo.CalculateSalaries();
            bfo.PrintResult();
        }
    }

    //Main BFO class
    class BFO
    {
        BFOCalculator bfoCalculator;
        BFOData bfoData;
        BFOStream bfoStream;
        BFODebug bfoDebug;

        public void LoadInitialDataByFile(string fileDest)
        {
            bfoStream.LoadInputParametersByFile(out bfoData.parents, out bfoData.salaries, fileDest);
            //bfoDebug.DebugInitialData(bfoData.parents, bfoData.salaries);
        }

        public void LoadInitialDataByUser()
        {
            bfoStream.LoadInputParametersByUser(out bfoData.parents, out bfoData.salaries);
        }

        public void CalculateVariables()
        {
            List<BFOData.SubTree> subTrees = bfoCalculator.CalculateSubTrees(bfoData.parents, bfoData.salaries);
            bfoData.subTrees = subTrees.OrderBy(x => x.upperSalaryLimit).ToList();
            //bfoDebug.DebugSubtrees(bfoData.subTrees);
        }

        public void CalculateSalaries()
        {
            bfoCalculator.CalculateSalaries(bfoData.subTrees,ref bfoData.salaries);
        }

        public void PrintResult()
        {
            bfoDebug.PrintSalaries(bfoData.salaries);
        }

        public BFO()
        {
            bfoCalculator = new BFOCalculator();
            bfoData = new BFOData();
            bfoStream = new BFOStream();
            bfoDebug = new BFODebug();
        }

        //Class for calculating data
        class BFOCalculator
        {
            //Calculating subTrees 
            public List<BFOData.SubTree> CalculateSubTrees(int[] parents, int[] salaries)
            {
                List<BFOData.SubTree> subTrees = new List<BFOData.SubTree>();

                for (int i = 0; i < parents.Length; i++)
                {
                    if(salaries[i] == 0 && salaries[parents[i] - 1] != 0)
                    {
                        //Console.WriteLine("Creating Subtree for workerID:{0} , ParentID:{1}", i + 1, parents[i]);
                        BFOData.SubTree subTree = new BFOData.SubTree();
                        subTree.treeSize = ExtensionMethods.CalculateSubtreeSize(i + 1, parents);
                        subTree.upperSalaryLimit = salaries[parents[i]- 1];
                        subTree.upperIDLimit = i;

                        subTree.onePath = ExtensionMethods.CalculateOnePath(i + 1, parents);
                        subTrees.Add(subTree);
                    }
                }

                return subTrees;
            }

            public void CalculateSalaries(List<BFOData.SubTree> subTrees, ref int[] salaries)
            {
                List<int> salariesQueue = new List<int>();
                int stackedSalarySum = 0;
                int lastTopSalary = 0;

                for (int i = 0; i < subTrees.Count; i++)
                {
                    int previousTopSalary = lastTopSalary;

                    while(lastTopSalary < subTrees[i].upperSalaryLimit)
                    {
                        lastTopSalary++;
                        if (!salaries.Any(x => x == lastTopSalary))
                        {
                            //Console.WriteLine("Adding to Queue:{0} , on It:{1}", lastTopSalary, i);
                            //salariesQueue.Enqueue(lastTopSalary);
                            salariesQueue.Add(lastTopSalary);
                        }
                    }

                    if(stackedSalarySum == 0 && salariesQueue.Count == subTrees[i].treeSize)
                    {
                        for (int j = 0; j < subTrees[i].onePath.Count; j++)
                        {
                            int salary = salariesQueue[salariesQueue.Count - 1];
                            salariesQueue.Remove(salary);
                            salaries[subTrees[i].onePath[j]] = salary;
                            //Console.WriteLine("Changing salaryID:{0} with salary:{1}", subTrees[i].onePath[j] , salary);
                        }
                        salariesQueue.Clear();
                    }
                    else
                    {
                        stackedSalarySum += subTrees[i].treeSize;
                        if(stackedSalarySum == salariesQueue.Count)
                        {
                            for (int j = 0; j < subTrees[i].onePath.Count; j++)
                            {
                                if (previousTopSalary > salariesQueue[salariesQueue.Count - 1]) break;
                                int salary = salariesQueue[salariesQueue.Count - 1];
                                salariesQueue.Remove(salary);
                                salaries[subTrees[i].onePath[j]] = salary;
                                //Console.WriteLine("Changing salary:{0} with:{1}", subTrees[i].onePath[j], salary);
                            }
                            salariesQueue.Clear();
                            stackedSalarySum = 0;
                        }
                    }

                }

            }

            public BFOCalculator() { }
        }

        class BFOData
        {
            //From 0 to n-1 elements, corresponding to WorkerID - 1;
            public int[] parents;
            //From 0 to n-1 elements, 
            public int[] salaries;
            //List of subtrees
            public List<SubTree> subTrees; 

            //SubTree with OnePath list (from top of tree until branch)
            public struct SubTree
            {
                public int upperSalaryLimit;
                public int upperIDLimit;
                public int treeSize;

                public List<int> onePath;
            }

            public BFOData() { }
        }
        class BFOStream
        {
            public BFOStream() { }

            public void LoadInputParametersByUser(out int[] parents , out int[] salaries)
            {
                int n = int.Parse(Console.ReadLine());
                parents = new int[n];
                salaries = new int[n];

                string workerData = "";

                for (int i = 0; i < n; i++)
                {
                    workerData = Console.ReadLine();

                    List<char> dimensionsChar = new List<char>();
                    int bossID = 0;
                    int salary = 0;

                    for (int j = 0; j < workerData.Length; j++)
                    {
                        //When Whitespace is met, Char[] is Parsed to Int
                        if (char.IsWhiteSpace(workerData[j]))
                        {
                            string numberString = new string(dimensionsChar.ToArray());
                            int number = int.Parse(numberString);
                            bossID = number;
                            dimensionsChar = new List<char>();
                        }
                        //Else, char numbers are added
                        else dimensionsChar.Add(workerData[j]);
                    }
                    string number2String = new string(dimensionsChar.ToArray());
                    salary = int.Parse(number2String);

                    //Ints are saved to arrays
                    parents[i] = bossID;
                    salaries[i] = salary;                    
                }

            }

            public void LoadInputParametersByFile(out int[] parents, out int[] salaries, string fileDest)
            {
                using (StreamReader sr = File.OpenText(fileDest))
                {
                    //Reading first line of Text file to initialize Array
                    string workersCountString = sr.ReadLine();
                    int workersCount = int.Parse(workersCountString);

                    parents = new int[workersCount];
                    salaries = new int[workersCount];

                    string workerData = "";
                    int workerArrayIterator = 0;

                    while ((workerData = sr.ReadLine()) != null)
                    {

                        List<char> dimensionsChar = new List<char>();
                        int bossID = 0;
                        int salary = 0;
                        for (int i = 0; i < workerData.Length; i++)
                        {
                            //When Whitespace is met, Char[] is Parsed to Int
                            if (char.IsWhiteSpace(workerData[i]))
                            {
                                string numberString = new string(dimensionsChar.ToArray());
                                int number = int.Parse(numberString);
                                bossID = number;
                                dimensionsChar = new List<char>();
                            }
                            //Else, char numbers are added
                            else dimensionsChar.Add(workerData[i]);
                        }
                        string number2String = new string(dimensionsChar.ToArray());
                        salary = int.Parse(number2String);

                        parents[workerArrayIterator] = bossID;
                        salaries[workerArrayIterator] = salary;

                        workerArrayIterator++;
                    }
                }
            }
        }

        class BFODebug
        {
            public void DebugInitialData(int[] parents , int[] salaries)
            {
                Console.WriteLine("*****InitialData*****");
                for (int i = 0; i < parents.Length; i++)
                {
                    Console.WriteLine("ID:{0} | Parent:{1} | Salary:{2}", i, parents[i], salaries[i]);
                }
                Console.WriteLine("**********************");
            }

            public void DebugSubtrees(List<BFOData.SubTree> subTrees)
            {
                Console.WriteLine("********SubTrees********");
                for (int i = 0; i < subTrees.Count; i++)
                {
                    object[] consoleParams = new object[4]
                    {
                        i,
                        subTrees[i].upperSalaryLimit,
                        subTrees[i].upperIDLimit + 1,
                        subTrees[i].treeSize
                    };
                    Console.WriteLine("SubTree:{0} | SalaryLimit:{1} | UpperIDLimit:{2} | Size:{3}", consoleParams);
                    Console.WriteLine("SubTree:{0}, OnePath", i);
                    for (int j = 0; j < subTrees[i].onePath.Count; j++)
                    {
                        Console.WriteLine(subTrees[i].onePath[j] + 1);
                    }
                }
            }

            public void PrintSalaries(int[] salaries)
            {
                for (int i = 0; i < salaries.Length; i++)
                {
                    Console.WriteLine(salaries[i]);
                }
            }

            public BFODebug() { }
        }
    }

    public static class ExtensionMethods
    {
        public static int CalculateSubtreeSize(int id, int[] parents)
        {
            int subtreeSize = 0;
            subtreeSize++;
            for (int i = 0; i < parents.Length; i++)
            {
                if (parents[i] == id)
                {
                    subtreeSize += CalculateSubtreeSize(i + 1, parents);
                }
            }
            return subtreeSize;
        }

        //Calculating OnePath ( Recursive SubWorkers until branching is met)
        public static List<int> CalculateOnePath(int id, int[] parents)
        {
            List<int> onePath = new List<int>();
            onePath.Add(id - 1);

            if ((parents.Where(x => x == id)).Count<int>() > 1)
            {
                return onePath;
            }
            else
            {
                for (int i = 0; i < parents.Length; i++)
                {
                    if (parents[i] == id)
                    {
                        onePath.AddRange(CalculateOnePath(i + 1, parents));
                        break;
                    }
                }
                return onePath;
            }
        }
    }

}
