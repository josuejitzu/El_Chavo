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

        while(lanzadores[r].disparando)
        {
            r = Random.Range(0, 3);
        }
        StartCoroutine( lanzadores[r].ComenzarDisparo());

        //foreach(Lanzador_Globos lg in lanzadores)
        //{
        //    if(!lg.disparando)
        //    {
        //        StartCoroutine(lg.ComenzarDisparo());
        //        break;
        //    }
        //}
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

}
