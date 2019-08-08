using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MasterLevel : MonoBehaviour
{
    public static MasterLevel masterlevel;
    [Header("Vida Jugador")]
    public float vidaMax = 200.0f;
    public float vidaJugador = 0.0f;
    public Slider vidaSlider;
    public float velocidadLlenado;

    [Space(10)]
    [Header("Rondas")]
    public int rondaNum;
    public Rondas[] rondas;

    [Space(10)]
    [Header("KEY")]
    public KeyCode teclaIniciar;

    void Start()
    {
        masterlevel = this;  
    }

    // Update is called once per frame
    void Update()
    {

        vidaSlider.value = Mathf.MoveTowards(vidaSlider.value, vidaJugador,Time.deltaTime * velocidadLlenado);
        if(Input.GetKey(teclaIniciar))
        {
           StartCoroutine( IniciarJuego());
        }


    }

    public IEnumerator IniciarJuego()
    {

        LanzamientosControl._lanzamientos.PrepararRound();
        yield return new WaitForSeconds(0.5f);
        LanzamientosControl._lanzamientos.disparar = true;

    }

    public void DañarJugador(float cantidad)
    {
        if (vidaJugador >= vidaMax)
            return;


        vidaJugador += cantidad;

    }
}
