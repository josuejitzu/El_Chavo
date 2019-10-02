using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonRamon_Debuff : MonoBehaviour
{
    //public float rangoX, rangoY, rangoZ;
    //public float rangoMinX, rangoMinY, rangoMinZ;
    //public Vector3 posZero;
    public SphereCollider trigger;
    public Vector3 posFinal;
    public float tiempoMovimiento;
    public float velocidad;
    public bool mover;
    public Transform padre;
    public float tiempoVida;

    public GameObject meshCalavera;
    [SerializeField]private ParticleSystem explosion_vfx;
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
        meshCalavera.SetActive(true);

        EventDispatcher.RondaTerminada += Reiniciar;
        EventDispatcher.DebuffActivado += Reiniciar;
        randomRotacion = RandomAxis();
        EncontrarPosicion();

        StartCoroutine(ActivarTrigger());
        
    }
    /// <summary>
    /// Se activa el trigger un segundo despues para que el jugador no desactive 
    /// inmediatamente el PowerUP debido a que se comparte el Tag
    /// </summary>
    /// <returns></returns>
    IEnumerator ActivarTrigger()
    {
        yield return new WaitForSeconds(1.0f);
        trigger.enabled = true;
        yield return new WaitForSeconds(tiempoVida);
        Reiniciar();
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
        

        
        mover = false;
        EventDispatcher.RondaTerminada -= Reiniciar;
        trigger.enabled = false;
        StartCoroutine(SecuenciaReinicio());
       

    }

    IEnumerator SecuenciaReinicio()
    {
        meshCalavera.SetActive(false);
        explosion_vfx.Play();

        yield return new WaitForSeconds(1.0f);
        if (padre != null)
            this.transform.parent = padre.transform;

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
