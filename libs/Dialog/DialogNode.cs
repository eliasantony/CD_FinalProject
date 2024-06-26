namespace libs;

public class DialogNode
{
    public string DialogID { get; set; }
    public string Text { get; set; }
    public List<Response> Responses { get; set; } = new List<Response>();
}