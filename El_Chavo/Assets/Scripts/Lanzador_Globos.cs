using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Lanzador_Globos : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Globos")]
    public GameObject globo_prefab;
    public int cantidad;
    public List<GameObject> globos = new List<GameObject>();
    public GameObject globo_temp;
    [Space(10)]
    [Header("Settings Disparo")]
    public float velocidad;
    public GameObject objetivo;
    public float velocidadRotacion;
    public Vector3 velocidadCalculada;
    public float anguloLanzamiento;

    public float delayDisparo;
    public float rateDisparo;
    float sigDisparo;

    void Start()
    {

        SpawnGlobo();
       // print(Physics.gravity.y);

        CalcularFuerza();
        sigDisparo = Time.time + rateDisparo;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.L))
        {
            Disparar();
        }
        if(Time.time >= sigDisparo)
        {
            Disparar();
            sigDisparo = Time.time + rateDisparo;
        }
        
        if(objetivo)
        {

        
            Vector3 dist = objetivo.transform.position - this.transform.position;
            transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(dist), Time.deltaTime * velocidadRotacion);
        }

    }
    public void SpawnGlobo()
    {

        for (int i = 0; i < cantidad; i++)
        {
            GameObject globo = Instantiate(globo_prefab, this.transform.position, Quaternion.identity) as GameObject;
            globo.transform.name =  "globo_" + this.transform.name + i;
            globo.SetActive(false);
            globos.Add(globo);

        }

        ActivarGlobo();

    }

    public void ActivarGlobo()
    {
        foreach(GameObject g in globos)
        {
            if(!g.activeInHierarchy)
            {
                g.transform.position = this.transform.position;
                g.transform.rotation = this.transform.rotation;

                g.SetActive(true);
                g.GetComponent<Rigidbody>().isKinematic = true;
                g.GetComponent<SphereCollider>().enabled = false;

                globo_temp = g;
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
        float vz = Mathf.Sqrt(g * r * r / (velocidad * (h - r * tanAlpha)));
        float vy = tanAlpha * vz;

        Vector3 velocidadLocal = new Vector3(0, vy, vz);
        Vector3 velocidadGlobal = transform.TransformDirection(velocidadLocal);

       // print(velocidadGlobal);
        velocidadCalculada = velocidadGlobal;

    }


    public IEnumerator ComenzarDisparo()
    {

      //reproducir animacion
        yield return new WaitForSeconds(delayDisparo);//El tiempo que esperamos para que el personaje este en posicion de disparo
        Disparar();

    }

    public void Disparar()
    {
        if (globo_temp == null)
            return;

        CalcularFuerza();

        globo_temp.GetComponent<GloboControl>().ActivarGlobo();
        globo_temp.GetComponent<Rigidbody>().isKinematic = false;
        //globo_temp.GetComponent<Rigidbody>().velocity = this.transform.forward * velocidad;
        globo_temp.GetComponent<Rigidbody>().velocity = velocidadCalculada;

        globo_temp.GetComponent<SphereCollider>().enabled = true;
        Invoke("ActivarGlobo", 0.2f);

    }
}
//https://vilbeyli.github.io/Projectile-Motion-Tutorial-for-Arrows-and-Missiles-in-Unity3D/