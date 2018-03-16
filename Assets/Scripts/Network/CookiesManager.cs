using System.Collections.Generic;
using BestHTTP.Cookies;
using PlatformSupport.Collections.ObjectModel;

public class CookiesManager
{    
    private static CookiesManager _instance; 
    public static CookiesManager Instance
    { get { return _instance == null ? _instance = new CookiesManager() : _instance; } }

    public ObservableDictionary<string, string> GetCookies()
    {
        ObservableDictionary<string, string> CookiesDictionary = new ObservableDictionary<string, string>();
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
        ObservableDictionary<string, string> Cookies = GetCookies();
        string result = "";
        foreach (KeyValuePair<string, string> cookie in Cookies)
        {
            result += cookie.Key + "=" + cookie.Value + ";";
        }
        return result;
    }
}