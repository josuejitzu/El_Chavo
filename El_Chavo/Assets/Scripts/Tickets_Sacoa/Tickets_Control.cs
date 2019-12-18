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
    //public string tickets;
    //public string cardNumber;
    //public string status;

    public int card;
    public int credits;
    public int bonus;
    public int courtesy;
    public int status;
    public int tickets;
    public int oldPassports;
    public string ticketType;
}
#endregion


[Serializable]
public class TarjetaDecode
{
    public int posId;
    public int empId;
    public string cardNumber;
    public string token;
    public int extEmpId;
}
[Serializable]
public class TarjetaDecodResponse
{
    public int statusCode;
    public CardNumberBody body;
}

[Serializable]
public class CardNumberBody
{
    public string cardNumber;
}


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

    public TarjetaDecode tarjetaDecode;
    public TarjetaDecodResponse tarjetaDecode_response;

    [Space(10)]
    [Header("URLS")]
    public string url_login;
    public string url_ticketsAgregar;
    public string url_cardBalance;
    public string url_tarjetaDecode;

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
    public string cardNumber_ascii;
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

    public GameObject acumular_btn;
    public GameObject letrero_puntosAcumulados;

    [Header("LectorRFID")]
    public TMP_Text tarjeta_id_text;
    public int tarjeta_id;
    Cardreader _cardReader;

    [Header("LectorRFID_MiFare")]
    private MiFare.Devices.SmartCardReader _lector;
    private MiFareCard _tarjeta;

    [Space(10)]
    [Header("Textos Informe")]
    public GameObject panelError_tickets;
    public TMP_Text textoMensaje_error;
    public string errorAcumulacionPuntos;
    public string errorLecturaTarjeta;
    public string errorToken;
    

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
            _tarjeta?.Dispose();

        }
        else if (_tickets != null)
        {
            _tarjeta?.Dispose();
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
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.C))
            {

                panelSacoa.SetActive(!panelSacoa.activeInHierarchy);
                panelTickets.SetActive(!panelTickets.activeInHierarchy);
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                ActivarLectorTarjeta();
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
    //    File.WriteAllText("D:\\Proyectos\\El_Chavo\\Unity\\datosenviados.txt", jsonString); //https://www.youtube.com/watch?v=ngX7-6jKIr8

        StartCoroutine(Post(url_login, jsonString));


    }



    /// <summary>
    /// Cuando se cambian los valores desde la consola de operacion para el inicio de sesion de sacoa
    /// y despues termina llamando a LoggeoSacoa();
    /// </summary>
    /// 
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

            computerID = int.Parse(computerID_text.text);
        }
        if (urlLogeo_text.text != null || urlLogeo_text.text != "")
        {
            url_login = urlLogeo_text.text;
        }

        LoggeoSacoa();
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
        //    File.WriteAllText("D:\\Proyectos\\El_Chavo\\Unity\\datosRecibidos.txt", m);
            usuario_logeado = JsonUtility.FromJson<UsuarioLoggeado>(m);
            Debug.Log("Exito!...Inicio de sesion aceptado con token:" + usuario_logeado.body.token);
            token_text.text = "Token: " + usuario_logeado.body.token;
            //  CardBalance_aJson();
        }
        else if (request.responseCode == 401)
        {
            //Esto es para cuando se vence el TOKEN hay que pedir otro
            Debug.Log("Error: " + request.responseCode);
            Debug.Log("El Token vencio o no esta autorizado...Solicitando nuevo Token...");
            token_text.text = "El Token vencio o no esta autorizado...Solicitando nuevo Token...";
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
        Debug.Log("Solicitando Balance de Tarjeta...");
        tarjeta_balance.posId = int.Parse(posID);
        tarjeta_balance.empId = usuario_logeado.body.empId;
        tarjeta_balance.cardNumber = cardNumber;
        tarjeta_balance.token = usuario_logeado.body.token;
        tarjeta_balance.extEmpId = int.Parse(extEmpID);

        string aJson = JsonUtility.ToJson(tarjeta_balance) ?? "";
        Debug.Log("Tarjeta convertida a: " + aJson);

        StartCoroutine(PostCardBalance(url_cardBalance, aJson));

    }
    IEnumerator PostCardBalance(string url, string bodyJsonString)
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
            // File.WriteAllText("D:\\Proyectos\\El_Chavo\\Unity\\datosRecibidos.txt", m);
            tarjeta_datos = JsonUtility.FromJson<CardBalanceResponse>(m);
            Debug.Log("Exito!... Balance de Tarjeta:" + tarjeta_datos.body.card + "| Credito: " + tarjeta_datos.body.credits + "|Tickets: " + tarjeta_datos.body.tickets);

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
        if (puntosJugador < 49)
        {
            Debug.Log("El jugador no gano ningun ticket...");
            return;
        }

        int _tickets = Mathf.FloorToInt(puntosJugador / valorTicket);
        tickets_Partida = _tickets;
        //TODO: Aparecer panel para el controlador mostrando 
        panelTickets.SetActive(true);
        //-EL panel debe mostrar los ticket a acumular
        tickets_jugador_text.text = tickets_Partida.ToString();
        //Activar el lector de Tarjetas para que comience a esperar la tarjeta
        ActivarLectorTarjeta();
        //Letrero que informe que debe pasar la tarjeta por el lector
        panelTickets.SetActive(true);
        acumular_btn.SetActive(true);
        letrero_puntosAcumulados.SetActive(false);

        

    }



    /// <summary>
    /// Cambia el valor de los puntos y de la URL si el operador lo cambio en la consola
    /// </summary>
    public void CambiarPuntos()
    {
        if (puntos_text.text != null || puntos_text.text != "")
        {
            valorTicket = int.Parse(puntos_text.text);

        }
        if (urlTicket_text.text != null || urlTicket_text.text != "")
        {
            url_ticketsAgregar = urlTicket_text.text;
        }
    }

    /// <summary>
    /// Llamado cuando se agrego manualmente por el operador la tarjeta desde la consola
    /// </summary>
    public void IngresarTarjeta()
    {
        if (tarjeta_id_input.text != null || tarjeta_id_input.text != "")
        {
            tarjeta_id_text.text = "Tarjeta #" + tarjeta_id_input.text;
            cardNumber = int.Parse(tarjeta_id_input.text);
        }
    }


    /// <summary>
    /// Crea el formato para agregar puntos al Jugador
    /// Se tiene que pasar la tarjeta o apretar el boton "Acumular" en consolar
    /// 
    /// </summary>
    [Button("Agregar Puntos")]
    public void AgregarPuntos_aJson()
    {
        Debug.Log("Agregando puntos a Tarjeta...");
        tarjetaAgregar.posId = 1;
        tarjetaAgregar.empId = usuario_logeado.body.empId;
        tarjetaAgregar.cardNumber = cardNumber;
        tarjetaAgregar.token = usuario_logeado.body.token;
        //TODO:  ticketAmount  = ticketsPartida
        tarjetaAgregar.ticketAmount = tickets_Partida; // tickets_Partida
        tarjetaAgregar.extEmpId = 1;

        string aJson = JsonUtility.ToJson(tarjetaAgregar);
        Debug.Log("Enviando puntos a: " + aJson);
       // StartCoroutine(PostTickets(url_ticketsAgregar, aJson));

        DoOnMainThread.ExecuteOnMainThread.Enqueue(() => {
            StartCoroutine(PostTickets(url_ticketsAgregar,aJson));
        });
    }
    IEnumerator PostTickets(string url, string bodyJsonString)
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

            if (tarjetaVerificacion.statusCode == 200)
            {
                acumular_btn.SetActive(false);
                letrero_puntosAcumulados.SetActive(true);
                Debug.Log("Exito!...Tickets en Tarjeta:" + tarjetaVerificacion.body.tickets);
                CerrarMensajeError();

            }else
            {
                Debug.Log("Error: No se pudieron acumular los puntos, Pase de nuevo la tarjeta o agregue los puntos manualmente");
                ActivarPanelError_Tickets(errorAcumulacionPuntos);
                ActivarLectorTarjeta();
            }



            //if (!panelSacoa.activeInHierarchy)
            //    panelTickets.SetActive(false);
            //  AgregarPuntos_aJson();
        }
        else if (request.responseCode == 401)
        {
            ActivarPanelError_Tickets(errorToken);
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

    //TODO
    //Se podria refactorizar esta area de LectorTarjetas a una clase

    #region LectorTarjetas


    /// <summary>
    /// Activa el Lector de Tarjetas NFC
    /// Llamado desde EventDispatcher_TotalScore
    /// </summary>
    [Button]
    public void ActivarLectorTarjeta()
    {
        
        GetDevices();
        Debug.Log("Activando lector de tarjetas");

    }
    

    /// <summary>
    /// Revisa los lectores NFC conectados y crea un objeto SmartCardReader y lo suscribe a
    /// eventos de CardAdded y CardRemoved
    /// Preocurar solo tener conectado 1 lector
    /// </summary>
    /// <returns>None</returns>
    async private void GetDevices()
    {
        try
        {

            //MiFare.Devices.SmartCardReader smartCardReader = await CardReader.FindAsync();

            //_lector = smartCardReader;
            _lector?.Dispose();
            _lector = await CardReader.FindAsync();

            if (_lector == null)
            {
                Debug.Log("Lectores no encontrados, trata de que sea solo 1");
                ActivarPanelError_Tickets("Lectores no encontrados, trata de que sea solo 1");
                return;
            }

           
            _lector.CardAdded += _lector_CardAdded;
            _lector.CardRemoved += _lector_CardRemoved;


        }
        catch (Exception e)
        {
            Debug.Log("Exception: " + e.Message);
            _lector.Dispose();
            ActivarPanelError_Tickets(errorLecturaTarjeta);

        }
    }

    private void _lector_CardRemoved(object sender, MiFare.Devices.CardRemovedEventArgs e)
    {
        _tarjeta?.Dispose();
    }

    private async void _lector_CardAdded(object sender, MiFare.Devices.CardAddedEventArgs args)
    {
        try
        {
            Debug.Log("Leyendo tarjeta");
            await HandleCard(args);
        }
        catch (Exception e)
        {
            Debug.Log("TarjetaAgregada Exception: " + e.Message);

              ActivarPanelError_Tickets(errorLecturaTarjeta);
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


            Debug.Log("Connected to card\r\nPC/SC device class: " + cardIdentification.PcscDeviceClass.ToString() + "\r\nCard name: " + cardIdentification.PcscCardName.ToString());

            if (cardIdentification.PcscDeviceClass == MiFare.PcSc.DeviceClass.StorageClass
                 && (cardIdentification.PcscCardName == CardName.MifareStandard1K || cardIdentification.PcscCardName == CardName.MifareStandard4K))
            {
                // Handle MIFARE Standard/Classic
                Debug.Log("MIFARE Standard/Classic card detected");


                var uid = await _tarjeta.GetUid();
                Debug.Log("UID:  " + BitConverter.ToString(uid));




                // 16 sectors, print out each one
                for (var sector = 0; sector < 2; sector++)
                {
                    //try
                    //{
                    var data = await _tarjeta.GetData(sector, 0, 48);

                    string hexString = "";
                    for (int i = 0; i < data.Length; i++)
                    {
                        hexString += data[i].ToString("X2");
                    }

                    Debug.Log(string.Format("Sector '{0}':{1}", sector, hexString));



                    if (sector == 1)
                    {
                        if (hexString != null || hexString != "")
                        {
                            //Sutraemos el valor Hexadecimal que buscamos, es la pos5 pero no nos lo da asi, 
                            //asi que lo buscamos por posicion en el array de string
                            string hexTarjeta = hexString.Substring(0, 24);
                            Debug.Log("Sacoa Hex: " + hexTarjeta);
                            //despues convertimos hexTarjeta a un valor ascii en string
                            string ascii = ConvertHex(hexTarjeta.ToString());
                            //Se lo pasamos a cardNumber_ascci para que lo mande a decodficar a DecodeTarjeta()
                            cardNumber_ascii = ascii;
                            DecodeTarjeta();
                            
                            return;
                        }
                    }

                    //}
                    //catch (Exception e)
                    //{
                    //    print("Failed to load sector: " + sector);
                    //    print("Failed to load sector error: " + e);
                    //    _tarjeta?.Dispose();

                    //}
                }



            }
            return;
        }
        catch (Exception e)
        {
            Debug.Log("HandleCard Exception: " + e.Message);
            Debug.Log("Es probable que el lector no este activado");
            //_tarjeta?.Dispose();

            ActivarPanelError_Tickets(errorLecturaTarjeta);
            ////CUIDADO PUEDE VOLVERSE UN LOOP
            ActivarLectorTarjeta();

        }
    }

    /// <summary>
    /// Llamado desde Enviar_DecodeTarjeta, recibe el numero de la tarjeta y se lo asigna a la UI 
    /// y la variable de cardNumber para su futuro envio a la funcion de  AgregarPuntos_aJson();
    /// </summary>
    /// <param name="num"></param>
    private void TarjetaEscaneada(string num)
    {
        Debug.Log("Se escaneo la tarjeta");
        if (num.Length >= 6)
        {
            tarjeta_id_text.SetText("Tarjeta #" + num);
            cardNumber = int.Parse(num);
            Debug.Log("Numero de tarjeta correcto");
           

            //_lector.CardAdded -= _lector_CardAdded;
            //_lector.CardRemoved -= _lector_CardRemoved;
            _lector?.Dispose();
            if (_lector != null)
            {
                //En teoria esto deberia desactivar el lector
                _lector = null;
            }

            _tarjeta?.Dispose();
           
            AgregarPuntos_aJson();

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

   
    #region DecodificarTarjeta

    /// <summary>
    /// Ensambla la rutina de envio a la API de Sacoa convirtiendolo en un jSON
    /// podrias ponerle una propiedad para no tener la cardNumber_ascii en global pero asi la puedes llamar
    /// desde el Inspector
    /// </summary>
    [Button]
    public void DecodeTarjeta()
    {
        
        if(cardNumber_ascii =="")
        {
            Debug.Log("Atencion: agregar un numero ascii");
            return;
        }
        Debug.Log("Decodificando Tarjeta...");
        tarjetaDecode.posId = 1;
        tarjetaDecode.empId = usuario_logeado.body.empId;
        tarjetaDecode.cardNumber = cardNumber_ascii;//tiene que ser el asscii
        tarjetaDecode.token = usuario_logeado.body.token;
        tarjetaDecode.extEmpId = 1;

        string aJson = JsonUtility.ToJson(tarjetaDecode);
        Debug.Log("Enviando tarjeta codificada: " + aJson);


        // StartCoroutine(Enviar_DecodeTarjeta(url_tarjetaDecode, aJson));
        DoOnMainThread.ExecuteOnMainThread.Enqueue(() => {
            StartCoroutine(Enviar_DecodeTarjeta(url_tarjetaDecode, aJson));
        });

    }
    IEnumerator Enviar_DecodeTarjeta(string url, string bodyJsonString)
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
            //File.WriteAllText("D:\\Proyectos\\El_Chavo\\Unity\\datosRecibidos.txt", m);
            tarjetaDecode_response = JsonUtility.FromJson<TarjetaDecodResponse>(m);
           

            if (tarjetaDecode_response.statusCode == 200)
            {
                TarjetaEscaneada(tarjetaDecode_response.body.cardNumber);
                Debug.Log("Exito!...Tarjeta numero:" + tarjetaDecode_response);
            }
            else
            {
                Debug.Log("Error: No se pudo decodificar la tarjeta, checar si se esta leyendo bien la tarjeta \n" +
                    "Checar si estamos conectados a Sacoa o la url a donde tenemos que enviar esta bien");
                ActivarPanelError_Tickets(errorLecturaTarjeta);
                ActivarLectorTarjeta();

            }
            cardNumber_ascii = "";
           
        }
        else if (request.responseCode == 401)
        {
            ActivarPanelError_Tickets(errorToken);

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


    #region Panel Error Tickets
    /// <summary>
    /// Cierra el letrero de Error en el panel Tickets
    /// </summary>
    public void CerrarMensajeError()
    {
        panelError_tickets.SetActive(false);
    }

    public void ActivarPanelError_Tickets(string mensaje)
    {
        UnityMainThread.wkr.AddJob(() =>
        {
            textoMensaje_error.text = mensaje;
            panelError_tickets.SetActive(true);


        });
    }
    #endregion

}


/*
https:\\200.80.220.110:33001
user: jitzu
pass: @@jitzu    
*/
