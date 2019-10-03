using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nubes_Sistema : MonoBehaviour
{
    public GameObject[] nubes;
    public float minVel, maxVel;
    public Transform[] posiciones;
    int posAnterior;
    public float rateSpawn = 10.0f;
    float sigSpawn;
  
    // Update is called once per frame
    void Update()
    {
        if(Time.time >= sigSpawn)
        {
            ActivarNube();
            sigSpawn = Time.time + rateSpawn;
        }

        ///Update para las nubes
        for (int i = 0; i < nubes.Length; i++)
        {
            if (nubes[i].activeInHierarchy)
                nubes[i].GetComponent<Nube_control>().MiUpdate();
        }
    }
    public void ActivarNube()
    {
        GameObject nube = NubeRandom();
        if (nube == null)
            return;

        Transform pos = posiciones[PosicionRandom()];
        nube.transform.position = pos.position;
        nube.GetComponent<Nube_control>().velocidad = VelocidadRandom();
        nube.SetActive(true);
    }

    GameObject NubeRandom()
    {
        GameObject n = null;
        for (int i = 0; i < nubes.Length; i++)
        {
            if(nubes[i].activeInHierarchy == false)
            {
                n = nubes[i];
                break;
            }
        }
        return n;
    }
    int PosicionRandom()
    {
        int r = Random.Range(0, posiciones.Length);

        while( r == posAnterior)
        {
            r = Random.Range(0, posiciones.Length);
        }
        posAnterior = r;

        return r;
    }
    float VelocidadRandom()
    {
        float r = Random.Range(minVel, maxVel);
        return r;
    }

}
