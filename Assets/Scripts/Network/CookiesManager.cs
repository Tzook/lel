using System.Collections.Generic;
using BestHTTP.Cookies;

public class CookiesManager
{    
    private static CookiesManager _instance; 
    public static CookiesManager Instance
    { get { return _instance == null ? _instance = new CookiesManager() : _instance; } }

    public Dictionary<string, string> GetCookies()
    {
        Dictionary<string, string> CookiesDictionary = new Dictionary<string, string>();
        foreach (Cookie cookie in BestHTTP.Cookies.CookieJar.GetAll())
        {
            if (cookie.Name == "unicorn" && cookie.Domain == Config.DOMAIN)
            {
                CookiesDictionary.Add("unicorn", cookie.Value);
                break;
            }
        }

        return CookiesDictionary;
    }

    public string GetCookiesString()
    {
        Dictionary<string, string> Cookies = GetCookies();
        string result = "";
        foreach (KeyValuePair<string, string> cookie in Cookies)
        {
            result += cookie.Key + "=" + cookie.Value + ";";
        }
        return result;
    }
}