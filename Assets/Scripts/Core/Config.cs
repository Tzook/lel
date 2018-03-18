using UnityEngine;
using System.Collections;

public class Config
{
    #if UNITY_WEBGL
    private bool _local;
    private bool LOCAL
    { get {return _local == null ? _local = GetHostName() == DOMAIN_LOCAL : _local;} }
    [DllImport("__Internal")]
    private static extern string GetHostName();
    #else 
    private const bool LOCAL = !true; // Used for developing locally. This should always be falsy.
    #endif

    public const string DOMAIN_LOCAL = "localhost";
    public const string DOMAIN_PROD = "lul.herokuapp.com";
    public const string DOMAIN = LOCAL ? DOMAIN_LOCAL : DOMAIN_PROD;
    public const string BASE_URL = LOCAL ? ("http://" + DOMAIN + ":5000") : ("https://" + DOMAIN);
    public const string SOCKET_URL = BASE_URL + "/socket.io/";
}