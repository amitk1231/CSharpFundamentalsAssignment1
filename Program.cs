using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

class Program
{
    static void Main(string[] args)
    {
        string inputFilePath = "InputFile1.json";
        string outputFilePath = "OutputFile1.json";
        string input2FilePath = "InputFile2.json";

        // Read input files
        List<ResourceData> resources = ReadResourceData(inputFilePath);
        int requiredReputation = ReadRequiredReputation(input2FilePath);

        // Allocate resources
        List<AllocationResult> allocations = AllocateResources(resources, requiredReputation);

        // Determine the optimal allocation(s)
        AllocationResult optimalAllocation = GetOptimalAllocation(allocations);

        // Prepare the output data
        var outputData = new
        {
            SE1 = optimalAllocation.GetCountForResource("SE1"),
            SE2 = optimalAllocation.GetCountForResource("SE2"),
            SSE1 = optimalAllocation.GetCountForResource("SSE1"),
            SSE2 = optimalAllocation.GetCountForResource("SSE2"),
            Lead = optimalAllocation.GetCountForResource("Lead"),
            totalBudget = optimalAllocation.Budget,
            totalHeadCount = optimalAllocation.GetTotalCount()
        };

        // Write output to a file
        WriteOutputData(outputFilePath, outputData);

        Console.WriteLine("Resource allocation completed. Output written to OutputFile1.json.");
    }

    static List<ResourceData> ReadResourceData(string filePath)
    {
        string jsonData = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<List<ResourceData>>(jsonData);
    }

    static int ReadRequiredReputation(string filePath)
    {
        string jsonData = File.ReadAllText(filePath);
        var input = JsonConvert.DeserializeObject<Dictionary<string, int>>(jsonData);
        return input["RequiredReputation"];
    }

    static List<AllocationResult> AllocateResources(List<ResourceData> resources, int requiredReputation)
    {
        var allocations = new List<AllocationResult>();
        var memo = new Dictionary<string, AllocationResult>(); // Memoization dictionary
        Allocate(resources, requiredReputation, new List<ResourceData>(), allocations, memo);
        return allocations;
    }

    static void Allocate(List<ResourceData> resources, int remainingReputation, List<ResourceData> currentAllocation, List<AllocationResult> allocations, Dictionary<string, AllocationResult> memo)
    {
        if (remainingReputation == 0)
        {
            // Add current allocation to the list of allocations
            var allocationResult = new AllocationResult();
            allocationResult.AddRange(currentAllocation);
            allocations.Add(allocationResult);
            return;
        }

        if (remainingReputation < 0 || resources.Count == 0)
            return;

        var memoKey = $"{remainingReputation}-{resources.Count}";
        if (memo.ContainsKey(memoKey))
        {
            // If the result is already memoized, use it directly
            var memoizedAllocation = memo[memoKey];
            currentAllocation.AddRange(memoizedAllocation);
            allocations.Add(memoizedAllocation);
            return;
        }

        // Try allocating the current resource and continue allocation recursively
        var currentResource = resources[0];
        currentAllocation.Add(currentResource);
        Allocate(resources, remainingReputation - currentResource.Reputation, currentAllocation, allocations, memo);

        // Try allocating without the current resource and continue allocation recursively
        currentAllocation.Remove(currentResource);
        Allocate(resources.Skip(1).ToList(), remainingReputation, currentAllocation, allocations, memo);

        // Store the result in the memo dictionary
        var newAllocation = new AllocationResult();
        newAllocation.AddRange(currentAllocation);
        memo[memoKey] = newAllocation;
    }

    static AllocationResult GetOptimalAllocation(List<AllocationResult> allocations)
    {
        int minBudget = int.MaxValue;
        int minHeadCount = int.MaxValue;
        AllocationResult optimalAllocation = null;

        foreach (var allocation in allocations)
        {
            int budget = allocation.Sum(a => a.Budget);
            int headCount = allocation.GetTotalCount();

            if (budget < minBudget || (budget == minBudget && headCount < minHeadCount))
            {
                minBudget = budget;
                minHeadCount = headCount;
                optimalAllocation = allocation;
            }
        }

        return optimalAllocation;
    }

    static void WriteOutputData(string filePath, object outputData)
    {
        string jsonData = JsonConvert.SerializeObject(outputData, Formatting.Indented);
        File.WriteAllText(filePath, jsonData);
    }
}

class ResourceData
{
    public string Role { get; set; }
    public int Budget { get; set; }
    public int Reputation { get; set; }
}

class AllocationResult : List<ResourceData>
{
    public int Budget => this.Sum(a => a.Budget);

    public int GetCountForResource(string role)
    {
        return this.Count(a => a.Role == role);
    }

    public int GetTotalCount()
    {
        // var counts = this.GroupBy(a => a.Role)
        //                  .Select(g => new { Role = g.Key, Count = g.Count() })
        //                  .ToDictionary(x => x.Role, x => x.Count);

        // int totalCount = 0;
        // foreach (var resource in counts.Keys)
        // {
        //     totalCount += GetCountForResource(resource);
        // }

        return this.Count;
    }
}
