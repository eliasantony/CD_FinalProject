namespace libs;

public class Response {
    public string ResponseText { get; set; }
    public DialogNode NextNode { get; set; }

    public Response(string responseText, DialogNode nextNode)
    {
        ResponseText = responseText;
        NextNode = nextNode;
    }
}