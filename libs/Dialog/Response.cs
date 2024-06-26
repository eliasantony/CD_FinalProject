namespace libs;

public class Response
{
    public string ResponseText { get; set; }
    public DialogNode NextNode { get; set; }
    
    public bool IsCorrect { get; set; } // Indicates if this response is the correct answer
}
