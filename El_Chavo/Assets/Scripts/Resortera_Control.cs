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
                municionTemp.SetActive(true);
                municionTemp.GetComponent<Rigidbody>().isKinematic = true;
                municionTemp.GetComponent<SphereCollider>().enabled = false;
                municionTemp.transform.position = posMunicion.position;
                municionTemp.transform.parent = posMunicion.transform;
                break;
            }
        }
    }

    public void Disparar()
    {
        fuerzaTotal = fuerza * multiplicadorFuerza;
       
          if(municionTemp != null)
          {
                 municionTemp.transform.parent = null;
                
                 municionTemp.GetComponent<Rigidbody>().isKinematic = false;
                 municionTemp.GetComponent<Rigidbody>().velocity = this.transform.forward * fuerzaTotal;//multiplicar la fuerza por el vector de distancia entre los controles
                 municionTemp.GetComponent<SphereCollider>().enabled = true;

          }
        posMunicion.transform.position = posInicialTirante.position;
        ligaResortera_blendShape.SetBlendShapeWeight(0, 0);
        Invoke("CargarMunicion",0.5f);
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
        ligaResortera_blendShape.SetBlendShapeWeight(0,  Mathf.Clamp((dist * 99),0,100));
    }
}
