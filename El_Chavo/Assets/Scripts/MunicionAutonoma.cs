using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class MunicionAutonoma : MonoBehaviour
{
    public Transform _padre;

    public SphereCollider trigger, zonaBusqueda;
    public bool buscando;

    public bool conObjetivo;
    private int interval = 1;
    public LayerMask layerIgnorar;
    public Transform objetivo;
    // Start is called before the first frame update
    public bool disparar;
    public float velocidad;
    public int daño;
    public ParticleSystem explosion_vfx;
    public GameObject mesh;
    public ParticleSystem smokeVFX;

    [Space(10)]
    [Header("SFX")]
    public StudioEventEmitter sfxEmitter;
    [FMODUnity.EventRef] public string chiflido_sfx;
  

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!conObjetivo)
        {
            //if (Time.frameCount % interval == 0)
            //{
            //    EscanearZona();
            //}
           // EscanearZona();
        }
        else if(conObjetivo)
        {
            Vector3 objetivoDist = objetivo.transform.position - this.transform.position;
            Debug.DrawRay(this.transform.position, objetivoDist,Color.red);
            transform.LookAt(objetivo.position);
        }

        if(Input.GetKeyDown(KeyCode.Y))
        {
            Lanzar();
        }
      //  EscanearZona();
      if(disparar)
      {
            
            this.transform.position = Vector3.MoveTowards(this.transform.position, objetivo.position, Time.deltaTime * velocidad);
      }

    }
    public void EscanearZona()//NO SE ESTA UTILIZANDO
    {
        return;
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 70.0f);
        int i = 0;

         while (i < hitColliders.Length)
         {
                    if (hitColliders[i].tag == "globo")
                    {
                        if (!hitColliders[i].GetComponent<GloboControl>().enMira)
                        {
                            hitColliders[i].GetComponent<GloboControl>().GloboEnMira();
                            objetivo = hitColliders[i].transform;
                            conObjetivo = true;
                            print(this.transform.name+" encontro globo: "+ objetivo.transform.name + i);
                            break;
                        }

                    }
                    else if(hitColliders[i].tag == "personaje")
                    {
                      if(!hitColliders[i].GetComponent<Lanzador_Globos>().enMira)
                        {
                            hitColliders[i].GetComponent<Lanzador_Globos>().ActivarMira();
                            objetivo = hitColliders[i].transform;
                            conObjetivo = true;
                            print(this.transform.name + " encontro globo: " + objetivo.transform.name + i);
                            break;
                        }
                     
                    }
               i++;
         }
      
    }

    public void Lanzar()
    {
        if(objetivo == null)//Cuando se lancen si alguno no tiene objetivo que se desactive
        {
            StartCoroutine(Desactivar());
           
            return;
        }
     //   objetivo.GetComponent<GloboControl>().QuitarMira();
        mesh.SetActive(true);
        smokeVFX.Play();
        GetComponent<SphereCollider>().enabled = true;
        disparar = true;
        sfxEmitter.Event = chiflido_sfx;
        sfxEmitter.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (buscando)
            return;

        if (other.transform.tag == "globo")
        {
            other.GetComponent<GloboControl>().RecibirDaño(daño);
            StartCoroutine(Explotar());
        }
        else if (other.transform.tag == "MainCamera" || other.transform.tag =="municion" 
             || other.transform.tag =="municionAutonoma")
        {
            return;
        }
        else if (other.transform.tag == "mano" || other.transform.tag == "tirante" || other.transform.tag == "resortera")
        {

        }
        else if (other.transform.tag == "personaje")
        {
            //StartCoroutine(other.GetComponent<Lanzador_Globos>().Lanzador_Golpeado());//quitar si no queemos dañar al personaje
            StartCoroutine(Explotar());
        }
        else
        {
            print(this.transform.name + " choco con " + other.transform.name);
            StartCoroutine(Explotar());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!buscando)
            return;

        if (other.transform.tag == "globo")
        {
            if (other.GetComponent<GloboControl>().enMira)
                return;

        
            objetivo = other.transform;
            objetivo.GetComponent<GloboControl>().GloboEnMira();
            buscando = false;
            conObjetivo = true;
            zonaBusqueda.enabled = false;
            trigger.enabled = true;

        }
        else if( other.transform.tag == "personaje")
        {
            if (other.GetComponent<Lanzador_Globos>().enMira)
                return;

            objetivo = other.transform;
            objetivo.GetComponent<Lanzador_Globos>().ActivarMira();
            buscando = false;
            conObjetivo = true;
            zonaBusqueda.enabled = false;
            trigger.enabled = true;
        }
    }
    IEnumerator Explotar()
    {
        QuitarMira();
        if (sfxEmitter.IsPlaying())
            sfxEmitter.Stop();
        explosion_vfx.Play();
        mesh.SetActive(false);
        smokeVFX.Stop();
        trigger.enabled = false;
        zonaBusqueda.enabled = false;
        objetivo = null;
        disparar = false;
        conObjetivo = false;
    
        yield return new WaitForSeconds(1.0f);
        this.transform.position = _padre.transform.position;
        this.transform.parent = _padre.transform;
        this.gameObject.SetActive(false);
    }
    IEnumerator Desactivar()
    {

        if(sfxEmitter.IsPlaying())
            sfxEmitter.Stop();
        mesh.SetActive(false);
        smokeVFX.Stop();
        trigger.enabled = false;
        zonaBusqueda.enabled = false;
        objetivo = null;
        disparar = false;
        conObjetivo = false;

        QuitarMira();

      //  yield return new WaitForSeconds(1.0f);
        this.transform.position = _padre.transform.position;
        this.transform.parent = _padre.transform;
        this.gameObject.SetActive(false);
        yield break;
    }

    public void Reiniciar()
    {
        mesh.SetActive(false);
        trigger.enabled = false;
        zonaBusqueda.enabled = true;
        QuitarMira();
        objetivo = null;
        disparar = false;
        conObjetivo = false;
        buscando = true;

    }

    public void QuitarMira()
    {
        if (objetivo != null)
        {
            if(objetivo.GetComponent<GloboControl>())
              objetivo.GetComponent<GloboControl>().QuitarMira();
            if(objetivo.GetComponent<Lanzador_Globos>())
                objetivo.GetComponent<Lanzador_Globos>().QuitarMira();


        }


    }
}
