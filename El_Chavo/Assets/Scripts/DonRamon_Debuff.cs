using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonRamon_Debuff : MonoBehaviour
{
    //public float rangoX, rangoY, rangoZ;
    //public float rangoMinX, rangoMinY, rangoMinZ;
    //public Vector3 posZero;
    public Vector3 posFinal;
    public float tiempoMovimiento;
    public float velocidad;
    public bool mover;
    public Transform padre;
    public float tiempoVida;

    public GameObject meshCalavera;

    Vector3 randomRotacion;
    public float velocidadRotacion = 2.0f;

    void Start()
    {
        //EncontrarPosicion();
        //Invoke("Reiniciar", tiempoVida);
        ActivarDebuff();
    }
  
    // Update is called once per frame
    void Update()
    {
        if(mover)
        {
            Mover();
        }
    }


    public void ActivarDebuff()
    {
        EventDispatcher.RondaTerminada += Reiniciar;
        randomRotacion = RandomAxis();
        EncontrarPosicion();

        Invoke("Reiniciar", tiempoVida);
    }
    public void EncontrarPosicion()
    {

        //float x = Mathf.Clamp( Random.Range(posZero.x - rangoX, rangoX),rangoMinX,rangoX); 
        //float y = Mathf.Clamp(Random.Range(posZero.y - rangoY, rangoY), rangoMinY, rangoY);
        //float z = Mathf.Clamp(Random.Range(posZero.z - rangoZ, rangoZ), rangoMinZ, rangoZ);
        //float x = Random.Range(posZero.x - rangoX, rangoX); 
        //float y = Random.Range(posZero.y - rangoY, rangoY);
        //float z = Random.Range(posZero.z - rangoZ, rangoZ);
        //posFinal = new Vector3( x, y, z);
        randomRotacion = RandomAxis();


        int r = Random.Range(0, Spawn_Sphere._PosicionesEsfera.puntos.Count);
        posFinal =   Spawn_Sphere._PosicionesEsfera.puntos[r].position;
        mover = true;
    }

    public void Mover()
    {
        transform.position = Vector3.MoveTowards(this.transform.position, posFinal, Time.deltaTime * velocidad);
        Vector3 dis = posFinal - this.transform.position;
        // meshCalavera.transform.Rotate(Vector3.right * (Time.deltaTime * velocidadRotacion));
        meshCalavera.transform.Rotate(randomRotacion * (Time.deltaTime * velocidadRotacion));
      
        if (dis.magnitude <= 0.2f)
        {
            mover = false;
            EncontrarPosicion();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "paredes")
        {
            print(this.transform.name +" choco con pared: " + other.transform.name);
            mover = false;
            EncontrarPosicion();
        }
    }

    public void Reiniciar()
    {
        if(padre != null)
           this.transform.parent = padre.transform;

        mover = false;
        EventDispatcher.RondaTerminada -= Reiniciar;

        this.gameObject.SetActive(false);

    }


    Vector3 RandomAxis()
    {
        Vector3 v = Vector3.right;
        int r = Random.Range(0, 3);
        if(r == 0)
        {
            v = Vector3.right;
        }
        else if(r == 1)
        {
            v = Vector3.up;
        }
        else if(r== 2)
        {
            v = Vector3.back;
        }
        return v;

    }
}
