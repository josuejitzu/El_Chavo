using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;
using FMODUnity;
using System;
using EasyButtons;
using UnityEngine.Playables;

public class MasterLevel : MonoBehaviour
{
    public static MasterLevel masterlevel;
    public bool jugando;
    [SerializeField] private bool enIntro;
    [Header("Vida Jugador")]
    public float vidaMax = 200.0f;
    public float vidaJugador = 0.0f;
    public Slider vidaSlider;
    public Image vidaCirculo;
    public Image vidaCirculo_Operador;
    public float velocidadLlenado;
    public bool inmortal;
    public Sprite[] circulosVida;

    [Header("LetreroInicio")]
    public GameObject letroInicio;
    public ParticleSystem particulaGolpe_letrero;
    [Space(10)]
    [Header("AnimIntro")]
    [SerializeField] private PlayableDirector intro_timeLine;

    [Space(10)]
    [Header("Rondas")]
    public int rondaNum;
    public Rondas[] rondas;
  

    [Space(10)]
    [Header("Tiempo")]
    public float tiempoJuego;
    public bool contando;
    public float tiempoEspera = 5.0f;

    [Space(10)]
    [Header("Scores")]
    public int scoreJugador;
    int scoreLerp;
   

    public bool contarCombo;
    public int numCombo;
    public TMP_Text numCombo_text;

    [Space(10)]
    [Header("KEY")]
    public KeyCode teclaIniciar;

    [Space(10)]
    [Header("UIX-Operador")]
    public TMP_Text rondaOperador_txt;
    public TMP_Text tiempoRonda_txt;
    public Slider vidaSlider_operador;

    [Space(10)]
    [Header("Manos")]
    public ManoControl manoL;
    public ManoControl manoR;
    [Header("Resortera")]
    public Resortera_Control resortera;

    [Space(10)]
    [Header("UpdateScripts")]
    public List<GloboControl> globosUpdate = new List<GloboControl>();

    [Space(10)]
    [Header("SFX")]
    public StudioEventEmitter jugadorGolpeado_sfx;
    public StudioEventEmitter musicaTitulo, musicaJuego, siguienteRonda_sfx;
    [Space(5)]
    [SerializeField]private StudioEventEmitter donRamon_intro_sfx;
    [SerializeField] private Animator donRamon_intro_anim;
    public StudioEventEmitter donRamon_inicioRonda_sfx;
    public StudioEventEmitter donRamon_finRonda_sfx;
    public StudioEventEmitter donRamon_GameOver_sfx;
    
    //  [FMODUnity.EventRef] public string chiflido_sfx;
    [Header("VFX")]
    public Animator splash_anim;
  

    [Header("Consola")]
    public GameObject consolaPanel;
    public GameObject panelOperador;
    public GameObject botonAbrir;
    [Range(0.0f, 3.0f)]
    public float tiempoVelocidad = 1.0f;
   [SerializeField] private GameObject camaraB_proyector;

    private void Awake()
    {
        masterlevel = this;
    }
    void Start()
    {
        //masterlevel = this;  
        // musicaTitulo.Play();
        contarCombo = true;
        StartCoroutine(PreJuego());
        enIntro = false;
        //EventDispatcher.RondaTerminada += ResetearCombo;
    }

    private void OnDisable()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        Time.timeScale = tiempoVelocidad;
        vidaSlider.value = Mathf.MoveTowards(vidaSlider.value, vidaJugador, Time.deltaTime * velocidadLlenado);
        vidaSlider_operador.value = vidaSlider.value;
       

        if (contando)
        {
            Tiempo();
        }

        for (int i = 0; i < globosUpdate.Count; i++)
        {
            if(globosUpdate[i] != null && globosUpdate[i].gameObject.activeInHierarchy)
                globosUpdate[i].MiUpdate();
        }


        if(Input.GetKey(KeyCode.LeftControl))
        {
            ComandosConsola();
        }
      

        //splashImg.color = Color.Lerp(splashImg.color, alfaSplash, Time.deltaTime * 1.0f);
    }

    //private void FixedUpdate()
    //{
    //    if (canvasJuego.activeInHierarchy)
    //    {
    //         if (scoreLerp < scoreJugador)
    //           scoreLerp += 10;
          

    //        scoreEnRonda_text.text = scoreLerp.ToString("0000");
    //    }
    //}

    /// <summary>
    /// Donde se puede pedir cargar ciertas cosas antes de que Inicie el Juego
    /// </summary>
    /// <returns></returns>
    IEnumerator PreJuego()
    {
        yield return new WaitForSeconds(1.0f);
        letroInicio.SetActive(true);

    }

    [Button("Intro", ButtonSpacing.Before)]

    public void IniciarIntroCall()
    {
        if (enIntro || jugando)
            return;
        StartCoroutine(IniciarIntro());
        enIntro = true;
        particulaGolpe_letrero.Play();
        letroInicio.GetComponent<Animator>().SetTrigger("golpeado");


    }

    [Button("Jugar",ButtonSpacing.Before)]
    public void IniciarJuegoCall()
    {
        if (jugando)//evitar duplicados de inicio
            return;

      
        donRamon_intro_sfx.Stop();
        StopCoroutine(IniciarIntro());
        

        StartCoroutine(IniciarJuego());
        //StartCoroutine(IniciarIntro());
    }

    public void ReiniciarJuegoCall()
    {

        StartCoroutine(ReiniciarJuego());
    }
    private IEnumerator ReiniciarJuego()
    {
        jugando = false;
        contando = false;

        EventDispatcher.LlamarFinDeRonda();


        LanzamientosControl._lanzamientos.disparar = false;
        LanzamientosControl._lanzamientos.DesactivarLanzadores();
        //   donRamon_GameOver_sfx.Play();
        yield return new WaitForSeconds(0.05f);
        SceneManager.LoadScene(0);
    }

    IEnumerator IniciarIntro()
    {

        //Animacion de DonRamon 
        //Audio intro de Don Ramon


        intro_timeLine.gameObject.SetActive(true);
        letroInicio.GetComponent<Animator>().SetTrigger("salir");

        if (intro_timeLine.state != PlayState.Playing)
             intro_timeLine.Play();

        yield return new WaitForSeconds(22.0f);

        if (jugando)
            yield break;
        StartCoroutine(IniciarJuego());

    }

    public IEnumerator IniciarJuego()
    {
        //letroInicio.GetComponent<Animator>().SetTrigger("salir");
        //intro_timeLine.Pause();
        if (intro_timeLine.state == PlayState.Playing && intro_timeLine.time < 21.0f)
            intro_timeLine.time = 21.0f;

        if (enIntro)
            enIntro = false;
        //animacion de letrero
        yield return new WaitForSeconds(2.0f);
        letroInicio.SetActive(false);
        LanzamientosControl._lanzamientos.PrepararRound();
        tiempoJuego = rondas[rondaNum].duracion;

        ///Muestra el panel de Inicio
        Score_Control._score.MostrarRondaSiguiente();
        //siguienteRonda_text.text = (rondaNum + 1).ToString("00");//si da 1 es 2
        //rondaOperador_txt.text = "Ronda: " + (rondaNum + 1).ToString("00");
        //canvasSiguienteRonda.SetActive(true);


        numCombo = 0;
        numCombo_text.text = "Combo x" + numCombo.ToString("00");
        musicaTitulo.Stop();
        musicaJuego.Play();


       

        yield return new WaitForSeconds(2.0f);
        Score_Control._score.canvasSiguienteRonda.SetActive(false);
        intro_timeLine.gameObject.SetActive(false);

        // siguienteRonda_sfx.Play();
        yield return new WaitForSeconds(0.5f);
        jugando = true;
        contando = true;
        LanzamientosControl._lanzamientos.disparar = true;

    }

    public IEnumerator FinRonda()
    {

        EventDispatcher.LlamarFinDeRonda();

        jugando = false;
        donRamon_finRonda_sfx.Play();
        ResetearCombo();

        LanzamientosControl._lanzamientos.disparar = false;
        LanzamientosControl._lanzamientos.DesactivarLanzadores();//Se intento llamar con eventdispatcher pero la logica no funciono
       // Score_Control._score.MostrarFinRonda();//Llamando por evenDispather
        yield return new WaitForSeconds(3.0f);
        Score_Control._score.canvasJuego.SetActive(false);//canvasJuego.SetActive(false);

        if (rondaNum < rondas.Length)
        {
            rondaNum++;
        }
        else
        {
            Debug.Log("Error: NO HAY MAS RONDAS");
            yield break;
        }

        Score_Control._score.MostrarRondaSiguiente();
        yield return new WaitForSeconds(2.0f);
        // siguienteRonda_sfx.Play();
        donRamon_inicioRonda_sfx.Play();


        Score_Control._score.canvasSiguienteRonda.SetActive(false);

        numCombo = 0;
        numCombo_text.text = "Combo x" + numCombo.ToString("00");
        // StartCoroutine(IniciarJuego());

        ///Lanza los parametros y comienza la siguiente Ronda
        LanzamientosControl._lanzamientos.PrepararRound();
        tiempoJuego = rondas[rondaNum].duracion;
        yield return new WaitForSeconds(0.5f);
        jugando = true;
        contando = true;
        LanzamientosControl._lanzamientos.disparar = true;

    }

    public IEnumerator FinJuego()
    {
        jugando = false;
        contando = false;

        EventDispatcher.LlamarFinDeRonda();
        ResetearCombo();
       
        LanzamientosControl._lanzamientos.disparar = false;
        LanzamientosControl._lanzamientos.DesactivarLanzadores();
     //   donRamon_GameOver_sfx.Play();
        yield return new WaitForSeconds(0.05f);
        Score_Control._score.MostrarFinJuego();
        Score_Control._score.jugadorActual = new Jugador("", scoreJugador, rondaNum + 1);
        Score_Control._score.CompararScore();
    }

    void Tiempo()
    {
        if(tiempoJuego > 0.0f)
        {
            tiempoJuego -= Time.deltaTime;

        }else if(tiempoJuego <= 0.0f)
        {
            contando = false;
            StartCoroutine(FinRonda());
            
        }
        tiempoRonda_txt.text = "00:"+ tiempoJuego.ToString("00");
    }

    public void DañarJugador(float cantidad)
    {
        if (!jugando)
            return;
       
        splash_anim.SetTrigger("mojar");
        jugadorGolpeado_sfx.Play();
        EventDispatcher.LlamarJugadorGolpeado();
        if (!inmortal)
        {

            numCombo = 0;
            numCombo_text.text = "Combo x" + numCombo.ToString("00");

        }

        StartCoroutine(PowerUp_Control._powerUps.CancelarPowerUP());


        vidaJugador += cantidad;


        int pie = Mathf.FloorToInt((vidaJugador / vidaMax) * 10);

       // print(pie);
        if (pie < circulosVida.Length)
        {
            vidaCirculo.sprite = circulosVida[pie];
            vidaCirculo_Operador.sprite = circulosVida[pie];
        }

        if (vidaJugador >= vidaMax)
        {
            if (inmortal)
                return;

            vidaCirculo.sprite = circulosVida[10];
            vidaCirculo_Operador.sprite = circulosVida[10];

            StartCoroutine(FinJuego());
            return;
        }

    }

    public void ScoreJugador(int valor)
    {
        if (!jugando)
            return;

        scoreJugador += valor;

        if (contarCombo)
        {
            numCombo += 1;
            if (numCombo == 5)
            {
                contarCombo = false;
                numCombo_text.gameObject.SetActive(false);
                PowerUp_Control._powerUps.ActivarPowerUp();//Activa los letreros de powerUp
            }
            numCombo_text.text = "Combo x" + numCombo.ToString("00");

        }

        //Se quito para hacer el LERP del Score
        //scoreEnRonda_text.text = scoreJugador.ToString("0000");

    }

    public void RegistrarUpdate(string tipo,GameObject objeto)
    {
        if(tipo == "globo")
        {
            globosUpdate.Add(objeto.GetComponent<GloboControl>());
        }
    }

    public void RemoverUpdate(GameObject objeto, string tipo)
    {
        if(tipo == "globo")
        {
            globosUpdate.Remove(objeto.GetComponent<GloboControl>());
        }
       // globosUpdate.Sort();
    }

    public void ColocarResortera(string mano)
    {
        if(mano == "derecha")
        {
            manoL.SoltarResortera();

            manoR.sobreResortera = true;
            manoR.resortera = resortera.gameObject;
            manoR.TomarResortera();
        }
        else if(mano == "izquierda")
        {
            manoR.SoltarResortera();

            manoL.sobreResortera = true;
            manoL.resortera = resortera.gameObject;
            manoL.TomarResortera();
        }

    }


    public void ResetearCombo()
    {
        numCombo = 0;
        numCombo_text.gameObject.SetActive(true);
        numCombo_text.text = "Combo x" + numCombo.ToString("00");
        contarCombo = true;
    }

    #region Consola
    private void ToggleConsola()
    {
        if(consolaPanel.activeInHierarchy)
        {
            consolaPanel.SetActive(false);

        }else if(consolaPanel.activeInHierarchy == false)
        {
            consolaPanel.SetActive(true);
        }

    }

    /// <summary>
    /// Todos estos comandos solo se activan si se esta presionando ctrl + la tecla del comando
    /// </summary>
    private void ComandosConsola()
    {

        if (Input.GetKeyDown(teclaIniciar))
        {
            IniciarJuegoCall();
        }

        if (Input.GetKeyDown(KeyCode.C))
            ToggleConsola();


        if (Input.GetKeyDown(KeyCode.P))
        {
            // ActivarPowerUp();
            Resortera_Control._resortera.ActivarPowerUp(MunicionTipo.Explosiva);
            StartCoroutine(PowerUp_Control._powerUps.DesactivacionPowerUP());
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            //  ActivarPowerUp(MunicionTipo.Normal);
            Resortera_Control._resortera.InputPowerUp();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            Resortera_Control._resortera.Disparar();
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            ReiniciarJuegoCall();
        }
        if(Input.GetKeyDown(KeyCode.I))
        {
            inmortal = !inmortal;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        { Application.Quit(); }


    }


    public void TogglePanelOperador()
    {
        panelOperador.SetActive(!panelOperador.activeInHierarchy);//el contrario de su estado
        botonAbrir.SetActive(!botonAbrir.activeInHierarchy);
    }
    public void ToggleCamaraB()
    {
        camaraB_proyector.SetActive(!camaraB_proyector.activeInHierarchy);
    }
    #endregion


    //public void DonRamonRonda_SFX(string evento)
    //{
    //    if (donRamon_inicioRonda_sfx.IsPlaying())
    //        donRamon_inicioRonda_sfx.Stop();

    //    donRamon_inicioRonda_sfx.Event = evento;
    //    donRamon_inicioRonda_sfx.Play();
    //}
}
