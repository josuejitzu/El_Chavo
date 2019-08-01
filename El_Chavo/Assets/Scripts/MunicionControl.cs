using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MunicionControl : MonoBehaviour
{
    public MunicionTipo _tipoMunicion;
    public ParticleSystem explosion_vfx;


    void Start()
    {

        
    }
  
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "mano" || collision.transform.tag == "tirante" || collision.transform.tag == "resortera")
        {

        }
        else
        {
            StartCoroutine(Explotar());
        }
    }
    IEnumerator Explotar()
    {
        explosion_vfx.Play();
        yield return new WaitForSeconds(1.0f);
        this.gameObject.SetActive(false);
    }
}
