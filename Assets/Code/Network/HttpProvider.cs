using UnityEngine;
using System;
using BestHTTP;
using System.Text;
using SimpleJSON;

abstract public class HttpProvider 
{
	public static bool debug = true;
	private const string BASE_URL = "https://lul.herokuapp.com";
	private Action<JSONNode> _callback;
	
	public HttpProvider(Action<JSONNode> callback)
	{
		_callback = callback;
	}
	
	protected void performRequest(string partialUrl, JSONNode parametersObject, bool isPost)
	{
		string url = BASE_URL + partialUrl;
		if (!isPost) {
			// TODO add url parameters 
		}
		if (debug) {
			Debug.Log("Performing request for the url: " + url);
		}
		HTTPRequest request = new HTTPRequest(new Uri(url), isPost ? HTTPMethods.Post : HTTPMethods.Get, OnRequestFinished);
		if (isPost) {
			string postParameters = parametersObject.ToString();
			if (debug) {
				Debug.Log("Parsed parameters for request are: " + postParameters);
			}
			request.RawData = Encoding.UTF8.GetBytes(postParameters);
			request.AddHeader("Content-Type", "application/json");
		}
		request.Send();
	}
	
	protected void OnRequestFinished(HTTPRequest request, HTTPResponse response)
	{
		JSONNode parsedResponse;
		if (response != null) {
			parsedResponse = JSON.Parse(response.DataAsText);
			if (debug) {
				Debug.Log(response.Cookies);
				Debug.Log(response.DataAsText);
			}
		} else {
			parsedResponse = new JSONClass();
			parsedResponse["error"] = "Had an empty response.";
			if (debug) {
				Debug.LogError(parsedResponse["error"]);
			}
		}
		
		_callback(parsedResponse);
	}
}