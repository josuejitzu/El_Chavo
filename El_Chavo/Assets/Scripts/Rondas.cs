using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Rondas 
{
    public float duracion = 30.0f;
    public float minRate = 2.0f;
    public float maxRate = 3.0f;
    //ojo maxLanzadores no puede ser mayor a 6 pues de 0 a 6 , 6 queda desacartado por  la funcion Random
    // por lo tanto personajes puede ser 6 pero 6 quedaria descartado
    public int personajes = 5;//de 0 a 5 

}
