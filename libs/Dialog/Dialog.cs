using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace libs;

public class Dialog
{
    public List<DialogNode> Nodes { get; set; }
    public bool IsCompletedSuccessfully { get; set; }
    private Dictionary<string, DialogNode> nodeDictionary;

    public static Dialog LoadFromJson(string jsonFilePath)
    {
        dynamic jsonData = FileHandler.ReadJson(jsonFilePath);
        Dialog dialog = new Dialog
        {
            Nodes = JsonConvert.DeserializeObject<List<DialogNode>>(jsonData.ToString())
        };
        dialog.nodeDictionary = new Dictionary<string, DialogNode>();
        foreach (var node in dialog.Nodes)
        {
            dialog.nodeDictionary[node.DialogID] = node;
        }
        dialog.IsCompletedSuccessfully = false; // Initialize as not completed
        return dialog;
    }

    public void Start()
    {
        var currentNode = Nodes[0];
        bool correctAnswerGiven = false;

        while (true)
        {
            Console.WriteLine(currentNode.Text);
            if (currentNode.Responses == null || currentNode.Responses.Count == 0)
            {
                break;
            }

            for (int i = 0; i < currentNode.Responses.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {currentNode.Responses[i].ResponseText}");
            }

            int choice = -1;
            while (choice < 0 || choice >= currentNode.Responses.Count)
            {
                Console.Write("Select an option: ");
                string input = Console.ReadLine();
                if (int.TryParse(input, out choice))
                {
                    choice -= 1;
                }
                else
                {
                    choice = -1;
                }
            }

            if (choice >= 0 && choice < currentNode.Responses.Count)
            {
                var selectedResponse = currentNode.Responses[choice];
                if (selectedResponse.IsCorrect)
                {
                    correctAnswerGiven = true;
                }
                currentNode = selectedResponse.NextNode;
                if (currentNode == null)
                {
                    Console.WriteLine("Error: Next node is null.");
                    break;
                }
            }
            else
            {
                Console.WriteLine("Invalid choice, please try again.");
            }
        }

        IsCompletedSuccessfully = correctAnswerGiven;
    }
}