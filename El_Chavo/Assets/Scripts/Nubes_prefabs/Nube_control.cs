using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider),typeof(Rigidbody))]
public class Nube_control : MonoBehaviour
{
    public float velocidad = 5.0f;
 

    public void MiUpdate()
    {
        this.transform.Translate(Vector3.left * (Time.deltaTime * velocidad));
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "FinNubes")
        {
            this.gameObject.SetActive(false);
        }
    }
}
