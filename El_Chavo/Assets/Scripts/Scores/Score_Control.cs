using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System.IO;

[System.Serializable]
public class Jugador
{
    public string nombre = "---";
    public int score= 0;
    public int ronda= 0;
    public TMP_Text nombreText;
    public TMP_Text scoreText;
    public TMP_Text rondaText;


    public Jugador(string _nombre,int _score,int _ronda)
    {
        nombre = _nombre;
        score = _score;
        ronda = _ronda;
        //nombreText.text = _nombre;
        //scoreText.text = _score.ToString("0000");
        //rondaText.text = _ronda.ToString("00");
    }
    public void Rellenar(string _nombre, int _score, int _ronda)
    {
        nombre = _nombre;
        score = _score;
        ronda = _ronda;
        nombreText.text = nombre;
        scoreText.text = score.ToString("0000");
        rondaText.text = ronda.ToString("00");
    }
}

public class Score_Control : MonoBehaviour
{
    public static Score_Control _score;
    const string privateCode = "uYSjnkyYyE25WAhRUyZqAAxEEINGCGek-XSNlKnYdNhQ";
    const string publicCode   = "5d8271e2d1041303ecb5e86e";
    const string webURL = "http://dreamlo.com/lb/";//http://dreamlo.com/lb/uYSjnkyYyE25WAhRUyZqAAxEEINGCGek-XSNlKnYdNhQ

    public Jugador[] jugadores;
    public Jugador jugadorActual;
    
    public int numJugadorCambio;
    public Jugador[] jugadores_highScoreJuego;
    [Space(10)]
    [Header("UIX")]
    public GameObject panelNombre;
    public TMP_InputField nombre_input;

    [Header("UIX Ronda")]
    public TMP_Text ronda_text;
    public TMP_Text siguienteRonda_text;
    public TMP_Text rondaFinal_text;
    public TMP_Text scoreEnRonda_text;
    public TMP_Text scoreFinal_text;
    public GameObject canvasJuego;
    public GameObject canvasSiguienteRonda;
    public GameObject canvasFinJuego;
    public TMP_Text nuevoHighscore_text;

    public TMP_Text scoreBest_text;
    public TMP_Text nombreBest_text;

    [Header("UIX-Operador")]
    public TMP_Text rondaOperador_txt;
    public TMP_Text tiempoRonda_txt;
    public TMP_Text scoreCanvasJugador_txt;
    public Slider vidaSlider_operador;

    public int scoreJugador;
    int scoreLerp;


     MasterLevel _master;

    private void Awake()
    {
        _score = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        BajarScore();
        _master = FindObjectOfType<MasterLevel>();
        nuevoHighscore_text.gameObject.SetActive(false);

        EventDispatcher.RondaTerminada += EventDispatcher_RondaTerminada;
    }
    private void OnDisable()
    {
        EventDispatcher.RondaTerminada -= EventDispatcher_RondaTerminada;
    }
    private void FixedUpdate()
    {
        if (canvasJuego.activeInHierarchy)
        {
            if (scoreLerp < scoreJugador)
                scoreLerp += 10;


            scoreEnRonda_text.text = scoreLerp.ToString("0000");
            scoreCanvasJugador_txt.SetText(scoreEnRonda_text.text);
        }
    }
    private void EventDispatcher_RondaTerminada()
    {
        MostrarFinRonda();
    }

    public void MostrarFinRonda()
    {
        if (!MasterLevel.masterlevel.jugando)
            return;

        ronda_text.text = (_master.rondaNum + 1).ToString("00");//iniciando en 0 da 
        scoreJugador = _master.scoreJugador;
        canvasJuego.SetActive(true);
    }

    public void MostrarRondaSiguiente()
    {
        siguienteRonda_text.text = (_master.rondaNum + 1).ToString("00");//si da 1 es 2
        rondaOperador_txt.text = "Ronda: " + (_master.rondaNum + 1).ToString("00");
        //rondaOperador_txt.SetText("Ronda");//otra manera de ponerle texto en TMPro
        canvasSiguienteRonda.SetActive(true);
    }

    public void MostrarFinJuego()
    {
        scoreFinal_text.text = "Score: " + _master.scoreJugador.ToString("0000") + "    " + "Ronda: " + (_master.rondaNum + 1).ToString("00");

        EventDispatcher.IngresarTicketsPartida(_master.scoreJugador);
        scoreBest_text.text = "Score: " + jugadores[0].score.ToString("0000") + "    " + "Ronda: " + jugadores[0].ronda.ToString("00");
        nombreBest_text.text = jugadores[0].nombre;
        canvasFinJuego.SetActive(true);

    }

    /// <summary>
    /// Llamado al final del juego para saber si necesitamos anotar un nuevo score
    /// </summary>
    public void CompararScore()
    {
        if (jugadores.Length <= 0)//Si es = 0 entonces no hubo entrada de internet
        {
            Debug.Log("No hubo entrada de internet");
        }
        else
        {
            for (int i = 0; i < jugadores.Length; i++)
            {
                if(jugadorActual.score > jugadores[i].score)
                {
                    Debug.Log("Nuevo HighScore...");

                    numJugadorCambio = i;
                    panelNombre.SetActive(true);
                    //TODO: aparecer letrero de nuevo highscore
                    nuevoHighscore_text.gameObject.SetActive(true);
                    break;
                }
            }
        }
    }

    public void SubirScore()
    {
        for (int i = 0; i < jugadores.Length; i++)
        {
            StartCoroutine(SubirHighScore(jugadores[i].nombre, jugadores[i].score,jugadores[i].ronda));
        }
       
    }

    public void BajarScore()
    {
        StartCoroutine(DescargarScore());
    }

    public IEnumerator SubirHighScore(string user, int score,int oleada)
    {
        UnityWebRequest webRequest =  UnityWebRequest.Get(webURL + privateCode + "/add/" + UnityWebRequest.EscapeURL(user)+"/"+score+"/"+ oleada);
        webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        yield return webRequest.SendWebRequest();

        if (webRequest.error != null)
        {
            Debug.Log(webRequest.error);
        }
        else
        {
        
            Debug.Log("Se subio score con correctamente");
          
        }
        StartCoroutine(DescargarScore());

    }

    public IEnumerator DescargarScore()
    {
      
        UnityWebRequest webRequest = UnityWebRequest.Get(webURL + publicCode + "/pipe/0/5");
        webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        yield return webRequest.SendWebRequest();

        if(string.IsNullOrEmpty(webRequest.error))
        {
            Debug.Log("Scores bajados con exito");
            Debug.Log(webRequest.downloadHandler.text);
            //Escribir en una hoja por si no hay datos o internet
            //File.WriteAllText("D:\\Proyectos\\El_Chavo\\Unity\\datosRecibidos.txt", m);
            FormatoWeb(webRequest.downloadHandler.text);
            ActualizarTableroJuego();


        }else
        {
            Debug.Log("Error:" + webRequest.error);
        }

       
    }


    public void IngresarNuevoNombre()
    {
        if (string.IsNullOrEmpty(nombre_input.text))
        {
            Debug.Log("Ingrese nombre para continuar");
            return;
        }

        jugadores[numJugadorCambio].Rellenar(nombre_input.text, jugadorActual.score, jugadorActual.ronda);
        panelNombre.SetActive(false);
        SubirScore();
        
    }
   
    /// <summary>
    /// Actualiza el tablero de Highscore que se encuentra
    /// dentro del juego
    /// </summary>
    void ActualizarTableroJuego()
    {
        for (int i = 0; i < jugadores.Length; i++)
        {
            jugadores_highScoreJuego[i].Rellenar(jugadores[i].nombre, jugadores[i].score, jugadores[i].ronda);
        }
    }

    /// <summary>
    /// Se encarga de contextualizar la informacion y rellenar los UIX correspondientes de 5 HighScores
    /// </summary>
    /// <param name="texto"></param>
    void FormatoWeb(string texto)
    {
        string[] entradas = texto.Split(new char[] {'\n'},System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < entradas.Length; i++)
        {
            string[] entradasInfo = entradas[i].Split(new char[] { '|' });
            string jugadorNombre = entradasInfo[0];
            int scoreInfo = int.Parse(entradasInfo[1]);
            int oleadaInfo = int.Parse(entradasInfo[2]);
            //Jugador nJugador = new Jugador(jugadorNombre, scoreInfo, oleadaInfo);
            //jugadores[i] = nJugador;
            jugadores[i].Rellenar(jugadorNombre, scoreInfo, oleadaInfo);
         
        }
    }

}

/*
 NOTAS:
 -La relacion tickets puntos por el momento es 50pts = 1 tickets
 -El score por el momento necesita internet, de lo contrario nos vamos a encontrar con un error
     */