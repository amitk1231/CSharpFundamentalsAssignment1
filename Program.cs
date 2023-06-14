using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace ResourceAllocationApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Read the input file containing the Required Reputation
            string inputFile = "InputFile2.json";
            string inputJson = File.ReadAllText(inputFile);
            var requiredReputation = JsonConvert.DeserializeObject<int>(inputJson);

            // Define the available roles and their corresponding budgets
            var roles = new Dictionary<string, int>
            {
                { "SE1", 100 },
                { "SE2", 150 },
                { "SSE1", 400 },
                { "SSE2", 500 },
                { "Lead", 700 }
            };

            // Find the optimal resource allocation
            var allocation = FindOptimalAllocation(requiredReputation, roles);

            // Prepare the output object
            var output = new
            {
                SE1 = allocation["SE1"],
                SE2 = allocation["SE2"],
                SSE1 = allocation["SSE1"],
                SSE2 = allocation["SSE2"],
                Lead = allocation["Lead"],
                totalBudget = allocation["totalBudget"],
                totalHeadCount = allocation["totalHeadCount"]
            };

            // Serialize the output object to JSON
            string outputJson = JsonConvert.SerializeObject(output, Formatting.Indented);

            // Write the output JSON to the output file
            string outputFile = "OutputFile1.json";
            File.WriteAllText(outputFile, outputJson);

            Console.WriteLine("Resource allocation completed. Output written to OutputFile1.json.");
            Console.ReadLine();
        }

        static Dictionary<string, string> FindOptimalAllocation(int requiredReputation, Dictionary<string, int> roles)
        {
            var allocation = new Dictionary<string, string>
            {
                { "SE1", "0" },
                { "SE2", "0" },
                { "SSE1", "0" },
                { "SSE2", "0" },
                { "Lead", "0" },
                { "totalBudget", "" },
                { "totalHeadCount", "" }
            };

            int minBudget = int.MaxValue;
            int minHeadCount = int.MaxValue;

            // Iterate through all possible allocations
            for (int se1Count = 0; se1Count <= requiredReputation; se1Count++)
            {
                for (int se2Count = 0; se2Count <= requiredReputation; se2Count++)
                {
                    for (int sse1Count = 0; sse1Count <= requiredReputation; sse1Count++)
                    {
                        for (int sse2Count = 0; sse2Count <= requiredReputation; sse2Count++)
                        {
                            for (int leadCount = 0; leadCount <= requiredReputation; leadCount++)
                            {
                                int totalBudget = se1Count * roles["SE1"] + se2Count * roles["SE2"] +
                                    sse1Count * roles["SSE1"] + sse2Count * roles["SSE2"] + leadCount * roles["Lead"];
                                int totalHeadCount = se1Count + se2Count + sse1Count + sse2Count + leadCount;

                                if (totalBudget < minBudget && totalHeadCount <= requiredReputation)
                                {
                                    minBudget = totalBudget;
                                    minHeadCount = totalHeadCount;

                                    allocation["SE1"] = se1Count.ToString();
                                    allocation["SE2"] = se2Count.ToString();
                                    allocation["SSE1"] = sse1Count.ToString();
                                    allocation["SSE2"] = sse2Count.ToString();
                                    allocation["Lead"] = leadCount.ToString();
                                    allocation["totalBudget"] = minBudget.ToString();
                                    allocation["totalHeadCount"] = minHeadCount.ToString();
                                }
                                else if (totalBudget == minBudget && totalHeadCount <= requiredReputation && totalHeadCount < minHeadCount)
                                {
                                    minHeadCount = totalHeadCount;

                                    allocation["SE1"] = se1Count.ToString();
                                    allocation["SE2"] = se2Count.ToString();
                                    allocation["SSE1"] = sse1Count.ToString();
                                    allocation["SSE2"] = sse2Count.ToString();
                                    allocation["Lead"] = leadCount.ToString();
                                    allocation["totalBudget"] = minBudget.ToString();
                                    allocation["totalHeadCount"] = minHeadCount.ToString();
                                }
                            }
                        }
                    }
                }
            }
            Console.WriteLine(allocation);
            return allocation;
        }
    }
}
