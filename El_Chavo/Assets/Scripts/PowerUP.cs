﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FMODUnity;

public class PowerUP : MonoBehaviour
{
    // Start is called before the first frame update
    public MunicionTipo _tipoMunicion;
    public BoxCollider trigger;
    public GameObject meshExplosivo, meshAutonoma, meshAutomatica;
    public float tiempoDesactivacion = 5.0f;
    public Slider slideTiempo;
   public bool contarTiempo;
    public TMP_Text tipo_text;
    [Header("SFX")]
    public StudioEventEmitter powerActivo_sfx;
   

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
        {
            slideTiempo.value = Mathf.MoveTowards(slideTiempo.value, 0.0f, Time.deltaTime);
            if(slideTiempo.value <= 0.1f)
            {
                StartCoroutine(ConteoDesactivacion());
                contarTiempo = false;
            }
        }
    }

    public void ActivarTablero()
    {
        //Animacion?
        this.gameObject.SetActive(true);
        powerActivo_sfx.Play();
        trigger.enabled = true;
        contarTiempo = true;
        slideTiempo.maxValue = tiempoDesactivacion;
        slideTiempo.value = tiempoDesactivacion;
      
        print("Tablero " + this.transform.name + " activado");

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "municion")
        {
          StartCoroutine(ActivarPowerUP());
        }

    }
    /// <summary>
    /// Activa el PowerUP correspondiente mandando la orden a Resortera_Control.cs
    /// y de igual manera empieza el conteo de desactivacion de PowerUp_Control.cs
    /// </summary>
    /// <returns></returns>
    IEnumerator ActivarPowerUP()//Comunicarse con Resrotera_Control ActivarPowerUP
    {
        print("Se dio en el tablero, activando power UP: " + _tipoMunicion.ToString());
        contarTiempo = false;
        powerActivo_sfx.Stop();

        trigger.enabled = false;
        //if(_tipoMunicion == MunicionTipo.Automatica)
        //{
        //    yield break;
        //}
        Resortera_Control._resortera.ActivarPowerUp(_tipoMunicion);
        //Animacion salida?
        StartCoroutine(PowerUp_Control._powerUps.DesactivacionPowerUP());
        yield return new WaitForSeconds(1.5f);
        this.gameObject.SetActive(false);

    }

    IEnumerator ConteoDesactivacion()
    {

        //animacion salida
        contarTiempo = false;
        powerActivo_sfx.Stop();

        yield return new WaitForSeconds(1.0f);
        PowerUp_Control._powerUps.comboLetrero.SetActive(false);
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
            tipo_text.text = "Explosivo";
            this.transform.name = "PowerUP_Explosivo";
        }
        else if (_tipoMunicion == MunicionTipo.Automatica)
        {
            meshAutomatica.SetActive(true);
            tipo_text.text = "Automatica";
            this.transform.name = "PowerUP_Automatica";
        }
        else if (_tipoMunicion == MunicionTipo.Autonoma)
        {

            meshAutonoma.SetActive(true);
            tipo_text.text = "Autonoma";
            this.transform.name = "PowerUP_Autonoma";
        }
    }
}
