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
    public int computerId;

}
[Serializable]
public class UsuarioLoggeado//Esto se llenara para cuando recibamos ok
{
    //{"statusCode":200,"body":{"authorized":true,"errorCode":0,"empId":1017,"token":"c22bfa45e383cf3c536d1dc4edd775f0c737371ecd9488a0f3e14322a8ececc34b10de370e898cc06715d862bc7a94c6"}}
    public int statusCode;    public Body body;
}

[Serializable]
public class Body
{
    public bool authorized;
    public int errorCode;
    public int empId;
    public string token;
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

[System.Serializable]
public class ForceAcceptAll : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true;
    }
}
public class Tickets_Control : MonoBehaviour
{
    public UsuarioLogin usuario;
    public UsuarioLoggeado usuario_logeado;
    [Space(10)]
    public string url_login;
    public string url_tickets;

    public string user;
    public string pass;
    public int computerID;
    public string posID;
    public string empID;
    public int cardNumber;
    public string token;//pedir cada 8 horas
    public string extEmpID;
    //tipo de ticket
    public string jsonStr;
    // Start is called before the first frame update
    

    string recentData = "";

    private string json = @"{
		'hello':'world', 
		'foo':'bar', 
		'count':25
	}";

    private string ultJson = "{'user':'jitzu','pass':'@@jitzu','computerId': 0001 }";



    void Start()
    {
        jsonStr = "user:" + user + "," + "pass:" + pass + "," + "computerId:" + computerID;
        print(jsonStr);
        ConvertirJson();
    }

 

    public void ConvertirJson()
    {
        usuario.user = user;
        usuario.pass = pass;
        usuario.computerId = computerID;

        string jsonString = JsonUtility.ToJson(usuario) ?? "";
        print(jsonString);
        File.WriteAllText("D:\\Proyectos\\El_Chavo\\Unity\\datosenviados.txt", jsonString); //https://www.youtube.com/watch?v=ngX7-6jKIr8

        StartCoroutine(Post(url_login, jsonString));
       

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
        var cert = new ForceAcceptAll();////SUPER IMPORTANTE para los certificados HTTPS
        request.certificateHandler = cert;

        yield return request.SendWebRequest();

        if (request.responseCode == 200)
        {

            Debug.Log("Status Code: " + request.responseCode);
            Debug.Log("Errorc Code: " + request.error);
            cert?.Dispose();
            string m = System.Text.Encoding.UTF8.GetString(request.downloadHandler.data);
            Debug.Log("Mensaje Reicibido: " + m);
            File.WriteAllText("D:\\Proyectos\\El_Chavo\\Unity\\datosRecibidos.txt", m);
            usuario_logeado = JsonUtility.FromJson<UsuarioLoggeado>(m);
        }else
        {
            Debug.Log("Error...algo salio mal en la conexion");
            Debug.Log("Status Code: " + request.responseCode);
            Debug.Log("Errorc Code: " + request.error);
            cert?.Dispose();
        }
       

    }
    
}


/*
https:\\200.80.220.110:33001
user: jitzu
pass: @@jitzu    
*/
