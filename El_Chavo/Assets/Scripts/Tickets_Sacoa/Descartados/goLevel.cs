using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class goLevel : MonoBehaviour
{
    public string url_login;
    //With the @ before the string, we can split a long string in many lines without getting errors
    private string json = @"{
		'hello':'world', 
		'foo':'bar', 
		'count':25
	}";
    private string ultJson = @"{'user':'jitzu','pass':'@@jitzu','computerId':0001}";

    private void Start()
    {
        doPost();
        POST();
    }

    void doPost()
    {
        string URL = "//200.80.220.110:33001/login"; //"http://example.org/postData";
        string myAccessKey = "myAccessKey";
        string mySecretKey = "mySecretKey";

        //Auth token for http request
        string accessToken;
        //Our custom Headers
        Dictionary<string, string> headers = new Dictionary<string, string>();
        //Encode the access and secret keys
        accessToken = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(myAccessKey + ":" + mySecretKey));
        //Add the custom headers
        //headers.Add("Authorization", "Basic ");
        headers.Add("Content-Type", "application/json");
        headers.Add("AnotherHeader", "AnotherData");
        headers.Add("Content-Length", json.Length.ToString());
        //Replace single ' for double " 
        //This is usefull if we have a big json object, is more easy to replace in another editor the double quote by singles one
        json = ultJson.Replace("'", "\"");
        //Encode the JSON string into a bytes
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(ultJson);
        //Now we call a new WWW request
        WWW www = new WWW(URL, postData);
        //And we start a new co routine in Unity and wait for the response.
        StartCoroutine(WaitForRequest(www));
    }

    public WWW POST()
    {
        WWW www;
        Hashtable postHeader = new Hashtable();
        postHeader.Add("Content-Type", "application/json");
       // WWWForm form = new WWWForm();
        var form = System.Text.Encoding.UTF8.GetBytes(ultJson);
        //form.AddField("data", ultJson);
        www = new WWW(url_login, form, postHeader);
        StartCoroutine(WaitForRequest(www));
        return www;
    }
    //Wait for the www Request
    IEnumerator WaitForRequest(WWW www)
    {
        yield return www;
        if (www.error == null)
        {
            //Print server response
            Debug.Log(www.text);
        }
        else
        {
            //Something goes wrong, print the error response
            Debug.Log(www.error);
        }
    }
}