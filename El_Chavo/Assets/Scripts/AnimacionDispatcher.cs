using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimacionDispatcher : MonoBehaviour
{
    // Start is called before the first frame update
    public Lanzador_Globos lanzador;
    void Start()
    {
        
    }
    public void Disparar()
    {
        print(this.transform.name + " mando orden de disparo desde animacion");
        lanzador.Disparar();
    }
}
