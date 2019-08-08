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
   

    public bool estirando;


    void Start()
    {
        
    }


  
    void Update()
    {

        triggerPresion = squeezeAction.GetAxis(control.inputSource);

    
        if(triggerPresion >= sensibilidadTrigger)//Apretado
        {

            print(control.inputSource.ToString() + "presionando");
            if(sobreResortera)
            {
                TomarResortera();
            }
            else if(sobreGlobo)
            {

            }
            else if(sobreTirante)
            {
                estirando = true;
                manoContraria.GetComponent<ManoControl>().resortera.GetComponent<Resortera_Control>().MoverTirante(separacion * 2);
    
            }

        }


        if(triggerPresion < sensibilidadTrigger)//Soltar
        {

            //print(control.inputSource.ToString() + "soltado");
            if (conResortera)
            {

            }
            else if (conGlobo)
            {

            }
            else if (estirando)
            {

                manoContraria.GetComponent<ManoControl>().resortera.GetComponent<Resortera_Control>().multiplicadorFuerza = separacion * 2;
                manoContraria.GetComponent<ManoControl>().resortera.GetComponent<Resortera_Control>().estirando = false;
                manoContraria.GetComponent<ManoControl>().resortera.GetComponent<Resortera_Control>().Disparar();
               

                estirando = false;
            }
               
        }


        if(estirando)
        {
            // manoContraria.GetComponent<ManoControl>().resortera.GetComponent<Resortera_Control>().Disparar();
            if (manoContraria != null)
            {
                Vector3 dist = manoContraria.position - this.transform.position;
                separacion = dist.magnitude;
            }

        }


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
            sobreResortera = true;
            resortera = other.gameObject;
        }
        else if(other.transform.tag == "globo")
        {
            sobreGlobo = true;
            globoTemp = other.gameObject;
        }
        else if(other.transform.tag == "tirante" && !conResortera)
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
        else if (other.transform.tag == "globo")
        {
            sobreGlobo = false;
            globoTemp = null;
        }
        else if(other.transform.tag == "tirante")
        {
            sobreTirante = false;
        }

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
