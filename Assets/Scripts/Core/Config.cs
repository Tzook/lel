using UnityEngine;
using System.Collections;

public class Config
{
    private const bool LOCAL = false; // Used for developing locally. This should always be falsy.

    public const string DOMAIN = LOCAL ? "localhost" : "lul.herokuapp.com";
    public const string BASE_URL = LOCAL ? ("http://" + DOMAIN + ":5000") : ("https://" + DOMAIN);
    public const string SOCKET_URL = BASE_URL + "/socket.io/";
}