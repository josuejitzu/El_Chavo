using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class Lanzador_Globos : MonoBehaviour
{
    // Start is called before the first frame update
    public TipoPersonaje _tipoPersonaje;

    public Rigidbody rigid;
    public BoxCollider colider;
  
    [Header("Globos")]
    public GameObject globo_prefab;
    public int cantidad;
    public List<GameObject> globos = new List<GameObject>();
    public GameObject globo_temp;
    [Space(10)]
    [Header("Personajes")]
    public GameObject chavo;
    public GameObject kiko, ñoño, poppy, doñaFlorinda, donRamon;
    public GameObject personajeActivo;
    [Space(10)]
    [Header("Settings Disparo")]
    public float velocidad;
    public GameObject objetivo;
    public float velocidadRotacion;
    public Vector3 velocidadCalculada;
    public float anguloLanzamiento;
    public Transform posLanzamiento;

    public float delayDisparo;
    public float rateDisparo;
    float sigDisparo;

    public bool disparando;

    public float tiempoEsperaDisparo;//tiene que ser igual al maximo valor del slider
    float esperandoDisparo;
    public bool esperandoLanzamiento;
    public Slider sliderDisparo;
    [Space(10)]
    [Header("VFX")]
    public ParticleSystem golpe_vfx;


    private void OnValidate()
    {
        CambiarPersonaje(_tipoPersonaje);
    }

    void Start()
    {

        SpawnGlobo();
       // print(Physics.gravity.y);

       // CalcularFuerza();
        sigDisparo = Time.time + rateDisparo;
       // sliderDisparo.value = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.L))
        {
            OrdenDisparo();
        }
        //if(Time.time >= sigDisparo)
        //{
        //    //Disparar();
        //    //sigDisparo = Time.time + rateDisparo;
        //}
        
        if(objetivo)
        {

        
            Vector3 dist = objetivo.transform.position - this.transform.position;
            transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(dist), Time.deltaTime * velocidadRotacion);

        }



        if(esperandoLanzamiento)
        {
            sliderDisparo.value = Mathf.MoveTowards(sliderDisparo.value, tiempoEsperaDisparo, Time.deltaTime);
            if(sliderDisparo.value >= 2.99f)
            {
                esperandoLanzamiento = false;
                //print("Se lleno la barra,lanzando");
                //Disparar();
            }
        }


    }
    public void SpawnGlobo()
    {

        for (int i = 0; i < cantidad; i++)
        {
            GameObject globo = Instantiate(globo_prefab, this.transform.position, Quaternion.identity) as GameObject;
            globo.transform.name =  "globo_" + this.transform.name + i;
            globo.SetActive(false);
            globo.GetComponent<GloboControl>()._tipoPersonaje = _tipoPersonaje;
            globo.GetComponent<GloboControl>().CambiarGlobo();
            MasterLevel.masterlevel.RegistrarUpdate("globo",globo);
            globos.Add(globo);

        }

       // ActivarGlobo();

    }

    public void ActivarGlobo()
    {
        foreach(GameObject g in globos)
        {
            if(!g.activeInHierarchy)
            {
                g.transform.position = posLanzamiento.position;
                g.transform.rotation = posLanzamiento.rotation;

                g.SetActive(true);
                // g.GetComponent<Rigidbody>().isKinematic = true;
                g.GetComponent<GloboControl>().colision.enabled = false;

                globo_temp = g;
                //globo_temp.GetComponent<GloboControl>()._tipoPersonaje = _tipoPersonaje;
                //globo_temp.GetComponent<GloboControl>().CambiarGlobo();
                
                break;

            }

        }
    }

    public void CalcularFuerza()
    {
        Vector3 proyectilPosXZ = new Vector3(transform.position.x, 0.0f, transform.position.z);
        Vector3 objetivoPosXZ = new Vector3(objetivo.transform.position.x, 0.0f, objetivo.transform.position.z);

    

       //anguloLanzamiento = objetivoPosXZ.y;//esto funciona pero es mejor darselo en el Inspector para que lance en parabola


        float r = Vector3.Distance(proyectilPosXZ, objetivoPosXZ);
        float g = Physics.gravity.y;
        float tanAlpha = Mathf.Tan(anguloLanzamiento * Mathf.Deg2Rad);
        float h = objetivo.transform.position.y - transform.position.y;


        //float vz = Mathf.Sqrt(g * r * r / (2.0f * (h - r * tanAlpha)));
        float vz = Mathf.Sqrt(g * r * r / (velocidad * (h - r * tanAlpha)));//al parecer tiene que ser 2
        float vy = tanAlpha * vz;

        Vector3 velocidadLocal = new Vector3(0, vy, vz);
        Vector3 velocidadGlobal = transform.TransformDirection(velocidadLocal);

       // print(velocidadGlobal);
        velocidadCalculada = velocidadGlobal;

    }

    public void OrdenDisparo()
    {
        sliderDisparo.value = 0.0f;
        this.gameObject.SetActive(true);
        disparando = true;//este setting lo reviza el manager para saber si este personaje puede lanzar
        colider.enabled = true;
     
        //CambiarPersonaje(personaje);
        //ActivarGlobo(personaje);
        //personajeActivo.SetActive(true);
        //ActivarGlobo();
        StartCoroutine(ComenzarDisparo());
    }
    public IEnumerator ComenzarDisparo()
    {
        esperandoLanzamiento = true; // se utliza para llenar la barra como feedback para dispararte
                                    
        ActivarGlobo();

        if(chavo.activeInHierarchy)
            chavo.GetComponent<Animator>().SetTrigger("preparar");

        yield return new WaitForSeconds(tiempoEsperaDisparo);

        if (chavo.activeInHierarchy)
            chavo.GetComponent<Animator>().SetTrigger("disparar");

        yield return new WaitForSeconds(delayDisparo);//El tiempo que esperamos para que el personaje este en posicion de disparo
        Disparar();

    }

    public void Disparar()
    {
        if (globo_temp == null)
            return;

      
      //  CalcularFuerza();
      //  globo_temp.GetComponent<Rigidbody>().useGravity = true;
        globo_temp.GetComponent<GloboControl>().ActivarGlobo();
        globo_temp.GetComponent<GloboControl>().posFinal = objetivo.transform.position;
        globo_temp.GetComponent<GloboControl>().objetivo = objetivo.transform;
        globo_temp.GetComponent<GloboControl>().brincar = true;
      //  globo_temp.GetComponent<Rigidbody>().isKinematic = false;
        //globo_temp.GetComponent<Rigidbody>().velocity = this.transform.forward * velocidad;
      //  globo_temp.GetComponent<Rigidbody>().velocity = velocidadCalculada;

        globo_temp.GetComponent<GloboControl>().colision.enabled = true;
        StartCoroutine(TerminoDisparo());
    }

    IEnumerator TerminoDisparo()
    {
        yield return new WaitForSeconds(0.5f);//tiempo de terminaicon de abanico
        //iniciar animacion de personaje escondiendose
        if (chavo.activeInHierarchy)
            chavo.GetComponent<Animator>().SetTrigger("agacharse");
        globo_temp = null;
        yield return new WaitForSeconds(1.0f);//lo que dure la animacion de esconido + 0.2f
        //ActivarGlobo
       
        
       // personajeActivo.SetActive(false);
       // personajeActivo = null;
        disparando = false;
        sliderDisparo.value = 0.0f;
       // yield return new WaitForSeconds(0.2f);
        this.gameObject.SetActive(false);
    }

    void CambiarPersonaje(TipoPersonaje t)
    {
        chavo.SetActive(false);
        kiko.SetActive(false);
        ñoño.SetActive(false);
        poppy.SetActive(false);
        donRamon.SetActive(false);
        doñaFlorinda.SetActive(false);


        if (t == TipoPersonaje.chavo)
        {
            personajeActivo = chavo;
            
        }
        else if(t == TipoPersonaje.kiko)
        {
            personajeActivo = kiko;
        }
        else if (t == TipoPersonaje.ñoño)
        {
            personajeActivo = ñoño;
        }
        else if (t == TipoPersonaje.poppy)
        {
            personajeActivo = poppy;
        }
        else if (t == TipoPersonaje.donRamon)
        {
            personajeActivo = donRamon;
        }
        else if (t == TipoPersonaje.doñaFlorinda)
        {
            personajeActivo = doñaFlorinda;
        }

        personajeActivo.SetActive(true);
        this.transform.name = t.ToString();
    }

    public IEnumerator Lanzador_Golpeado()
    {
        StopCoroutine(ComenzarDisparo());
        disparando = false;
        colider.enabled = false;

        if (globo_temp != null)
        {

            globo_temp.SetActive(false);
            globo_temp = null;
        }
      
        MasterLevel.masterlevel.ScoreJugador(10);
        golpe_vfx.Play();
        //animacion de golpe

        yield return new WaitForSeconds(1.0f);


        this.gameObject.SetActive(false);

    }


    public void DesactivarLanzador()
    {
        this.StopAllCoroutines();
        disparando = false;
        if(globo_temp != null)
        {
            globo_temp.SetActive(false);
            globo_temp = null;
        }
        foreach(GameObject g in globos)
        {
            if (g.activeInHierarchy)
                g.SetActive(false);
        }

        this.gameObject.SetActive(false);

    }
}
//https://vilbeyli.github.io/Projectile-Motion-Tutorial-for-Arrows-and-Missiles-in-Unity3D/