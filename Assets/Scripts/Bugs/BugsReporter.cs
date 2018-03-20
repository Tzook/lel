using System.Collections.Generic;
using System.Text;
using SimpleJSON;
using UnityEngine;

public class BugsReporter
{
    private static BugsReporter _instance; 
    public static BugsReporter Instance
    { get { return _instance == null ? _instance = new BugsReporter() : _instance; } }

    public void ReportBug(string title, string body)
    {
        JSONNode node = new JSONClass();
        Debug.Log("Sending issue...");

        node["title"] = title;
        node["body"] = body;
        node["name"] = LocalUserInfo.Me.ClientCharacter.Name;

        byte[] rawdata = Encoding.UTF8.GetBytes(node.ToString());

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");
        headers.Add("Cookie", CookiesManager.Instance.GetCookiesString());

        WWW req = new WWW(Config.BASE_URL + "/issues/create", rawdata, headers);

        ContinuationManager.Add(() => req.isDone, () =>
        {
            if (!string.IsNullOrEmpty(req.error)) Debug.Log("WWW failed: " + req.error);
            Debug.Log("WWW result : " + req.text);
        });
    }
}