using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
public class PowerUp_Control : MonoBehaviour
{
    // Start is called before the first frame update
    public static PowerUp_Control _powerUps;

    public PowerUP explosivaPU, autonomaPU, automaticaPU;
    public PowerUP puSeleccionado;
    public Transform[] posicionesPU;
    int posAnterior;
    public float tiempoDesactivacion = 10.0f;
    public Slider slidePower;
    public bool conteoPowerUp;

    [Space(10)]
    [Header("SFX")]
    public StudioEventEmitter powerUpActivado_sfx;
    public StudioEventEmitter powerUpCancelado_sfx;
    public StudioEventEmitter powerUpDesactivado_sfx;


    private void OnDrawGizmos()
    {
      
       
       Gizmos.color = new Color(1.0f,0.0f,0.0f,0.5f);
        for (int i = 0; i < posicionesPU.Length; i++)
        {
            Gizmos.DrawCube(posicionesPU[i].position, posicionesPU[i].localScale);
            posicionesPU[i].transform.name = "PosicionPU_" + i;
        }    
        
    }

    void Start()
    {
        _powerUps = this;


        explosivaPU._tipoMunicion = MunicionTipo.Explosiva;
        autonomaPU._tipoMunicion = MunicionTipo.Autonoma;
        automaticaPU._tipoMunicion = MunicionTipo.Automatica;


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            // ActivarPowerUp();
            Resortera_Control._resortera.ActivarPowerUp(MunicionTipo.Explosiva);
            StartCoroutine(DesactivacionPowerUP());
        }
        if(conteoPowerUp)
        {
            slidePower.value = Mathf.MoveTowards(slidePower.value, 0.0f, Time.deltaTime);
            if(slidePower.value <= 0)
            {
                conteoPowerUp = false;
                slidePower.gameObject.SetActive(false);
                Resortera_Control._resortera.ActivarPowerUp(MunicionTipo.Normal);
                print("Se pidio desactivar powerUP");
            }

        }
    }

    
    /// <summary>
    /// Activa las tabla con un Power Up aleatorio para que el jugador pueda
    /// darle y activar ese mismo PowerUP
    /// </summary>
    public void ActivarPowerUp()
    {
        SeleccionarPU();//lo regresa a puSeleccionado;
        int posElegida = PosRand();
        puSeleccionado.transform.position = posicionesPU[posElegida].position;
        puSeleccionado.transform.rotation = posicionesPU[posElegida].rotation;
        puSeleccionado.gameObject.SetActive(true);
        puSeleccionado.ActivarTablero();
        print("Se activo un power up...esperando a jugador para recolectarlo");
      //  StartCoroutine(DesactivacionPowerUP());
    }

    void SeleccionarPU()
    {


        float probabilidad = Random.Range(0.0f, 1.0f);
        int r = Random.Range(0, 3);


        //Tiene que ser de la menor probabilidad a la mayor probabilidad para que no se repita
        if (probabilidad > 0.7) //%30 percent chance (1 - 0.7 is 0.3)
        {
            print("Probabilidad de 30%: " + probabilidad + " autonoma");

            puSeleccionado = autonomaPU;
            return;
        }
        else if (probabilidad > 0.5) //%50 percent chance
        {
            print("Probabilidad de 50%: " + probabilidad +" automatica");
           
            puSeleccionado = automaticaPU;
            return;
        }
        else if (probabilidad > 0.2) //%80 percent chance (1 - 0.2 is 0.8)
        {
            print("Probabilidad de 80%: " + probabilidad + " explosiva");
         
            puSeleccionado = explosivaPU;
            return;
        }



    }

    int PosRand()
    {
        int r = Random.Range(0, posicionesPU.Length);
        while(r == posAnterior)
        {
            r = Random.Range(0, posicionesPU.Length);

        }
        posAnterior = r;
        return r;
    }

    /// <summary>
    /// Desactiva el PowerUp por vencmiento de Tiempo
    /// </summary>
    /// <returns></returns>
    public IEnumerator DesactivacionPowerUP()
    {
        powerUpActivado_sfx.Play();
        print("se pido activar el PowerUp");
        slidePower.gameObject.SetActive(true);
        slidePower.maxValue = tiempoDesactivacion;
        slidePower.value = tiempoDesactivacion;
        conteoPowerUp = true;
        yield return new WaitForSeconds(tiempoDesactivacion);
        powerUpDesactivado_sfx.Play();
        conteoPowerUp = false;
        slidePower.gameObject.SetActive(false);
        Resortera_Control._resortera.ActivarPowerUp(MunicionTipo.Normal);
        print("Se pidio desactivar powerUP por Tiempo");

    }
    /// <summary>
    /// Se llama con el globo de Don Ramon o Sus Debufs para quitarle
    /// el powerUp al Jugador
    /// </summary>
    /// <returns></returns>
    public IEnumerator CancelarPowerUP()
    {
        if (!conteoPowerUp)
            yield break;

        conteoPowerUp = false;
        slidePower.gameObject.SetActive(false);
        Resortera_Control._resortera.ActivarPowerUp(MunicionTipo.Normal);
        print("Se pidio desactivar powerUP porque Don Ramon te golpeo o porque golpeaste Debuff");
        powerUpCancelado_sfx.Play();

        yield break;

    }
}
