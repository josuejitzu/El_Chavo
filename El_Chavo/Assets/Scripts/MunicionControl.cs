using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class MunicionControl : MonoBehaviour
{
    public MunicionTipo _tipoMunicion;
    public Rigidbody rigid;
    [Header("Settings Municion")]
    public Municion normal;
    public Municion explosiva, autonoma;
    [Space(10)]
    public int daño;

    [Header("MeshMunicion")]
    public GameObject mesh;
    [Space(10)]
    [Header("Municion Autonoma")]
    public MunicionAutonoma[] municionesAutonomas;
   [Space(10)]
   [Header("VFX")]
    public ParticleSystem explosion_vfx;
    public ParticleSystem explosionPesada_vfx;


    [Space(10)]
    [Header("SFX")]
    public StudioEventEmitter woosh_sfx;

    public void ActivarMuncion()
    {
        // mesh.SetActive(true);
        CambiarMunicion();
    }
  

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "globo")
        {
            if(other.GetComponent<GloboControl>() != null)//para los mini globos de Florinda porque no tienen este script
                 other.GetComponent<GloboControl>().RecibirDaño(daño);

            StartCoroutine(Explotar());
        }
        else if (other.transform.tag == "MainCamera" || other.transform.tag == "municion" 
              || other.transform.tag == "municionAutonoma")
        {
            return;
        }
        else if (other.transform.tag == "mano" 
              || other.transform.tag == "tirante" || other.transform.tag == "resortera")
        {
            return;
        }
        else if(other.transform.tag == "posicionDebuff")//Si le da a los debuffs de donRamon
        {
            StartCoroutine(PowerUp_Control._powerUps.CancelarPowerUP());
        }
        else if (other.transform.tag == "personaje")
        {
            //Por alguna extraña razon trababa el reinicio del Personaje atorando la Courutine peor no daba error
            //StartCoroutine(other.GetComponent<Lanzador_Globos>().Lanzador_Golpeado());
            StartCoroutine(Explotar());

        }
        else
        {
            StartCoroutine(Explotar());
        }
    }

    IEnumerator Explotar()
    {
        explosion_vfx.Play();

        if (_tipoMunicion == MunicionTipo.Explosiva)
            explosionPesada_vfx.Play();

        rigid.isKinematic = true;
        mesh.SetActive(false);
        ActivarTrail(false);
        yield return new WaitForSeconds(1.0f);
        woosh_sfx.Stop();
        this.gameObject.SetActive(false);
    }

    public void CambiarMunicion()
    {
        normal.mesh.SetActive(false);
        explosiva.mesh.SetActive(false);
        autonoma.mesh.SetActive(false);

        if (_tipoMunicion == MunicionTipo.Normal)
        {
            mesh = normal.mesh;
            daño = normal.daño;
        }
        else if(_tipoMunicion == MunicionTipo.Explosiva)
        {
            mesh = explosiva.mesh;
            daño = explosiva.daño;///OJO Tiene que ser mayor al del globo mas fuerte
        }
        else if (_tipoMunicion == MunicionTipo.Autonoma)
        {
            mesh = autonoma.mesh;
            daño = autonoma.daño;
            ActivarMunicionAutonoma();
        }

        mesh.SetActive(true);
    }

    public void ActivarMunicionAutonoma()//La llama CambiarMunicion()
    {
        foreach (MunicionAutonoma ma in municionesAutonomas)
        {
            ma._padre = mesh.transform;
            ma.transform.position = mesh.transform.position;
            ma.Reiniciar();
            ma.gameObject.SetActive(true);
          //  ma.EscanearZona();

        }

    }
    public IEnumerator DisparoAutonomo()//Lamada en ResorteraControl().DispararAutomatica()
    {
        foreach (MunicionAutonoma ma in municionesAutonomas)
        {
            ma.transform.parent = null;
            ma.Lanzar();
        }
        mesh.SetActive(false);
       // this.gameObject.SetActive(false);
        //Esperamos un poco para que sus proyectiles den en el blanco y se puedan reiniciar, 
        //de lo contrario va a estar el error de que se desactivan
        yield return new WaitForSeconds(3.0f);
        foreach (MunicionAutonoma ma in municionesAutonomas)
        {
            if(ma.objetivo != null)
               ma.objetivo.GetComponent<GloboControl>().QuitarMira();

            ma.gameObject.SetActive(false);
        }
        this.gameObject.SetActive(false);
    }
    public void ActivarTrail(bool estado)
    {
        if(_tipoMunicion == MunicionTipo.Normal)
        {
            normal.trail.enabled = estado;
        }
        else if(_tipoMunicion == MunicionTipo.Explosiva)
        {
            explosiva.trail.enabled = estado;
        }
    }
}
