using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

public class TestQuery : MonoBehaviour 
{
	const string cliendID = "1062117613756-9spt3go2ssafkvgbahjk3pit4qtqlp70.apps.googleusercontent.com";
	const string clientSecret = "DaR-tWiCTLZFHcy7uu3jRtHs";
	const string apiKey = "AIzaSyD0MIvlWgPs-K_IWnfiPrSU4OggMTg-ZvA";
	string deviceID = "";
	string accessToken = "";

	// TODO: Add refresh token usage
	const string userCodeURL = "https://accounts.google.com/o/oauth2/device/code";
	const string accessTokenURL = "https://accounts.google.com/o/oauth2/token";

	const string userInfoURL = "https://www.googleapis.com/oauth2/v1/userinfo";
	const string calendarURL = "https://www.googleapis.com/auth/calendar.readonly";
	const string calendarListURL = "https://www.googleapis.com/calendar/v3/users/me/calendarList";

	// Use this for initialization
	void Start () {
		// Calendar "PixelPunch": imbg8arupp7u9lf8taiosshau8@group.calendar.google.com
		StartCoroutine(InitApplication());

	}

	void OnGUI()
	{
		if (GUI.Button(new Rect(5, 5, 100, 40), "User Info"))
			StartCoroutine(GetUserInfo());

		if (GUI.Button(new Rect(5, 50, 100, 40), "Calendar Info"))
			StartCoroutine(GetCalendarInfo());
	}

	void OnApplicationPause(bool isPaused)
	{
		if (!isPaused && deviceID.Length > 0 && accessToken.Length == 0)
			StartCoroutine(GetAccessToken());
		//Debug.Log("OnApplicationPause: " + isPaused + ", " + Time.time);
	}

	IEnumerator InitApplication()
	{
		//string url = "https://accounts.google.com/o/oauth2/device/code";

		string txt = "client_id=" + cliendID +
			"&scope=https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fuserinfo.email%20https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fuserinfo.profile%20https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fcalendar.readonly%20https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fcalendar";
		byte[] opts = System.Text.Encoding.UTF8.GetBytes(txt);

		Hashtable headers = new Hashtable();
		headers.Add("Content-Type", "application/x-www-form-urlencoded");
		headers.Add("Host", "accounts.google.com");

		WWW www = new WWW(userCodeURL, opts, headers);
		yield return www;

		if (www.error != null)
		{
			Debug.Log("Error text: " + www.error);
			yield break;
		}
		
		Debug.Log("Got text: " + www.text);

		Dictionary<string,object> dict = Json.Deserialize(www.text) as Dictionary<string,object>;
		string accessURL = (string) dict["verification_url"];
		EditorGUIUtility.systemCopyBuffer = (string) dict["user_code"];
		deviceID = (string) dict["device_code"];
		//Debug.Log("Found string: " + accessURL);
		Application.OpenURL(accessURL);
	}

	IEnumerator GetAccessToken()
	{
		string txt = "client_id=" + cliendID +
			"&client_secret=" + clientSecret +
			"&code=" + deviceID +
			"&grant_type=http://oauth.net/grant_type/device/1.0";
		byte[] opts = System.Text.Encoding.UTF8.GetBytes(txt);

		Hashtable headers = new Hashtable();
		headers.Add("Content-Type", "application/x-www-form-urlencoded");
		headers.Add("Host", "accounts.google.com");

		WWW www = new WWW(accessTokenURL, opts, headers);
		yield return www;

		if (www.error != null)
		{
			Debug.Log("Error text: " + www.error);
			yield break;
		}
		
		Debug.Log("Got text: " + www.text);
		Dictionary<string,object> dict = Json.Deserialize(www.text) as Dictionary<string,object>;
		accessToken = (string) dict["access_token"];

	}

	IEnumerator GetCalendarInfo()
	{
		WWW www = new WWW(calendarListURL + "?key=" +apiKey + "&access_token=" + accessToken);

		//Hashtable headers = new Hashtable();
		//headers.Add()

		yield return www;

		if (www.error != null)
		{
			Debug.Log("Error text: " + www.error);
			yield break;
		}
		
		Debug.Log("Got text: " + www.text);
	}

	IEnumerator GetUserInfo()
	{
		string txt = "?access_token=" + accessToken;

		WWW www = new WWW(userInfoURL + txt);//, null, headers);
		yield return www;

		if (www.error != null)
		{
			Debug.Log("Error text: " + www.error);
			yield break;
		}
		
		Debug.Log("Got text: " + www.text);
	}
}
