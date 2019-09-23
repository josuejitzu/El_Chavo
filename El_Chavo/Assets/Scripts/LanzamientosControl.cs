using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanzamientosControl : MonoBehaviour
{
    public static LanzamientosControl _lanzamientos;
    // Start is called before the first frame update
    public Lanzador_Globos[] lanzadores;
    public Transform[] posiciones;
    public int posicionAnterior;
    public bool conFlorinda;//sabemos si hay un globo de doña Florinda en escena, pues solo debe haber uno
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
        PrepararLanzadores();
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
    void PrepararLanzadores()
    {
        foreach (Lanzador_Globos l in lanzadores)
        {
            l.SpawnGlobo();
            l.sliderDisparo.value = 0.0f;

        }
       
        foreach (Lanzador_Globos lanzador in lanzadores)
        {
            if (lanzador.gameObject.activeInHierarchy)
                lanzador.gameObject.SetActive(false);
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
        if (!MasterLevel.masterlevel.jugando)
            return;

        int r = RandomLanzador();

        if (lanzadores[r].disparando || lanzadores[r].gameObject.activeInHierarchy)//Hay que checar si no conviene mejor saber si esta activo
        {
          //  sigDisparo = Time.time + RandomRate();
            Invoke("SeleccionarLanzador", 0.5f);//Al parece aqui tenemos un problema cuando mandamos a llamar tan rapido a la misma funcion, por eso le puse un delay
            print("No se encontro lanzador libre...buscando otro...");
            return;

        }else if(lanzadores[r]._tipoPersonaje == TipoPersonaje.doñaFlorinda && conFlorinda)
        {
            SeleccionarLanzador();
            return;
        }

        int posSeleccionada = PosicionRandom();
        lanzadores[r].transform.position = posiciones[posSeleccionada].position;
        lanzadores[r].transform.rotation =  posiciones[posSeleccionada].rotation;
        //Al lanzador se le asigna un
        lanzadores[r].GetComponent<Lanzador_Globos>().posicion_elegida = posiciones[posSeleccionada].GetComponent<PosicionControl>();
        //lanzadores[r].gameObject.SetActive(true);
        lanzadores[r].OrdenDisparo();


        //Si spawneamos a Doña Florinda 
        if(lanzadores[r]._tipoPersonaje == TipoPersonaje.doñaFlorinda)
        {
            conFlorinda = true;
        }
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

        while (r == personajeAnterior)
        {
            r = Random.Range(0, maxLanzadores);
        }


        float probabilidad = Random.Range(0.0f, 1.0f);
        if (probabilidad > 0.5) //%50 percent chance
        {//code here
           // print("Probabilidad de 50%: " + probabilidad);
        }

        if (probabilidad > 0.2) //%80 percent chance (1 - 0.2 is 0.8)
        { //code here
            //print("Probabilidad de 80%: " + probabilidad);
        }

        if (probabilidad> 0.7) //%30 percent chance (1 - 0.7 is 0.3)
        { //code here
           // print("Probabilidad de 30%: " + probabilidad);
        }
       

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

        personajeAnterior = r;

        return p;

    } ///NO SE ESTA UTLIZANDO

    public void DesactivarLanzadores()
    {
        foreach(Lanzador_Globos l in lanzadores)
        {
            if(l.gameObject.activeInHierarchy)
            {
               StartCoroutine(l.DesactivarLanzador());
            }
        }
    }
}
