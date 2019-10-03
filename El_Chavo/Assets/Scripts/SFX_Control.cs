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

    [SerializeField]private StudioEventEmitter golpeA_Personaje;
    [SerializeField]private StudioEventEmitter golpeDe_Personaje;
    [SerializeField] bool reproduciendoA;
    [SerializeField]bool reproduciendoDE;
    
    [Space(10)]
    [Header("Golpes a Personajes")]
    [EventRef]public string golpeA_DonRamon_sfx;
    [EventRef] public string golpeA_chavo;
    [EventRef] public string golpeA_quico;
    [EventRef] public string golpeA_poppis;
    [EventRef] public string golpeA_ñoño;
    [EventRef] public string golpeA_DoñaFlorinda;

    [Space(10)]
    [Header("Golpes de Personajes")]
    [EventRef] public string golpeDe_DonRamon_sfx;
    [EventRef] public string golpeDe_chavo;
    [EventRef] public string golpeDe_quico;
    [EventRef] public string golpeDe_poppis;
    [EventRef] public string golpeDe_ñoño;
    [EventRef] public string golpeDe_DoñaFlorinda;

     
    // Start is called before the first frame update
    void Start()
    {
        sfx_control = this;
    }

    public void PersonajeGolpeado(TipoPersonaje personaje)
    {

        if (reproduciendoA)
            return;

        float proba = Random.Range(0.0f, 1.0f);
        //ej: si probabilidad es de 0.9 y proba es menor entonces 
        //hay un chance muy grande de que si se reproduzca
        if (proba > probabilidadReproduccion) 
            return;

        string e = "";


        if (personaje == TipoPersonaje.chavo)
        {
            e = golpeA_chavo;
        }
        else if (personaje == TipoPersonaje.kiko)
        {
            e = golpeA_quico;
        }
        else if (personaje == TipoPersonaje.poppy)
        {
            e = golpeA_poppis;

        }
        else if (personaje == TipoPersonaje.ñoño)
        {
            e = golpeA_ñoño;
        }
        else if (personaje == TipoPersonaje.donRamon)
        {
            e = golpeA_DonRamon_sfx;
        }
        else if (personaje == TipoPersonaje.doñaFlorinda)
        {
            e = golpeA_DoñaFlorinda;
        }



        var dialogueInstance = RuntimeManager.CreateInstance(e);

        dialogueInstance.start();
        reproduciendoA = true;
        StartCoroutine(PararAudiosPersonaje());
      
    }

    public void JugadorGolpeado(TipoPersonaje personaje)//Cuando un globo golpea al Jugador
    {
        if (reproduciendoDE)
            return;

        float proba = Random.Range(0.0f, 1.0f);
        //ej: si probabilidad es de 0.9 y proba es menor entonces 
        //hay un chance muy grande de que si se reproduzca
        if (proba > probabilidadReproduccion)
            return;

        if(golpeDe_Personaje.IsPlaying())
        {
            print("Hay un personaje hablando...cancelando");
            return;
        }
        

        string e = "";


        if (personaje == TipoPersonaje.chavo)
        {
           e = golpeDe_chavo;
        }
        else if (personaje == TipoPersonaje.kiko)
        {
            e = golpeDe_quico;
        }
        else if (personaje == TipoPersonaje.poppy)
        {
            e = golpeDe_poppis;
        }
        else if (personaje == TipoPersonaje.ñoño)
        {
            e = golpeDe_ñoño;
        }
        else if (personaje == TipoPersonaje.donRamon)
        {
            e = golpeDe_DonRamon_sfx;
        }
        else if (personaje == TipoPersonaje.doñaFlorinda)
        {
            e = golpeDe_DoñaFlorinda;
        }



        var dialogueInstance = RuntimeManager.CreateInstance(e);
        dialogueInstance.start();
      
        reproduciendoDE = true;
        StartCoroutine(PararAudioJugador());

    }


    /// <summary>
    /// De Personaje Golpeado()
    /// </summary>
    IEnumerator PararAudiosPersonaje()
    {
        yield return new WaitForSeconds(2.0f);

        reproduciendoA = false;
    }

    /// <summary>
    /// De JugadorGolpeado()
    /// </summary>
    IEnumerator PararAudioJugador()
    {
        yield return new WaitForSeconds(3.0f);
        reproduciendoDE = false;
    }  
    
}
