using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class Config
{
    private const string DOMAIN_LOCAL = "localhost";

#if UNITY_WEBGL
        private static bool LOCAL
        { get { return DOMAIN == DOMAIN_LOCAL; } }

        [DllImport("__Internal")]
        private static extern string GetHostName(); 
        private static string hostName;
        public static string DOMAIN
        { get { return hostName = hostName == null ? GetHostName() : hostName; } }        
#else
    private static bool LOCAL = true; // Used for developing locally. This should always be falsy.
    private const string DOMAIN_PROD = "lul.herokuapp.com";
    public static string DOMAIN = LOCAL ? DOMAIN_LOCAL : DOMAIN_PROD;
#endif

    public static string BASE_URL = LOCAL ? ("http://" + DOMAIN + ":5000") : ("https://" + DOMAIN);
    public static string SOCKET_URL = BASE_URL + "/socket.io/";
}