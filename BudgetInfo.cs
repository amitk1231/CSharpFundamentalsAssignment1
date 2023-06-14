// using System;
// using System.Text.Json;
// class BudgetInfo
// {
//     static void Main(string[] args)
//     {
//         //Read the contents of json file
//         string jsonContents = File.ReadAllText("InputFile1.json");
//         //Parse the JSON into a JsonDocument object
//         JsonDocument jsonDocument = JsonDocument.Parse(jsonContents);

//         //Access the values from the JSON document
//         for (int i = 0; i < 5; i++)
//         {
//             int budget = jsonDocument.RootElement[i].GetProperty("Budget").GetInt32();
//             Console.WriteLine($"Budget: {budget}");
//             string role = jsonDocument.RootElement[i].GetProperty("Role").GetString();
//             Console.WriteLine($"Role: {role}");
//             int reputation = jsonDocument.RootElement[i].GetProperty("Reputation").GetInt32();
//             Console.WriteLine($"Reputation: {reputation}");
//         }

//     }
// }