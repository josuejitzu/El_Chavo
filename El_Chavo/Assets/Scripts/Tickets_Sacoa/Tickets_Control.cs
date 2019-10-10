using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.IO;
using System;
using EasyButtons;

#region Area de Clases para Logeo
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
    public int statusCode;
    public Body body;

}
[Serializable]
public class Body
{
    public bool authorized;
    public int errorCode;
    public int empId;
    public string token;
}
#endregion

#region Area Balance de Tarjeta
[Serializable]
public class CardBalanceRequest  //http://www.devindia.biz/how-to-post-and-get-json-data-to-a-service-in-unity-2/
{
    public int posId;
    public int empId;
    public int cardNumber;
    public string token;
    public int extEmpId;
}
[Serializable]
public class CardBalanceResponse
{
    
    public int statusCode;
    public CardBalanceBody body;///importante no se pasan los datos si la variable no se llama body
}
[Serializable]
public class CardBalanceBody
{
    public string card;
    public string credits;
    public string bonus;
    public string courtesy;
    public string status;
    public int minutes;
    public string tickets;
    public string oldPassports;
    public string ticketType;
    //public List<HpList> hpList;
}
[Serializable]
public class HpList
{
    public string id;
    public string name;
    public string qty;
}

#endregion

#region Area Agregar Tickets Tarjeta
[Serializable]
public class TicketsAddRequest
{
    public int posId;
    public int empId;
    public int cardNumber;
    public string token;
    public int ticketAmount;
    public int extEmpId;
}
[Serializable]
public class TicketsAddResponse
{
    public int statusCode;
    public TicketsResponseBody body;
}
[Serializable]
public class TicketsResponseBody
{
    //    este body es temporal PREGUNTAR CUAL ES LA RESPONSE CORRECTA
    public string tickets;
    public string cardNumber;
    public string status;

    //public int card;
    //public int credits;
    //public int bonus;
    //public int courtesy;
    //public int status;
    //public int tickets;
    //public int oldPassports;
    //public string ticketType;
}
#endregion

/////IMPORTANTISIMO POR EL PROTOCOLO HTTPS
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
    public static Tickets_Control _tickets;


    public UsuarioLogin usuario;
    public UsuarioLoggeado usuario_logeado;

    public CardBalanceRequest tarjeta_balance;
    public CardBalanceResponse tarjeta_datos;

    public TicketsAddRequest tarjetaAgregar;
    public TicketsAddResponse tarjetaVerificacion;//recibe los datos para saber si se agregaron correctamente los puntos


    [Space(10)]
    [Header("URLS")]
    public string url_login;
    public string url_ticketsAgregar;
    public string url_cardBalance;
    [Space(10)]
    [Header("URLS")]
    public int tickets_Partida;
    [Space(10)]
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

    #region Ejemplo de hardcoding la liga de JSON
    //private string json = @"{
    //	'hello':'world', 
    //	'foo':'bar', 
    //	'count':25
    //}";

    //private string ultJson = "{'user':'jitzu','pass':'@@jitzu','computerId': 0001 }";
    #endregion

    private void Awake()
    {

        if (_tickets == null)
        {
            _tickets = this;
        }
        else if (_tickets != null)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
       //  jsonStr = "user:" + user + "," + "pass:" + pass + "," + "computerId:" + computerID;
       // print(jsonStr);
      //  LoggeoSacoa();
        EventDispatcher.TotalScore += EventDispatcher_TotalScore;
    }

    /*
     * TODO:
     * Crear funcion que lea, reciba y asigne a la variable el numero de tarjeta RFID
     * Una vez ejecutada esos 3 puntos que la funcion llame a AgregarPUntos_aJson
     * Verificar que haya tickets que agregar para no causar un null
     * 
     */
  


    #region Loggeo a Serivdor de Sacoa (Token)
    [Button("Loggear a Sacoa")]
    public void LoggeoSacoa()
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
        var request = new UnityWebRequest(url, "POST");//url a enviar y metodo en este caso solo dejan POST
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
      //  print(bodyRaw.ToString());//Esto solo se establece
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        var cert = new ForceAcceptAll();////SUPER IMPORTANTE para los certificados HTTPS
        request.certificateHandler = cert;

        yield return request.SendWebRequest();

        if (request.responseCode == 200)
        {

            Debug.Log("Status Code: " + request.responseCode);
          //  Debug.Log("Errorc Code: " + request.error);
            cert?.Dispose();
            string m = System.Text.Encoding.UTF8.GetString(request.downloadHandler.data);
            Debug.Log("Mensaje Reicibido: " + m);
            File.WriteAllText("D:\\Proyectos\\El_Chavo\\Unity\\datosRecibidos.txt", m);
            usuario_logeado = JsonUtility.FromJson<UsuarioLoggeado>(m);
            print("Exito!...Inicio de sesion aceptado con token:" + usuario_logeado.body.token);

            CardBalance_aJson();
        }
        else if(request.responseCode == 401)
        {
            //Esto es para cuando se vence el TOKEN hay que pedir otro
            Debug.Log("Error: " + request.responseCode);
            Debug.Log("El Token vencio o no esta autorizado...Solicitando nuevo Token...");
            yield return new WaitForSeconds(2.0f);//simulamos una espera
            LoggeoSacoa();

        }
        else
        {
            Debug.Log("Error...algo salio mal en la conexion");
            Debug.Log("Status Code: " + request.responseCode);
            Debug.Log("Errorc Code: " + request.error);
            cert?.Dispose();
        }


       

    }
    #endregion

    #region Solicitar Saldo de Usuario(Actualmente no veo la necesidad de saber los tickets)
    //Funciona pero no esta guardando los datos donde corresponde
    /// <summary>
    /// Llamada cuando el Loggeo fue un exito
    /// Pero no veo la necesidad de llamarla, el usuario tendria que pasar su tarjeta para activarla
    /// </summary>
    /// 
    [Button("Obtener Saldo")]
    public void CardBalance_aJson()
    {
        print("Solicitando Balance de Tarjeta...");
        tarjeta_balance.posId = int.Parse(posID);
        tarjeta_balance.empId = usuario_logeado.body.empId;
        tarjeta_balance.cardNumber = cardNumber;
        tarjeta_balance.token = usuario_logeado.body.token;
        tarjeta_balance.extEmpId = int.Parse(extEmpID);

        string aJson = JsonUtility.ToJson(tarjeta_balance) ?? "";
        print("Tarjeta convertida a: " + aJson);

        StartCoroutine(PostCardBalance(url_cardBalance, aJson));

    }
    IEnumerator PostCardBalance(string url,string bodyJsonString)
    {
        var request = new UnityWebRequest(url, "POST");//url a enviar y metodo en este caso solo dejan POST
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
      //  print(bodyRaw.ToString());//Esto solo se establece
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        var cert = new ForceAcceptAll();////SUPER IMPORTANTE para los certificados HTTPS
        request.certificateHandler = cert;

        yield return request.SendWebRequest();

        if (request.responseCode == 200)
        {

            Debug.Log("Status Code: " + request.responseCode);
          //  Debug.Log("Errorc Code: " + request.error);
            cert?.Dispose();
            string m = System.Text.Encoding.UTF8.GetString(request.downloadHandler.data);
            Debug.Log("Mensaje Reicibido: " + m);
          //  File.WriteAllText("D:\\Proyectos\\El_Chavo\\Unity\\datosRecibidos.txt", m);
            tarjeta_datos = JsonUtility.FromJson<CardBalanceResponse>(m);
            print("Exito!... Balance de Tarjeta:" + tarjeta_datos.body.card+"| Credito: "+tarjeta_datos.body.credits+"|Tickets: "+tarjeta_datos.body.tickets);

            AgregarPuntos_aJson();
        }
        else if (request.responseCode == 401)
        {
            //Esto es para cuando se vence el TOKEN hay que pedir otro
            Debug.Log("Error: " + request.responseCode);
            Debug.Log("El Token vencio o no esta autorizado...Solicitando nuevo Token...");
            yield return new WaitForSeconds(2.0f);//simulamos una espera
            LoggeoSacoa();

        }
        else
        {
            Debug.Log("Error...algo salio mal en la conexion");
            Debug.Log("Status Code: " + request.responseCode);
            Debug.Log("Errorc Code: " + request.error);
            cert?.Dispose();
        }

    }
    #endregion

    #region Dar Tickets


    /// <summary>
    /// Convierte el Score del Jugador en los tickets 
    /// que se le asignaran PERO NO DEBE AGREGARLOS A LA TARJETA, eso es otra funcion
    /// </summary>
    /// <param name="obj"></param>
    private void EventDispatcher_TotalScore(int puntosJugador)
    {
        if(puntosJugador < 49)
        {
            print("El jugador no gano ningun ticket...");
            return;
        }

        int _tickets = Mathf.FloorToInt( puntosJugador / 50);
        tickets_Partida = _tickets;
        //AgregarPuntos_aJson();

    }

    /// <summary>
    /// Crea el formato para agregar puntos al Jugador
    /// Se tiene que pasar la tarjeta
    /// Actualmente llamada por PostCardBalance()
    /// </summary>
    [Button("Agregar Puntos")]
    /// 
    public void AgregarPuntos_aJson()
    {
        print("Agregando puntos a Tarjeta...");
        tarjetaAgregar.posId = 1;
        tarjetaAgregar.empId = usuario_logeado.body.empId;
        tarjetaAgregar.cardNumber = cardNumber;
        tarjetaAgregar.token = usuario_logeado.body.token;
        tarjetaAgregar.ticketAmount = 10; // tickets_Partida
        tarjetaAgregar.extEmpId = 1;

        string aJson = JsonUtility.ToJson(tarjetaAgregar);
        print("Enviando puntos a: " + aJson);
        StartCoroutine(PostTickets(url_ticketsAgregar, aJson));
    }
    IEnumerator PostTickets(string url,string bodyJsonString)
    {
        var request = new UnityWebRequest(url, "POST");//url a enviar y metodo en este caso solo dejan POST
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
       // print(bodyRaw.ToString());//Esto solo se establece
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        var cert = new ForceAcceptAll();////SUPER IMPORTANTE para los certificados HTTPS
        request.certificateHandler = cert;

        yield return request.SendWebRequest();

        if (request.responseCode == 200)
        {

            Debug.Log("Status Code: " + request.responseCode);
          //  Debug.Log("Errorc Code: " + request.error);
            cert?.Dispose();
            string m = System.Text.Encoding.UTF8.GetString(request.downloadHandler.data);
            Debug.Log("Mensaje Reicibido: " + m);
            //  File.WriteAllText("D:\\Proyectos\\El_Chavo\\Unity\\datosRecibidos.txt", m);
            tarjetaVerificacion = JsonUtility.FromJson<TicketsAddResponse>(m);
            print("Exito!...Tickets en Tarjeta:" + tarjetaVerificacion.body.tickets);

          //  AgregarPuntos_aJson();
        }
        else if (request.responseCode == 401)
        {
            //Esto es para cuando se vence el TOKEN hay que pedir otro
            Debug.Log("Error: " + request.responseCode);
            Debug.Log("El Token vencio o no esta autorizado...Solicitando nuevo Token...");
            yield return new WaitForSeconds(2.0f);//simulamos una espera
            LoggeoSacoa();

        }
        else
        {
            Debug.Log("Error...algo salio mal en la conexion");
            Debug.Log("Status Code: " + request.responseCode);
            Debug.Log("Errorc Code: " + request.error);
            cert?.Dispose();
        }
    }

    #endregion


}


/*
https:\\200.80.220.110:33001
user: jitzu
pass: @@jitzu    
*/
