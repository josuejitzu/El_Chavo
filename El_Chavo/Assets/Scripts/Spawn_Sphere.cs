using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Spawn_Sphere : MonoBehaviour
{
    public static Spawn_Sphere _PosicionesEsfera;
    public GameObject puntoPrefab;
    public List<Transform> puntos = new List<Transform>();
    public int cantidadPuntos;

    public float pX, pY, pZ;
    public float separacionX, separacionY, separacionZ;
    public int xSize, ySize, zSize;

    private void OnValidate()
    {
       
    }
    private void Awake()
    {
        foreach (GameObject p in GameObject.FindGameObjectsWithTag("posicionDebuff"))
        {
            puntos.Add(p.transform);
            p.SetActive(false);
        }
    }
    void Start()
    {
        _PosicionesEsfera = this;
       // SpawnPuntos();
    }

    // Update is called once per frame
    public void SpawnPuntos()
    {
        for (int i = 0; i < cantidadPuntos; i++)
        {
            GameObject punto = Instantiate(puntoPrefab, this.transform.position, Quaternion.identity) as GameObject;
            punto.transform.parent = this.transform;
            puntos.Add(punto.transform);
        }
        AcomodarPuntos();
    }

    void AcomodarPuntos()
    {
        for (int i = 0; i < zSize; i++)
        {
            for (int j = 0; j < xSize; j++)
            {
                for (int k = 0; k < ySize; k++)
                {
                    puntos[k].position = new Vector3(pX, pY, pZ);
                    pY += separacionY;
            
                }
                
                pX += separacionX;
            }
         
            pZ += separacionZ ;
        }

    }
    
}
