using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MunicionAutonoma : MonoBehaviour
{
    public Transform _padre;

    public bool conObjetivo;
    private int interval = 5;
    public LayerMask layerIgnorar;
    public Transform objetivo;
    // Start is called before the first frame update
    public bool disparar;
    public float velocidad;
    public int daño;
    public ParticleSystem explosion_vfx;
    public GameObject mesh;

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
            EscanearZona();
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
    public void EscanearZona()
    {
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 50.0f);
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
        objetivo.GetComponent<GloboControl>().QuitarMira();
        mesh.SetActive(true);
        GetComponent<SphereCollider>().enabled = true;
        disparar = true;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "globo")
        {
            other.GetComponent<GloboControl>().RecibirDaño(daño);
            StartCoroutine(Explotar());
        }
        else if (other.transform.tag == "MainCamera" || other.transform.tag =="municion")
        {
            return;
        }
        else if (other.transform.tag == "mano" || other.transform.tag == "tirante" || other.transform.tag == "resortera")
        {

        }
        else if (other.transform.tag == "personaje")
        {
          //  StartCoroutine(other.GetComponent<Lanzador_Globos>().Lanzador_Golpeado());

        }
        else
        {
            print(this.transform.name + " choco con " + other.transform.name);
            StartCoroutine(Explotar());
        }
    }
    IEnumerator Explotar()
    {
        if (objetivo != null)
            objetivo.GetComponent<GloboControl>().QuitarMira();

        explosion_vfx.Play();
        mesh.SetActive(false);
        GetComponent<SphereCollider>().enabled = false;
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
      
          

        mesh.SetActive(false);
        GetComponent<SphereCollider>().enabled = false;
        objetivo = null;
        disparar = false;
        conObjetivo = false;
        yield return new WaitForSeconds(1.0f);
        this.transform.position = _padre.transform.position;
        this.transform.parent = _padre.transform;
        this.gameObject.SetActive(false);
    }

    public void Reiniciar()
    {
        mesh.SetActive(true);
        GetComponent<SphereCollider>().enabled = false;
        objetivo = null;
        disparar = false;
        conObjetivo = false;
    }
}
