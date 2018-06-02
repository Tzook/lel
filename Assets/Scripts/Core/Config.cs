using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class Config
{
    #if UNITY_WEBGL
    private static string hostName;
    private static bool LOCAL
    { get 
    {
        return (hostName = hostName == null ? GetHostName() : hostName) == DOMAIN_LOCAL;
    } }
    [DllImport("__Internal")]
    private static extern string GetHostName();
    #else 
    private static bool LOCAL = true; // Used for developing locally. This should always be falsy.
    #endif

    public const string DOMAIN_LOCAL = "localhost";
    public const string DOMAIN_PROD = "lul.herokuapp.com";
    public static string DOMAIN = LOCAL ? DOMAIN_LOCAL : DOMAIN_PROD;
    public static string BASE_URL = LOCAL ? ("http://" + DOMAIN + ":5000") : ("https://" + DOMAIN);
    public static string SOCKET_URL = BASE_URL + "/socket.io/";
}