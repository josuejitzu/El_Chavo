using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GloboControl : MonoBehaviour
{
    public GameObject mesh;
    public SphereCollider colision;
    public Rigidbody rigid;
    public ParticleSystem explosion_vfx;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ActivarGlobo()
    {
        mesh.SetActive(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(Explotar());
    }
    IEnumerator Explotar()
    {
        rigid.isKinematic = true;
        mesh.SetActive(false);
        explosion_vfx.Play();
        yield return new WaitForSeconds(1.0f);
        this.gameObject.SetActive(false);

    }
}
