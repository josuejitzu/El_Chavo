using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using FMODUnity;
public class Resortera_Control : MonoBehaviour
{
    public static Resortera_Control _resortera;
    // Start is called before the first frame update
    public GameObject municion_prefab;
    public List<GameObject> municiones = new List<GameObject>();
    public int cantidad;
    public float fuerza;
    public float multiplicadorFuerza;
    public float fuerzaTotal;
    public ManoControl mano;
    public bool estirando;//debe entrar una de las dos manos para que esto se vuelva TRUE
    public Transform posMunicion;
    public Transform posInicialTirante, posFinalTirante;
    public SkinnedMeshRenderer ligaResortera_blendShape;
    [Space(10)]
    public BoxCollider trigger;
    public BoxCollider triggerTirante;
    public GameObject municionTemp;
    public Animator resortera_anim;

    [Header("PowerUps")]
    public bool explosivo_PU;
    public bool autonoma_PU;
    public bool automatica_PU;
    public GameObject meshNormal, meshAutomatica;
    public Animator automatica_anim;
    public Transform posMunicionAutomatica;
    

    public bool enMano;

    [Space(10)]
    [Header("SFX")]
    
    public  StudioEventEmitter estirandose_sfx;
    public  StudioEventEmitter soltada_sfx;
   

    void Start()
    {
        _resortera = this;
        CrearMunicion();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            Disparar();
        }
        if(mano != null)
        {
           // multiplicadorFuerza = mano.separacion * 2;//busca la separacion en ManoControl.cs de la mano que tiene agarrado esta resortera
            //fuerzaTotal = fuerza * multiplicadorFuerza;
        }
         if(!estirando)
         {
            MoverTirante(0);
         }
       if(Input.GetKeyDown(KeyCode.C))
       {
            ActivarPowerUp(MunicionTipo.Explosiva);
       }
       if(Input.GetKeyDown(KeyCode.V))
        {
            ActivarPowerUp(MunicionTipo.Autonoma);

        }
       if(Input.GetKeyDown(KeyCode.N))
        {
            //  ActivarPowerUp(MunicionTipo.Normal);
            InputPowerUp();
        }

    }

    void CrearMunicion()
    {
        for (int i = 0; i < cantidad; i++)
        {
            GameObject municion = Instantiate(municion_prefab, this.transform.position, Quaternion.identity) as GameObject;
            municion.transform.name = "muncion " + i;
            municion.SetActive(false);
            municiones.Add(municion);

            
        }
        print("municiones creadas");
        CargarMunicion();
    }

    public void CargarMunicion()
    {
        
        foreach (GameObject m in municiones)
        {
            if (!m.activeInHierarchy)
            {
                municionTemp = m;

                //En caso de activar un PowerUp//nota debe haber un conteo de municion
                if (explosivo_PU)
                    municionTemp.GetComponent<MunicionControl>()._tipoMunicion = MunicionTipo.Explosiva;
                else if (autonoma_PU)
                    municionTemp.GetComponent<MunicionControl>()._tipoMunicion = MunicionTipo.Autonoma;
                else
                    municionTemp.GetComponent<MunicionControl>()._tipoMunicion = MunicionTipo.Normal;
                //

                municionTemp.SetActive(true);
                municionTemp.GetComponent<Rigidbody>().isKinematic = true;
                municionTemp.GetComponent<SphereCollider>().enabled = false;
                
                if (automatica_PU)
                {
                    municionTemp.transform.position = posMunicionAutomatica.position;
                    municionTemp.transform.rotation = posMunicionAutomatica.rotation;
                    municionTemp.transform.parent = posMunicionAutomatica.transform;
                }
                else
                {
                    municionTemp.transform.position = posMunicion.position;
                    municionTemp.transform.rotation = posMunicion.rotation;
                    municionTemp.transform.parent = posMunicion.transform;
                }
                municionTemp.GetComponent<MunicionControl>().ActivarMuncion();
               

                break;
            }
        }
    }
 

    public void Disparar()
    {
        if (autonoma_PU)
        {
            DispararAutomatica();
            return;
        }

        fuerzaTotal = fuerza * multiplicadorFuerza;
       
          if(municionTemp != null)
          {
                 municionTemp.transform.parent = null;
                
                 municionTemp.GetComponent<Rigidbody>().isKinematic = false;
                 municionTemp.GetComponent<Rigidbody>().velocity = this.transform.forward * fuerzaTotal;//multiplicar la fuerza por el vector de distancia entre los controles
                 municionTemp.GetComponent<SphereCollider>().enabled = true;
                 municionTemp.GetComponent<MunicionControl>().ActivarMuncion();
                 municionTemp.GetComponent<MunicionControl>().ActivarTrail(true);
                 municionTemp.GetComponent<MunicionControl>().woosh_sfx.Play();



          }

        if (automatica_PU)
        {
            //animacione resortera automatica
            automatica_anim.SetTrigger("disparar");
        }
        else
        {

           
        }
        soltada_sfx.Play();
        posMunicion.transform.position = posInicialTirante.position;
        fuerzaTotal = 0f;
        //ligaResortera_blendShape.SetBlendShapeWeight(0, 0);
        municionTemp = null;
      
        Invoke("CargarMunicion",0.2f);
    }

    public void DispararAutomatica()//Autonoma
    {

        if (municionTemp == null)
            return;



        StartCoroutine(municionTemp.GetComponent<MunicionControl>().DisparoAutonomo());

        resortera_anim.SetTrigger("soltada");
        posMunicion.transform.position = posInicialTirante.position;
        municionTemp = null;
        Invoke("CargarMunicion", 0.2f);
    }

    public void ResorteraTomada()
    {
        enMano = true;
        this.GetComponent<Rigidbody>().isKinematic = true;
        trigger.enabled = false;
        triggerTirante.enabled = true;
    }
    public void ResorteraSoltada()
    {
        enMano = false;
        this.GetComponent<Rigidbody>().isKinematic = true;
        trigger.enabled = true;
        triggerTirante.enabled = false;
    }

    public void MoverTirante(float dist)
    {
        estirando = true;
        posMunicion.transform.position = Vector3.Lerp(posInicialTirante.position,posFinalTirante.position, dist);
      //ligaResortera_blendShape.SetBlendShapeWeight(0,  Mathf.Clamp((dist * 99),0,100));
        resortera_anim.SetFloat("jale", dist * 100.0f);
        if(!estirandose_sfx.IsPlaying())
        {
           // estirandose_sfx.Stop();
            estirandose_sfx.Play();

        }

        

    }


    void InputPowerUp()//TEMP DEBUG
    {
        if(!explosivo_PU && !autonoma_PU && !automatica_PU)
        {
            ActivarPowerUp(MunicionTipo.Explosiva);
        }
        else if(explosivo_PU)
        {
            ActivarPowerUp(MunicionTipo.Autonoma);
        }
        else if(autonoma_PU)
        {
            ActivarPowerUp(MunicionTipo.Automatica);
        }
        else if(automatica_PU)
        {
            ActivarPowerUp(MunicionTipo.Normal);
        }
    }
    public void ActivarPowerUp(MunicionTipo tipoMunicion)
    {
        print("Cambiando municion....");
        explosivo_PU = false;
        autonoma_PU = false;
        automatica_PU = false;
        mano.disparoAutomatico = false;

        if (tipoMunicion == MunicionTipo.Normal)
        {
            meshNormal.SetActive(true);
            meshAutomatica.SetActive(false);
            mano.disparoAutomatico = false;
        }
        else if(tipoMunicion == MunicionTipo.Explosiva)
        {
            explosivo_PU = true;
        }
        else if (tipoMunicion == MunicionTipo.Autonoma)
        {
            autonoma_PU = true;
        }
        else if(tipoMunicion == MunicionTipo.Automatica)
        {
            print("Se elegio Automatica..cambiar tipo de resortera");
            automatica_PU = true;
            meshNormal.SetActive(false);
            meshAutomatica.SetActive(true);
            mano.disparoAutomatico = true;
        }
        if(municionTemp != null)
        {
            municionTemp.SetActive(false);
            municionTemp = null;
            CargarMunicion();

        }

        print("MunicionCambiada....");

    }
}
