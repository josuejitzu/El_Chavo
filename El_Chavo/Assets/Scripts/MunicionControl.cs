using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MunicionControl : MonoBehaviour
{
    public MunicionTipo _tipoMunicion;
    public ParticleSystem explosion_vfx;
    public GameObject mesh;

    void Start()
    {

        
    }

    public void ActivarMuncion()
    {
        mesh.SetActive(true);
    }
  
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "mano" || collision.transform.tag == "tirante" || collision.transform.tag == "resortera")
        {
            print(this.transform.name + " colisiono con " + collision.transform.name);
        }
        else
        {
            print(this.transform.name + " colisiono con " + collision.transform.name);
            StartCoroutine(Explotar());
           
        }


    }
    IEnumerator Explotar()
    {
        explosion_vfx.Play();
        mesh.SetActive(false);
        yield return new WaitForSeconds(1.0f);
        this.gameObject.SetActive(false);
    }
}
