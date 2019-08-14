﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class MasterLevel : MonoBehaviour
{
    public static MasterLevel masterlevel;
    public bool jugando;
    [Header("Vida Jugador")]
    public float vidaMax = 200.0f;
    public float vidaJugador = 0.0f;
    public Slider vidaSlider;
    public float velocidadLlenado;

    [Space(10)]
    [Header("Rondas")]
    public int rondaNum;
    public Rondas[] rondas;
    public TMP_Text ronda_text;
    public TMP_Text siguienteRonda_text;
    public TMP_Text rondaFinal_text;

    [Space(10)]
    [Header("Tiempo")]
    public float tiempoJuego;
    public bool contando;
    public float tiempoEspera = 5.0f;

    [Space(10)]
    [Header("Scores")]
    public int scoreJugador;
    int scoreLerp;
    public TMP_Text scoreEnRonda_text;
    public TMP_Text scoreFinal_text;
    public GameObject canvasJuego;

    public GameObject canvasSiguienteRonda;
    public GameObject canvasFinJuego;

    [Space(10)]
    [Header("KEY")]
    public KeyCode teclaIniciar;

    [Space(10)]
    [Header("UpdateScripts")]
    public List<GloboControl> globosUpdate = new List<GloboControl>();

    private void Awake()
    {
        masterlevel = this;
    }
    void Start()
    {
     //   masterlevel = this;  
    }

    // Update is called once per frame
    void Update()
    {

        vidaSlider.value = Mathf.MoveTowards(vidaSlider.value, vidaJugador, Time.deltaTime * velocidadLlenado);
        if (Input.GetKey(teclaIniciar))
        {
            StartCoroutine(IniciarJuego());
        }

        if (contando)
        {
            Tiempo();
        }

        for (int i = 0; i < globosUpdate.Count; i++)
        {
            if(globosUpdate[i].gameObject.activeInHierarchy)
                globosUpdate[i].MiUpdate();
        }

    
    }
    private void FixedUpdate()
    {
        if (canvasJuego.activeInHierarchy)
        {
            if (scoreLerp < scoreJugador)
                scoreLerp += 1;

            scoreEnRonda_text.text = scoreLerp.ToString("0000");
        }
    }

    public IEnumerator IniciarJuego()
    {

        LanzamientosControl._lanzamientos.PrepararRound();
        tiempoJuego = rondas[rondaNum].duracion;
        yield return new WaitForSeconds(0.5f);
        jugando = true;
        contando = true;
        LanzamientosControl._lanzamientos.disparar = true;

    }

    public IEnumerator FinRonda()
    {
        jugando = false;
        LanzamientosControl._lanzamientos.disparar = false;
        LanzamientosControl._lanzamientos.DesactivarLanzadores();
        ronda_text.text = (rondaNum + 1).ToString("00");//si es 0 da 1
        canvasJuego.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        canvasJuego.SetActive(false);

        if (rondaNum < rondas.Length)
        {
            rondaNum++;
        }
        else
        {
            print("Error: NO HAY MAS RONDAS");
            yield break;
        }

        siguienteRonda_text.text = (rondaNum + 1).ToString("00");//si da 1 es 2
        canvasSiguienteRonda.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        canvasSiguienteRonda.SetActive(false);

        StartCoroutine(IniciarJuego());
       
    }

    public IEnumerator FinJuego()
    {
        jugando = false;
        contando = false;
        LanzamientosControl._lanzamientos.disparar = false;
        LanzamientosControl._lanzamientos.DesactivarLanzadores();
        yield return new WaitForSeconds(0.05f);
        scoreFinal_text.text = "Score: " + scoreJugador.ToString("0000") + "    " + "Ronda: " + (rondaNum + 1).ToString("00");
        canvasFinJuego.SetActive(true);
    }

    void Tiempo()
    {
        if(tiempoJuego > 0.0f)
        {
            tiempoJuego -= Time.deltaTime;

        }else if(tiempoJuego <= 0.0f)
        {
            contando = false;
            StartCoroutine(FinRonda());
            
        }
    }

    public void DañarJugador(float cantidad)
    {
        if (!jugando)
            return;
        if (vidaJugador >= vidaMax)
        {
            StartCoroutine(FinJuego());
            return;
        }


        vidaJugador += cantidad;

    }
    public void ScoreJugador(int valor)
    {
        if (!jugando)
            return;

        scoreJugador += valor;
        //scoreEnRonda_text.text = scoreJugador.ToString("0000");

    }

    public void RegistrarUpdate(string tipo,GameObject objeto)
    {
        if(tipo == "globo")
        {
            globosUpdate.Add(objeto.GetComponent<GloboControl>());
        }
    }

    public void RemoverUpdate(GameObject objeto, string tipo)
    {
        if(tipo == "globo")
        {
            globosUpdate.Remove(objeto.GetComponent<GloboControl>());
        }
       // globosUpdate.Sort();
    }
}
