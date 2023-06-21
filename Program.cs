using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace ResourceAllocationApp
{
    public class BudgetInfo
    {
        public string? Role { get; set; }
        public int Budget { get; set; }
        public int Reputation { get; set; }
    }

    public class InputData
    {
        public int RequiredReputation { get; set; }
    }

    public class OutputData
    {
        public string? SE1 { get; set; }
        public string? SE2 { get; set; }
        public string? SSE1 { get; set; }
        public string? SSE2 { get; set; }
        public string? Lead { get; set; }
        public string? TotalBudget { get; set; }
        public string? TotalHeadCount { get; set; }
    }

    class Program
    {
        static void Main()
        {
            // Read input files
            string resourceFile = "InputFile1.json";
            string requiredReputationFile = "InputFile2.json";
            string outputFile = "OutputFile1.json";

            List<BudgetInfo>? resources = ReadResourceData(resourceFile);
            int requiredReputation = ReadRequiredReputation(requiredReputationFile);

            if (resources != null && requiredReputation >= 0)
            {
                OutputData outputData = AllocateResources(resources, requiredReputation);
                if (outputData != null)
                {
                    // Write output to a JSON file
                    WriteOutputData(outputData, outputFile);
                    // Console.WriteLine("Resource allocation completed. Output written to OutputFile1.json");
                    // Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("Failed to allocate resources.");
                }
            }
            else
            {
                Console.WriteLine("Failed to read input data.");
            }

            Console.ReadLine();
        }

        static List<BudgetInfo>? ReadResourceData(string resourceFile)
        {
            try
            {
                string resourceJson = File.ReadAllText(resourceFile);
                List<BudgetInfo>? resources = JsonConvert.DeserializeObject<List<BudgetInfo>>(resourceJson);
                // Console.WriteLine(resources);
                return resources;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading resource data: " + ex.Message);
                return null;
            }
        }

        static int ReadRequiredReputation(string requiredReputationFile)
        {
            try
            {
                string reputationJson = File.ReadAllText(requiredReputationFile);
                InputData inputData = JsonConvert.DeserializeObject<InputData>(reputationJson)!;
                return inputData.RequiredReputation;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading required reputation: " + ex.Message);
                return -1;
            }
        }

        static void WriteOutputData(OutputData outputData, string outputFile)
        {
            try
            {
                string outputJson = JsonConvert.SerializeObject(outputData, Formatting.Indented);

                //Determine the output file path
                // string outputFilePath = Path.Combine(Directory.GetCurrentDirectory(), "OutputFile1.json");
                File.WriteAllText(outputFile, outputJson);
                // File.WriteAllText("OutputFile1.json", outputJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing output file: " + ex.Message);
            }
        }

        static OutputData AllocateResources(List<BudgetInfo> resources, int requiredReputation)
        {
            // Sort resources by budget in ascending order
            resources.Sort((r1, r2) => r1.Budget.CompareTo(r2.Budget));

            // Initialize variables for best allocation
            int minBudget = int.MaxValue;
            int minHeadCount = int.MaxValue;
            Dictionary<string, int> bestAllocation = null!;

            // Iterate through all possible combinations of resources
            int[] allocation = new int[resources.Count];
            int currentHeadCount = 0;

            while (true)
            {
                // Calculate the total budget and reputation of the current allocation
                int totalBudget = 0;
                int totalReputation = 0;
                for (int i = 0; i < resources.Count; i++)
                {
                    totalBudget += allocation[i] * resources[i].Budget;
                    totalReputation += allocation[i] * resources[i].Reputation;
                }

                // Check if the current allocation meets the required reputation
                if (totalReputation == requiredReputation)
                {
                    // Check if the current allocation has the minimum budget and head count
                    if (totalBudget < minBudget || (totalBudget == minBudget && currentHeadCount < minHeadCount))
                    {
                        minBudget = totalBudget;
                        minHeadCount = currentHeadCount;
                        bestAllocation = new Dictionary<string, int>();
                        for (int i = 0; i < resources.Count; i++)
                        {
                            if (allocation[i] > 0)
                                bestAllocation[resources[i].Role] = allocation[i];
                            else
                                bestAllocation[resources[i].Role] = 0;
                        }
                    }
                }

                // Generate the next allocation combination
                int index = resources.Count - 1;
                while (index >= 0 && allocation[index] >= 2)
                {
                    currentHeadCount -= allocation[index];
                    allocation[index] = 0;
                    index--;
                }
                if (index < 0)
                    break;
                allocation[index]++;
                currentHeadCount++;
            }

            // Create the output data object
            OutputData outputData = new()
            {
                // ResourceAllocation = bestAllocation,
                SE1 = bestAllocation.GetValueOrDefault("SE1").ToString(),
                SE2 = bestAllocation.GetValueOrDefault("SE2").ToString(),
                SSE1 = bestAllocation.GetValueOrDefault("SSE1").ToString(),
                SSE2 = bestAllocation.GetValueOrDefault("SSE2").ToString(),
                Lead = bestAllocation.GetValueOrDefault("Lead").ToString(),
                TotalBudget = minBudget.ToString(),
                TotalHeadCount = minHeadCount.ToString()
            };

            return outputData;
        }
    }
}
