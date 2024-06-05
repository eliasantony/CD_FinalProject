using System.Data.Common;
using System.Net;

namespace libs;

public class DialogNode {

    public string dialogID { get; set; }
    public string Text { get; set; }
    public List<Response> Responses = new List<Response>();

    public DialogNode (string text) {
        Text = text;
    }

    public DialogNode (string id, string text) {
        Text = text;
        dialogID = id;
    }

    public DialogNode (string text, List<Response> responses) {
        Text = text;
        Responses = responses;
    }

    public void AddResponse(string responseText, DialogNode nextNode)
    {
        Responses.Add(new Response(responseText, nextNode));
    }

}