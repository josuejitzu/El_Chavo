using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanzamientosControl : MonoBehaviour
{
    // Start is called before the first frame update
    public Lanzador_Globos[] lanzadores;

    [Space(10)]
    [Header("Tiempos")]
    public float rateDisparo;
    public float minRate, maxRate;
    float sigDisparo;
    public bool disparar;


    int personajeAnterior = 5;
    public int ronda;

    void Start()
    {
        sigDisparo = Time.time + RandomRate();
    }

    // Update is called once per frame
    void Update()
    {

        if(disparar)
        {
            if(Time.time >= sigDisparo)
            {
                SeleccionarLanzador();
                sigDisparo = Time.time + RandomRate();
            }
        }

    }

    public void SeleccionarLanzador()
    {
        int r = Random.Range(0, 3);

        while(lanzadores[r].disparando)//si esta disparando elige otro, debo suponer que esta activado
        {
            r = Random.Range(0, 3);
        }

        lanzadores[r].gameObject.SetActive(true);
        lanzadores[r].OrdenDisparo(RandomPersonaje());
        //StartCoroutine(lanzadores[r].ComenzarDisparo());

    }

    float RandomRate()
    {
        float r = Mathf.Floor(Random.Range(minRate, maxRate));

        while(r == rateDisparo)
        {
            r = Mathf.Floor(Random.Range(minRate, maxRate));
        }

        rateDisparo = r;

        return r;
    }
    
    TipoPersonaje RandomPersonaje()
    {
        int r = 0;
        if(ronda == 1)
        {
            r = Random.Range(0, 2);


            while (r == personajeAnterior)
            {
                r = Random.Range(0, 2);
            }
        }
     
        TipoPersonaje p = TipoPersonaje.chavo;

        if (r == 0) { p = TipoPersonaje.chavo;}
        else if( r == 1) { p = TipoPersonaje.kiko; }
        else if( r == 2) { p = TipoPersonaje.poppy; }
        else if( r == 3) { p = TipoPersonaje.donRamon; }
        else if( r == 4) { p = TipoPersonaje.ñoño; }
        else if( r == 5) { p = TipoPersonaje.doñaFlorinda; }


        return p;

    }

}
