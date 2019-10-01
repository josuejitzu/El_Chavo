using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FMODUnity;

public class GloboControl : MonoBehaviour
{
    public TipoPersonaje _tipoPersonaje;
    public GloboTipo _tipoGoblo;
    
    public GameObject globoChavo,globoKiko,globoÑoño,globoDonRamon,globoPoppy,globoDoñaFlorinda,globoMini;
    public GameObject meshGlobo;
    public SphereCollider trigger;
    public Rigidbody rigid;
    public ParticleSystem explosion_vfx,explosionPoppy_vfx;

    [Space(10)]
    [Header("Settings Lanzamiento")]
    public float alturaArco = 5.0f;
    public float tiempoDeRecorrido = 2.0f;
    public bool brincar;
    public float timer = 0.0f;
    public Transform objetivo;
    public Vector3 posInicial, posFinal;
    public int valorGlobo;
    public Vector3 posFlorinda;
    [Space(10)]
    [Header("Settings Personaje")]
    public SettingLanzamiento chavo_s;
    public SettingLanzamiento kiko_s,poppy_s,ñoño_s,donRamon_s,doñaFlorinda_s,miniFlorinda_s;

    [Space(10)]
    [Header("Vida")]
    public Slider vidaSlider;
    public int vida;
    public int vidaInicial;
    public float dañoJugador;
    public TMP_Text puntos_text;
    public Image marcoVida;
    public Image marcoVidax1,marcoVidax2,marcoVidax3,marcoVidax4;

    [Space(10)]
    [Header("MiniGlobos_Florinda")]
    public GameObject[] globosFlorinda;
    public Transform globoPadre;
    public Transform mini_posInicial;
    public int cantMiniGlobos;
    public bool bombardeo;
    public float tiempoDisparoMiniGlobos;
    bool kamikaze;
    public float velocidadKamikaze = 2.0f;
    float tiempoMini;
    public Slider sliderFlorinda;
    public Vector3 initSize, finalSize;
    [Space(10)]
    [Header("Globos Donramon")]
    public GameObject[] globosRamon;

    public bool enMira;
    public Image lockedImg;

    [Space(10)]
    [Header("VFX")]
    public ParticleSystem[] golpes_vfx;
    [Header("SFX")]
    public StudioEventEmitter fmod_globos;
    public StudioEventEmitter golpeGlobo_sfx;
    public StudioEventEmitter globoPoppis_sfx;
    public StudioEventEmitter quico_sfx;
    [FMODUnity.EventRef]
    public string golpe_sfx;
    [FMODUnity.EventRef]
    public string explosionAgua_sfx;
    [FMODUnity.EventRef] public string globoChavo_sfx;
    [FMODUnity.EventRef] public string globoQuico_sfx;
    [FMODUnity.EventRef] public string globoPopis_sfx;
    [FMODUnity.EventRef] public string globoÑoño_sfx;
    [FMODUnity.EventRef] public string globoDonRamon_sfx;
    [FMODUnity.EventRef] public string globoDoñaFlorinda_sfx;

    void Start()
    {
        tiempoMini = Time.time + tiempoDisparoMiniGlobos;
    }

    

    public void MiUpdate()
    {

        if (brincar)
        {
            
            Lanzando();
            vidaSlider.gameObject.transform.LookAt(objetivo.position);
        }
        if(bombardeo)
        {
            if (Time.time >= tiempoMini)
            {
                ComenzarBombardeo();
                tiempoMini = Time.time + tiempoDisparoMiniGlobos;
            }
        }

        if(kamikaze)
        {
            if(_tipoPersonaje == TipoPersonaje.doñaFlorinda)
            {
                this.transform.position = Vector3.MoveTowards(this.transform.position, objetivo.position, Time.deltaTime * velocidadKamikaze);
            }
        }
    }
    public void ActivarGlobo()
    {
        EventDispatcher.RondaTerminada += DesactivarGlobo;
        timer = 0.0f;

        if (_tipoPersonaje == TipoPersonaje.doñaFlorinda)
        {
            // vidaSlider.transform.localPosition = new Vector3(-0.5f, 1.50f, 0.0f);
            cantMiniGlobos = 0;
            this.transform.localScale = initSize;
            sliderFlorinda.gameObject.SetActive(true);
            vida = vidaInicial;
            sliderFlorinda.maxValue = vida;
            sliderFlorinda.value = vida;
            for (int i = 0; i < globosFlorinda.Length; i++)
            {
                //globosFlorinda[i].GetComponent<GloboControl>().CambiarGlobo();
                ////  globosFlorinda[i].transform.parent = this.transform;
                //globosFlorinda[i].GetComponent<GloboControl>().meshGlobo.SetActive(true);
                globosFlorinda[i].transform.parent = globoDoñaFlorinda.transform;

                globosFlorinda[i].GetComponent<GloboMini_Florinda>().RegresarAPosicion();
            }

        }
        else
        { 

           // this.transform.LookAt(this.transform.forward);
            vidaSlider.gameObject.SetActive(true);
            vida = vidaInicial;
            vidaSlider.maxValue = vida;
            vidaSlider.value = vida;
        }

        if(_tipoPersonaje == TipoPersonaje.kiko)
        {
            quico_sfx.Play();
        }

        trigger.enabled = true;

        if (_tipoPersonaje != TipoPersonaje.miniFlorinda)
        {
            meshGlobo.SetActive(true);
            marcoVida.enabled = true;
        }


         
        posInicial = this.transform.position;

    }

    public void ActivarMiniGlobos()//Esto es llamado por Lanzador_Globo cs de tipo DoñaFlorinda
    {
        for (int i = 0; i < globosFlorinda.Length; i++)
        {
            //globosFlorinda[i].GetComponent<GloboControl>().CambiarGlobo();
            ////  globosFlorinda[i].transform.parent = this.transform;
            //globosFlorinda[i].GetComponent<GloboControl>().meshGlobo.SetActive(true);
            globosFlorinda[i].transform.parent = globoDoñaFlorinda.transform;
            globosFlorinda[i].GetComponent<GloboMini_Florinda>().RegresarAPosicion();
        }
    }

    /// <summary>
    /// Llamado para cuando termina la Ronda, por el momento es llamado de dos maneras:
    /// MasterLevel o el LanzadorGlobos.cs correspondiente
    /// </summary>
    public void DesactivarGlobo()
    {
        EventDispatcher.RondaTerminada -= DesactivarGlobo;

        //StopAllCoroutines();
        //brincar = false;
        //trigger.enabled = false;
        //marcoVida.enabled = false;
        //kamikaze = false;

        ////Desactivar globos de doña florinda
        //if (_tipoPersonaje == TipoPersonaje.doñaFlorinda)
        //{
        //    foreach (GameObject gm in globosFlorinda)
        //    {
        //        if (gm.activeInHierarchy)
        //        {
        //            gm.GetComponent<GloboMini_Florinda>().DesactivarGlobo();
        //        }
        //    }
        //}

        //this.gameObject.SetActive(false);


        //
        brincar = false;
        trigger.enabled = false;
        //rigid.isKinematic = true;
        vidaSlider.gameObject.SetActive(false);
        kamikaze = false;
        CancelInvoke("ComenzarBombardeo");

        if (sliderFlorinda != null)
            sliderFlorinda.gameObject.SetActive(false);

        if (_tipoPersonaje == TipoPersonaje.miniFlorinda)
        {
            meshGlobo.SetActive(false);
        }
        else
        {
            meshGlobo.SetActive(false);
        }

        fmod_globos.Event = explosionAgua_sfx;
        fmod_globos.Play();
        marcoVida.enabled = false;

        explosion_vfx.Play();
        EfectoGolpe();

       

        if (_tipoPersonaje == TipoPersonaje.doñaFlorinda)
        {
            LanzamientosControl._lanzamientos.conFlorinda = false;
        }
        QuitarMira();
        this.gameObject.SetActive(false);
    }

    void Lanzando()
    {
        if (timer <= 1.0f)
        {
         

            if(_tipoPersonaje == TipoPersonaje.doñaFlorinda)
            {
                this.transform.position = Vector3.Lerp(posInicial, posFlorinda, timer);
                this.transform.localScale = Vector3.Lerp(initSize, finalSize, timer);
                trigger.radius = Mathf.Lerp(1.0f, 5.5f, timer);
                timer += Time.deltaTime / tiempoDeRecorrido;
                Vector3 dist = posFlorinda - posInicial;

                if(dist.magnitude <= 0.1f)
                {
                    brincar = false;
                   // bombardeo = true;
                }
            }
            else 
            {

                float altura = Mathf.Sin(Mathf.PI * timer) * alturaArco;
                transform.position = Vector3.Lerp(posInicial, posFinal, timer) + Vector3.up * altura;
                timer += Time.deltaTime / tiempoDeRecorrido;
                this.transform.LookAt(posFinal);
                if (_tipoPersonaje == TipoPersonaje.kiko)
                {
                    Vector3 dist = posFinal - transform.position;
                    if (dist.magnitude <= 0.1f)
                    {

                        Explotar();

                    }
                }
            }



        }

        else if (timer >= 1.0f)
        {
            brincar = false;

            if (_tipoPersonaje == TipoPersonaje.doñaFlorinda)
            {
                // bombardeo = true;
                Invoke("ComenzarBombardeo", 1.0f);
                print("Llego a posicion, comenzando bombardeo...");

            }

        }


    }

    public void ComenzarBombardeo()///Mini globod DoñaFlorinda
    {

        if(cantMiniGlobos < globosFlorinda.Length)
        {

            globosFlorinda[cantMiniGlobos].GetComponent<GloboMini_Florinda>().ActivarGlobo();
            globosFlorinda[cantMiniGlobos].GetComponent<GloboMini_Florinda>().posFinal = objetivo.transform.position;
            globosFlorinda[cantMiniGlobos].GetComponent<GloboMini_Florinda>().objetivo = objetivo;
            globosFlorinda[cantMiniGlobos].GetComponent<GloboMini_Florinda>().brincar = true;
            
                print("Disparando mini globo");
                cantMiniGlobos++;
                Invoke("ComenzarBombardeo", tiempoDisparoMiniGlobos);
            

        }else if (cantMiniGlobos >= globosFlorinda.Length)
        {
            print("Se acabo el bomardeo");
           
            posInicial = this.transform.position;
            posFinal = objetivo.position;
            kamikaze = true;
          //  timer = 0.0f;
          //  brincar = true;
        }
        //this.transform.LookAt(objetivo);
    }
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "municion" 
            || other.transform.tag == "personaje" 
            || other.transform.tag == "globo" 
            || other.transform.tag == "municionAutonoma" 
            || other.transform.tag == "paredes")
        {


        }else if(other.transform.tag == "MainCamera")//Le hace daño al Jugador y Explota
        {
            MasterLevel.masterlevel.DañarJugador(dañoJugador);

            if (gameObject.activeInHierarchy)
                StartCoroutine(Explotar());

            print(this.transform.name +" golpeo "+other.transform.name);

        }
        else
        {
            StartCoroutine(Explotar());
        }
        //print(other.transform.name);
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.transform.tag == "MainCamera")
        {
            MasterLevel.masterlevel.DañarJugador(dañoJugador);
            if(gameObject.activeInHierarchy)
               StartCoroutine(Explotar());
            print(other.transform.name);

        }
    }

    /// <summary>
    /// Calcula el Daño que Recibe un Globo por parte de Municion, por logica calcula 
    /// si debe ser destruido y llama a Destuir(), debe ser invocada por MunicionControl.cs
    /// </summary>
    /// <param name="cantidad">cantidad de daño que descontarle a la vida</param>
    public void RecibirDaño(int cantidad)
    {
        vida -= cantidad;
        vidaSlider.value = vida;

        if(sliderFlorinda != null)
            sliderFlorinda.value = vida;

        QuitarMira();

        if(_tipoPersonaje == TipoPersonaje.poppy)
        {
            globoPoppis_sfx.Play();
        }


        if (vida <= 0)
        {

            StartCoroutine(Destruir());
        }
        else
        {
            EfectoGolpe();
        }
    }

    /// <summary>
    /// Destruye el Globo porque su vida llego a 0 y fue llamada por RecibirDaño, tambien activa
    /// poderes secundarios si el TipoGlobo los tiene ej:(Poppy,DoñaFlorinda,DonRamon)
    /// </summary>
    /// <returns></returns>
    IEnumerator Destruir()
    {
        EventDispatcher.RondaTerminada -= DesactivarGlobo;

        CancelInvoke("ComenzarBombardeo");
        trigger.enabled = false;
        brincar = false;
        kamikaze = false;
        //rigid.isKinematic = true;
        vidaSlider.gameObject.SetActive(false);

        if (sliderFlorinda != null)
            sliderFlorinda.gameObject.SetActive(false);

        fmod_globos.Event = explosionAgua_sfx;
        fmod_globos.Play();
        marcoVida.enabled = false;
        MasterLevel.masterlevel.ScoreJugador(valorGlobo);

        puntos_text.text = "+" + valorGlobo.ToString();
        puntos_text.gameObject.SetActive(true);
        this.transform.LookAt(posFinal);

        EfectoGolpe();

       
         meshGlobo.SetActive(false);
        
        if (_tipoPersonaje == TipoPersonaje.poppy)
        {
            explosionPoppy_vfx.Play();
            yield return new WaitForSeconds(6.0f);

        }
        else
        {
            explosion_vfx.Play();
        }

        if(_tipoPersonaje == TipoPersonaje.donRamon )
        {
            ActivarDebuffDonRamon();
        }
        golpeGlobo_sfx.Play();

        yield return new WaitForSeconds(1.5f);

        puntos_text.gameObject.SetActive(false);
        //if (_tipoPersonaje == TipoPersonaje.miniFlorinda)
        //{
        //    this.transform.position = mini_posInicial.position;
        //    this.transform.parent = globoPadre;
            
        //}

        if(_tipoPersonaje == TipoPersonaje.doñaFlorinda)
        {
            LanzamientosControl._lanzamientos.conFlorinda = false;
        }
        lockedImg.gameObject.SetActive(false);
        enMira = false;
        this.gameObject.SetActive(false);

    }

    IEnumerator Explotar()//Cuando choca con algo, incluido el jugador pero no cuenta los puntos eso lo hace en TriggerEnter
    {
        brincar = false;
        trigger.enabled = false;
        //rigid.isKinematic = true;
        vidaSlider.gameObject.SetActive(false);
        kamikaze = false;
        EventDispatcher.RondaTerminada -= DesactivarGlobo;

        if (sliderFlorinda != null)
            sliderFlorinda.gameObject.SetActive(false);

       
        meshGlobo.SetActive(false);


        fmod_globos.Event = explosionAgua_sfx;
        fmod_globos.Play();
        marcoVida.enabled = false;

        explosion_vfx.Play();
        EfectoGolpe();
        QuitarMira();

        yield return new WaitForSeconds(1.0f);

        if (_tipoPersonaje == TipoPersonaje.doñaFlorinda)
        {
            LanzamientosControl._lanzamientos.conFlorinda = false;
        }
        this.gameObject.SetActive(false);

    }

    public void CambiarGlobo()
    {
        globoChavo.SetActive(false);
        globoKiko.SetActive(false);
        globoÑoño.SetActive(false);
        globoDonRamon.SetActive(false);
        globoPoppy.SetActive(false);
        globoDoñaFlorinda.SetActive(false);

        marcoVidax1.enabled = false;
        marcoVidax2.enabled = false;
        marcoVidax3.enabled = false;
        marcoVidax4.enabled = false;

        if(_tipoPersonaje == TipoPersonaje.chavo)
        {
            meshGlobo = globoChavo;
            alturaArco = chavo_s.arco;
            tiempoDeRecorrido = chavo_s.tiempo;
            vida = chavo_s.vidaGlobo;
            dañoJugador = chavo_s.dañoGlobo;
            valorGlobo = chavo_s.valorGlobo;
            marcoVida =  marcoVidax1;
            
        }
        else if(_tipoPersonaje == TipoPersonaje.kiko)
        {
            meshGlobo = globoKiko;
            alturaArco = kiko_s.arco;
            tiempoDeRecorrido = kiko_s.tiempo;
            vida = kiko_s.vidaGlobo;
            dañoJugador = kiko_s.dañoGlobo;
            valorGlobo = kiko_s.valorGlobo;
            marcoVida =  marcoVidax2;

        }
        else if (_tipoPersonaje == TipoPersonaje.poppy)
        {
            meshGlobo = globoPoppy;
            alturaArco = poppy_s.arco;
            tiempoDeRecorrido = poppy_s.tiempo;
            vida = poppy_s.vidaGlobo;
            dañoJugador = poppy_s.dañoGlobo;
            valorGlobo = poppy_s.valorGlobo;
             marcoVida =  marcoVidax1;

        }
        else if (_tipoPersonaje == TipoPersonaje.ñoño)
        {
            meshGlobo = globoÑoño;
            alturaArco = ñoño_s.arco;
            tiempoDeRecorrido = ñoño_s.tiempo;
            vida = ñoño_s.vidaGlobo;
            dañoJugador = ñoño_s.dañoGlobo;
            valorGlobo = ñoño_s.valorGlobo;
            marcoVida =  marcoVidax3;

        }
        else if (_tipoPersonaje == TipoPersonaje.donRamon)
        {
            meshGlobo = globoDonRamon;
            alturaArco = donRamon_s.arco;
            tiempoDeRecorrido = donRamon_s.tiempo;
            vida = donRamon_s.vidaGlobo;
            dañoJugador = donRamon_s.dañoGlobo;
            valorGlobo = donRamon_s.valorGlobo;
            marcoVida =  marcoVidax1;

        }
        else if (_tipoPersonaje == TipoPersonaje.doñaFlorinda)
        {
            meshGlobo = globoDoñaFlorinda;
            alturaArco = doñaFlorinda_s.arco;
            tiempoDeRecorrido = doñaFlorinda_s.tiempo;
            vida = doñaFlorinda_s.vidaGlobo;
            dañoJugador = doñaFlorinda_s.dañoGlobo;
            valorGlobo = doñaFlorinda_s.valorGlobo;
            marcoVida =  marcoVidax4;


        }
        else if(_tipoPersonaje == TipoPersonaje.miniFlorinda)
        {
            meshGlobo = globoMini;
            alturaArco = miniFlorinda_s.arco;
            tiempoDeRecorrido = miniFlorinda_s.tiempo;
            vida = miniFlorinda_s.vidaGlobo;
            dañoJugador = miniFlorinda_s.dañoGlobo;
            valorGlobo = miniFlorinda_s.valorGlobo;

        }
        marcoVida.enabled = false;
        vidaInicial = vida;
    }


    public void GloboEnMira()
    {
        if (enMira)
            return;

       
        enMira = true;
        lockedImg.gameObject.SetActive(true);
        //Aqui se activarian los efectos de que esta lockedo
    }


    public void QuitarMira()
    {
        enMira = false;
        lockedImg.gameObject.SetActive(false);
    }


    void ActivarDebuffDonRamon()
    {
        if (globosRamon.Length <= 0)
            return;

         foreach(GameObject gr in globosRamon)
         {
            gr.transform.position = this.transform.position;
            gr.transform.parent = null;
            gr.SetActive(true);
            gr.GetComponent<DonRamon_Debuff>().ActivarDebuff();

         }

    }


    void EfectoGolpe()
    {
        int r = Random.Range(0, golpes_vfx.Length);
        golpes_vfx[r].Play();
    }


}
