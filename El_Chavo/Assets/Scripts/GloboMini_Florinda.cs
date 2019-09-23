using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GloboMini_Florinda : MonoBehaviour
{
  
    public enum GloboColor { rosa,azul,verde};
    public GloboColor _globoColor;
    public TipoPersonaje _tipoPersonaje = TipoPersonaje.miniFlorinda;
    [Header("Meshes")]
    public GameObject meshActiva;
    public GameObject globoRosa, globoAzul, globoVerde;

    [Space(10)]
    public SphereCollider trigger;
    public Rigidbody rigid;
    public ParticleSystem explosion_vfx;
    [Space(10)]
    [Header("Settings Lanzamiento")]
    public float alturaArco = 5.0f;
    public float tiempoDeRecorrido = 2.0f;
    public bool brincar;
    public float timer = 0.0f;
    public Transform objetivo;
    public Transform posInicial;
    public Vector3 posFinal;
    public int valorGlobo;
    public Vector3 posFlorinda;

    [Space(10)]
    [Header("Vida")]
    public Slider vidaSlider;
    public int vida;
    public int vidaInicial;
    public float dañoJugador;
    public TMP_Text puntos_text;
    [Space(10)]
    [Header("Target Autonoma")]
    public bool enMira;
    public Image lockedImg;

    private void OnValidate()
    {
        CambiarMesh();
    }
    void Update()
    {

        if (brincar)
        {
            Lanzando();
        }

    }

    public void MiUpdate()
    {
        if(brincar)
        {
            Lanzando();
        }
       
    }

    public void Lanzando()
    {
        if (timer <= 1.0f)
        {

            float altura = Mathf.Sin(Mathf.PI * timer) * alturaArco;
            transform.position = Vector3.Lerp(posInicial.position, posFinal, timer) + Vector3.up * altura;
            timer += Time.deltaTime / tiempoDeRecorrido;
          //  meshActiva.transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(posFinal), Time.deltaTime * 5.0f);


        }
        else if (timer >= 1.0f)
        {
            brincar = false;
        }
    }

    public void ActivarGlobo()
    {
        timer = 0.0f;
       // posInicial.position = this.transform.position;//asignarla manualmente para el reinicio
        gameObject.transform.parent = null;
        MasterLevel.masterlevel.RegistrarUpdate("globo", this.gameObject);
        meshActiva.SetActive(true);
        trigger.enabled = true;
       // CambiarMesh();
     
        vidaSlider.gameObject.SetActive(true);
        vida = vidaInicial;
        vidaSlider.maxValue = vida;
        vidaSlider.value = vida;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "personaje" || other.transform.tag == "globo" || other.transform.tag == "paredes")
        {

        }else if(other.transform.tag == "municion" || other.transform.tag == "municionAutonoma")
        {
            StartCoroutine(Destruir());

        }
        else if (other.transform.tag == "MainCamera")
        {
            MasterLevel.masterlevel.DañarJugador(dañoJugador);
            StartCoroutine(Explotar());
            print(other.transform.name);

        }
        else
        {
            StartCoroutine(Explotar());
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.transform.tag == "MainCamera")
        {
            MasterLevel.masterlevel.DañarJugador(dañoJugador);
            StartCoroutine(Explotar());
            print(other.transform.name);

        }
    }



    public IEnumerator Explotar()//Cuando choca con algo, incluido el jugador pero no cuenta los puntos eso lo hace en TriggerEnter
    {

        brincar = false;
        trigger.enabled = false;
        //rigid.isKinematic = true;
        vidaSlider.gameObject.SetActive(false);
        meshActiva.SetActive(false);
        MasterLevel.masterlevel.RemoverUpdate(this.gameObject, "globo");

        explosion_vfx.Play();


        yield return new WaitForSeconds(1.0f);

        
        QuitarMira();
        this.gameObject.SetActive(false);
    }
    IEnumerator Destruir()//Cuando la destruye el Jugador
    {
        trigger.enabled = false;
        brincar = false;
        //rigid.isKinematic = true;
        vidaSlider.gameObject.SetActive(false);
        meshActiva.SetActive(false);

        MasterLevel.masterlevel.RemoverUpdate(this.gameObject, "globo");

        MasterLevel.masterlevel.ScoreJugador(valorGlobo);

        puntos_text.text = "+" + valorGlobo.ToString();
        puntos_text.gameObject.SetActive(true);
        this.transform.LookAt(posFinal);

        yield return new WaitForSeconds(1.5f);
        puntos_text.gameObject.SetActive(false);



        QuitarMira();
        this.gameObject.SetActive(false);

    }



    void CambiarMesh()
    {
        globoVerde.SetActive(false);
        globoRosa.SetActive(false);
        globoAzul.SetActive(false);

        if (_globoColor == GloboColor.rosa)
        {
            meshActiva = globoRosa;
        }
        else if(_globoColor == GloboColor.azul)
        {
            meshActiva = globoAzul;
        }
        else if(_globoColor == GloboColor.verde)
        {
            meshActiva = globoVerde;
        }

        meshActiva.SetActive(true);
    }

    public void DesactivarGlobo()
    {
        StopAllCoroutines();
        brincar = false;
        trigger.enabled = false;
        this.gameObject.SetActive(false);
    }

    public void RegresarAPosicion()
    {
        this.transform.localPosition = posInicial.localPosition;
      //  meshActiva.transform.rotation = Quaternion.Euler(0.0f,0.0f,0.0f);
        meshActiva.SetActive(true);
        this.gameObject.SetActive(true);
        brincar = false;
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
        if(lockedImg != null)
        lockedImg.gameObject.SetActive(false);
    }


}
