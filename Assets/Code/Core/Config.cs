using UnityEngine;
using System.Collections;

public class Config
{
    //public const string BASE_URL = "http://localhost:5000"; // for testing locally
    public const string BASE_URL = "https://lul.herokuapp.com";
    public const string SOCKET_URL = BASE_URL + "/socket.io/";
}