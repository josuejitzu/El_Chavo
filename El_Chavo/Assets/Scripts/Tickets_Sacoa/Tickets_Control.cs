using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.IO;

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
    
    void Start()
    {
        jsonStr = "user:" + user + "," + "pass:" + pass + "," + "computerId:" + computerID;
        print(jsonStr);
        ConvertirJson();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //public WWW POST()
    //{
    //    UnityWebRequest www = UnityWebRequest.Put("http://localhost:3000/api/rawCoords", jsonStr);
    //    www.SetRequestHeader("Content-Type", "application/json");


    //    WWW web;
    //    Hashtable postHeader = new Hashtable();
    //    postHeader.Add("Content-Type", "application/json");

    //    // convert json string to byte
    //    var formData = System.Text.Encoding.UTF8.GetBytes(jsonStr);

    //    web = new WWW(POSTAddUserURL, formData, postHeader);
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
        print(jsonString);
        File.WriteAllText("D:\\Proyectos\\El_Chavo\\Unity\\datosenviados.txt", jsonString); //https://www.youtube.com/watch?v=ngX7-6jKIr8
    }

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
        Debug.Log("Status Code: " + request.responseCode);
  
        
    }
}
