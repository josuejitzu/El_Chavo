﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanzamientosControl : MonoBehaviour
{
    public static LanzamientosControl _lanzamientos;
    // Start is called before the first frame update
    public Lanzador_Globos[] lanzadores;
    public Transform[] posiciones;
    public int posicionAnterior;
    [Space(10)]
    [Header("Tiempos")]
    public float rateDisparo;
    public float minRate, maxRate;//checar el tiempo que se tarda uno en lanzar para que 
    public int maxLanzadores;
    float sigDisparo;
    public bool disparar;



    int personajeAnterior;
    public int ronda;

    void Start()
    {
        _lanzamientos = this;
        sigDisparo = Time.time + RandomRate();
        foreach(Lanzador_Globos l in lanzadores)
        {
            l.SpawnGlobo();
            l.sliderDisparo.value = 0.0f;
            if (l.gameObject.activeInHierarchy)
                l.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if(disparar)
        {
            if(Time.time >= sigDisparo)
            {
                sigDisparo = Time.time + RandomRate();
                SeleccionarLanzador();
                return;
            }
        }

    }

    public void PrepararRound()
    {
        minRate = MasterLevel.masterlevel.rondas[MasterLevel.masterlevel.rondaNum].minRate;
        maxRate = MasterLevel.masterlevel.rondas[MasterLevel.masterlevel.rondaNum].maxRate;
        maxLanzadores = MasterLevel.masterlevel.rondas[MasterLevel.masterlevel.rondaNum].personajes;
    }

    public void SeleccionarLanzador()
    {
        int r = RandomLanzador();

        if (lanzadores[r].disparando)
        {
            SeleccionarLanzador();
            return;
        }

        int posSeleccionada = PosicionRandom();
        lanzadores[r].transform.position = posiciones[posSeleccionada].position;
        lanzadores[r].transform.rotation =  posiciones[posSeleccionada].rotation;
        //lanzadores[r].gameObject.SetActive(true);
        lanzadores[r].OrdenDisparo();
        //StartCoroutine(lanzadores[r].ComenzarDisparo());

    }
    int PosicionRandom()
    {
        

        int r = Random.Range(0, posiciones.Length);

        while(r == posicionAnterior)
        {
            r = Random.Range(0, posiciones.Length);
        }

        posicionAnterior = r;


        return r;
    }

    int RandomLanzador()
    {
        int r = Random.Range(0, maxLanzadores);

        //while (r == personajeAnterior)
        //{
        //    r = Random.Range(0, lanzadores.Length);
        //}


        personajeAnterior = r;
        return r;
    }

    float RandomRate()
    {
        float r = Mathf.Floor(Random.Range(minRate, maxRate));

        //while(r == rateDisparo)
        //{
        //    r = Mathf.Floor(Random.Range(minRate, maxRate));
        //}

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
