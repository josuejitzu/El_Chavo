using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System;
using EasyButtons;
using TMPro;
using System.Runtime.InteropServices;
using Lando;
using MiFare;
using MiFare.Classic;
using MiFare.PcSc;

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


/// <summary>
/// MAIN
/// </summary>
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
    [Space(10)]
    [Header("UI")]
    public GameObject panelSacoa;
    public TMP_InputField usuario_text;
    public TMP_InputField pass_text;
    public TMP_InputField computerID_text;
    public TMP_InputField urlLogeo_text;

    public TMP_InputField cardID_text;

    public TMP_Text token_text;

    public TMP_InputField puntos_text;
    public TMP_InputField urlTicket_text;

    [Space(10)]
    [Header("UI Tickets")]
    public GameObject panelTickets;
    [Tooltip("El valor de puntos x 1 ticket ej: 50pts = 1ticket")]
    public int valorTicket = 50;
    public TMP_Text tickets_jugador_text;
    public TMP_InputField tarjeta_id_input;


    [Header("LectorRFID")]
    public TMP_Text tarjeta_id_text;
    public int tarjeta_id;
    Cardreader _cardReader;

    [Header("LectorRFID_MiFare")]
    private MiFare.Devices.SmartCardReader _lector;
    private MiFareCard _tarjeta;

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
            EventDispatcher.TotalScore += this.EventDispatcher_TotalScore;


        }
        else if (_tickets != null)
        {
           
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        panelSacoa.SetActive(false);
        panelTickets.SetActive(false);
        //  jsonStr = "user:" + user + "," + "pass:" + pass + "," + "computerId:" + computerID;
        // print(jsonStr);
        //  LoggeoSacoa();
        EventDispatcher.TotalScore += this.EventDispatcher_TotalScore;
        LoggeoSacoa();

    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.LeftControl))
        {
           if( Input.GetKeyDown(KeyCode.C))
            {

                panelSacoa.SetActive(!panelSacoa.activeInHierarchy);
                panelTickets.SetActive(!panelTickets.activeInHierarchy);
            }
        }
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
            token_text.text = "Token: " + usuario_logeado.body.token;
          //  CardBalance_aJson();
        }
        else if(request.responseCode == 401)
        {
            //Esto es para cuando se vence el TOKEN hay que pedir otro
            Debug.Log("Error: " + request.responseCode);
            Debug.Log("El Token vencio o no esta autorizado...Solicitando nuevo Token...");
            token_text.text =  "El Token vencio o no esta autorizado...Solicitando nuevo Token...";
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
    /// Es llamado por Un EVENT en EventDispatcher.cs
    /// </summary>
    /// <param name="obj"></param>
    private void EventDispatcher_TotalScore(int puntosJugador)
    {
        if(puntosJugador < 49)
        {
            print("El jugador no gano ningun ticket...");
            return;
        }

        int _tickets = Mathf.FloorToInt( puntosJugador / valorTicket);
        tickets_Partida = _tickets;
        //TODO: Aparecer panel para el controlador mostrando 
        panelTickets.SetActive(true);
        //-EL panel debe mostrar los ticket a acumular
        tickets_jugador_text.text = tickets_Partida.ToString();
        //Activar el lector de Tarjetas para que comience a esperar la tarjeta
        ActivarLectorTarjeta();
        //Letrero que informe que debe pasar la tarjeta por el lector
        panelTickets.SetActive(true);

        //AgregarPuntos_aJson();

    }

   

    /// <summary>
    /// Cambia el valor de los puntos y de la URL si el operador lo cambio en la consola
    /// </summary>
    public void CambiarPuntos()
    {
        if(puntos_text.text != null || puntos_text.text != "")
        {
            valorTicket = int.Parse(puntos_text.text);

        }
        if(urlTicket_text.text != null || urlTicket_text.text != "")
        {
            url_ticketsAgregar = urlTicket_text.text;
        }
    }

    /// <summary>
    /// Llamado cuando se agrego manualmente por el operador la tarjeta desde la consola
    /// </summary>
    public void IngresarTarjeta()
    {
        if(tarjeta_id_input.text != null || tarjeta_id_input.text != "")
        {
            tarjeta_id_text.text = "Tarjeta #"  +  tarjeta_id_input.text;
            cardNumber = int.Parse(tarjeta_id_text.text);
        }
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
        //TODO:  ticketAmount  = ticketsPartida
        tarjetaAgregar.ticketAmount = tickets_Partida; // tickets_Partida
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




    #region LectorTarjetas



    [Button]
    private void ActivarLectorTarjeta()
    {
        ///LANDO version
        //this._cardReader = new Lando.Cardreader();

        //if (_cardReader != null)
        //    print("Se creo el Objeto _cardReader");

        //this._cardReader.CardConnected += CardReader_CardConnected;

        //this._cardReader.StartWatch();

        ///version MiFare
        GetDevices();


    }

    private void CardReader_CardConnected(object sender, CardreaderEventArgs e)
    {
        try
        {

            string tempId = e.Card.Id.Replace("-", string.Empty);
            long decValue = Convert.ToInt64(tempId, 16);
            print("IdDEC" + decValue);
            //Convertimos decValue a String
            string deValue_string = decValue.ToString();
            //Nos quedamos con los 6 primeros numeros
            deValue_string = deValue_string.Remove(6);
            //Se lo asignamos a un int32
            int decValue_int = int.Parse(deValue_string);

            print("idHEX:" + tempId + " idDEC:" + decValue + " idDEC_6: " + decValue_int);


            tarjeta_id_text.text = decValue.ToString();
            cardNumber = decValue_int;
            //  tarjeta_id = int.Parse(decValue.ToString());

        }
        catch (Exception error)
        {
            print(error);
        }

        this._cardReader.StopWatch();
        this._cardReader.Dispose();
        this._cardReader.CardConnected -= CardReader_CardConnected;
    }


    /// <summary>
    /// Enumerates NFC reader and registers event handlers for card added/removed
    /// </summary>
    /// <returns>None</returns>
    async private void GetDevices()
    {
        try
        {
            //_lector = await CardReader.FindAsync();
            MiFare.Devices.SmartCardReader smartCardReader = await CardReader.FindAsync();
            _lector = smartCardReader;

            if (_lector == null)
            {
                print("No Readers Found");
                return;
            }

            //_lector.TarjetaAgregada += TarjetaAgregada;
            //_lector.CardRemoved += CardRemoved;


            // this._lector.CardAdded += TarjetaAgregada;
            _lector.CardAdded += _lector_CardAdded;

            //_lector.StartWatch();
            //_lector.

        }
        catch (Exception e)
        {
            print("Exception: " + e.Message);
        }
    }
    private async void _lector_CardAdded(object sender, MiFare.Devices.CardAddedEventArgs args)
    {
        try
        {
            print("Leyendo tarjeta");
            await HandleCard(args);
        }
        catch (Exception e)
        {
            print("TarjetaAgregada Exception: " + e.Message);
        }

        // throw new NotImplementedException();
    }
    /// <summary>
    /// Sample code to hande a couple of different cards based on the identification process
    /// </summary>
    /// <returns>None</returns>
    private async Task HandleCard(MiFare.Devices.CardAddedEventArgs args)
    {
        try
        {
            _tarjeta?.Dispose();
            _tarjeta = args.SmartCard.CreateMiFareCard();



            var cardIdentification = await _tarjeta.GetCardInfo();


            print("Connected to card\r\nPC/SC device class: " + cardIdentification.PcscDeviceClass.ToString() + "\r\nCard name: " + cardIdentification.PcscCardName.ToString());

            if (cardIdentification.PcscDeviceClass == MiFare.PcSc.DeviceClass.StorageClass
                 && (cardIdentification.PcscCardName == CardName.MifareStandard1K || cardIdentification.PcscCardName == CardName.MifareStandard4K))
            {
                // Handle MIFARE Standard/Classic
                print("MIFARE Standard/Classic card detected");


                var uid = await _tarjeta.GetUid();
                print("UID:  " + BitConverter.ToString(uid));




                // 16 sectors, print out each one
                for (var sector = 0; sector < 2; sector++)
                {
                    try
                    {
                        var data = await _tarjeta.GetData(sector, 0, 48);

                        string hexString = "";
                        for (int i = 0; i < data.Length; i++)
                        {
                            hexString += data[i].ToString("X2");
                        }

                        print(string.Format("Sector '{0}':{1}", sector, hexString));


                        
                        if (sector == 1)
                        {
                            //Sutraemos el valor Hexadecimal que buscamos, es la pos5 pero no nos lo da asi, 
                            //asi que lo buscamo por posicion
                            string hexTarjeta = hexString.Substring(32, 14);
                            print("Sacoa Hex: " + hexTarjeta);
                            //despues convertimos hexTarjeta a un valor ascii en string
                            string ascii = ConvertHex(hexTarjeta.ToString());
                            //le quitamos el ultimo numero pues deben ser 6 digitos y nos da 7
                            string tarjeta_num = ascii.Remove(6);
                            print("Sacoa Tarjeta:" + tarjeta_num);
                            //TODO: asignar tarjeta_num a un int, puede ser directamente a cardNumber
                            tarjeta_id_text.SetText("Tarjeta #"+tarjeta_num);
                            cardNumber = int.Parse(tarjeta_num);
                            
                            //TODO: agregar una condicion donde si tarjeta_num es correcto se automatice enviar la info a la API
                            ag
                        }

                    }
                    catch (Exception)
                    {
                        print("Failed to load sector: " + sector);
                    }
                }



            }
        }
        catch (Exception e)
        {
            print("HandleCard Exception: " + e.Message);
        }
    }

    /// <summary>
    /// Convertidor de string hexadecimal a ASCII
    /// </summary>
    /// <param name="hexString"></param>
    /// <returns></returns>
    public static string ConvertHex(string hexString)
    {
        try
        {
            string ascii = string.Empty;

            for (int i = 0; i < hexString.Length; i += 2)
            {
                string hs = string.Empty;

                hs = hexString.Substring(i, 2);
                ulong decval = Convert.ToUInt64(hs, 16);
                long deccc = Convert.ToInt64(hs, 16);
                char character = Convert.ToChar(deccc);
                ascii += character;

            }

            return ascii;
        }
        catch (Exception ex) { Console.WriteLine(ex.Message); }

        return string.Empty;
    }



    #endregion



    public void IniciarSesion_Boton()
    {
        if (usuario_text.text != null || usuario_text.text != "")
        {
            user = usuario_text.text;
        }
        if (pass_text.text != null || pass_text.text != "")
        {
            pass = pass_text.text;
        }
        if (computerID_text.text != null || computerID_text.text != "")
        {
            
            computerID =  int.Parse(computerID_text.text);
        }
        if(urlLogeo_text.text != null || urlLogeo_text.text != "")
        {
            url_login = urlLogeo_text.text;
        }

        LoggeoSacoa();
    }


}


/*
https:\\200.80.220.110:33001
user: jitzu
pass: @@jitzu    
*/
