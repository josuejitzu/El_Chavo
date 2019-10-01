using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class SFX_Control : MonoBehaviour
{

    public static SFX_Control sfx_control;

    [Range(0.1f,1.0f)]
    [Tooltip("Probabilidad de 0.0 a 1.0 de que suene la voz de DonRamon")]
    public float probabilidadReproduccion;

    [Header("Golpes a Personajes")]
    [SerializeField] private StudioEventEmitter golpeA_DonRamon_sfx;
    [SerializeField] private StudioEventEmitter golpeA_chavo;
    [SerializeField] private StudioEventEmitter golpeA_quico;
    [SerializeField] private StudioEventEmitter golpeA_poppis;
    [SerializeField] private StudioEventEmitter golpeA_ñoño;
    [SerializeField] private StudioEventEmitter golpeA_DoñaFlorinda;

    [Space(10)]
    [Header("Golpes de Personajes")]
    [SerializeField] private StudioEventEmitter golpeDe_DonRamon_sfx;
    [SerializeField] private StudioEventEmitter golpeDe_chavo;
    [SerializeField] private StudioEventEmitter golpeDe_quico;
    [SerializeField] private StudioEventEmitter golpeDe_poppis;
    [SerializeField] private StudioEventEmitter golpeDe_ñoño;
    [SerializeField] private StudioEventEmitter golpeDe_DoñaFlorinda;

    // Start is called before the first frame update
    void Start()
    {
        sfx_control = this;
    }

    public void PersonajeGolpeado(TipoPersonaje personaje)
    {

        float proba = Random.Range(0.0f, 1.0f);
        //ej: si probabilidad es de 0.9 y proba es menor entonces 
        //hay un chance muy grande de que si se reproduzca
        if (proba > probabilidadReproduccion) 
            return;


        if (personaje == TipoPersonaje.chavo)
        {
            golpeA_chavo.Play();
        }
        else if(personaje == TipoPersonaje.kiko)
        {
            golpeA_quico.Play();
        }
        else if (personaje == TipoPersonaje.poppy)
        {
            golpeA_poppis.Play();

        }
        else if (personaje == TipoPersonaje.ñoño)
        {
            golpeA_ñoño.Play();
        }
        else if (personaje == TipoPersonaje.donRamon)
        {
            golpeA_DonRamon_sfx.Play();
        }
        else if (personaje == TipoPersonaje.doñaFlorinda)
        {
            golpeA_DoñaFlorinda.Play();
        }
    }

    public void JugadorGolpeado(TipoPersonaje personaje)
    {
        float proba = Random.Range(0.0f, 1.0f);
        //ej: si probabilidad es de 0.9 y proba es menor entonces 
        //hay un chance muy grande de que si se reproduzca
        if (proba > probabilidadReproduccion)
            return;

        if (personaje == TipoPersonaje.chavo)
        {
            golpeDe_chavo.Play();
        }
        else if (personaje == TipoPersonaje.kiko)
        {
            golpeDe_quico.Play();
        }
        else if (personaje == TipoPersonaje.poppy)
        {
            golpeDe_poppis.Play();
        }
        else if (personaje == TipoPersonaje.ñoño)
        {
            golpeDe_ñoño.Play();
        }
        else if (personaje == TipoPersonaje.donRamon)
        {
            golpeDe_DonRamon_sfx.Play();
        }
        else if (personaje == TipoPersonaje.doñaFlorinda)
        {
            golpeDe_DoñaFlorinda.Play();
        }
    }

    void Probabilidad()
    {

    }
    
}
