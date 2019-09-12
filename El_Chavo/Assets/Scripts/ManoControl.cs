using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;




public class ManoControl : MonoBehaviour
{
    // Start is called before the first frame update
    public enum TipoMano {izquierda,derecha};
    public TipoMano _tipoMano;
    public Rigidbody rigid;
    public BoxCollider trigger;
    public Transform manoContraria;
    public float separacion;

    [Space(5)]
    public SteamVR_Behaviour_Pose control;
    public SteamVR_Action_Single squeezeAction;
    public SteamVR_Action_Vector2 touchPadAction;
    public SteamVR_Action_Boolean presionado;
    public SteamVR_Action_Boolean Grip;
    public SteamVR_Action_Boolean norte, sur;
    public SteamVR_Action_Vibration vibracion;
    float triggerPresion;
    public float sensibilidadTrigger;

    [Space(10)]
    public bool sobreResortera;
    public bool conResortera;
    public bool sobreGlobo, conGlobo,sobreTirante;
    public GameObject resortera, globoTemp, globoEnMano;
    public Transform posResortera;


    public bool disparoAutomatico;
    public bool estirando;
    public float delayDisparo = 0.7f;
    public float delayDisparoAutomatico = 0.4f;
    public bool puedeDisparar;

    void Start()
    {

        puedeDisparar = true;


    }


  
    void Update()
    {

        triggerPresion = squeezeAction.GetAxis(control.inputSource);

    
        if(triggerPresion >= sensibilidadTrigger)//Apretado
        {

            print(control.inputSource.ToString() + "presionando");
            if (sobreResortera)
            {
                TomarResortera();
            }
            if (sobreTirante)
            {
                estirando = true;
                manoContraria.GetComponent<ManoControl>().resortera.GetComponent<Resortera_Control>().MoverTirante(separacion * 1.3f);

            }
            if (conResortera)
            {
                if (disparoAutomatico && puedeDisparar)
                {
                  //  print("Disaparando automaticamente");
                    resortera.GetComponent<Resortera_Control>().multiplicadorFuerza = 1.5f;
                    resortera.GetComponent<Resortera_Control>().Disparar();
                    puedeDisparar = false;
                    Invoke("ActivarDisparo", delayDisparoAutomatico);
                }
            }

        }


        if(triggerPresion < sensibilidadTrigger)//Soltar
        {

            //print(control.inputSource.ToString() + "soltado");
            if (conResortera)
            {
                //if(disparoAutomatico && puedeDisparar)
                //{
                //    print("Disaparando automaticamente");
                //    resortera.GetComponent<Resortera_Control>().multiplicadorFuerza = 2.0f;
                //    resortera.GetComponent<Resortera_Control>().Disparar();
                //    puedeDisparar = false;
                //    Invoke("ActivarDisparo", delayDisparo);
                //}
            }
            else if (estirando)//Si estaba estirando significa que solto la liga
            {
                if (!puedeDisparar)
                    return;

                manoContraria.GetComponent<ManoControl>().resortera.GetComponent<Resortera_Control>().multiplicadorFuerza = separacion;// * 2.0f;
                manoContraria.GetComponent<ManoControl>().resortera.GetComponent<Resortera_Control>().estirando = false;
                manoContraria.GetComponent<ManoControl>().resortera.GetComponent<Resortera_Control>().Disparar();
                sobreTirante = false;
                separacion = 0.0f;
                estirando = false;
                puedeDisparar = false;
                Invoke("ActivarDisparo", delayDisparo);
            }
               
        }


        if (estirando)
        {
            // manoContraria.GetComponent<ManoControl>().resortera.GetComponent<Resortera_Control>().Disparar();
            if (manoContraria != null)
            {
                Vector3 dist = manoContraria.position - this.transform.position;
                separacion = dist.magnitude;
            }
            else
                separacion = 0.0f;
        }

        Vector3 dist2 = manoContraria.position - this.transform.position;
        separacion = dist2.magnitude;

    }

    private void OnTriggerEnter(Collider other)
    {

        if(other.transform.tag == "resortera")
        {
            
        }
        if(other.transform.tag == "tirante")
        {
            
        }

    }

    private void OnTriggerStay(Collider other)
    {

        if(other.transform.tag == "resortera")
        {
            if (other.GetComponent<Resortera_Control>().enMano == true)
                return;

            sobreResortera = true;
            resortera = other.gameObject;
        }
   
        if(other.transform.tag == "tirante" && !conResortera)
        {
            sobreTirante = true;
        }


    }

    private void OnTriggerExit(Collider other)
    {

        if (other.transform.tag == "resortera")
        {
            sobreResortera = false;
            resortera = null;
        }
        
        if(other.transform.tag == "tirante")
        {
           // sobreTirante = false;
        }

    }


    public void ActivarDisparo()

    {
        puedeDisparar = true;
    }
    void TomarResortera()
    {

        if (!sobreResortera)
            return;
      

        resortera.GetComponent<Resortera_Control>().mano = this;
        resortera.GetComponent<Resortera_Control>().ResorteraTomada();
        
        resortera.transform.position = posResortera.position;
        resortera.transform.rotation = posResortera.rotation;
        resortera.transform.parent = this.transform;

        trigger.enabled = false;

        conResortera = true;

    }


    void TomarGlobo()
    {
        if (!sobreGlobo)
            return;

    }
}
