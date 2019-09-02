using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GloboControl : MonoBehaviour
{
    public TipoPersonaje _tipoPersonaje;
    public GloboTipo _tipoGoblo;
    public GameObject globoChavo,globoKiko,globoÑoño,globoDonRamon,globoPoppy,globoDoñaFlorinda,globoMini;
    public GameObject meshGlobo;
    public SphereCollider colision;
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

    [Space(10)]
    [Header("MiniGlobos_Florinda")]
    public GameObject[] globosFlorinda;
    int cantMiniGlobos;
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

    void Start()
    {
        tiempoMini = Time.time + tiempoDisparoMiniGlobos;
    }
  
    public void MiUpdate()
    {

        if (brincar)
        {
            
            Lanzando();
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

        timer = 0.0f;
        if (_tipoPersonaje == TipoPersonaje.miniFlorinda)
        {
            gameObject.transform.parent = null;
            MasterLevel.masterlevel.RegistrarUpdate("globo", this.gameObject);
        }

        if (_tipoPersonaje == TipoPersonaje.doñaFlorinda)
        {
            // vidaSlider.transform.localPosition = new Vector3(-0.5f, 1.50f, 0.0f);
            this.transform.localScale = initSize;
            sliderFlorinda.gameObject.SetActive(true);
            vida = vidaInicial;
            sliderFlorinda.maxValue = vida;
            sliderFlorinda.value = vida;
    
        }
        else
        {          
            vidaSlider.gameObject.SetActive(true);
            vida = vidaInicial;
            vidaSlider.maxValue = vida;
            vidaSlider.value = vida;
        }


        colision.enabled = true;

        if (_tipoPersonaje != TipoPersonaje.miniFlorinda)
            meshGlobo.SetActive(true);


         
        posInicial = this.transform.position;

    }
    void Lanzando()
    {
        if (timer <= 1.0f)
        {
            //if(_tipoPersonaje == TipoPersonaje.kiko)
            //{
            //    this.transform.position = Vector3.MoveTowards(this.transform.position, posFinal, Time.deltaTime * 2.0f);
            //    Vector3 dist = posFinal - transform.position;
            //    if(dist.magnitude <= 0.3f)
            //    {
            //        Explotar();
            //    }
            //}

            if(_tipoPersonaje == TipoPersonaje.doñaFlorinda)
            {
                this.transform.position = Vector3.Lerp(posInicial, posFlorinda, timer);
                this.transform.localScale = Vector3.Lerp(initSize, finalSize, timer);
                colision.radius = Mathf.Lerp(1.0f, 5.5f, timer);
                timer += Time.deltaTime / tiempoDeRecorrido;
                Vector3 dist = posFlorinda - posInicial;

                if(dist.magnitude <= 0.2f)
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

                if (_tipoPersonaje == TipoPersonaje.kiko)
                {
                    Vector3 dist = posFinal - transform.position;
                    if (dist.magnitude <= 0.3f)
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

        
                globosFlorinda[cantMiniGlobos].GetComponent<GloboControl>().enabled = true;
                globosFlorinda[cantMiniGlobos].GetComponent<GloboControl>().ActivarGlobo();
                globosFlorinda[cantMiniGlobos].GetComponent<GloboControl>().posFinal = objetivo.transform.position;
               //mg.GetComponent<GloboControl>().objetivo = objetivo;
                globosFlorinda[cantMiniGlobos].GetComponent<GloboControl>().brincar = true;
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
        if (other.transform.tag == "municion" || other.transform.tag == "personaje" || other.transform.tag == "globo")
        {

        }else if(other.transform.tag == "MainCamera")
        {
            MasterLevel.masterlevel.DañarJugador(dañoJugador);
            StartCoroutine(Explotar());
            print(other.transform.name);

        }
        else
        {
            StartCoroutine(Explotar());
        }
        //print(other.transform.name);
    }

    public void RecibirDaño(int cantidad)
    {
        vida -= cantidad;
        vidaSlider.value = vida;

        if(sliderFlorinda != null)
            sliderFlorinda.value = vida;

        QuitarMira();

        if(vida <= 0)
        {
            StartCoroutine(Destruir());
        }
    }

    IEnumerator Destruir()//Cuando la destruye el Jugador
    {
        brincar = false;
        //rigid.isKinematic = true;
        vidaSlider.gameObject.SetActive(false);

        if (sliderFlorinda != null)
            sliderFlorinda.gameObject.SetActive(false);

        MasterLevel.masterlevel.ScoreJugador(valorGlobo);
        colision.enabled = false;



        if (_tipoPersonaje != TipoPersonaje.miniFlorinda)
            meshGlobo.SetActive(false);

        if (_tipoPersonaje == TipoPersonaje.poppy)
        {
            explosionPoppy_vfx.Play();
            yield return new WaitForSeconds(4.0f);

        }
        else
        {
            explosion_vfx.Play();
        }

        if(_tipoPersonaje == TipoPersonaje.donRamon && globosRamon.Length > 0)
        {
           foreach(GameObject gr in globosRamon)
            {
                gr.transform.parent = null;
                gr.SetActive(true);

            }
        }
        yield return new WaitForSeconds(1.5f);
      


        if(_tipoPersonaje == TipoPersonaje.doñaFlorinda)
        {
            LanzamientosControl._lanzamientos.conFlorinda = false;
        }
        lockedImg.gameObject.SetActive(false);
        enMira = false;
        this.gameObject.SetActive(false);

    }

    IEnumerator Explotar()//Cuando choca con algo, incluido el jugador
    {
        brincar = false;
        //rigid.isKinematic = true;
        vidaSlider.gameObject.SetActive(false);

        if (sliderFlorinda != null)
            sliderFlorinda.gameObject.SetActive(false);

        if (_tipoPersonaje != TipoPersonaje.miniFlorinda)
             meshGlobo.SetActive(false);

        explosion_vfx.Play();
        colision.enabled = false;
        
        yield return new WaitForSeconds(1.0f);

        if (_tipoPersonaje == TipoPersonaje.doñaFlorinda)
        {
            LanzamientosControl._lanzamientos.conFlorinda = false;
        }
        lockedImg.gameObject.SetActive(false);
        enMira = false;
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

        if(_tipoPersonaje == TipoPersonaje.chavo)
        {
            meshGlobo = globoChavo;
            alturaArco = chavo_s.arco;
            tiempoDeRecorrido = chavo_s.tiempo;
            vida = chavo_s.vidaGlobo;
            dañoJugador = chavo_s.dañoGlobo;
            valorGlobo = chavo_s.valorGlobo;
        }
        else if(_tipoPersonaje == TipoPersonaje.kiko)
        {
            meshGlobo = globoKiko;
            alturaArco = kiko_s.arco;
            tiempoDeRecorrido = kiko_s.tiempo;
            vida = kiko_s.vidaGlobo;
            dañoJugador = kiko_s.dañoGlobo;
            valorGlobo = kiko_s.valorGlobo;
        }
        else if (_tipoPersonaje == TipoPersonaje.poppy)
        {
            meshGlobo = globoPoppy;
            alturaArco = poppy_s.arco;
            tiempoDeRecorrido = poppy_s.tiempo;
            vida = poppy_s.vidaGlobo;
            dañoJugador = poppy_s.dañoGlobo;
            valorGlobo = poppy_s.valorGlobo;
        }
        else if (_tipoPersonaje == TipoPersonaje.ñoño)
        {
            meshGlobo = globoÑoño;
            alturaArco = ñoño_s.arco;
            tiempoDeRecorrido = ñoño_s.tiempo;
            vida = ñoño_s.vidaGlobo;
            dañoJugador = ñoño_s.dañoGlobo;
            valorGlobo = ñoño_s.valorGlobo;
        }
        else if (_tipoPersonaje == TipoPersonaje.donRamon)
        {
            meshGlobo = globoDonRamon;
            alturaArco = donRamon_s.arco;
            tiempoDeRecorrido = donRamon_s.tiempo;
            vida = donRamon_s.vidaGlobo;
            dañoJugador = donRamon_s.dañoGlobo;
            valorGlobo = donRamon_s.valorGlobo;
        }
        else if (_tipoPersonaje == TipoPersonaje.doñaFlorinda)
        {
            meshGlobo = globoDoñaFlorinda;
            alturaArco = doñaFlorinda_s.arco;
            tiempoDeRecorrido = doñaFlorinda_s.tiempo;
            vida = doñaFlorinda_s.vidaGlobo;
            dañoJugador = doñaFlorinda_s.dañoGlobo;
            valorGlobo = doñaFlorinda_s.valorGlobo;

        }else if(_tipoPersonaje == TipoPersonaje.miniFlorinda)
        {
            meshGlobo = globoMini;
            alturaArco = miniFlorinda_s.arco;
            tiempoDeRecorrido = miniFlorinda_s.tiempo;
            vida = miniFlorinda_s.vidaGlobo;
            dañoJugador = miniFlorinda_s.dañoGlobo;
            valorGlobo = miniFlorinda_s.valorGlobo;

        }
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
}
