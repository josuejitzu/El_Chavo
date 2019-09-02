using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Resortera_Control : MonoBehaviour
{
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

    void Start()
    {
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
                municionTemp.transform.position = posMunicion.position;
                municionTemp.transform.parent = posMunicion.transform;
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


          }
        resortera_anim.SetTrigger("soltada");
        posMunicion.transform.position = posInicialTirante.position;
        //ligaResortera_blendShape.SetBlendShapeWeight(0, 0);
        municionTemp = null;
        Invoke("CargarMunicion",0.2f);
    }

    public void DispararAutomatica()
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
        this.GetComponent<Rigidbody>().isKinematic = true;
        trigger.enabled = false;
        triggerTirante.enabled = true;
    }

    public void MoverTirante(float dist)
    {
        estirando = true;
        posMunicion.transform.position = Vector3.Lerp(posInicialTirante.position,posFinalTirante.position, dist);
      //ligaResortera_blendShape.SetBlendShapeWeight(0,  Mathf.Clamp((dist * 99),0,100));
        resortera_anim.SetFloat("jale", dist * 100);
    }


    void InputPowerUp()
    {
        if(!explosivo_PU && !autonoma_PU)
        {
            ActivarPowerUp(MunicionTipo.Explosiva);
        }
        else if(explosivo_PU)
        {
            ActivarPowerUp(MunicionTipo.Autonoma);
        }
        else if(autonoma_PU)
        {
            ActivarPowerUp(MunicionTipo.Normal);
        }
    }
    public void ActivarPowerUp(MunicionTipo tipoMunicion)
    {
        explosivo_PU = false;
        autonoma_PU = false;

        if (tipoMunicion == MunicionTipo.Normal)
        {
           
        }
        else if(tipoMunicion == MunicionTipo.Explosiva)
        {
            explosivo_PU = true;
        }
        else if (tipoMunicion == MunicionTipo.Autonoma)
        {
            autonoma_PU = true;
        }

        if(municionTemp != null)
        {
            municionTemp.SetActive(false);
            municionTemp = null;
            CargarMunicion();

        }
    }
}
