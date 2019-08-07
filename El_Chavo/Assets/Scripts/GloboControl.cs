using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GloboControl : MonoBehaviour
{
    public TipoPersonaje _tipoPersonaje;
    public GloboTipo _tipoGoblo;
    public GameObject globoChavo,globoKiko,globoÑoño,globoDonRamon,globoPoppy,globoDoñaFlorinda;
    public GameObject meshGlobo;
    public SphereCollider colision;
    public Rigidbody rigid;
    public ParticleSystem explosion_vfx;

    // Start is called before the first frame update

    void Start()
    {
        
    }

    public void ActivarGlobo()
    {
      
        meshGlobo.SetActive(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(Explotar());
    }
    IEnumerator Explotar()
    {
        rigid.isKinematic = true;
        meshGlobo.SetActive(false);
        explosion_vfx.Play();
        yield return new WaitForSeconds(1.0f);
        this.gameObject.SetActive(false);

    }

    public void CambiarGlobo()
    {
        globoChavo.SetActive(false);
        globoKiko.SetActive(false);
        globoÑoño.SetActive(false);
        globoDonRamon.SetActive(false);
        globoPoppy.SetActive(false);
        globoDoñaFlorinda.SetActive(false);

        if(_tipoPersonaje == TipoPersonaje.chavo)
        {
            meshGlobo = globoChavo;
        }
        else if(_tipoPersonaje == TipoPersonaje.kiko)
        {
            meshGlobo = globoKiko;
        }
    }
}
