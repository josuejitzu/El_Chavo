using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
public class PosicionControl : MonoBehaviour
{

    public Animator puertaVentana_anim;//animator de la puerta o ventana que tenga que abrir
    public bool ocupada;//Para que LanzamientosControl.cs sepa que no puede elegir esta posicion porque esta ocupada
                        //SFX de puerta o ventana abriendose/cerrandose


    public bool puerta;
    [Space(10)]
    [Header("SFX")]
    public StudioEventEmitter puertaAbriendose_sfx;
    public StudioEventEmitter puertaCerrandose_sfx;
    public StudioEventEmitter ventanaAbriendose_sfx;
    public StudioEventEmitter ventanaCerrandose_sfx;
  
    //[FMODUnity.EventRef] public string puertaAbriendose_sfx;
    //[FMODUnity.EventRef] public string puertaCerrandose_sfx;
    //[FMODUnity.EventRef] public string ventanaAbriendose_sfx;
    //[FMODUnity.EventRef] public string ventanaCerrandose_sfx;

    // Start is called before the first frame update
    void Start()
    {
        EventDispatcher.RondaTerminada += EventDispatcher_RondaTerminada;
    }

    private void EventDispatcher_RondaTerminada()
    {
        StartCoroutine(Cerrar());
    }


    /// <summary>
    /// Activa la secuencia de apertura de la ventana o puerta 
    /// debe ser llamada por LanzamientoControl.cs
    /// Si no hay animacion ignora y hace break
    /// </summary>
    /// <returns></returns>
    public IEnumerator Abrir()
   {
        ocupada = true;
        if (puertaVentana_anim == null)
        {
            print("Esta posicion no necesita abrir Ventana o Puerta");
            yield break;
        }
        //animacion de apertura
        puertaVentana_anim.ResetTrigger("cerrar");
        puertaVentana_anim.SetTrigger("abrir");

        if(puerta)
        {
            puertaAbriendose_sfx.Play();

        }else if(!puerta)
        {
            ventanaAbriendose_sfx.Play();
        }
        yield break;
   }
    public IEnumerator Cerrar()
    {
        if (puertaAbriendose_sfx != null)
        {
            puertaAbriendose_sfx.Stop();
        }
        if (ventanaAbriendose_sfx != null)
        {
            ventanaAbriendose_sfx.Stop();
        }

        if (puertaVentana_anim != null)
        {
            //animacion de cerrado
            puertaVentana_anim.SetTrigger("cerrar");
            if (puerta)
            {
                puertaCerrandose_sfx.Play();
            }
            else if(!puerta)
            {
                ventanaCerrandose_sfx.Play();
            }

        }
    
        yield return new WaitForSeconds(1.0f);//duracion de la animacion
        ocupada = false;
    }


}
