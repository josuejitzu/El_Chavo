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
    public Animator mano_anim;

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

    [Space(5)]
    [SerializeField] private float duracionVibracion = 1.0f;
    [SerializeField] private float frequencia_hz = 150;
    [SerializeField] private float intensidad = 0.5f;
    [SerializeField] private float duracionVibracion_daño = 1.0f;
    [SerializeField] private float frequencia_hz_daño = 150;
    [SerializeField] private float intensidad_daño = 0.5f;

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
    public bool puedenVibrar;

    void Start()
    {

        puedeDisparar = true;

        EventDispatcher.JugadorGolpeado += VibrarControl;
    }


  
    void Update()
    {

        triggerPresion = squeezeAction.GetAxis(control.inputSource);


        if (triggerPresion >= sensibilidadTrigger)//Apretado
        {

   
           // print(control.inputSource.ToString() + "presionando");
            if (sobreResortera)
            {
                TomarResortera();
            }
            if (sobreTirante)
            {
                estirando = true;
                manoContraria.GetComponent<ManoControl>().resortera.GetComponent<Resortera_Control>().MoverTirante(separacion * 1.3f);
                if (puedenVibrar)
                {
                    vibracion.Execute(0.0f, duracionVibracion, frequencia_hz, intensidad, control.inputSource);
                    manoContraria.GetComponent<ManoControl>().VibrarControlEstirado();
                }
                mano_anim.SetTrigger("municion");

            }else
            {
                mano_anim.SetTrigger("resortera");

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
                //vibracion.Execute(0.0f, duracionVibracion, frequencia_hz, intensidad, control.inputSource);

            }


        }


        if(triggerPresion < sensibilidadTrigger)//Soltar
        {

            //print(control.inputSource.ToString() + "soltado");
            if (conResortera)
            {
             

            }
            else if (estirando)//Si estaba estirando significa que solto la liga
            {
                if (!puedeDisparar)
                    return;

                manoContraria.GetComponent<ManoControl>().resortera.GetComponent<Resortera_Control>().multiplicadorFuerza = separacion;// * 2.0f;
                manoContraria.GetComponent<ManoControl>().resortera.GetComponent<Resortera_Control>().estirando = false;
                manoContraria.GetComponent<ManoControl>().resortera.GetComponent<Resortera_Control>().Disparar();
                sobreTirante = false;
                mano_anim.SetTrigger("abrir");

                separacion = 0.0f;
                estirando = false;
                puedeDisparar = false;
                Invoke("ActivarDisparo", delayDisparo);

            }
            if (!conResortera)
            {
                mano_anim.SetTrigger("abrir");
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

            vibracion.Execute(0.0f, duracionVibracion, 70, intensidad, control.inputSource);

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

    public void TomarResortera()
    {

        if (!sobreResortera)
            return;

        if (resortera.GetComponent<Resortera_Control>().enMano)
            return;
        mano_anim.SetTrigger("resortera");

        resortera.GetComponent<Resortera_Control>().mano = this;
        resortera.GetComponent<Resortera_Control>().ResorteraTomada();
        
        resortera.transform.position = posResortera.position;
        resortera.transform.rotation = posResortera.rotation;
        resortera.transform.parent = this.transform;

        trigger.enabled = false;

        conResortera = true;

    }

    public void SoltarResortera()
    {
        if (!conResortera)
            return;

        resortera.GetComponent<Resortera_Control>().mano = null;
        resortera.GetComponent<Resortera_Control>().ResorteraSoltada();
       // resortera.GetComponent<Resortera_Control>().ResorteraTomada();

        
        resortera.transform.parent = null;

        trigger.enabled = true;

        conResortera = false;

    }


    void TomarGlobo()
    {
        if (!sobreGlobo)
            return;

    }

    public void VibrarControl()
    {

        vibracion.Execute(0.0f, duracionVibracion_daño, frequencia_hz_daño, intensidad_daño, control.inputSource);

    }

    public void VibrarControlEstirado()
    {
        vibracion.Execute(0.0f, duracionVibracion, frequencia_hz, intensidad, control.inputSource);

    }

    public void VibracionActivada()
    {
        puedenVibrar = !puedenVibrar;
    }
}
