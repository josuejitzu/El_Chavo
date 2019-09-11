using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp_Control : MonoBehaviour
{
    // Start is called before the first frame update
    public static PowerUp_Control _powerUps;

    public PowerUP explosivaPU, autonomaPU, automaticaPU;
    public PowerUP puSeleccionado;
    public Transform[] posicionesPU;
    int posAnterior;

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
            ActivarPowerUp();

        }

    }

    public void ActivarPowerUp()
    {
        SeleccionarPU();//lo regresa a puSeleccionado;
        int posElegida = PosRand();
        puSeleccionado.transform.position = posicionesPU[posElegida].position;
        puSeleccionado.transform.rotation = posicionesPU[posElegida].rotation;
        puSeleccionado.gameObject.SetActive(true);
        puSeleccionado.ActivarTablero();
        print("Se activo un power up...esperando a jugador para recolectarlo");
    }

    void SeleccionarPU()
    {


        float probabilidad = Random.Range(0.0f, 1.0f);
        int r = Random.Range(0, 3);
        if (probabilidad > 0.5) //%50 percent chance
        {
            print("Probabilidad de 50%: " + probabilidad +" automatica");
           
            puSeleccionado = automaticaPU;
            return;
        }

         if (probabilidad > 0.2) //%80 percent chance (1 - 0.2 is 0.8)
        {
            print("Probabilidad de 80%: " + probabilidad + " explosiva");
         
            puSeleccionado = explosivaPU;
            return;
        }

         if (probabilidad > 0.7) //%30 percent chance (1 - 0.7 is 0.3)
        {
            print("Probabilidad de 30%: " + probabilidad + " autonoma");
           
            puSeleccionado = autonomaPU;
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

}
