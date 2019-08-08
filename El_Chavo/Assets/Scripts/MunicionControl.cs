using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MunicionControl : MonoBehaviour
{
    public MunicionTipo _tipoMunicion;
    public ParticleSystem explosion_vfx;
    public GameObject mesh;

  

    public void ActivarMuncion()
    {
        mesh.SetActive(true);
    }
  
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.transform.tag == "mano" || collision.transform.tag == "tirante" || collision.transform.tag == "resortera")
    //    {
    //        print(this.transform.name + " colisiono con " + collision.transform.name);
    //    }else if(collision.transform.tag == "globo")
    //    {
            
    //    }
    //    else
    //    {
    //        print(this.transform.name + " colisiono con " + collision.transform.name);
    //        StartCoroutine(Explotar());
           
    //    }


    //}
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "globo")
        {
            other.GetComponent<GloboControl>().RecibirDaño(1);
            StartCoroutine(Explotar());
        }
        else if (other.transform.tag == "MainCamera")
        {

        }
        else if (other.transform.tag == "mano" || other.transform.tag == "tirante" || other.transform.tag == "resortera")
        {

        }else
        { 
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
