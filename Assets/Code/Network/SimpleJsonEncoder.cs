using System.Collections.Generic;
using BestHTTP.SocketIO.JsonEncoders;
using SimpleJSON;

public class SimpleJsonEncoder : IJsonEncoder
{
    public List<object> Decode(string json)
    {
        JSONNode node = JSON.Parse(json);
        List<object> list = new List<object>();
        list.Add(node[0]);
        list.Add(node[1]);
        return list;
    }

    public string Encode(List<object> obj)
    {
        string result = "[\"" + obj[0] + "\"," + obj[1].ToString() + "]";
        return result;
    }
}