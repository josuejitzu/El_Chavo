using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.IO;
using System;

[System.Serializable]
public class UsuarioLogin
{
    public string user;
    public string pass;
    public string computerId;
   
}
public class CardBalanceRequest  //http://www.devindia.biz/how-to-post-and-get-json-data-to-a-service-in-unity-2/
{
    public int posId;
    public int empId;
    public int cardNumber;
    public string token;
    public int extEmpId;
}
public class CardBalanceResponse
{

}
public class TicketsAddRequest
{
    public int posId;
    public int empId;
    public int cardNumber;
    public string token;
    public int ticketAmount;
    public int extEmpId;
}

public class Tickets_Control : MonoBehaviour
{
    public UsuarioLogin usuario;
    [Space(10)]
    public string url_login;
    public string url_tickets;

    public string user;
    public string pass;
    public string computerID;
    public string posID;
    public string empID;
    public int cardNumber;
    public string token;//pedir cada 8 horas
    public string extEmpID;
    //tipo de ticket
    public string jsonStr;
    // Start is called before the first frame update
    

    string recentData = "";
    void Start()
    {
        jsonStr = "user:" + user + "," + "pass:" + pass + "," + "computerId:" + computerID;
        print(jsonStr);
        ConvertirJson();
    }

    
    //public UnityWebRequest POST()
    //{
    //    UnityWebRequest www = UnityWebRequest.Put(url_login, jsonStr);
    //    www.SetRequestHeader("Content-Type", "application/json");


    //    WWW web;
    //    Hashtable postHeader = new Hashtable();
    //    postHeader.Add("Content-Type", "application/json");

    //    convert json string to byte
    //   var formData = System.Text.Encoding.UTF8.GetBytes(jsonStr);

    //    web = new WWW(url_login, formData, postHeader);
    //    StartCoroutine(WaitForRequest(www));
    //    return www;
    //}

    //IEnumerator Login(string url, string bodyJson)
    //{
    //    jsonStr = "{ user:admin, pass:admin, computerId:11 }";
    //    UnityWebRequest request = new UnityWebRequest(url, "POST");

    //}

    public void ConvertirJson()
    {
        usuario.user = user;
        usuario.pass = pass;
        usuario.computerId = computerID;

        string jsonString = JsonUtility.ToJson(usuario);
        //JsonUtility.FromJson
        print(jsonString);
        File.WriteAllText("D:\\Proyectos\\El_Chavo\\Unity\\datosenviados.txt", jsonString); //https://www.youtube.com/watch?v=ngX7-6jKIr8
            
        StartCoroutine(Post(url_login, jsonString));
      
        //StartCoroutine(PostRequestCoroutine(url_login, jsonString));
        //StartCoroutine(RequestRoutine(url_login, jsonString, ResponseCallback));
    }


    //IEnumerator Postear()
    //{
    //    WWWForm from = new WWWForm();
    //}


    //https://forum.unity.com/threads/unitywebrequest-post-url-jsondata-sending-broken-json.414708/
    IEnumerator Post(string url, string bodyJsonString)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        print(bodyRaw);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        File.WriteAllText("D:\\Proyectos\\El_Chavo\\Unity\\datosRecibidos.txt", request.responseCode.ToString());
        // yield return request.Send();
        // string deJson = JsonUtility.FromJson(request.downloadHandler);
        Debug.Log("Status Code: " + request.responseCode);
        //string d = JsonUtility.FromJson(request.ToString(),);
 
        Debug.Log("Status text: " + request.GetResponseHeaders());
        Debug.Log("Status text: " + request.downloadHandler.text);

       // string deJason = File.ReadAllText(request.url);
        //print(deJason);
        Debug.Log("Status text: " + request.downloadHandler.text.ToString());
        Debug.Log(string.Format("Respuesta: ", request.downloadHandler.text));
  
        
    }


    private IEnumerator RequestRoutine(string url,string bodyJsonString, Action<string> callback = null)
    {
        var request = UnityWebRequest.Post(url,bodyJsonString);

        yield return request.SendWebRequest();
        var data = request.downloadHandler.text;

        if (callback != null)
            callback(data);


    }
    private void ResponseCallback(string data)
    {
        Debug.Log(data);
        File.WriteAllText("D:\\Proyectos\\El_Chavo\\Unity\\datosRecibidos.txt", data);
        recentData = data;
    }

    private IEnumerator PostRequestCoroutine(string url, string json)
    {
        var jsonBinary = System.Text.Encoding.UTF8.GetBytes(json);

        DownloadHandlerBuffer downloadHandlerBuffer = new DownloadHandlerBuffer();

        UploadHandlerRaw uploadHandlerRaw = new UploadHandlerRaw(jsonBinary);
        uploadHandlerRaw.contentType = "application/json";
        
        UnityWebRequest www =
            new UnityWebRequest(url, "POST", downloadHandlerBuffer, uploadHandlerRaw);

        yield return www.SendWebRequest();

        if (www.isNetworkError)
            Debug.LogError(string.Format("{0}: {1}", www.url, www.error));
        else
            Debug.Log(string.Format("Response: {0}", www.downloadHandler.text));
    }
}


/*
 https:\\200.80.220.110:33001

user: jitzu

pass: @@jitzu
    
 */
