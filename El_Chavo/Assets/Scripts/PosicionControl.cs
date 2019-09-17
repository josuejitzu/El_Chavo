using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosicionControl : MonoBehaviour
{

    public Animator puertaVentana_anim;//animator de la puerta o ventana que tenga que abrir
    public bool ocupada;//Para que LanzamientosControl.cs sepa que no puede elegir esta posicion porque esta ocupada
    //SFX de puerta o ventana abriendose/cerrandose

    // Start is called before the first frame update
    void Start()
    {
        
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
        puertaVentana_anim.SetTrigger("abrir");
        yield break;
   }
    public IEnumerator Cerrar()
    {
        if (puertaVentana_anim != null)
        {
            //animacion de cerrado
            puertaVentana_anim.SetTrigger("cerrar");

        }

        yield return new WaitForSeconds(1.0f);//duracion de la animacion
        ocupada = false;
    }


}
