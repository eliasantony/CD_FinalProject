namespace libs;

public class Dialog
{  
    private DialogNode _currentNode;
    private DialogNode _startingNode;

    private DialogNode _endNode;

    public Dialog (DialogNode startingNode)
    {
        _startingNode = startingNode;
        _currentNode = startingNode;
        _endNode = new DialogNode("There is nothing left to say...");
    }

    public void Start()
    {
        //(int x, int y) = Console.GetCursorPosition();
        //TODO Clear Buffer of Console to overwrite the nodes
        while (_currentNode != null)
        {
            Console.WriteLine(_currentNode.Text);
            for (int i = 0; i < _currentNode.Responses.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {_currentNode.Responses[i].ResponseText}");
            }

            int choice;
            if(_currentNode.Responses.Count == 0)
                break;

            while (true)
            {
                Console.Write("Choose an option: ");
                if (int.TryParse(Console.ReadLine(), out choice) && choice > 0 && choice <= _currentNode.Responses.Count)
                {
                    break;
                }
                Console.WriteLine("Invalid choice, please try again.");
            }

            _currentNode = _currentNode.Responses[choice - 1].NextNode;
        }

        _currentNode = _endNode;

        Console.WriteLine("End of dialog.");
        Thread.Sleep(2000);

    }
}