using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using PlatformSupport.Collections.ObjectModel;
using UnityEngine;

public class CookiesManager
{    
    private static CookiesManager _instance; 
    public static CookiesManager Instance
    { get { return _instance == null ? _instance = new CookiesManager() : _instance; } }

    public const string UNICORN = "unicorn";

    public ObservableDictionary<string, string> GetCookies()
    {
        ObservableDictionary<string, string> CookiesDictionary = new ObservableDictionary<string, string>();
        
        #if UNITY_WEBGL
        // unfortunately, besthttp does not support cookies in webgl yet
        FillCookieUsingBrowser(CookiesDictionary);
        #else
        FillCookiesWithBestHTTP(CookiesDictionary);
        #endif

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

    public bool HasCookie(string cookieName)
    {
        ObservableDictionary<string, string> CookiesDictionary = GetCookies();
        return CookiesDictionary.ContainsKey(cookieName);
    }

    #if UNITY_WEBGL
    
    [DllImport("__Internal")]
    private static extern void SetCookie(string str);
    
    [DllImport("__Internal")]
    private static extern string GetCookie();

    public void UpdateCookie(List<string> cookies)
    {
        string cookieString = "";
        foreach (string cookie in cookies)
        {
            cookieString += cookie;
        }
        SetCookie(cookieString);
    }

    protected void FillCookieUsingBrowser(ObservableDictionary<string, string> cookiesDictionary)
    {
        string cookieString = GetCookie();

        if (String.IsNullOrEmpty(cookieString)) {
            return;
        }

        string[] values = cookieString.TrimEnd(';').Split(';');

        List<string[]> cookieParts = new List<string[]>();

        foreach (string value in values) 
        {
            cookieParts.Add(value.Split(new[] { '=' }, 2));
        }
        foreach (string[] parts in cookieParts)
        {
            string cookieName = parts[0].Trim();
            string cookieValue = parts.Length == 1 ? String.Empty : parts[1];

            cookiesDictionary[cookieName] = cookieValue;
        }
    }

    #else
    
    protected void FillCookiesWithBestHTTP(ObservableDictionary<string, string> CookiesDictionary)
    {
        foreach (BestHTTP.Cookies.Cookie cookie in BestHTTP.Cookies.CookieJar.GetAll())
        {
            if (cookie.Name == UNICORN && cookie.Domain == Config.DOMAIN && !String.IsNullOrEmpty(cookie.Value))
            {
                CookiesDictionary.Add(UNICORN, cookie.Value);
                break;
            }
        }
    }

    #endif
}