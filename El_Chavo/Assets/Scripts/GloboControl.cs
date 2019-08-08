using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GloboControl : MonoBehaviour
{
    public TipoPersonaje _tipoPersonaje;
    public GloboTipo _tipoGoblo;
    public GameObject globoChavo,globoKiko,globoÑoño,globoDonRamon,globoPoppy,globoDoñaFlorinda;
    public GameObject meshGlobo;
    public SphereCollider colision;
    public Rigidbody rigid;
    public ParticleSystem explosion_vfx;

    [Space(10)]
    [Header("Settings Lanzamiento")]
    public float alturaArco = 5.0f;
    public float tiempoDeRecorrido = 2.0f;
    public bool brincar;
    public float timer = 0.0f;
    public Transform objetivo;
    public Vector3 posInicial, posFinal;

    [Space(10)]
    [Header("Settings Personaje")]
    public SettingLanzamiento chavo_s;
    public SettingLanzamiento kiko_s,poppy_s,ñoño_s,donRamon_s,doñaFlorinda_s;

    [Space(10)]
    [Header("Vida")]
    public Slider vidaSlider;
    public int vida;
    public int vidaInicial;
    public float dañoJugador;


    void Start()
    {
        
    }
    void Update()
    {
      
        if(brincar)
        {
            Lanzando();
        }

    }
    public void ActivarGlobo()
    {
        vida = vidaInicial; 
        vidaSlider.maxValue = vida;
        vidaSlider.value = vida;
        meshGlobo.SetActive(true);
        timer = 0.0f;
        posInicial = this.transform.position;

    }
    void Lanzando()
    {
        if (timer <= 1.0f)
        {
            float altura = Mathf.Sin(Mathf.PI * timer) * alturaArco;
            transform.position = Vector3.Lerp(posInicial, posFinal, timer) + Vector3.up * altura;
            timer += Time.deltaTime / tiempoDeRecorrido;
        }
        else if (timer >= 1.0f)
        {
            brincar = false;

        }
    }
  
    //private void OnCollisionEnter(Collision collision)
    //{

    //    StartCoroutine(Explotar());
    //    print(collision.transform.name);
    //}
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "municion")
        {

        }
        else
        {
            StartCoroutine(Explotar());
        }
        print(other.transform.name);
    }

    public void RecibirDaño(int cantidad)
    {
        vida -= cantidad;
        vidaSlider.value = vida;
        if(vida <= 0)
        {
            StartCoroutine(Destruir());
        }
    }
    IEnumerator Destruir()
    {
        brincar = false;
        //rigid.isKinematic = true;
        meshGlobo.SetActive(false);
        explosion_vfx.Play();
        yield return new WaitForSeconds(1.0f);
        this.gameObject.SetActive(false);
    }
    IEnumerator Explotar()
    {
        brincar = false;
      //rigid.isKinematic = true;
        meshGlobo.SetActive(false);
        explosion_vfx.Play();
        yield return new WaitForSeconds(1.0f);
        MasterLevel.masterlevel.DañarJugador(dañoJugador);
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
        }
        else if(_tipoPersonaje == TipoPersonaje.kiko)
        {
            meshGlobo = globoKiko;
            alturaArco = kiko_s.arco;
            tiempoDeRecorrido = kiko_s.tiempo;
            vida = kiko_s.vidaGlobo;
            dañoJugador = kiko_s.dañoGlobo;
        }
        else if (_tipoPersonaje == TipoPersonaje.poppy)
        {
            meshGlobo = globoPoppy;
            alturaArco = poppy_s.arco;
            tiempoDeRecorrido = poppy_s.tiempo;
            vida = poppy_s.vidaGlobo;
            dañoJugador = poppy_s.dañoGlobo;
        }
        else if (_tipoPersonaje == TipoPersonaje.ñoño)
        {
            meshGlobo = globoÑoño;
            alturaArco = ñoño_s.arco;
            tiempoDeRecorrido = ñoño_s.tiempo;
            vida = ñoño_s.vidaGlobo;
            dañoJugador = ñoño_s.dañoGlobo;
        }
        else if (_tipoPersonaje == TipoPersonaje.donRamon)
        {
            meshGlobo = globoDonRamon;
            alturaArco = donRamon_s.arco;
            tiempoDeRecorrido = donRamon_s.tiempo;
            vida = donRamon_s.vidaGlobo;
            dañoJugador = donRamon_s.dañoGlobo;
        }
        else if (_tipoPersonaje == TipoPersonaje.doñaFlorinda)
        {
            meshGlobo = globoDoñaFlorinda;
            alturaArco = doñaFlorinda_s.arco;
            tiempoDeRecorrido = doñaFlorinda_s.tiempo;
            vida = doñaFlorinda_s.vidaGlobo;
            dañoJugador = doñaFlorinda_s.dañoGlobo;
        }
        vidaInicial = vida;
    }
}
