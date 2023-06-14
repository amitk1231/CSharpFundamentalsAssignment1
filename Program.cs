// See https://aka.ms/new-console-template for more information
// using System;
// using System.IO;

// class Program
// {
//     static void Main(string[] args)
//     {
//         string path1 = "InputFile1.json";
//         string path2 = "InputFile2.json";
//         Console.WriteLine(path1 + " " + path2);
//     }
// }
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace ResourceAllocationApp
{
    class Resource
    {
        public int Budget { get; set; }
        public string Role { get; set; }
        public int Reputation { get; set; }
    }

    class RequiredReputation
    {
        public int RequiredReputation { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Read the input files
            List<Resource> resources = ReadResourcesFromFile("InputFile1.json");
            RequiredReputation requiredReputation = ReadRequiredReputationFromFile("InputFile2.json");

            // Find the optimal resource allocation
            List<Resource> optimalAllocation = FindOptimalAllocation(resources, requiredReputation.RequiredReputation);

            // Display the optimal allocation
            Console.WriteLine("Optimal Resource Allocation:");
            foreach (var resource in optimalAllocation)
            {
                Console.WriteLine($"Role: {resource.Role}, Budget: {resource.Budget}");
            }

            Console.ReadLine();
        }

        static List<Resource> ReadResourcesFromFile(string fileName)
        {
            string json = File.ReadAllText(fileName);
            return JsonConvert.DeserializeObject<List<Resource>>(json);
        }

        static RequiredReputation ReadRequiredReputationFromFile(string fileName)
        {
            string json = File.ReadAllText(fileName);
            return JsonConvert.DeserializeObject<RequiredReputation>(json);
        }

        static List<Resource> FindOptimalAllocation(List<Resource> resources, int requiredReputation)
        {
            // Sort resources by reputation (descending order)
            resources.Sort((x, y) => y.Reputation.CompareTo(x.Reputation));

            List<Resource> optimalAllocation = new List<Resource>();

            foreach (var resource in resources)
            {
                int count = requiredReputation / resource.Reputation;
                if (count > 0)
                {
                    int allocatedBudget = resource.Budget * count;
                    optimalAllocation.Add(new Resource { Budget = allocatedBudget, Role = resource.Role, Reputation = resource.Reputation });
                    requiredReputation -= resource.Reputation * count;
                }
            }

            return optimalAllocation;
        }
    }
}
