using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUP : MonoBehaviour
{
    // Start is called before the first frame update
    public MunicionTipo _tipoMunicion;
    public BoxCollider trigger;
    public GameObject meshExplosivo, meshAutonoma, meshAutomatica;
    public float tiempoDesactivacion = 5.0f;
    public Slider slideTiempo;
   public bool contarTiempo;

    private void OnValidate()
    {
        CambiarMesh();
    }
    void Start()
    {
        
    }
    private void Update()
    {
        if (contarTiempo)
            slideTiempo.value = Mathf.MoveTowards(slideTiempo.value, 0.0f, Time.deltaTime); 
    }
    public void ActivarTablero()
    {
        //Animacion?
        //this.gameObject.SetActive(true);
        trigger.enabled = true;
        StartCoroutine(ConteoDesactivacion());
        slideTiempo.maxValue = tiempoDesactivacion;
        slideTiempo.value = tiempoDesactivacion;
        contarTiempo = true;
        print("Tablero " + this.transform.name + " activado");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "municion")
        {
          StartCoroutine(ActivarPowerUP());
        }

    }
    IEnumerator ActivarPowerUP()//Comunicarse con Resrotera_Control ActivarPowerUP
    {
        trigger.enabled = false;
        if(_tipoMunicion == MunicionTipo.Automatica)
        {
            yield break;
        }
        Resortera_Control._resortera.ActivarPowerUp(_tipoMunicion);
        //Animacion salida?
        yield return new WaitForSeconds(1.5f);
        this.gameObject.SetActive(false);

    }

    IEnumerator ConteoDesactivacion()
    {
        yield return new WaitForSeconds(tiempoDesactivacion);
        contarTiempo = false;
        this.gameObject.SetActive(false);
        print("Tablero " + this.transform.name + " desactivado por tiempo");
    }

    void CambiarMesh()
    {
        meshExplosivo.SetActive(false);
        meshAutomatica.SetActive(false);
        meshAutonoma.SetActive(false);
        if (_tipoMunicion == MunicionTipo.Explosiva)
        {
            meshExplosivo.SetActive(true);
            this.transform.name = "PowerUP_Explosivo";
        }
        else if (_tipoMunicion == MunicionTipo.Automatica)
        {
            meshAutomatica.SetActive(true);
            this.transform.name = "PowerUP_Automatica";
        }
        else if (_tipoMunicion == MunicionTipo.Autonoma)
        {
            meshAutonoma.SetActive(true);
            this.transform.name = "PowerUp_Autonoma";
        }
    }
}
