using UnityEngine;
using System.Collections;

public class Config
{
    // public const string BaseUrl = "http://localhost:5000"; // for testing locally
    public const string BaseUrl = "https://lul.herokuapp.com";
    public const string SocketUrl = BaseUrl + "/socket.io/";
}